# Quickstart: JWT Auth Feature — Dev Setup

**Branch**: `001-jwt-auth-refresh` | **Date**: 2026-05-02

## Prerequisites

- .NET 8 SDK
- Docker Desktop (PostgreSQL + RabbitMQ via docker-compose)
- Existing project running (`dotnet run` works on `master`)

## 1. Install New NuGet Packages

```powershell
# From repo root
dotnet add "Skyress\Skyress.API.csproj" package Microsoft.AspNetCore.Authentication.JwtBearer --version 8.0.0
dotnet add "Skyress.Infrastructure\Skyress.Infrastructure.csproj" package BCrypt.Net-Next --version 4.0.3
```

## 2. Add JWT Configuration

Add to `Skyress/appsettings.json` (non-secret fields only):

```json
"Jwt": {
  "Issuer": "Skyress",
  "Audience": "SkyressApi",
  "AccessTokenExpiryMinutes": 15,
  "RefreshTokenExpiryDays": 7
}
```

Store the secret key using User Secrets (never commit to source control):

```powershell
dotnet user-secrets set "Jwt:SecretKey" "your-super-secret-key-minimum-32-characters" --project "Skyress\Skyress.API.csproj"
```

## 3. Run EF Core Migration

After implementing the new entities and DbContext changes:

```powershell
dotnet ef migrations add AddAuthEntities --project Skyress.Infrastructure --startup-project Skyress
dotnet ef database update --project Skyress.Infrastructure --startup-project Skyress
```

## 4. Start the Application

```powershell
# Start dependencies (PostgreSQL + RabbitMQ)
docker-compose up -d

# Run API
dotnet run --project Skyress
```

## 5. Verify Auth Endpoints

Using Swagger UI at `https://localhost:{port}/swagger`:

1. **Register** — `POST /api/v1/auth/register` with `{ "email": "admin@test.com", "password": "Admin123!" }`
2. **Login** — `POST /api/v1/auth/login` with same credentials
3. **Authorize in Swagger** — click "Authorize", enter `Bearer <accessToken>`
4. **Access protected endpoint** — `GET /api/v1/items` should now return 200
5. **Refresh** — `POST /api/v1/auth/refresh` with the `refreshToken` from login
6. **Logout** — `POST /api/v1/auth/logout` with the refresh token

## 6. Seed Admin User (Optional)

Until an admin management endpoint exists, seed an admin user directly via migration `HasData` or a SQL script:

```sql
-- After migration: manually update a user's role to Admin
INSERT INTO user_roles (user_id, role_id)
SELECT id, 1 FROM users WHERE email = 'admin@test.com';
```

## Key Files Reference

| File | Purpose |
|------|---------|
| `Skyress/Extenstions/DependencyInjection.cs` | Add JWT bearer auth + auth/authz services |
| `Skyress.Infrastructure/Services/JwtTokenService.cs` | JWT generation logic |
| `Skyress.Infrastructure/Services/PasswordHasher.cs` | BCrypt wrapper |
| `Skyress.Infrastructure/Persistence/SkyressDbContext.cs` | Add User/Role/RefreshToken DbSets |
| `Skyress.Application/Auth/Commands/` | MediatR command handlers |
| `Skyress/Endpoints/Auth/AuthApiRegistration.cs` | Endpoint definitions |
| `specs/001-jwt-auth-refresh/contracts/auth-api.md` | Full API contract |
| `specs/001-jwt-auth-refresh/data-model.md` | Entity definitions |
