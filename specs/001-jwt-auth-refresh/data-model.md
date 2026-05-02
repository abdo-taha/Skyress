# Data Model: JWT Authentication & Authorization

**Branch**: `001-jwt-auth-refresh` | **Date**: 2026-05-02

## Entities

---

### User

**Layer**: `Skyress.Domain/Aggregates/Auth/User.cs`
**Base**: `AggregateRoot` → `BaseEntity` (Id: `long`, identity column)

| Field | Type | Constraints |
|-------|------|-------------|
| Id | long | PK, identity (auto) |
| Email | string | Required, max 256, unique index |
| PasswordHash | string | Required, max 512 |
| IsActive | bool | Required, default `true` |
| CreatedAt | DateTime | Required (IAuditable) |
| LastEditDate | DateTime | Required (IAuditable) |
| LastEditBy | string? | Max 50 (IAuditable) |

**Relationships**:
- Has many `UserRole` (N:M through junction)
- Has many `RefreshToken` (1:N)

**Domain rules**:
- Email stored in lowercase, trimmed
- `IsActive = false` blocks login and all token operations
- Registration raises `UserRegisteredDomainEvent`

---

### Role

**Layer**: `Skyress.Domain/Aggregates/Auth/Role.cs`
**Base**: `BaseEntity` (Id: `int` — override; small enum-like table)

| Field | Type | Constraints |
|-------|------|-------------|
| Id | int | PK |
| Name | string | Required, max 50, unique |

**Seeded values**:

| Id | Name |
|----|------|
| 1 | Admin |
| 2 | Customer |

**Note**: Id is `int` (not `long`) — this is the only entity that overrides the base. Roles are seeded via `HasData` in EF configuration; they are never created at runtime.

---

### UserRole

**Layer**: `Skyress.Domain/Aggregates/Auth/UserRole.cs`
**Base**: Plain entity (no `AggregateRoot`)

| Field | Type | Constraints |
|-------|------|-------------|
| UserId | long | FK → User.Id, PK (composite) |
| RoleId | int | FK → Role.Id, PK (composite) |

**Composite PK**: `(UserId, RoleId)`

**Domain rules**:
- New Customer registrations get RoleId = 2 (Customer) automatically
- Admin role assigned only by another Admin (out of scope for v1 — seeded manually or via migration)

---

### RefreshToken

**Layer**: `Skyress.Domain/Aggregates/Auth/RefreshToken.cs`
**Base**: `BaseEntity` (Id: `long`)

| Field | Type | Constraints |
|-------|------|-------------|
| Id | long | PK, identity |
| Token | string | Required, max 128, unique index |
| UserId | long | FK → User.Id, required |
| FamilyId | Guid | Required — groups all rotated tokens from one login session |
| ExpiresAt | DateTime | Required |
| IsUsed | bool | Required, default `false` |
| IsRevoked | bool | Required, default `false` |
| ReplacedByToken | string? | Max 128 — set when this token is rotated |
| CreatedAt | DateTime | Required |

**Relationships**:
- Belongs to one `User` (N:1)

**Domain rules**:
- A token is **valid** when: `IsUsed = false` AND `IsRevoked = false` AND `ExpiresAt > UtcNow`
- On rotation: set `IsUsed = true`, `ReplacedByToken = newToken.Token`, insert new token with same `FamilyId`
- On replay detection (used token re-presented): set `IsRevoked = true` on ALL tokens sharing the same `FamilyId`
- On logout: set `IsRevoked = true` on the token presented at logout

---

## EF Core Configuration Notes

### `SkyressDbContext` additions

```csharp
public DbSet<User> Users { get; set; }
public DbSet<Role> Roles { get; set; }
public DbSet<UserRole> UserRoles { get; set; }
public DbSet<RefreshToken> RefreshTokens { get; set; }
```

### `UserConfiguration`

- Table: `users`
- Index: `Email` (unique)
- `PasswordHash` column max length: 512
- Cascade delete: `UserRoles` on User delete

### `RoleConfiguration`

- Table: `roles`
- Seed: Admin (Id=1), Customer (Id=2)

### `UserRoleConfiguration`

- Table: `user_roles`
- Composite PK: `(UserId, RoleId)`

### `RefreshTokenConfiguration`

- Table: `refresh_tokens`
- Index: `Token` (unique)
- Index: `FamilyId` (non-unique — for family revocation queries)
- Index: `UserId` (non-unique — for logout queries)

---

## State Transitions

### RefreshToken lifecycle

```
[Created]
    │ valid (not used, not revoked, not expired)
    ▼
[Active] ──────── used in refresh ──────→ [Used]  ──→ ReplacedByToken set
    │                                                       │
    │ logout OR replay detection                            │ (new token created)
    ▼                                                       ▼
[Revoked]                                            [Active] (new token)
    │
    │ replay of revoked token
    ▼
[All family tokens Revoked]
```

### User lifecycle

```
[Unregistered] → register → [Active] → admin deactivate → [Inactive]
                                              ↑                  │
                                              └── reactivate ────┘
```
