# Feature Specification: JWT Authentication & Authorization with Refresh Tokens

**Feature Branch**: `001-jwt-auth-refresh`
**Created**: 2026-05-02
**Status**: Draft
**Input**: User description: "add authentication and authorization to the project, use jwt and add refresh tokens"

## User Scenarios & Testing *(mandatory)*

### User Story 1 - User Registration & Login (Priority: P1)

A new user visits the shop, creates an account with their email and password, and logs in to receive a session that allows them to access protected features such as their basket, orders, and invoices.

**Why this priority**: Without login, no user can access any personalized or transactional feature. This is the entry point to the entire authenticated experience.

**Independent Test**: Can be tested by submitting registration credentials, receiving tokens, and verifying that a protected endpoint (e.g., view basket) succeeds with those tokens.

**Acceptance Scenarios**:

1. **Given** a visitor with a valid email and password, **When** they register and log in, **Then** they receive a short-lived access token and a long-lived refresh token.
2. **Given** a user with incorrect credentials, **When** they attempt to log in, **Then** they receive a clear error and no tokens are issued.
3. **Given** a user attempting to register with an already-used email, **When** they submit the form, **Then** they are notified the email is taken and no account is created.

---

### User Story 2 - Accessing Protected Resources (Priority: P2)

A logged-in user can access protected parts of the system (basket, invoices, payments, customers) using their access token. Unauthenticated requests are rejected.

**Why this priority**: The core value of authentication is protecting resources. Without this, the token system has no practical effect.

**Independent Test**: Can be tested by calling a protected endpoint with a valid token (succeeds) and without a token (returns 401 Unauthorized).

**Acceptance Scenarios**:

1. **Given** a user with a valid access token, **When** they request a protected resource, **Then** the system returns the resource.
2. **Given** a request with no token, **When** it targets a protected endpoint, **Then** the system returns 401 Unauthorized.
3. **Given** a request with an expired or tampered token, **When** it targets a protected endpoint, **Then** the system returns 401 Unauthorized.

---

### User Story 3 - Silent Session Renewal via Refresh Token (Priority: P3)

When a user's short-lived access token expires, the client automatically exchanges their refresh token for a new access token, keeping the session alive without requiring the user to log in again.

**Why this priority**: Without this, users would be forced to re-authenticate every 15 minutes, creating a very poor experience.

**Independent Test**: Can be tested by letting an access token expire, then calling the refresh endpoint with a valid refresh token and verifying a new valid access token is returned.

**Acceptance Scenarios**:

1. **Given** a user with an expired access token and a valid refresh token, **When** they call the refresh endpoint, **Then** they receive a new access token and a new refresh token (rotation).
2. **Given** a user attempting to reuse an already-consumed refresh token, **When** they call the refresh endpoint, **Then** the system rejects the request and revokes the entire token family (replay protection).
3. **Given** a user with an expired refresh token, **When** they call the refresh endpoint, **Then** the system returns an error and the user must log in again.

---

### User Story 4 - Role-Based Access Control (Priority: P4)

Different user types (Admin and Customer) have access to different parts of the system. Admins can manage all data; Customers can only access their own records.

**Why this priority**: Without role separation, any authenticated user could access or modify data belonging to others, posing a security and business risk.

**Independent Test**: Can be tested by logging in as a Customer and attempting to access an Admin-only endpoint (should be rejected), then doing the same as an Admin (should succeed).

**Acceptance Scenarios**:

1. **Given** a Customer user, **When** they attempt to access an Admin-only endpoint, **Then** the system returns 403 Forbidden.
2. **Given** an Admin user, **When** they access any endpoint, **Then** the system grants access.
3. **Given** a Customer user, **When** they access their own orders or basket, **Then** the system returns only their data.

---

### User Story 5 - Logout & Session Invalidation (Priority: P5)

A user can explicitly log out, which immediately invalidates their refresh token so that it cannot be used to generate new access tokens.

**Why this priority**: Logout is a basic security expectation; without revocation, stolen refresh tokens remain usable until they naturally expire.

**Independent Test**: Can be tested by logging out, then attempting to use the previously valid refresh token — it should be rejected.

**Acceptance Scenarios**:

1. **Given** a logged-in user, **When** they log out, **Then** their refresh token is invalidated and can no longer be used.
2. **Given** a user who has logged out, **When** they try to refresh using the old token, **Then** the system returns an error.

---

### Edge Cases

- What happens when a refresh token is used after the user's account is deactivated?
- How does the system handle the same user logged in from multiple devices simultaneously?
- What happens if a refresh request is made with a token that belongs to a different user?
- How does the system behave under concurrent refresh requests using the same token?

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: System MUST allow new users to register with an email address and password.
- **FR-002**: System MUST validate credentials and issue a short-lived access token and a long-lived refresh token upon successful login.
- **FR-003**: System MUST protect all non-public endpoints so that only authenticated users with a valid access token can access them.
- **FR-004**: System MUST allow clients to exchange a valid refresh token for a new access token and a new refresh token.
- **FR-005**: System MUST rotate refresh tokens on every use — each exchange invalidates the old token and issues a new one.
- **FR-006**: System MUST detect refresh token reuse and revoke the entire token family when a previously used token is presented (replay attack protection).
- **FR-007**: System MUST invalidate a user's refresh token upon explicit logout.
- **FR-008**: System MUST enforce role-based access, distinguishing at minimum between Admin and Customer roles.
- **FR-009**: System MUST reject requests bearing expired, invalid, or tampered tokens with a 401 Unauthorized response.
- **FR-010**: System MUST reject role-insufficient requests with a 403 Forbidden response.
- **FR-011**: Customers MUST only be able to view and modify their own data (basket, orders, invoices).
- **FR-012**: System MUST store refresh tokens server-side to enable revocation.

### Key Entities

- **User**: A registered account identified by email; holds credentials, assigned roles, and account status.
- **Role**: A permission level assigned to a user (Admin, Customer) that determines access rights.
- **Access Token**: A short-lived, self-contained credential presented on every request to a protected resource.
- **Refresh Token**: A long-lived server-stored credential used exclusively to obtain new access tokens; supports revocation.
- **Token Family**: A chain of related refresh tokens from a single login session; used to detect and respond to replay attacks.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Users can complete registration and first login in under 60 seconds.
- **SC-002**: 100% of protected endpoints reject requests that lack a valid access token.
- **SC-003**: Session renewal (token refresh) completes in under 2 seconds without user intervention.
- **SC-004**: A revoked or logged-out session is rendered unusable within 1 second of the logout action.
- **SC-005**: Replay of a previously used refresh token results in revocation of the full token family within 1 second.
- **SC-006**: Role enforcement is applied consistently — 0% of unauthorized role-based accesses succeed.
- **SC-007**: Authenticated sessions remain active for up to 7 days without requiring re-login, given the user is actively refreshing tokens.

## Assumptions

- Users are uniquely identified by their email address.
- JWT (JSON Web Token) is the chosen format for access tokens, as explicitly requested.
- Refresh tokens are opaque, randomly generated values stored server-side (not JWT) to enable revocation.
- Initial roles are limited to two: **Admin** and **Customer**.
- Access tokens expire after 15 minutes; refresh tokens expire after 7 days (industry standard defaults).
- Email verification on registration is out of scope for v1.
- Social login (OAuth2/Google/etc.) is out of scope for v1.
- Password reset / forgot-password flow is out of scope for v1.
- All existing API endpoints (basket, invoices, payments, customers, items) will be protected and require authentication.
- Public endpoints (registration, login, token refresh) remain unauthenticated.
- A single user may be logged in from multiple devices simultaneously.
- The application is single-tenant (one organization, no multi-tenancy).
