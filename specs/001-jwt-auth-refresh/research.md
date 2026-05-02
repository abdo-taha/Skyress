# Research: JWT Authentication & Authorization with Refresh Tokens

**Branch**: `001-jwt-auth-refresh` | **Date**: 2026-05-02

## Decision Log

---

### D-001: Authentication Framework — Custom vs ASP.NET Core Identity

**Decision**: Custom auth entities (not ASP.NET Core Identity)

**Rationale**: The project uses `long` primary keys via `BaseEntity`, a custom auditing pattern (`IAuditable`), and domain event sourcing via `AggregateRoot`. ASP.NET Core Identity uses `string` IDs by default and brings significant opinion about schema that would conflict with the established conventions. A custom User aggregate follows the existing patterns for every other aggregate (Customer, Item, Invoice) in the codebase and keeps the layer boundaries clean.

**Alternatives considered**:
- `Microsoft.AspNetCore.Identity.EntityFrameworkCore` — rejected: forces `IdentityUser<TKey>` base class which conflicts with `AggregateRoot`, and Identity's schema (AspNetUsers, AspNetRoles etc.) doesn't align with the outbox + auditable pattern.
- `OpenIddict` / `Duende IdentityServer` — rejected: full OAuth2/OIDC server is overkill for a single-tenant API.

---

### D-002: Access Token Format — JWT

**Decision**: JWT (JSON Web Token) via `Microsoft.AspNetCore.Authentication.JwtBearer` 8.0.0

**Rationale**: Explicitly requested. JWT is stateless — no database lookup on every request. Claims include `sub` (userId), `email`, `roles[]`, `jti` (unique token ID). Validated by ASP.NET Core's built-in middleware using a symmetric HMAC-SHA256 signing key.

**Configuration**:
- Algorithm: HMAC-SHA256 (`SecurityAlgorithms.HmacSha256`)
- Expiry: 15 minutes (`AccessTokenExpiryMinutes: 15`)
- Claims: `sub`, `email`, `role` (multiple), `jti`, `iat`, `exp`
- Signing key: stored in `appsettings.json` under `Jwt:SecretKey` (min 32 bytes; use User Secrets in development)

**Alternatives considered**:
- Opaque reference tokens — rejected: requires a database lookup on every request, defeating the stateless API goal.
- RSA asymmetric keys — not needed for single-service deployment; symmetric HMAC is simpler and sufficient.

---

### D-003: Refresh Token Format — Opaque + Server-Stored

**Decision**: Cryptographically random opaque string (Base64-encoded 64 bytes), stored in PostgreSQL

**Rationale**: Unlike JWTs, opaque refresh tokens can be revoked instantly by deleting or flagging the record. Server-side storage enables replay detection and family-based revocation. The spec explicitly requires logout revocation and replay-attack protection.

**Token family pattern**: Each login session creates a `FamilyId` (GUID). Every token rotation links back to the same `FamilyId`. If a consumed token is re-presented, all tokens in that family are immediately revoked, forcing re-login.

**Expiry**: 7 days (`RefreshTokenExpiryDays: 7`)

**Alternatives considered**:
- JWT refresh tokens — rejected: cannot be revoked without a blocklist, which requires database reads anyway.
- Short-lived refresh tokens (1 day) — rejected: spec requires sessions of up to 7 days.

---

### D-004: Password Hashing — BCrypt

**Decision**: `BCrypt.Net-Next` 4.0.3 with work factor 12

**Rationale**: BCrypt is adaptive (work factor tunable as hardware improves), widely battle-tested, and includes the salt in the hash output. The project does not use ASP.NET Core Identity, so `IPasswordHasher<T>` from Identity is not available without the full Identity package.

**Alternatives considered**:
- `Microsoft.AspNetCore.Identity.IPasswordHasher<T>` — available without full Identity via the `Microsoft.Extensions.Identity.Core` package, but adds a dependency only to re-use a hasher; BCrypt.Net-Next is simpler.
- Argon2 via `Konscious.Security.Cryptography` — stronger than BCrypt but more complex setup; BCrypt is sufficient for this use case.
- SHA-256/PBKDF2 — rejected: PBKDF2 is fine but BCrypt is the standard for greenfield .NET auth without Identity.

---

### D-005: Authorization Model — Policy-Based with Role Claims

**Decision**: ASP.NET Core policy-based authorization using role claims from JWT

**Rationale**: Roles (`Admin`, `Customer`) are embedded as `role` claims in the JWT. Endpoints use `[Authorize(Roles = "Admin")]` or `[Authorize]` as appropriate. For Customer data isolation (FR-011), handlers extract `userId` from `IHttpContextAccessor` → `ClaimsPrincipal` and filter queries accordingly.

**Alternatives considered**:
- Attribute-based with custom `IAuthorizationHandler` — adds complexity without benefit for two-role system.
- Resource-based authorization — considered for Customer data isolation, but simpler `userId` filter in query handlers is sufficient at this scale.

---

### D-006: Endpoint Protection Strategy — Protect All, Whitelist Public

**Decision**: Add `[Authorize]` globally as a default policy; whitelist `/api/v1/auth/*` endpoints as `[AllowAnonymous]`

**Rationale**: Defense-in-depth: any new endpoint added in the future is automatically protected. The spec states all existing endpoints (basket, invoices, payments, customers, items) must be protected. Opt-out is safer than opt-in.

**Implementation**: Set `options.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build()` in `AddAuthorization()`.

---

### D-007: New NuGet Packages Required

| Project | Package | Version |
|---------|---------|---------|
| Skyress.API | `Microsoft.AspNetCore.Authentication.JwtBearer` | 8.0.0 |
| Skyress.Infrastructure | `BCrypt.Net-Next` | 4.0.3 |

No other new packages required. JWT validation is built into ASP.NET Core 8. EF Core and Npgsql are already present for token persistence.

---

### D-008: Configuration Schema

New settings in `appsettings.json`:

```json
"Jwt": {
  "SecretKey": "REPLACE_WITH_32+_CHAR_SECRET_IN_USER_SECRETS",
  "Issuer": "Skyress",
  "Audience": "SkyressApi",
  "AccessTokenExpiryMinutes": 15,
  "RefreshTokenExpiryDays": 7
}
```

`SecretKey` must be stored in User Secrets (`dotnet user-secrets`) or environment variables in production — never committed to source control.

---

## Unresolved Items

None — all decisions finalized.
