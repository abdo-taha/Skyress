# API Contracts: Authentication & Authorization

**Branch**: `001-jwt-auth-refresh` | **Date**: 2026-05-02
**Base URL**: `/api/v1/auth`
**Auth**: All endpoints in this group are `[AllowAnonymous]`

---

## POST /api/v1/auth/register

**Description**: Create a new user account. Assigns the `Customer` role automatically.

### Request

```json
{
  "email": "user@example.com",
  "password": "SecurePass123!"
}
```

| Field | Type | Rules |
|-------|------|-------|
| email | string | Required, valid email format, max 256 chars |
| password | string | Required, min 8 chars, max 100 chars |

### Response — 201 Created

```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "base64opaquestring...",
  "expiresIn": 900,
  "tokenType": "Bearer"
}
```

| Field | Type | Description |
|-------|------|-------------|
| accessToken | string | JWT, valid for `expiresIn` seconds |
| refreshToken | string | Opaque token, valid for 7 days |
| expiresIn | int | Access token TTL in seconds (900 = 15 min) |
| tokenType | string | Always `"Bearer"` |

### Error Responses

| Status | Code | Condition |
|--------|------|-----------|
| 400 | `VALIDATION_ERROR` | Missing/invalid fields |
| 409 | `EMAIL_ALREADY_EXISTS` | Email already registered |

---

## POST /api/v1/auth/login

**Description**: Authenticate with email and password. Returns access + refresh tokens.

### Request

```json
{
  "email": "user@example.com",
  "password": "SecurePass123!"
}
```

| Field | Type | Rules |
|-------|------|-------|
| email | string | Required |
| password | string | Required |

### Response — 200 OK

```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "base64opaquestring...",
  "expiresIn": 900,
  "tokenType": "Bearer"
}
```

### Error Responses

| Status | Code | Condition |
|--------|------|-----------|
| 400 | `VALIDATION_ERROR` | Missing fields |
| 401 | `INVALID_CREDENTIALS` | Email not found or password mismatch |
| 403 | `ACCOUNT_INACTIVE` | Account has been deactivated |

---

## POST /api/v1/auth/refresh

**Description**: Exchange a valid refresh token for a new access token and rotated refresh token. The old refresh token is immediately invalidated.

### Request

```json
{
  "refreshToken": "base64opaquestring..."
}
```

| Field | Type | Rules |
|-------|------|-------|
| refreshToken | string | Required |

### Response — 200 OK

```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "newbase64opaquestring...",
  "expiresIn": 900,
  "tokenType": "Bearer"
}
```

### Error Responses

| Status | Code | Condition |
|--------|------|-----------|
| 400 | `VALIDATION_ERROR` | Missing token field |
| 401 | `TOKEN_NOT_FOUND` | Token does not exist in DB |
| 401 | `TOKEN_EXPIRED` | Refresh token TTL has passed |
| 401 | `TOKEN_REVOKED` | Token was revoked (logout or replay) |
| 401 | `TOKEN_ALREADY_USED` | Token reuse detected → full family revoked |
| 403 | `ACCOUNT_INACTIVE` | Associated user account is inactive |

---

## POST /api/v1/auth/logout

**Description**: Invalidate the provided refresh token. The client must discard the access token; it will expire naturally within 15 minutes.

### Request

```json
{
  "refreshToken": "base64opaquestring..."
}
```

| Field | Type | Rules |
|-------|------|-------|
| refreshToken | string | Required |

### Response — 204 No Content

Empty body.

### Error Responses

| Status | Code | Condition |
|--------|------|-----------|
| 400 | `VALIDATION_ERROR` | Missing token field |

> **Note**: Logout always returns 204 even if the token is already revoked or not found, to avoid leaking information about token validity.

---

## JWT Access Token Claims

| Claim | Value | Description |
|-------|-------|-------------|
| `sub` | `"12345"` | User ID (string representation of long) |
| `email` | `"user@example.com"` | User email |
| `role` | `["Customer"]` | Array of role names |
| `jti` | `"uuid"` | Unique token ID |
| `iat` | Unix timestamp | Issued-at |
| `exp` | Unix timestamp | Expiry (iat + 900s) |
| `iss` | `"Skyress"` | Issuer |
| `aud` | `"SkyressApi"` | Audience |

---

## Authorization Rules for Existing Endpoints

All existing endpoints require authentication (`[Authorize]` via fallback policy).

| Endpoint Group | Required Role | Notes |
|---------------|--------------|-------|
| `GET /api/v1/items` | Any authenticated | Public catalog read |
| `POST /api/v1/items` | Admin | Create items |
| `PUT/DELETE /api/v1/items/{id}` | Admin | Modify items |
| `GET /api/v1/customers` | Admin | Full customer list |
| `GET /api/v1/customers/{id}` | Admin or own ID | Customer can view self |
| `POST /api/v1/customers` | Admin | Create customer records |
| `GET /api/v1/baskets/{customerId}` | Admin or own customerId | Data isolation |
| `POST /api/v1/baskets` | Customer, Admin | Add to basket |
| `GET /api/v1/invoices` | Admin | All invoices |
| `GET /api/v1/invoices/{id}` | Admin or own | Customer sees own invoices |
| `GET /api/v1/payments` | Admin | All payments |
| `GET /api/v1/tags` | Any authenticated | Read tags |
| `POST/PUT/DELETE /api/v1/tags` | Admin | Manage tags |

> **Note**: Customer data isolation (FR-011) is enforced at the query-handler level by comparing the requesting user's `sub` claim against the resource owner ID. This is not a contract concern — handlers resolve this internally.
