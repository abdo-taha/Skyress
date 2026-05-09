# Research: Constitution Compliance Refactor

**Branch**: `002-constitution-refactor` | **Date**: 2026-05-09

## Decision Log

### Decision 1: Response DTO Placement
- **Decision**: Place response DTOs in `Skyress.Application\[Feature]\Responses\`
  alongside the existing `Commands\` and `Queries\` folders.
- **Rationale**: Keeps all application-layer contracts in the Application project.
  DTOs in the Domain project would couple the domain to API shape; DTOs in the
  Presentation project would make the Application layer depend on Presentation.
- **Alternatives considered**: Shared `Skyress.Contracts` project (too heavy for a
  single-team app); in the Presentation `DTOs/` folder (breaks layering).

### Decision 2: Folder Rename Strategy for `primitives` → `Primitives`
- **Decision**: Rename only the namespace declaration in each file and the containing
  folder. Do NOT rename the physical folder on disk via file-system commands — instead
  rename namespace strings in-place and move files by editing them.
- **Rationale**: On Windows/NTFS, folder rename is case-insensitive; `primitives` and
  `Primitives` are the same folder. The fix is to update the `namespace` keyword in
  every affected `.cs` file from `Skyress.Domain.primitives` to
  `Skyress.Domain.Primitives`, and update all `using Skyress.Domain.primitives`
  directives to `using Skyress.Domain.Primitives`.
- **Alternatives considered**: Physical folder move (requires git mv; risky in Windows).

### Decision 3: TagAssignmnet Typo Fix
- **Decision**: Rename the `TagAssignmnet` folder to `TagAssignment` on disk AND update
  all namespace/using references. This is a real typo that git can track as a file move.
- **Rationale**: Unlike the PascalCase fix above, this is a genuine spelling error that
  changes the logical name. It's visible in public namespaces.
- **Files affected**:
  - `Skyress.Domain\Aggregates\TagAssignmnet\TagAssignment.cs`
  - `Skyress.Infrastructure\Persistence\SkyressDbContext.cs` (using directive)
  - `Skyress.Infrastructure\Repository\TagAssignmentRepository.cs` (using directive)
  - `Skyress.Application\Contracts\Persistence\ITagAssignmentRepository.cs`
  - `Skyress.Application\TagAssignments\Commands\CreateTagAssignment\CreateTagAssignmentCommand.cs`
  - `Skyress.Application\TagAssignments\Commands\DeleteTagAssignment\DeleteTagAssignmentCommand.cs`
  - `Skyress.Application\TagAssignments\Queries\*` files

### Decision 4: `Customer.Create` Factory Method
- **Decision**: Add a static `Create(string name, string notes, CustomerState state)`
  factory method to `Customer`, matching the pattern used in `Item.Create`.
- **Rationale**: Direct `new Customer { ... }` construction bypasses the factory
  method pattern mandated by Principle III.
- **Impact**: `CreateCustomerCommandHandler` must be updated to use `Customer.Create`.

### Decision 5: CancellationToken in Repository Interface
- **Decision**: Add `CancellationToken cancellationToken = default` as the final
  parameter to `GetAllAsync`, `GetByIdAsync`, `DeleteByIdAsync`, and `CreateAsync` in
  `IGenericRepository<T>`. Update `GenericRepository<T>` accordingly.
- **Rationale**: Principle VII mandates `CancellationToken` in all async methods.
- **Callers**: All handlers that call these methods must forward their `cancellationToken`.

### Decision 6: Logging Pattern
- **Decision**: Inject `ILogger<THandler>` via primary constructor and add two log
  calls per handler: one at entry, one on success.
- **Pattern**:
  ```csharp
  _logger.LogInformation("Handling {Command} for {Key}", nameof(TCommand), keyValue);
  // ... handler logic ...
  _logger.LogInformation("{Command} succeeded. Result: {Id}", nameof(TCommand), result.Id);
  ```
- **Rationale**: Matches Principle VI exactly.

### Decision 7: ExceptionHandlingMiddleware
- **Decision**: Create `Skyress\Middleware\ExceptionHandlingMiddleware.cs` in the
  Presentation project. Register it in `Program.cs` before `app.UseRouting()`.
- **Rationale**: Principle III and Principle IV mandate centralized exception mapping.
- **Mappings**: `DomainException` → 422, `NotFoundException` → 404, all others → 500,
  all using `ProblemDetails` bodies.

### Decision 8: Endpoint Error Shape
- **Decision**: Replace all `BadRequest<string>` and similar raw-string error returns
  with `UnprocessableEntity<ProblemDetails>` or the appropriate typed result.
- **Rationale**: Principle VIII anti-pattern #4 explicitly forbids raw string errors.
- **Scope**: Only endpoint files in `Skyress\Endpoints\` — not saga consumers or
  background jobs.

### Decision 9: Scope of Handlers Requiring DTOs
- **Decision**: Only handlers that currently return a domain `AggregateRoot` subtype
  need updating. Handlers returning `Result` (void) or `Result<IReadOnlyList<T>>`
  where T is already a non-aggregate DTO are lower priority.
- **Handlers returning domain entities (must change)**:
  - All Create* command handlers (return the created entity)
  - GetById* query handlers (return a single entity)
  - GetAll* query handlers (return lists of entities)
- **Approach**: Create one `*Response` record per aggregate in
  `Skyress.Application\[Feature]\Responses\[Feature]Response.cs`.

## Technical Context Summary

- **Framework**: ASP.NET Core Minimal API (.NET 8/9)
- **ORM**: EF Core 8 + PostgreSQL (Npgsql)
- **CQRS**: MediatR
- **Validation**: FluentValidation (already installed for Auth)
- **Logging**: Microsoft.Extensions.Logging (ILogger)
- **No new packages required** — all dependencies already present
