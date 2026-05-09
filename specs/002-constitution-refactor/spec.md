# Feature Specification: Constitution Compliance Refactor

**Feature Branch**: `002-constitution-refactor`
**Created**: 2026-05-09
**Status**: Draft
**Input**: User description: "refactor the code base to follow the constitution"

## User Scenarios & Testing *(mandatory)*

### User Story 1 — Domain Layer Compliance (Priority: P1)

As a developer maintaining Skyress, I need domain aggregates and the project structure
to strictly follow the Skyress Constitution so that the domain layer is clean,
predictable, and free of infrastructure leakage.

**Why this priority**: The domain layer is the foundation of the architecture; violations
here cascade into every other layer. Fixing domain issues first unblocks all other stories.

**Independent Test**: Domain project compiles with zero direct construction of aggregates
outside factory methods; all folder names follow PascalCase; no infrastructure references
exist in `Skyress.Domain`.

**Acceptance Scenarios**:

1. **Given** `CreateCustomerCommandHandler` constructs `Customer` with `new Customer { ... }`,
   **When** the refactor is applied,
   **Then** `Customer.Create(...)` factory method is used instead, and the handler no longer
   directly sets properties.

2. **Given** the `Skyress.Domain\primitives\` folder uses lowercase naming,
   **When** the refactor is applied,
   **Then** the folder is renamed to `Skyress.Domain\Primitives\` and all namespace
   references are updated throughout the codebase.

3. **Given** the `Skyress.Domain\Aggregates\TagAssignmnet\` folder has a typo,
   **When** the refactor is applied,
   **Then** the folder is renamed to `Skyress.Domain\Aggregates\TagAssignment\`.

---

### User Story 2 — Application Layer Compliance (Priority: P1)

As a developer consuming CQRS handlers, I need all command and query handlers to return
typed response DTOs (not raw domain entities), include structured logging, and have
FluentValidation validators for every command, so that the Application layer is
independently testable and doesn't leak domain internals to consumers.

**Why this priority**: Handlers returning domain entities tightly couples consumers to
the domain model and prevents independent evolution. Logging and validation are baseline
operational requirements.

**Independent Test**: Every handler returns a response DTO or a primitive (not a domain
aggregate). Every command has a corresponding `*Validator` file. Every handler logs entry
and success. Handlers compile without `CancellationToken` warnings.

**Acceptance Scenarios**:

1. **Given** `CreateItemCommandHandler` returns `Result<Item>` (a domain entity),
   **When** the refactor is applied,
   **Then** `CreateItemCommand` returns `Result<ItemResponse>` and `ItemResponse` is a
   new DTO record containing the fields consumers need.

2. **Given** no FluentValidation validators exist for Items, Customers, Invoices,
   Payments, Todos, Tags, Baskets, or TagAssignments commands,
   **When** the refactor is applied,
   **Then** every command class has a corresponding `*Validator : AbstractValidator<*Command>`
   file in the same feature folder.

3. **Given** no handlers produce log output except Auth, Checkout, and background jobs,
   **When** the refactor is applied,
   **Then** every handler logs entry with relevant context at `LogInformation` level and
   logs success with the resulting resource identifier.

4. **Given** `IGenericRepository<T>` methods (`GetAllAsync`, `GetByIdAsync`) lack
   `CancellationToken` parameters,
   **When** the refactor is applied,
   **Then** all repository interface methods and their implementations accept
   `CancellationToken cancellationToken = default` as their final parameter.

---

### User Story 3 — Presentation Layer Compliance (Priority: P2)

As an API consumer of Skyress, I need all endpoints to return structured
`ProblemDetails` error responses and have a global exception handling middleware,
so that errors are consistent, safe, and never leak internal stack traces.

**Why this priority**: Presentation compliance is externally visible and security-relevant;
lower priority than Application compliance only because it depends on response DTOs from
Story 2 being in place first.

**Independent Test**: All endpoints return `ProblemDetails` for error cases. An
`ExceptionHandlingMiddleware` is registered in the middleware pipeline. No endpoint
returns a raw `string` or domain entity on error. A deliberate unhandled exception
results in a `500` with a `ProblemDetails` body.

**Acceptance Scenarios**:

1. **Given** `CreateItemEndpoint` returns `BadRequest<string>` on failure,
   **When** the refactor is applied,
   **Then** the endpoint returns `UnprocessableEntity<ProblemDetails>` with a structured
   `ProblemDetails` body containing `Title` and `Detail`.

2. **Given** no global exception handling middleware exists in the pipeline,
   **When** the refactor is applied,
   **Then** `ExceptionHandlingMiddleware` is registered before routing, maps
   `DomainException` to HTTP 422, `NotFoundException` to HTTP 404, and all other
   exceptions to HTTP 500 — all with `ProblemDetails` bodies.

3. **Given** multiple endpoints across Items, Customers, Invoices, Payments, Todos,
   Tags, Baskets use inconsistent error response shapes,
   **When** the refactor is applied,
   **Then** all endpoints use the same `ProblemDetails`-based error shape.

---

### Edge Cases

- Tag Assignment folder typo rename must update all `namespace` declarations and
  `using` directives across Application, Infrastructure, and Presentation projects.
- `CancellationToken` addition to generic repository must not break the existing
  `DeleteByIdAsync(long id)` method (which internally calls `GetByIdAsync`).
- Response DTOs for query handlers that return lists (e.g., `GetAllItems`) must use
  `IReadOnlyList<ItemResponse>` rather than raw `IReadOnlyList<Item>`.
- Auth commands already have validators; those MUST NOT be duplicated.
- Saga and background job logging already exists; those MUST NOT be modified.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: All command handlers MUST use aggregate factory methods to construct
  domain entities; direct `new Aggregate { ... }` construction is FORBIDDEN.
- **FR-002**: All command and query handlers MUST return response DTOs, not domain
  entities (aggregates or entities).
- **FR-003**: Every command class MUST have a corresponding FluentValidation
  `AbstractValidator<TCommand>` in the same feature folder, unless one already exists.
- **FR-004**: Every handler MUST log entry (`LogInformation`) with relevant IDs and
  log success (`LogInformation`) with the resulting resource identifier.
- **FR-005**: All repository interface methods MUST accept `CancellationToken` as
  their final parameter.
- **FR-006**: An `ExceptionHandlingMiddleware` MUST be added to the Presentation
  project and registered in the pipeline; it MUST map `DomainException` → 422,
  `NotFoundException` → 404, and unhandled → 500 using `ProblemDetails`.
- **FR-007**: All endpoint error responses MUST use `ProblemDetails`; raw `string`
  error returns are FORBIDDEN.
- **FR-008**: The `Skyress.Domain\primitives\` folder MUST be renamed to
  `Skyress.Domain\Primitives\` with namespace updates throughout.
- **FR-009**: The `TagAssignmnet` typo in folder and namespace MUST be corrected
  to `TagAssignment`.

### Key Entities

- **Response DTOs**: New records/classes (`ItemResponse`, `CustomerResponse`,
  `InvoiceResponse`, etc.) that represent the data returned to API consumers;
  contain no domain logic.
- **Validators**: FluentValidation `AbstractValidator<TCommand>` classes, one per
  command, placed in the same folder as the command file.
- **ExceptionHandlingMiddleware**: ASP.NET Core middleware in the Presentation project
  that centralizes error-to-HTTP mapping.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Zero handlers return a domain aggregate or entity type directly —
  all return a response DTO or a primitive.
- **SC-002**: Zero commands lack a corresponding `AbstractValidator<TCommand>` class
  (excluding already-compliant Auth commands).
- **SC-003**: Every handler produces at least two log lines per successful execution
  (entry + success).
- **SC-004**: All repository interface methods accept `CancellationToken`.
- **SC-005**: A deliberate unhandled exception in any endpoint returns HTTP 500 with
  a `ProblemDetails` JSON body — no stack trace exposed.
- **SC-006**: The solution compiles with zero errors after folder renames and namespace
  updates.
- **SC-007**: All existing endpoint behaviors (HTTP verb, route, happy-path response)
  remain unchanged after the refactor.

## Assumptions

- The `Customer` aggregate will receive a `Create` factory method analogous to
  `Item.Create(...)`.
- The Presentation project (`Skyress/`) will remain named `Skyress` at the filesystem
  level to avoid a full project rename; the `Skyress.Presentation` naming from the
  constitution applies to logical project identity, not the physical folder/assembly name.
- The `IGenericRepository<T>` will be updated with `CancellationToken` parameters;
  callers that omit the token may use `CancellationToken.None` as default.
- Auth command validators already satisfy FR-003; no new validators are needed for
  `LoginCommand`, `RegisterCommand`, `LogoutCommand`, or `RefreshTokenCommand`.
- Saga consumers and background jobs are out of scope for logging changes (already
  compliant or architecturally separate).
- No new EF Core migrations are needed — this refactor is purely code-level.
