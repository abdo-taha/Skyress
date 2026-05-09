# Implementation Plan: Constitution Compliance Refactor

**Branch**: `002-constitution-refactor` | **Date**: 2026-05-09 | **Spec**: [spec.md](spec.md)
**Input**: Feature specification from `/specs/002-constitution-refactor/spec.md`

## Summary

Refactor the Skyress codebase to fully comply with the project constitution.
The existing architecture is Clean Architecture + CQRS with MediatR, but has nine
violations: wrong folder casing (`primitives`), a typo (`TagAssignmnet`), direct
aggregate construction instead of factory methods, handlers returning domain entities
instead of response DTOs, missing FluentValidation validators, missing logging in
handlers, missing `CancellationToken` in repository methods, missing
`ExceptionHandlingMiddleware`, and raw-string error responses in endpoints.

No new packages are required. No EF Core migrations are needed.

## Technical Context

**Language/Version**: C# 12 / .NET 8 (targeting net8.0; net9.0 build artifacts also present)
**Primary Dependencies**: ASP.NET Core Minimal API, MediatR 12, EF Core 8, FluentValidation,
Microsoft.Extensions.Logging, Npgsql
**Storage**: PostgreSQL via EF Core (no schema changes needed)
**Testing**: Not requested — zero new test files
**Target Platform**: Web service (Minimal API)
**Project Type**: Web service refactor — no new features, behavior-preserving changes only
**Performance Goals**: N/A (pure refactor)
**Constraints**: No new migrations; all existing HTTP routes and response schemas preserved;
Auth feature validators must not be duplicated
**Scale/Scope**: ~50 `.cs` files touched across 4 projects

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| Principle | Status | Notes |
|-----------|--------|-------|
| I. Folder Structure | ⚠️ Violations | `primitives` casing; `TagAssignmnet` typo; fixed by this refactor |
| II. Naming Conventions | ⚠️ Violations | `new Customer{...}` direct construction; handlers return raw entities |
| III. Code Patterns | ⚠️ Violations | No factory method on Customer; no ExceptionHandlingMiddleware |
| IV. Error Handling | ⚠️ Violations | Endpoints use `BadRequest<string>`; no middleware |
| V. Configuration | ✅ Compliant | JwtSettings uses IOptions<T>; all enums in use |
| VI. Logging | ⚠️ Violations | Handlers produce zero log output (only saga + background job log) |
| VII. Async/Await | ⚠️ Violations | IGenericRepository methods lack CancellationToken |
| VIII. Anti-Patterns | ⚠️ Violations | Direct construction; entities returned as results |
| IX. Spec Kit Integration | ✅ Compliant | Spec and plan in place |

All violations are addressed by the tasks below. No Complexity Tracking entry needed.

## Project Structure

### Documentation (this feature)

```text
specs/002-constitution-refactor/
├── plan.md              # This file
├── research.md          # Phase 0 — research findings
├── data-model.md        # Phase 1 — response DTO definitions
├── quickstart.md        # Phase 1 — validation guide
├── checklists/
│   └── requirements.md  # Spec quality checklist
└── tasks.md             # Phase 2 output (next step)
```

### Source Code (repository root)

```text
Skyress.Domain/
├── Aggregates/
│   ├── Auth/             (unchanged)
│   ├── Basket/           (unchanged)
│   ├── Customer/         Customer.cs — add Create() factory method
│   ├── Invoice/          (unchanged)
│   ├── Item/             (unchanged)
│   ├── Payment/          (unchanged)
│   ├── Tag/              (unchanged)
│   ├── TagAssignment/    ← rename from TagAssignmnet/ + namespace fix
│   └── Todo/             (unchanged)
├── Primitives/           ← namespace fix only (Windows: same folder, update .cs files)
│   ├── AggregateRoot.cs
│   ├── BaseEntity.cs
│   ├── IAuditable.cs
│   ├── IDomainEvent.cs
│   ├── ISoftDeletable.cs
│   └── ValueObject.cs
└── (rest unchanged)

Skyress.Application/
├── Contracts/Persistence/
│   └── IGenericRepository.cs  — add CancellationToken to 4 methods
├── Items/
│   ├── Responses/        ← NEW: ItemResponse.cs
│   ├── Commands/*/       — update handlers + add validators
│   └── Queries/*/        — update handlers
├── Customers/
│   ├── Responses/        ← NEW: CustomerResponse.cs
│   ├── Commands/*/       — update handlers + add validators
│   └── Queries/*/        — update handlers
├── Invoices/
│   ├── Responses/        ← NEW: InvoiceResponse.cs
│   ├── Commands/*/       — update handlers + add validators
│   └── Queries/*/        — update handlers
├── Payments/
│   ├── Responses/        ← NEW: PaymentResponse.cs
│   ├── Commands/*/       — update handlers + add validators
│   └── Queries/*/        — update handlers
├── Todos/
│   ├── Responses/        ← NEW: TodoResponse.cs
│   ├── Commands/*/       — update handlers + add validators
│   └── Queries/*/        — update handlers
├── Tags/
│   ├── Responses/        ← NEW: TagResponse.cs
│   ├── Commands/*/       — update handlers + add validators
│   └── Queries/*/        — update handlers
├── TagAssignments/
│   ├── Responses/        ← NEW: TagAssignmentResponse.cs
│   ├── Commands/*/       — update handlers + add validators
│   └── Queries/*/        — update handlers
└── Baskets/
    ├── Responses/        ← NEW: BasketResponse.cs
    ├── Commands/*/       — update handlers + add validators
    └── Queries/*/        — update handlers

Skyress.Infrastructure/
└── Repository/
    └── GenericRepository.cs  — add CancellationToken to 4 methods

Skyress/ (Presentation)
├── Middleware/           ← NEW: ExceptionHandlingMiddleware.cs
├── Endpoints/*/          — update error responses to ProblemDetails
└── Program.cs            — register middleware
```

**Structure Decision**: Hybrid multi-layer with feature-based subfolders, adding
`Responses/` directories inside each feature area of the Application project.

## Complexity Tracking

> No violations — all changes are constitution-compliant.

---

## Phase 0: Research Summary

All unknowns resolved. See `research.md` for full decision log.
Key findings:
- No new packages needed
- `primitives` → `Primitives` is a namespace-only change (Windows FS case-insensitive)
- `TagAssignmnet` → `TagAssignment` requires physical file move (git rename)
- All response DTOs defined in `data-model.md`

---

## Phase 1: Design Artifacts

### Response DTOs

All DTO definitions are in `data-model.md`. Summary:

| DTO | File |
|-----|------|
| `ItemResponse` | `Skyress.Application\Items\Responses\ItemResponse.cs` |
| `CustomerResponse` | `Skyress.Application\Customers\Responses\CustomerResponse.cs` |
| `InvoiceResponse` | `Skyress.Application\Invoices\Responses\InvoiceResponse.cs` |
| `PaymentResponse` | `Skyress.Application\Payments\Responses\PaymentResponse.cs` |
| `TodoResponse` | `Skyress.Application\Todos\Responses\TodoResponse.cs` |
| `TagResponse` | `Skyress.Application\Tags\Responses\TagResponse.cs` |
| `TagAssignmentResponse` | `Skyress.Application\TagAssignments\Responses\TagAssignmentResponse.cs` |
| `BasketResponse` | `Skyress.Application\Baskets\Responses\BasketResponse.cs` |

### API Contracts

This is a behavior-preserving refactor. All existing HTTP routes and success response
schemas remain unchanged. Error responses change from raw strings to `ProblemDetails`
(this is the only externally observable change, and it is an improvement).

No new endpoints are added.

### Validation Rules for New Validators

Each validator enforces non-empty required fields. Exact rules:

| Command | Rules |
|---------|-------|
| `CreateItemCommand` | Name NotEmpty; Price > 0; QuantityLeft >= 0 |
| `UpdateItemNameCommand` | Name NotEmpty |
| `UpdateItemDescriptionCommand` | (no validation needed — description is optional) |
| `UpdateItemPriceCommand` | Price > 0 or CostPrice > 0 (at least one provided) |
| `UpdateItemQrCodeCommand` | (no validation needed — qrcode is optional) |
| `UpdateItemQuantityLeftCommand` | QuantityLeft >= 0 |
| `UpdateItemUnitCommand` | Unit must be a valid enum value |
| `DeleteItemCommand` | Id > 0 |
| `CreateCustomerCommand` | Name NotEmpty; Notes NotEmpty |
| `UpdateCustomerStateCommand` | State must be valid enum value |
| `UpdateCustomerNotesCommand` | Notes NotEmpty |
| `DeleteCustomerCommand` | Id > 0 |
| `CreateInvoiceCommand` | BasketId > 0 |
| `UpdateInvoiceCustomerIdCommand` | CustomerId > 0 |
| `UpdateInvoiceStateCommand` | State must be valid enum value |
| `DeleteInvoiceCommand` | Id > 0 |
| `AddSoldItemToInvoiceCommand` | InvoiceId > 0; ItemId > 0; Quantity > 0 |
| `BuildInvoiceFromBasketCommand` | BasketId > 0 |
| `CreatePaymentCommand` | InvoiceId > 0; TotalDue > 0 |
| `CompleteCashPaymentCommand` | PaymentId > 0; AmountPaid > 0 |
| `CreateTodoCommand` | Context NotEmpty |
| `UpdateTodoStateCommand` | State must be valid enum value |
| `UpdateTodoContextCommand` | Context NotEmpty |
| `DeleteTodoCommand` | Id > 0 |
| `CreateTagCommand` | Name NotEmpty; Type valid enum value |
| `UpdateTagNameCommand` | Name NotEmpty |
| `UpdateTagTypeCommand` | Type valid enum value |
| `DeleteTagCommand` | Id > 0 |
| `CreateTagAssignmentCommand` | TagId > 0; ItemId > 0 |
| `DeleteTagAssignmentCommand` | Id > 0 |
| `CreateBasketCommand` | (no required fields — UserId optional) |
| `AddItemToBasketCommand` | BasketId > 0; ItemId > 0; Quantity > 0 |
| `RemoveItemFromBasketCommand` | BasketId > 0; ItemId > 0 |
| `ClearBasketCommand` | BasketId > 0 |
| `DeleteBasketCommand` | Id > 0 |
| `CancelBasketReservationCommand` | BasketId > 0 |
| `ReserveItemsCommand` | BasketId > 0 |
| `CompleteCheckoutCommand` | BasketId > 0 |

---

## Execution Notes for Haiku

> These notes exist because this plan will be executed by a less capable model.
> Follow each task exactly as written. Do not skip steps. Do not combine tasks.

### General Rules
1. Read each file before editing it — never guess at content.
2. For namespace changes: update BOTH the `namespace` declaration AND all `using` directives.
3. For handler updates: the command record and the handler class often live in the same file.
   Update the command's return type AND the handler's return type.
4. When adding logging: inject `ILogger<THandlerClass>` via primary constructor,
   and add `_logger.LogInformation(...)` as the **first line** of `Handle` and
   **immediately before the return** on the success path.
5. When adding a validator: create a NEW file `[CommandName]Validator.cs` in the same
   folder as the command file. Do not modify the command file.
6. For `ProblemDetails` imports: add `using Microsoft.AspNetCore.Mvc;` at the top of
   endpoint files that use it.
7. For response DTOs: create the file, then update the command/query file in the
   same step (both files are small and tightly coupled).

### Namespace Reference Table

| Old namespace | New namespace |
|---------------|---------------|
| `Skyress.Domain.primitives` | `Skyress.Domain.Primitives` |
| `Skyress.Domain.Aggregates.TagAssignmnet` | `Skyress.Domain.Aggregates.TagAssignment` |

Files using `Skyress.Domain.primitives` (must all be updated):
- `Skyress.Domain\Aggregates\Customer\Customer.cs`
- `Skyress.Domain\Aggregates\Invoice\Invoice.cs`
- `Skyress.Domain\Aggregates\Item\Item.cs`
- `Skyress.Domain\Aggregates\Payment\Payment.cs`
- `Skyress.Domain\Aggregates\Todo\Todo.cs`
- `Skyress.Domain\Aggregates\Basket\Basket.cs`
- `Skyress.Domain\Aggregates\Tag\Tag.cs`
- `Skyress.Domain\Aggregates\TagAssignmnet\TagAssignment.cs`
- `Skyress.Domain\Aggregates\Item\PricingHistory.cs`
- `Skyress.Domain\Aggregates\Item\Events\ItemPriceChangedDomainEvent.cs`
- `Skyress.Domain\Aggregates\Basket\BasketItem.cs`
- `Skyress.Domain\Aggregates\Invoice\SoldItem.cs`
- `Skyress.Domain\Aggregates\Payment\Installment.cs`
- `Skyress.Domain\Aggregates\Auth\User.cs`
- `Skyress.Domain\Aggregates\Auth\RefreshToken.cs`
- `Skyress.Domain\Aggregates\Auth\Role.cs`
- `Skyress.Domain\Aggregates\Auth\UserRole.cs`
- `Skyress.Domain\Aggregates\TagAssignmnet\TagAssignment.cs`
- `Skyress.Infrastructure\Repository\GenericRepository.cs`
- `Skyress.Infrastructure\Persistence\SkyressDbContext.cs`
- `Skyress.Application\Contracts\Persistence\IGenericRepository.cs`

Files using `Skyress.Domain.Aggregates.TagAssignmnet` (must all be updated):
- `Skyress.Domain\Aggregates\TagAssignmnet\TagAssignment.cs` (the domain file itself)
- `Skyress.Infrastructure\Persistence\SkyressDbContext.cs`
- `Skyress.Infrastructure\Repository\TagAssignmentRepository.cs`
- `Skyress.Application\Contracts\Persistence\ITagAssignmentRepository.cs`
- `Skyress.Application\TagAssignments\Commands\CreateTagAssignment\CreateTagAssignmentCommand.cs`
- `Skyress.Application\TagAssignments\Commands\DeleteTagAssignment\DeleteTagAssignmentCommand.cs`
- `Skyress.Application\TagAssignments\Queries\GetTagAssignmentsByItem\GetTagAssignmentsByItemQuery.cs`
- `Skyress.Application\TagAssignments\Queries\GetTagAssignmentsByTag\GetTagAssignmentsByTagQuery.cs`
- `Skyress.Application\Checkout\Sagas\*` (check if any reference TagAssignment)
