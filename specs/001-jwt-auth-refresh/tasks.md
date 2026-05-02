# Tasks: JWT Authentication & Authorization with Refresh Tokens

**Input**: Design documents from `/specs/001-jwt-auth-refresh/`
**Prerequisites**: plan.md ✅, spec.md ✅, research.md ✅, data-model.md ✅, contracts/auth-api.md ✅
**Implementation Model**: `claude-haiku-4-5-20251001` (user-specified)
**Tests**: Not requested — no test tasks included

**Organization**: Tasks grouped by user story for independent implementation and testing.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies on incomplete tasks)
- **[Story]**: Which user story this task belongs to (US1–US5)

## Path Conventions

- API project root: `Skyress/`
- Domain project: `Skyress.Domain/`
- Application project: `Skyress.Application/`
- Infrastructure project: `Skyress.Infrastructure/`

---

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Add new dependencies and configuration groundwork

- [x] T001 Add `Microsoft.AspNetCore.Authentication.JwtBearer` version 8.0.0 to `Skyress/Skyress.API.csproj`
- [x] T002 Add `BCrypt.Net-Next` version 4.0.3 to `Skyress.Infrastructure/Skyress.Infrastructure.csproj`
- [x] T003 [P] Add `Jwt` settings block (Issuer, Audience, AccessTokenExpiryMinutes, RefreshTokenExpiryDays) to `Skyress/appsettings.json` and create `JwtSettings.cs` record in `Skyress.Infrastructure/Services/JwtSettings.cs`

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Domain entities, database schema, token services, and auth middleware — all user stories depend on this phase

**⚠️ CRITICAL**: No user story work can begin until this phase is complete

### Domain Entities (Skyress.Domain)

- [x] T004 [P] Create `User` aggregate root in `Skyress.Domain/Aggregates/Auth/User.cs` — fields: Id (long), Email, PasswordHash, IsActive; implements IAuditable; raises UserRegisteredDomainEvent
- [x] T005 [P] Create `Role` entity in `Skyress.Domain/Aggregates/Auth/Role.cs` — fields: Id (int), Name; represents Admin/Customer roles
- [x] T006 [P] Create `UserRole` junction entity in `Skyress.Domain/Aggregates/Auth/UserRole.cs` — fields: UserId (long), RoleId (int); composite PK
- [x] T007 [P] Create `RefreshToken` entity in `Skyress.Domain/Aggregates/Auth/RefreshToken.cs` — fields: Id, Token, UserId, FamilyId (Guid), ExpiresAt, IsUsed, IsRevoked, ReplacedByToken, CreatedAt; validity logic: not used + not revoked + not expired

### Database (Skyress.Infrastructure)

- [x] T008 Add `DbSet<User>`, `DbSet<Role>`, `DbSet<UserRole>`, `DbSet<RefreshToken>` to `Skyress.Infrastructure/Persistence/SkyressDbContext.cs`
- [x] T009 [P] Create `UserConfiguration` in `Skyress.Infrastructure/Configurations/UserConfiguration.cs` — table `users`, unique index on Email, max lengths per data model
- [x] T010 [P] Create `RoleConfiguration` in `Skyress.Infrastructure/Configurations/RoleConfiguration.cs` — table `roles`, seed Admin (Id=1) and Customer (Id=2) via HasData
- [x] T011 [P] Create `UserRoleConfiguration` in `Skyress.Infrastructure/Configurations/UserRoleConfiguration.cs` — table `user_roles`, composite PK (UserId, RoleId)
- [x] T012 [P] Create `RefreshTokenConfiguration` in `Skyress.Infrastructure/Configurations/RefreshTokenConfiguration.cs` — table `refresh_tokens`, unique index on Token, index on FamilyId, index on UserId
- [x] T013 Create EF Core migration `AddAuthEntities` and verify it generates correct SQL: `dotnet ef migrations add AddAuthEntities --project Skyress.Infrastructure --startup-project Skyress`

### Repository & Services (Skyress.Application + Skyress.Infrastructure)

- [x] T014 Create `IUserRepository` interface in `Skyress.Application/Auth/Contracts/Persistence/IUserRepository.cs` — methods: GetByEmailAsync, GetByIdAsync, AddAsync, ExistsByEmailAsync, GetRefreshTokenAsync, GetRefreshTokensByFamilyIdAsync, UpdateRefreshTokenAsync
- [x] T015 Implement `UserRepository` in `Skyress.Infrastructure/Repository/UserRepository.cs` — implement IUserRepository using SkyressDbContext; family revocation query (set IsRevoked=true for all by FamilyId)
- [x] T016 [P] Create `PasswordHasher` in `Skyress.Infrastructure/Services/PasswordHasher.cs` — wrap BCrypt.Net-Next HashPassword (work factor 12) and Verify methods
- [x] T017 Create `JwtTokenService` in `Skyress.Infrastructure/Services/JwtTokenService.cs` — GenerateAccessToken(user, roles) returns signed JWT with claims: sub, email, role[], jti, iat, exp, iss, aud; GenerateRefreshToken() returns Base64(64 random bytes)

### Shared DTOs

- [x] T018 [P] Create `TokenResponse` record in `Skyress.Application/Auth/DTOs/TokenResponse.cs` — properties: AccessToken, RefreshToken, ExpiresIn (int, seconds), TokenType ("Bearer")

### Dependency Injection & Middleware

- [x] T019 Register `IUserRepository → UserRepository`, `PasswordHasher`, `JwtTokenService` (as scoped/singleton as appropriate) in `Skyress.Infrastructure/Extensions/ServiceCollectionExtensions.cs`
- [x] T020 Add `AddAuthentication(JwtBearer)` with JWT validation parameters (issuer, audience, signing key from JwtSettings) to `Skyress/Extenstions/DependencyInjection.cs`; add `AddAuthorization` with fallback policy `RequireAuthenticatedUser`
- [x] T021 Add `UseAuthentication()` and `UseAuthorization()` calls to the middleware pipeline in `Skyress/Program.cs` — must come after `UseHttpsRedirection` and before endpoint mapping

**Checkpoint**: Foundation ready — all domain entities exist, DB schema is in place, JWT service can generate tokens, password hashing works, middleware pipeline is configured

---

## Phase 3: User Story 1 — Registration & Login (Priority: P1) 🎯 MVP

**Goal**: Users can create an account and log in to receive a JWT access token and a refresh token

**Independent Test**: POST /api/v1/auth/register → 201 with tokens; POST /api/v1/auth/login → 200 with tokens; invalid credentials → 401

### Implementation for User Story 1

- [x] T022 [P] [US1] Create `RegisterCommand` record and `RegisterCommandValidator` in `Skyress.Application/Auth/Commands/Register/RegisterCommand.cs` and `RegisterCommandValidator.cs` — validates email format, password min 8 chars
- [x] T023 [P] [US1] Create `LoginCommand` record and `LoginCommandValidator` in `Skyress.Application/Auth/Commands/Login/LoginCommand.cs` and `LoginCommandValidator.cs` — validates email and password present
- [x] T024 [US1] Implement `RegisterCommandHandler` in `Skyress.Application/Auth/Commands/Register/RegisterCommandHandler.cs` — check email uniqueness (409 if taken), hash password, create User with Customer role, generate access + refresh tokens, persist refresh token, return TokenResponse
- [x] T025 [US1] Implement `LoginCommandHandler` in `Skyress.Application/Auth/Commands/Login/LoginCommandHandler.cs` — look up user by email (401 if not found), verify password (401 if mismatch), check IsActive (403 if inactive), generate new access + refresh tokens, persist refresh token, return TokenResponse
- [x] T026 [P] [US1] Create `RegisterRequest` and `LoginRequest` DTOs in `Skyress/DTOs/Auth/RegisterRequest.cs` and `Skyress/DTOs/Auth/LoginRequest.cs`
- [x] T027 [US1] Create `AuthApiRegistration` in `Skyress/Endpoints/Auth/AuthApiRegistration.cs` — register MapPost("/register") and MapPost("/login") using MediatR; both endpoints use `[AllowAnonymous]`; map to respective Request DTOs → Commands → return TokenResponse
- [x] T028 [US1] Add `MapAuthApi()` call to endpoint registration in `Skyress/Program.cs` alongside existing endpoint registrations

**Checkpoint**: User Story 1 complete — register and login return tokens; test independently before continuing

---

## Phase 4: User Story 2 — Protected Resource Access (Priority: P2)

**Goal**: All existing endpoints reject unauthenticated requests (401); authenticated requests with valid JWT succeed

**Independent Test**: GET /api/v1/items without Authorization header → 401; same request with `Bearer <accessToken>` → 200

### Implementation for User Story 2

- [x] T029 [US2] Confirm `UseAuthentication()` and `UseAuthorization()` are in pipeline and `RequireAuthenticatedUser` fallback policy is active (T020-T021); verify no existing endpoint registration uses `[AllowAnonymous]` — check all files under `Skyress/Endpoints/` and remove any inadvertent anonymous access
- [x] T030 [US2] Verify `AuthApiRegistration` endpoints are the ONLY `[AllowAnonymous]` routes in `Skyress/Endpoints/Auth/AuthApiRegistration.cs` — confirm register, login, and (later) refresh and logout are correctly marked

**Checkpoint**: User Story 2 complete — all non-auth endpoints return 401 without valid JWT; test with and without Authorization header

---

## Phase 5: User Story 3 — Silent Token Refresh (Priority: P3)

**Goal**: A valid refresh token can be exchanged for new access + refresh tokens (rotation); reuse of a consumed token revokes the whole family

**Independent Test**: POST /api/v1/auth/refresh with valid refresh token → 200 with new tokens; POST again with same (now-consumed) token → 401 and original token family fully revoked

### Implementation for User Story 3

- [x] T031 [P] [US3] Create `RefreshTokenCommand` record and `RefreshTokenCommandValidator` in `Skyress.Application/Auth/Commands/RefreshToken/RefreshTokenCommand.cs` and `RefreshTokenCommandValidator.cs`
- [x] T032 [US3] Implement `RefreshTokenCommandHandler` in `Skyress.Application/Auth/Commands/RefreshToken/RefreshTokenCommandHandler.cs` — look up token in DB; if not found → 401 TOKEN_NOT_FOUND; if revoked → 401 TOKEN_REVOKED; if already used → revoke entire family by FamilyId → 401 TOKEN_ALREADY_USED; if expired → 401 TOKEN_EXPIRED; check user IsActive; mark old token IsUsed + ReplacedByToken, generate new access + refresh tokens with same FamilyId, persist, return TokenResponse
- [x] T033 [P] [US3] Create `RefreshTokenRequest` DTO in `Skyress/DTOs/Auth/RefreshTokenRequest.cs`
- [x] T034 [US3] Add `MapPost("/refresh")` endpoint with `[AllowAnonymous]` to `Skyress/Endpoints/Auth/AuthApiRegistration.cs`

**Checkpoint**: User Story 3 complete — refresh rotation works; replay detection revokes token family; test independently

---

## Phase 6: User Story 4 — Role-Based Access Control (Priority: P4)

**Goal**: Admin role has full access; Customer role is restricted to read access and their own data; wrong role returns 403

**Independent Test**: Login as Customer → POST /api/v1/items → 403; Login as Admin → POST /api/v1/items → 200/201

### Implementation for User Story 4

- [x] T035 [US4] Confirm `JwtTokenService` includes role claims (`role` claim with all user roles) in the generated JWT in `Skyress.Infrastructure/Services/JwtTokenService.cs` — roles are retrieved from UserRole entities via IUserRepository
- [x] T036 [P] [US4] Add `[Authorize(Roles = "Admin")]` to write operations (POST, PUT, DELETE) in `Skyress/Endpoints/Items/` endpoint registration files — GET reads remain accessible to any authenticated user
- [x] T037 [P] [US4] Add `[Authorize(Roles = "Admin")]` to customer management endpoints (GET list, POST, PUT, DELETE) in `Skyress/Endpoints/Customers/` — allow GET by ID for own record (check userId from claims vs route param)
- [x] T038 [P] [US4] Add `[Authorize(Roles = "Admin")]` to invoice list and payment endpoints in `Skyress/Endpoints/Invoices/` and `Skyress/Endpoints/Payments/`; allow GET by ID for own invoices (userId claim check in handler)
- [x] T039 [P] [US4] Add `[Authorize(Roles = "Admin")]` to tag and tag assignment write operations in `Skyress/Endpoints/Tags/` and `Skyress/Endpoints/TagAssignments/`
- [x] T040 [US4] Add customer data isolation to basket query/command handlers in `Skyress.Application/Baskets/` — inject `IHttpContextAccessor`, extract `sub` claim as userId, filter BasketItems to only the requesting user's basket (unless Admin role present)

**Checkpoint**: User Story 4 complete — role enforcement works end-to-end; test Customer vs Admin access on admin-only endpoints

---

## Phase 7: User Story 5 — Logout & Session Invalidation (Priority: P5)

**Goal**: Logging out immediately invalidates the refresh token; subsequent refresh attempts are rejected

**Independent Test**: Login → logout (204) → POST /api/v1/auth/refresh with same refresh token → 401

### Implementation for User Story 5

- [x] T041 [P] [US5] Create `LogoutCommand` record and `LogoutCommandValidator` in `Skyress.Application/Auth/Commands/Logout/LogoutCommand.cs` and `LogoutCommandValidator.cs`
- [x] T042 [US5] Implement `LogoutCommandHandler` in `Skyress.Application/Auth/Commands/Logout/LogoutCommandHandler.cs` — look up refresh token; if found and not already revoked, set IsRevoked=true and save; always return success (no error on not-found to avoid information leakage)
- [x] T043 [P] [US5] Create `LogoutRequest` DTO in `Skyress/DTOs/Auth/LogoutRequest.cs`
- [x] T044 [US5] Add `MapPost("/logout")` endpoint with `[AllowAnonymous]` returning 204 No Content to `Skyress/Endpoints/Auth/AuthApiRegistration.cs`

**Checkpoint**: User Story 5 complete — logout revokes token; all 5 user stories independently functional

---

## Phase 8: Polish & Cross-Cutting Concerns

**Purpose**: Developer experience, security hardening, and documentation

- [x] T045 [P] Add JWT Bearer security definition to Swagger in `Skyress/OpenApi/ConfigureSwaggerGenOptions.cs` — adds "Authorize" button to Swagger UI accepting `Bearer <token>`
- [x] T046 [P] Add `IHttpContextAccessor` registration to DI in `Skyress/Extenstions/DependencyInjection.cs` (needed by T040 data isolation handlers)
- [ ] T047 Run quickstart.md validation — execute all steps from `specs/001-jwt-auth-refresh/quickstart.md`: install packages, set user secrets, run migration, start app, test all 4 auth endpoints via Swagger

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies — start immediately
- **Foundational (Phase 2)**: Depends on Phase 1 completion — **BLOCKS all user stories**
- **US1 (Phase 3)**: Depends on Phase 2 — first independently testable increment (MVP)
- **US2 (Phase 4)**: Depends on Phase 2 + Phase 3 (needs working login to test)
- **US3 (Phase 5)**: Depends on Phase 2 + Phase 3 (needs login to get refresh token)
- **US4 (Phase 6)**: Depends on Phase 2 + Phase 3 (needs login to get token with role claims)
- **US5 (Phase 7)**: Depends on Phase 2 + Phase 3 (needs login to get refresh token to revoke)
- **Polish (Phase 8)**: Depends on all user story phases complete

### Within Each User Story

- [P]-marked tasks within a story run in parallel
- Command + Validator tasks [P] before their Handler task
- Request DTO tasks [P] before Endpoint task
- Endpoint task last in each story

### Parallel Opportunities

- T004–T007 (domain entities): all 4 in parallel
- T009–T012 (EF configurations): all 4 in parallel after their entity
- T022–T023 (Commands + Validators for US1): in parallel
- T026 (DTOs): in parallel with T022–T023
- T031 + T033 (US3 command + DTO): in parallel
- T036–T039 (US4 endpoint role updates): all 4 in parallel
- T041 + T043 (US5 command + DTO): in parallel
- T045–T046 (Polish): in parallel

---

## Parallel Example: Phase 2 (Foundational) Start

```text
# Wave 1 — Domain entities (all parallel):
T004: User.cs
T005: Role.cs
T006: UserRole.cs
T007: RefreshToken.cs
T016: PasswordHasher.cs
T018: TokenResponse.cs

# Wave 2 — DB configuration (parallel after entities):
T008: SkyressDbContext.cs (after T004-T007)
T009: UserConfiguration.cs (after T004)
T010: RoleConfiguration.cs (after T005)
T011: UserRoleConfiguration.cs (after T006)
T012: RefreshTokenConfiguration.cs (after T007)

# Wave 3 — Repository + service (after T014 contract):
T014: IUserRepository.cs (after T004)
T015: UserRepository.cs (after T014)
T017: JwtTokenService.cs (after T003 + T004 + T005)

# Wave 4 — DI + pipeline (after services):
T019: ServiceCollectionExtensions.cs (after T015-T017)
T020: DependencyInjection.cs (after T017)
T021: Program.cs middleware (after T020)

# Wave 5 — Migration (after all DB config):
T013: EF migration (after T008-T012)
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup
2. Complete Phase 2: Foundational (**critical blocker**)
3. Complete Phase 3: User Story 1 (Register + Login)
4. **STOP and VALIDATE**: Test register → login → call protected endpoint with token
5. Demo working authentication before proceeding

### Incremental Delivery

1. Setup + Foundational → foundation working (T001–T021)
2. US1: Register + Login → MVP — users can authenticate (T022–T028)
3. US2: Confirm protection → all endpoints properly secured (T029–T030)
4. US3: Refresh tokens → long sessions without re-login (T031–T034)
5. US4: Role-based access → Admin vs Customer separation (T035–T040)
6. US5: Logout → revoke sessions (T041–T044)
7. Polish → developer experience + Swagger auth (T045–T047)

---

## Notes

- `[P]` tasks = different files, safe to run in parallel
- `[Story]` label maps each task to the user story for traceability
- **No tests generated** — user chose to implement without test tasks
- All new code must compile with `TreatWarningsAsErrors = true` (all 4 projects)
- `JwtSettings:SecretKey` must be set via `dotnet user-secrets` — never hardcode
- EF migration (T013) must run **before** application start; migration failure blocks US1+
- `IHttpContextAccessor` (T046) is needed before T040 — add it early in Polish or move to Foundational if implementing US4 first
