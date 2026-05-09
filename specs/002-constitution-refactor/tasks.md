---
description: "Task list for Constitution Compliance Refactor"
---

# Tasks: Constitution Compliance Refactor

**Input**: Design documents from `/specs/002-constitution-refactor/`
**Prerequisites**: plan.md ✅, spec.md ✅, data-model.md ✅, research.md ✅

**Tests**: Not requested — zero new test files.

**Organization**: Tasks grouped by user story (US1, US2, US3) to enable independent
implementation and checkpoint validation.

## Format: `[ID] [P?] [Story?] Description — exact file`

- **[P]**: Can run in parallel (different files, no shared dependencies)
- **[Story]**: US1/US2/US3 — matches spec user stories
- All tasks include **exact file paths** and **exact code to write/change**

---

## ⚠️ INSTRUCTIONS FOR HAIKU

Before each task: **READ the file first** with the Read tool, then **edit** it.
Never guess at existing content. If a file doesn't exist, **create** it with the Write tool.
After each task: move on — do NOT re-read the file to verify.

**Namespace change rule**: When a task says "fix `using` directive", find the line
`using Skyress.Domain.primitives;` and change it to `using Skyress.Domain.Primitives;`.
If the file also has `namespace Skyress.Domain.primitives;`, change it too.

**Logging pattern** (use exactly this in every handler Handle method):
```csharp
// Entry log — FIRST LINE of Handle()
_logger.LogInformation("Handling {Command}", nameof(XxxCommand));
// Success log — BEFORE the return on success path
_logger.LogInformation("{Command} completed. Id: {Id}", nameof(XxxCommand), result.Id);
```
For void/non-ID results use: `_logger.LogInformation("{Command} completed", nameof(XxxCommand));`

**Validator pattern** (use exactly this for every new validator file):
```csharp
namespace Skyress.Application.[Feature].Commands.[Folder];
using FluentValidation;
public sealed class [Command]Validator : AbstractValidator<[Command]>
{
    public [Command]Validator()
    {
        // rules here
    }
}
```

---

## Phase 1: Setup

**Purpose**: Confirm the solution builds before making any changes.

- [X] T001 Run `dotnet build skyress.sln` from repo root and confirm zero errors before starting

---

## Phase 2: Foundational — Response DTOs + GenericRepository

**Purpose**: Create the 8 response DTOs and update `IGenericRepository<T>`. These are
blocking prerequisites — every user story handler update depends on these DTOs existing.

**⚠️ CRITICAL**: No user story work until T002–T011 are complete.

- [X] T002 [P] Create new file `Skyress.Application\Items\Responses\ItemResponse.cs` with this exact content:

```csharp
namespace Skyress.Application.Items.Responses;

using Skyress.Domain.Aggregates.Item;
using Skyress.Domain.Enums;

public sealed record ItemResponse(
    long Id,
    string Name,
    string? Description,
    decimal Price,
    decimal? CostPrice,
    int QuantityLeft,
    int QuantityReserved,
    int QuantitySold,
    string? QrCode,
    Unit Unit,
    bool IsDeleted,
    DateTime CreatedAt)
{
    public static ItemResponse FromDomain(Item item) => new(
        item.Id,
        item.Name,
        item.Description,
        item.Price,
        item.CostPrice,
        item.QuantityLeft,
        item.QuantityReserved,
        item.QuantitySold,
        item.QrCode,
        item.Unit,
        item.IsDeleted,
        item.CreatedAt);
}
```

- [X] T003 [P] Create new file `Skyress.Application\Customers\Responses\CustomerResponse.cs` with this exact content:

```csharp
namespace Skyress.Application.Customers.Responses;

using Skyress.Domain.Aggregates.Customer;
using Skyress.Domain.Enums;

public sealed record CustomerResponse(
    long Id,
    string Name,
    string Notes,
    CustomerState State,
    DateTime CreatedAt)
{
    public static CustomerResponse FromDomain(Customer customer) => new(
        customer.Id,
        customer.Name,
        customer.Notes,
        customer.State,
        customer.CreatedAt);
}
```

- [X] T004 [P] Create new file `Skyress.Application\Invoices\Responses\InvoiceResponse.cs` with this exact content:

```csharp
namespace Skyress.Application.Invoices.Responses;

using Skyress.Domain.Aggregates.Invoice;
using Skyress.Domain.Enums;

public sealed record InvoiceResponse(
    long Id,
    decimal TotalAmount,
    long BasketId,
    long? CustomerId,
    long? PaymentId,
    InvoiceState State,
    DateTime CreatedAt)
{
    public static InvoiceResponse FromDomain(Invoice invoice) => new(
        invoice.Id,
        invoice.TotalAmount,
        invoice.BasketId,
        invoice.CustomerId,
        invoice.PaymentId,
        invoice.State,
        invoice.CreatedAt);
}
```

- [X] T005 [P] Create new file `Skyress.Application\Payments\Responses\PaymentResponse.cs` with this exact content:

```csharp
namespace Skyress.Application.Payments.Responses;

using Skyress.Domain.Aggregates.Payment;
using Skyress.Domain.Enums;

public sealed record PaymentResponse(
    long Id,
    PaymentType PaymentType,
    PaymentState PaymentState,
    decimal TotalPaid,
    decimal TotalDue,
    long InvoiceId,
    DateTime CreatedAt)
{
    public static PaymentResponse FromDomain(Payment payment) => new(
        payment.Id,
        payment.PaymentType,
        payment.PaymentState,
        payment.TotalPaid,
        payment.TotalDue,
        payment.InvoiceId,
        payment.CreatedAt);
}
```

- [X] T006 [P] Create new file `Skyress.Application\Todos\Responses\TodoResponse.cs` with this exact content:

```csharp
namespace Skyress.Application.Todos.Responses;

using Skyress.Domain.Aggregates.Todo;
using Skyress.Domain.Enums;

public sealed record TodoResponse(
    long Id,
    string Context,
    TodoState State,
    DateTime CreatedAt)
{
    public static TodoResponse FromDomain(Todo todo) => new(
        todo.Id,
        todo.context,
        todo.State,
        todo.CreatedAt);
}
```

- [X] T007 [P] Create new file `Skyress.Application\Tags\Responses\TagResponse.cs` with this exact content:

```csharp
namespace Skyress.Application.Tags.Responses;

using Skyress.Domain.Aggregates.Tag;
using Skyress.Domain.Enums;

public sealed record TagResponse(
    long Id,
    string Name,
    TagType Type)
{
    public static TagResponse FromDomain(Tag tag) => new(
        tag.Id,
        tag.Name,
        tag.Type);
}
```

- [X] T008 [P] Create new file `Skyress.Application\TagAssignments\Responses\TagAssignmentResponse.cs` with this exact content:

```csharp
namespace Skyress.Application.TagAssignments.Responses;

using Skyress.Domain.Aggregates.TagAssignmnet;

public sealed record TagAssignmentResponse(
    long Id,
    long TagId,
    long ItemId)
{
    public static TagAssignmentResponse FromDomain(TagAssignment assignment) => new(
        assignment.Id,
        assignment.TagId,
        assignment.ItemId);
}
```

*(Note: namespace still uses `TagAssignmnet` for now — T053 will fix the typo later)*

- [X] T009 [P] Create new file `Skyress.Application\Baskets\Responses\BasketResponse.cs` with this exact content:

```csharp
namespace Skyress.Application.Baskets.Responses;

using Skyress.Application.Baskets.DTOs;
using Skyress.Domain.Aggregates.Basket;
using Skyress.Domain.Enums;

public sealed record BasketResponse(
    long Id,
    long? UserId,
    BasketState State,
    long? InvoiceId,
    string? CheckoutId,
    IReadOnlyCollection<BasketItemDto> Items)
{
    public static BasketResponse FromDomain(Basket basket) => new(
        basket.Id,
        basket.UserId,
        basket.State,
        basket.InvoiceId,
        basket.CheckoutId,
        basket.BasketItems
            .Select(bi => new BasketItemDto(bi.BasketId, bi.ItemId, bi.Quantity))
            .ToList()
            .AsReadOnly());
}
```

- [X] T010 Update `Skyress.Application\Contracts\Persistence\IGenericRepository.cs`:
  Add `CancellationToken cancellationToken = default` as the final parameter to these 4 methods:
  - `GetAllAsync` → `GetAllAsync(CancellationToken cancellationToken = default)`
  - `GetByIdAsync` → `GetByIdAsync(long id, CancellationToken cancellationToken = default)`
  - `CreateAsync` → `CreateAsync(T entity, CancellationToken cancellationToken = default)`
  - `DeleteByIdAsync` → `DeleteByIdAsync(long id, CancellationToken cancellationToken = default)`

- [X] T011 Update `Skyress.Infrastructure\Repository\GenericRepository.cs` to match the updated interface signatures from T010:
  - `GetAllAsync()` → `GetAllAsync(CancellationToken cancellationToken = default)` — pass `cancellationToken` to `GetAsync().ToListAsync(cancellationToken)`
  - `GetByIdAsync(long id)` → `GetByIdAsync(long id, CancellationToken cancellationToken = default)` — pass token to `FirstOrDefaultAsync(..., cancellationToken)`
  - `CreateAsync(T entity)` → `CreateAsync(T entity, CancellationToken cancellationToken = default)` — pass token to `this.DbSet.AddAsync(entity, cancellationToken)`
  - `DeleteByIdAsync(long id)` → `DeleteByIdAsync(long id, CancellationToken cancellationToken = default)` — pass token to the internal `GetByIdAsync(id, cancellationToken)` call

**Checkpoint**: Build `Skyress.Application` project — should compile with zero errors.

---

## Phase 3: User Story 1 — Domain Layer Compliance (Priority: P1)

**Goal**: Fix `primitives` namespace casing, fix `TagAssignmnet` typo, add `Customer.Create` factory.

**Independent Test**: `dotnet build Skyress.Domain\Skyress.Domain.csproj` with zero errors.

### Namespace Fix: `primitives` → `Primitives`

Each task below is the same type of change in a different file. In every file, replace:
- `namespace Skyress.Domain.primitives` → `namespace Skyress.Domain.Primitives`
- `using Skyress.Domain.primitives;` → `using Skyress.Domain.Primitives;`

- [X] T012 [P] [US1] Fix namespace declaration in `Skyress.Domain\primitives\AggregateRoot.cs`:
  Change `namespace Skyress.Domain.primitives;` to `namespace Skyress.Domain.Primitives;`

- [X] T013 [P] [US1] Fix namespace declaration in `Skyress.Domain\primitives\BaseEntity.cs`:
  Change `namespace Skyress.Domain.primitives;` to `namespace Skyress.Domain.Primitives;`

- [X] T014 [P] [US1] Fix namespace declaration in `Skyress.Domain\primitives\IAuditable.cs`:
  Change `namespace Skyress.Domain.primitives;` to `namespace Skyress.Domain.Primitives;`

- [X] T015 [P] [US1] Fix namespace declaration in `Skyress.Domain\primitives\IDomainEvent.cs`:
  Change `namespace Skyress.Domain.primitives;` to `namespace Skyress.Domain.Primitives;`

- [X] T016 [P] [US1] Fix namespace declaration in `Skyress.Domain\primitives\ISoftDeletable.cs`:
  Change `namespace Skyress.Domain.primitives;` to `namespace Skyress.Domain.Primitives;`

- [X] T017 [P] [US1] Fix namespace declaration in `Skyress.Domain\primitives\ValueObject.cs`:
  Change `namespace Skyress.Domain.primitives;` to `namespace Skyress.Domain.Primitives;`

- [X] T018 [P] [US1] Fix `using` directive in `Skyress.Domain\Aggregates\Customer\Customer.cs`:
  Change `using Skyress.Domain.primitives;` to `using Skyress.Domain.Primitives;`

- [X] T019 [P] [US1] Fix `using` directive in `Skyress.Domain\Aggregates\Invoice\Invoice.cs`:
  Change `using Skyress.Domain.primitives;` to `using Skyress.Domain.Primitives;`

- [X] T020 [P] [US1] Fix `using` directive in `Skyress.Domain\Aggregates\Item\Item.cs`:
  Change `using Skyress.Domain.primitives;` to `using Skyress.Domain.Primitives;`

- [X] T021 [P] [US1] Fix `using` directive in `Skyress.Domain\Aggregates\Payment\Payment.cs`:
  Change `using Skyress.Domain.primitives;` to `using Skyress.Domain.Primitives;`

- [X] T022 [P] [US1] Fix `using` directive in `Skyress.Domain\Aggregates\Todo\Todo.cs`:
  Change `using Skyress.Domain.primitives;` to `using Skyress.Domain.Primitives;`

- [X] T023 [P] [US1] Fix `using` directive in `Skyress.Domain\Aggregates\Basket\Basket.cs`:
  Change `using Skyress.Domain.primitives;` to `using Skyress.Domain.Primitives;`

- [X] T024 [P] [US1] Fix `using` directive in `Skyress.Domain\Aggregates\Tag\Tag.cs`:
  Change `using Skyress.Domain.primitives;` to `using Skyress.Domain.Primitives;`

- [X] T025 [P] [US1] Fix `using` directive in `Skyress.Domain\Aggregates\Item\PricingHistory.cs`:
  Change `using Skyress.Domain.primitives;` to `using Skyress.Domain.Primitives;`

- [X] T026 [P] [US1] Fix `using` directive in `Skyress.Domain\Aggregates\Item\Events\ItemPriceChangedDomainEvent.cs`:
  Change `using Skyress.Domain.primitives;` to `using Skyress.Domain.Primitives;`

- [X] T027 [P] [US1] Fix `using` directive in `Skyress.Domain\Aggregates\Basket\BasketItem.cs`:
  Change `using Skyress.Domain.primitives;` to `using Skyress.Domain.Primitives;`

- [X] T028 [P] [US1] Fix `using` directive in `Skyress.Domain\Aggregates\Invoice\SoldItem.cs`:
  Change `using Skyress.Domain.primitives;` to `using Skyress.Domain.Primitives;`

- [X] T029 [P] [US1] Fix `using` directive in `Skyress.Domain\Aggregates\Payment\Installment.cs`:
  Change `using Skyress.Domain.primitives;` to `using Skyress.Domain.Primitives;`

- [X] T030 [P] [US1] Fix `using` directive in `Skyress.Domain\Aggregates\Auth\User.cs`:
  Change `using Skyress.Domain.primitives;` to `using Skyress.Domain.Primitives;`

- [X] T031 [P] [US1] Fix `using` directive in `Skyress.Domain\Aggregates\Auth\RefreshToken.cs`:
  Change `using Skyress.Domain.primitives;` to `using Skyress.Domain.Primitives;`

- [X] T032 [P] [US1] Fix `using` directive in `Skyress.Domain\Aggregates\Auth\Role.cs`:
  Change `using Skyress.Domain.primitives;` to `using Skyress.Domain.Primitives;`

- [X] T033 [P] [US1] Fix `using` directive in `Skyress.Domain\Aggregates\Auth\UserRole.cs`:
  Change `using Skyress.Domain.primitives;` to `using Skyress.Domain.Primitives;`

- [X] T034 [P] [US1] Fix `using` directive in `Skyress.Infrastructure\Repository\GenericRepository.cs`:
  Change `using Skyress.Domain.primitives;` to `using Skyress.Domain.Primitives;`

- [X] T035 [P] [US1] Fix `using` directive in `Skyress.Infrastructure\Persistence\SkyressDbContext.cs`:
  Change `using Skyress.Domain.primitives;` to `using Skyress.Domain.Primitives;`

- [X] T036 [P] [US1] Fix `using` directive in `Skyress.Application\Contracts\Persistence\IGenericRepository.cs`:
  Change `using Skyress.Domain.primitives;` to `using Skyress.Domain.Primitives;`

### TagAssignment Typo Fix

- [X] T037 [US1] Fix the `TagAssignmnet` → `TagAssignment` typo. This is a 7-file change:

  **File 1** — `Skyress.Domain\Aggregates\TagAssignmnet\TagAssignment.cs`:
  Change `namespace Skyress.Domain.Aggregates.TagAssignmnet;`
  to `namespace Skyress.Domain.Aggregates.TagAssignment;`

  **File 2** — `Skyress.Infrastructure\Persistence\SkyressDbContext.cs`:
  Change `using Skyress.Domain.Aggregates.TagAssignmnet;`
  to `using Skyress.Domain.Aggregates.TagAssignment;`

  **File 3** — `Skyress.Infrastructure\Repository\TagAssignmentRepository.cs`:
  Change `using Skyress.Domain.Aggregates.TagAssignmnet;`
  to `using Skyress.Domain.Aggregates.TagAssignment;`
  AND change `namespace Skyress.Infrastructure.Repository;` (keep as-is, no change needed there)

  **File 4** — `Skyress.Application\Contracts\Persistence\ITagAssignmentRepository.cs`:
  Change `using Skyress.Domain.Aggregates.TagAssignmnet;`
  to `using Skyress.Domain.Aggregates.TagAssignment;`

  **File 5** — `Skyress.Application\TagAssignments\Commands\CreateTagAssignment\CreateTagAssignmentCommand.cs`:
  Change `using Skyress.Domain.Aggregates.TagAssignmnet;`
  to `using Skyress.Domain.Aggregates.TagAssignment;`

  **File 6** — `Skyress.Application\TagAssignments\Commands\DeleteTagAssignment\DeleteTagAssignmentCommand.cs`:
  Change `using Skyress.Domain.Aggregates.TagAssignmnet;` (if present)
  to `using Skyress.Domain.Aggregates.TagAssignment;`

  **File 7** — `Skyress.Application\TagAssignments\Queries\GetTagAssignmentsByItem\GetTagAssignmentsByItemQuery.cs`:
  Change `using Skyress.Domain.Aggregates.TagAssignmnet;` (if present)
  to `using Skyress.Domain.Aggregates.TagAssignment;`

  Also update `Skyress.Application\TagAssignments\Queries\GetTagAssignmentsByTag\GetTagAssignmentsByTagQuery.cs` the same way.
  Also update `T008`'s file (`TagAssignmentResponse.cs`) namespace import to `using Skyress.Domain.Aggregates.TagAssignment;`.

### Customer Factory Method

- [X] T038 [US1] Add `Customer.Create()` factory method to `Skyress.Domain\Aggregates\Customer\Customer.cs`:

  Read the file first. Inside the `Customer` class body, add this method before the `SoftDelete` method:

  ```csharp
  public static Customer Create(string name, string notes, CustomerState state)
  {
      ArgumentException.ThrowIfNullOrWhiteSpace(name);
      return new Customer
      {
          Name = name,
          Notes = notes,
          State = state
      };
  }
  ```

**Checkpoint**: Run `dotnet build Skyress.Domain\Skyress.Domain.csproj` — must be zero errors.

---

## Phase 4: User Story 2 — Application Layer Compliance (Priority: P1)

**Goal**: Update all handlers to return DTOs, add logging, add FluentValidation validators.

**Independent Test**: `dotnet build Skyress.Application\Skyress.Application.csproj` with zero errors.
Every handler file contains `_logger.LogInformation`. No handler return type is a domain entity.

**Depends on**: T002–T011 (response DTOs and updated repository interface) must be complete.

---

### Items Feature

- [X] T039 [P] [US2] Update `Skyress.Application\Items\Commands\CreateItem\CreateItemCommand.cs`:

  1. Change the command record return type from `ICommand<Item>` to `ICommand<ItemResponse>`
  2. Change the handler class signature from `ICommandHandler<CreateItemCommand, Item>` to `ICommandHandler<CreateItemCommand, ItemResponse>`
  3. Change `Handle` return type from `Task<Result<Item>>` to `Task<Result<ItemResponse>>`
  4. Add `ILogger<CreateItemCommandHandler>` to the primary constructor: `CreateItemCommandHandler(IItemRepository itemRepository, ILogger<CreateItemCommandHandler> logger)`
  5. Add field: `private readonly ILogger<CreateItemCommandHandler> _logger = logger;`
  6. Add entry log as first line of Handle: `_logger.LogInformation("Handling {Command}", nameof(CreateItemCommand));`
  7. Change `return Result.Success(createdItem);` to `return Result.Success(ItemResponse.FromDomain(createdItem));`
  8. Add before the return: `_logger.LogInformation("{Command} completed. ItemId: {Id}", nameof(CreateItemCommand), createdItem.Id);`
  9. Add `using Skyress.Application.Items.Responses;` at the top
  10. Remove `using Skyress.Domain.Aggregates.Item;` only if Item is no longer referenced directly (keep it — ItemRepository needs it)

- [X] T040 [P] [US2] Create new file `Skyress.Application\Items\Commands\CreateItem\CreateItemCommandValidator.cs`:

  ```csharp
  namespace Skyress.Application.Items.Commands.CreateItem;

  using FluentValidation;

  public sealed class CreateItemCommandValidator : AbstractValidator<CreateItemCommand>
  {
      public CreateItemCommandValidator()
      {
          RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
          RuleFor(x => x.Price).GreaterThan(0).WithMessage("Price must be greater than zero.");
          RuleFor(x => x.QuantityLeft).GreaterThanOrEqualTo(0).WithMessage("Quantity must be non-negative.");
      }
  }
  ```

- [X] T041 [P] [US2] Update `Skyress.Application\Items\Commands\DeleteItem\DeleteItemCommand.cs`:

  1. Add `ILogger<DeleteItemCommandHandler>` to the primary constructor
  2. Add entry log as first line of Handle: `_logger.LogInformation("Handling {Command} for ItemId: {Id}", nameof(DeleteItemCommand), request.Id);`
  3. Add before return: `_logger.LogInformation("{Command} completed. ItemId: {Id}", nameof(DeleteItemCommand), request.Id);`
  4. Add `using Microsoft.Extensions.Logging;` at the top if not present

- [X] T042 [P] [US2] Create new file `Skyress.Application\Items\Commands\DeleteItem\DeleteItemCommandValidator.cs`:

  ```csharp
  namespace Skyress.Application.Items.Commands.DeleteItem;

  using FluentValidation;

  public sealed class DeleteItemCommandValidator : AbstractValidator<DeleteItemCommand>
  {
      public DeleteItemCommandValidator()
      {
          RuleFor(x => x.Id).GreaterThan(0).WithMessage("Item ID must be valid.");
      }
  }
  ```

- [X] T043 [P] [US2] Update `Skyress.Application\Items\Commands\UpdateItemDescription\UpdateItemDescriptionCommand.cs`:
  Add logger constructor injection and two log calls (entry + completion). Same pattern as T041.

- [X] T044 [P] [US2] Create `Skyress.Application\Items\Commands\UpdateItemDescription\UpdateItemDescriptionCommandValidator.cs`:

  ```csharp
  namespace Skyress.Application.Items.Commands.UpdateItemDescription;

  using FluentValidation;

  public sealed class UpdateItemDescriptionCommandValidator : AbstractValidator<UpdateItemDescriptionCommand>
  {
      public UpdateItemDescriptionCommandValidator()
      {
          RuleFor(x => x.Id).GreaterThan(0).WithMessage("Item ID must be valid.");
      }
  }
  ```

- [X] T045 [P] [US2] Update `Skyress.Application\Items\Commands\UpdateItemName\UpdateItemNameCommand.cs`:
  Add logger injection and two log calls (entry + completion).

- [X] T046 [P] [US2] Create `Skyress.Application\Items\Commands\UpdateItemName\UpdateItemNameCommandValidator.cs`:

  ```csharp
  namespace Skyress.Application.Items.Commands.UpdateItemName;

  using FluentValidation;

  public sealed class UpdateItemNameCommandValidator : AbstractValidator<UpdateItemNameCommand>
  {
      public UpdateItemNameCommandValidator()
      {
          RuleFor(x => x.Id).GreaterThan(0).WithMessage("Item ID must be valid.");
          RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
      }
  }
  ```

- [X] T047 [P] [US2] Update `Skyress.Application\Items\Commands\UpdateItemQrCode\UpdateItemQrCodeCommand.cs`:
  Add logger injection and two log calls.

- [X] T048 [P] [US2] Create `Skyress.Application\Items\Commands\UpdateItemQrCode\UpdateItemQrCodeCommandValidator.cs`:

  ```csharp
  namespace Skyress.Application.Items.Commands.UpdateItemQrCode;

  using FluentValidation;

  public sealed class UpdateItemQrCodeCommandValidator : AbstractValidator<UpdateItemQrCodeCommand>
  {
      public UpdateItemQrCodeCommandValidator()
      {
          RuleFor(x => x.Id).GreaterThan(0).WithMessage("Item ID must be valid.");
      }
  }
  ```

- [X] T049 [P] [US2] Update `Skyress.Application\Items\Commands\UpdateItemQuantityLeft\UpdateItemQuantityLeftCommand.cs`:
  Add logger injection and two log calls.

- [X] T050 [P] [US2] Create `Skyress.Application\Items\Commands\UpdateItemQuantityLeft\UpdateItemQuantityLeftCommandValidator.cs`:

  ```csharp
  namespace Skyress.Application.Items.Commands.UpdateItemQuantityLeft;

  using FluentValidation;

  public sealed class UpdateItemQuantityLeftCommandValidator : AbstractValidator<UpdateItemQuantityLeftCommand>
  {
      public UpdateItemQuantityLeftCommandValidator()
      {
          RuleFor(x => x.Id).GreaterThan(0).WithMessage("Item ID must be valid.");
          RuleFor(x => x.QuantityLeft).GreaterThanOrEqualTo(0).WithMessage("Quantity must be non-negative.");
      }
  }
  ```

- [X] T051 [P] [US2] Update `Skyress.Application\Items\Commands\UpdateItemUnit\UpdateItemUnitCommand.cs`:
  Add logger injection and two log calls.

- [X] T052 [P] [US2] Create `Skyress.Application\Items\Commands\UpdateItemUnit\UpdateItemUnitCommandValidator.cs`:

  ```csharp
  namespace Skyress.Application.Items.Commands.UpdateItemUnit;

  using FluentValidation;
  using Skyress.Domain.Enums;

  public sealed class UpdateItemUnitCommandValidator : AbstractValidator<UpdateItemUnitCommand>
  {
      public UpdateItemUnitCommandValidator()
      {
          RuleFor(x => x.Id).GreaterThan(0).WithMessage("Item ID must be valid.");
          RuleFor(x => x.Unit).IsInEnum().WithMessage("Unit must be a valid value.");
      }
  }
  ```

- [X] T053 [P] [US2] Update `Skyress.Application\Items\Commands\UpdateItemPrice\UpdateItemPriceCommand.cs`:
  Add logger injection and two log calls.

- [X] T054 [P] [US2] Create `Skyress.Application\Items\Commands\UpdateItemPrice\UpdateItemPriceCommandValidator.cs`:

  ```csharp
  namespace Skyress.Application.Items.Commands.UpdateItemPrice;

  using FluentValidation;

  public sealed class UpdateItemPriceCommandValidator : AbstractValidator<UpdateItemPriceCommand>
  {
      public UpdateItemPriceCommandValidator()
      {
          RuleFor(x => x.Id).GreaterThan(0).WithMessage("Item ID must be valid.");
      }
  }
  ```

- [X] T055 [P] [US2] Update `Skyress.Application\Items\Queries\GetAllItems\GetAllItemsQuery.cs`:

  1. Change `IQuery<IReadOnlyList<Item>>` to `IQuery<IReadOnlyList<ItemResponse>>`
  2. Change handler return type to `Task<Result<IReadOnlyList<ItemResponse>>>`
  3. Add logger injection and log calls
  4. Change `return Result.Success(items);` to:
     ```csharp
     var response = items.Select(ItemResponse.FromDomain).ToList().AsReadOnly();
     _logger.LogInformation("{Command} completed. Count: {Count}", nameof(GetAllItemsQuery), response.Count);
     return Result.Success<IReadOnlyList<ItemResponse>>(response);
     ```
  5. Add `using Skyress.Application.Items.Responses;`

- [X] T056 [P] [US2] Update `Skyress.Application\Items\Queries\GetItemById\GetItemByIdQuery.cs`:

  1. Change return type to `IQuery<ItemResponse>`
  2. Change handler to return `ItemResponse.FromDomain(item)` (wrap the found item)
  3. Add logger injection and log calls
  4. If item not found, return failure; if found, map to `ItemResponse.FromDomain(item!)`
  5. Add `using Skyress.Application.Items.Responses;`

- [X] T057 [P] [US2] Update `Skyress.Application\Items\Queries\GetItemPricingHistory\GetItemPricingHistoryQuery.cs`:
  Add logger injection and two log calls (entry + completion with count).

---

### Customers Feature

- [X] T058 [P] [US2] Update `Skyress.Application\Customers\Commands\CreateCustomer\CreateCustomerCommand.cs`:

  1. Change return type from `ICommand<Customer>` to `ICommand<CustomerResponse>`
  2. Change handler to `ICommandHandler<CreateCustomerCommand, CustomerResponse>`
  3. Change Handle return to `Task<Result<CustomerResponse>>`
  4. Add logger injection
  5. **Replace** `new Customer { Name = ..., Notes = ..., State = ... }` with `Customer.Create(request.Name, request.Notes, request.State)`
  6. Add entry and success log calls
  7. Change return to `Result.Success(CustomerResponse.FromDomain(createdCustomer))`
  8. Add `using Skyress.Application.Customers.Responses;`

- [X] T059 [P] [US2] Create `Skyress.Application\Customers\Commands\CreateCustomer\CreateCustomerCommandValidator.cs`:

  ```csharp
  namespace Skyress.Application.Customers.Commands.CreateCustomer;

  using FluentValidation;

  public sealed class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
  {
      public CreateCustomerCommandValidator()
      {
          RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
          RuleFor(x => x.Notes).NotEmpty().WithMessage("Notes are required.");
          RuleFor(x => x.State).IsInEnum().WithMessage("State must be a valid value.");
      }
  }
  ```

- [X] T060 [P] [US2] Update `Skyress.Application\Customers\Commands\DeleteCustomer\DeleteCustomerCommand.cs`:
  Add logger injection and two log calls.

- [X] T061 [P] [US2] Create `Skyress.Application\Customers\Commands\DeleteCustomer\DeleteCustomerCommandValidator.cs`:

  ```csharp
  namespace Skyress.Application.Customers.Commands.DeleteCustomer;

  using FluentValidation;

  public sealed class DeleteCustomerCommandValidator : AbstractValidator<DeleteCustomerCommand>
  {
      public DeleteCustomerCommandValidator()
      {
          RuleFor(x => x.Id).GreaterThan(0).WithMessage("Customer ID must be valid.");
      }
  }
  ```

- [X] T062 [P] [US2] Update `Skyress.Application\Customers\Commands\UpdateCustomerState\UpdateCustomerStateCommand.cs`:
  Add logger injection and two log calls.

- [X] T063 [P] [US2] Create `Skyress.Application\Customers\Commands\UpdateCustomerState\UpdateCustomerStateCommandValidator.cs`:

  ```csharp
  namespace Skyress.Application.Customers.Commands.UpdateCustomerState;

  using FluentValidation;

  public sealed class UpdateCustomerStateCommandValidator : AbstractValidator<UpdateCustomerStateCommand>
  {
      public UpdateCustomerStateCommandValidator()
      {
          RuleFor(x => x.Id).GreaterThan(0).WithMessage("Customer ID must be valid.");
          RuleFor(x => x.State).IsInEnum().WithMessage("State must be a valid value.");
      }
  }
  ```

- [X] T064 [P] [US2] Update `Skyress.Application\Customers\Commands\UpdateCustomerNotes\UpdateCustomerNotesCommand.cs`:
  Add logger injection and two log calls.

- [X] T065 [P] [US2] Create `Skyress.Application\Customers\Commands\UpdateCustomerNotes\UpdateCustomerNotesCommandValidator.cs`:

  ```csharp
  namespace Skyress.Application.Customers.Commands.UpdateCustomerNotes;

  using FluentValidation;

  public sealed class UpdateCustomerNotesCommandValidator : AbstractValidator<UpdateCustomerNotesCommand>
  {
      public UpdateCustomerNotesCommandValidator()
      {
          RuleFor(x => x.Id).GreaterThan(0).WithMessage("Customer ID must be valid.");
          RuleFor(x => x.Notes).NotEmpty().WithMessage("Notes are required.");
      }
  }
  ```

- [X] T066 [P] [US2] Update `Skyress.Application\Customers\Queries\GetAllCustomers\GetAllCustomersQuery.cs`:
  Change return type to `IReadOnlyList<CustomerResponse>`, map with `CustomerResponse.FromDomain`, add logger.

- [X] T067 [P] [US2] Update `Skyress.Application\Customers\Queries\GetCustomer\GetCustomerQuery.cs`:
  Change return type to `CustomerResponse`, map with `CustomerResponse.FromDomain`, add logger.

---

### Invoices Feature

- [X] T068 [P] [US2] Update `Skyress.Application\Invoices\Commands\CreateInvoice\CreateInvoiceCommand.cs`:
  Change return type to `InvoiceResponse`, add logger, change return to `InvoiceResponse.FromDomain(...)`.
  Add `using Skyress.Application.Invoices.Responses;`

- [X] T069 [P] [US2] Create `Skyress.Application\Invoices\Commands\CreateInvoice\CreateInvoiceCommandValidator.cs`:

  ```csharp
  namespace Skyress.Application.Invoices.Commands.CreateInvoice;

  using FluentValidation;

  public sealed class CreateInvoiceCommandValidator : AbstractValidator<CreateInvoiceCommand>
  {
      public CreateInvoiceCommandValidator()
      {
          RuleFor(x => x.BasketId).GreaterThan(0).WithMessage("Basket ID must be valid.");
      }
  }
  ```

- [X] T070 [P] [US2] Update `Skyress.Application\Invoices\Commands\DeleteInvoice\DeleteInvoiceCommand.cs`:
  Add logger and two log calls.

- [X] T071 [P] [US2] Create `Skyress.Application\Invoices\Commands\DeleteInvoice\DeleteInvoiceCommandValidator.cs`:

  ```csharp
  namespace Skyress.Application.Invoices.Commands.DeleteInvoice;

  using FluentValidation;

  public sealed class DeleteInvoiceCommandValidator : AbstractValidator<DeleteInvoiceCommand>
  {
      public DeleteInvoiceCommandValidator()
      {
          RuleFor(x => x.Id).GreaterThan(0).WithMessage("Invoice ID must be valid.");
      }
  }
  ```

- [X] T072 [P] [US2] Update `Skyress.Application\Invoices\Commands\UpdateInvoiceCustomerId\UpdateInvoiceCustomerIdCommand.cs`:
  Add logger and two log calls.

- [X] T073 [P] [US2] Create `Skyress.Application\Invoices\Commands\UpdateInvoiceCustomerId\UpdateInvoiceCustomerIdCommandValidator.cs`:

  ```csharp
  namespace Skyress.Application.Invoices.Commands.UpdateInvoiceCustomerId;

  using FluentValidation;

  public sealed class UpdateInvoiceCustomerIdCommandValidator : AbstractValidator<UpdateInvoiceCustomerIdCommand>
  {
      public UpdateInvoiceCustomerIdCommandValidator()
      {
          RuleFor(x => x.InvoiceId).GreaterThan(0).WithMessage("Invoice ID must be valid.");
          RuleFor(x => x.CustomerId).GreaterThan(0).WithMessage("Customer ID must be valid.");
      }
  }
  ```

- [X] T074 [P] [US2] Update `Skyress.Application\Invoices\Commands\UpdateInvoiceState\UpdateInvoiceStateCommand.cs`:
  Add logger and two log calls.

- [X] T075 [P] [US2] Create `Skyress.Application\Invoices\Commands\UpdateInvoiceState\UpdateInvoiceStateCommandValidator.cs`:

  ```csharp
  namespace Skyress.Application.Invoices.Commands.UpdateInvoiceState;

  using FluentValidation;

  public sealed class UpdateInvoiceStateCommandValidator : AbstractValidator<UpdateInvoiceStateCommand>
  {
      public UpdateInvoiceStateCommandValidator()
      {
          RuleFor(x => x.InvoiceId).GreaterThan(0).WithMessage("Invoice ID must be valid.");
          RuleFor(x => x.State).IsInEnum().WithMessage("State must be a valid value.");
      }
  }
  ```

- [X] T076 [P] [US2] Update `Skyress.Application\Invoices\Commands\AddSoldItemToInvoice\AddSoldItemToInvoiceCommand.cs`:
  Add logger and two log calls.

- [X] T077 [P] [US2] Create `Skyress.Application\Invoices\Commands\AddSoldItemToInvoice\AddSoldItemToInvoiceCommandValidator.cs`:

  ```csharp
  namespace Skyress.Application.Invoices.Commands.AddSoldItemToInvoice;

  using FluentValidation;

  public sealed class AddSoldItemToInvoiceCommandValidator : AbstractValidator<AddSoldItemToInvoiceCommand>
  {
      public AddSoldItemToInvoiceCommandValidator()
      {
          RuleFor(x => x.InvoiceId).GreaterThan(0).WithMessage("Invoice ID must be valid.");
      }
  }
  ```

- [X] T078 [P] [US2] Update `Skyress.Application\Invoices\Commands\BuildInvoiceFromBasketCommand\BuildInvoiceFromBasketCommand.cs`:
  Add logger and two log calls; change return to `InvoiceResponse` if it currently returns `Invoice`.

- [X] T079 [P] [US2] Create `Skyress.Application\Invoices\Commands\BuildInvoiceFromBasketCommand\BuildInvoiceFromBasketCommandValidator.cs`:

  ```csharp
  namespace Skyress.Application.Invoices.Commands.BuildInvoiceFromBasketCommand;

  using FluentValidation;

  public sealed class BuildInvoiceFromBasketCommandValidator : AbstractValidator<BuildInvoiceFromBasketCommand>
  {
      public BuildInvoiceFromBasketCommandValidator()
      {
          RuleFor(x => x.BasketId).GreaterThan(0).WithMessage("Basket ID must be valid.");
      }
  }
  ```

- [X] T080 [P] [US2] Update `Skyress.Application\Invoices\Queries\GetAllInvoices\GetAllInvoicesQuery.cs`:
  Change return type to `IReadOnlyList<InvoiceResponse>`, add logger and mapping.

- [X] T081 [P] [US2] Update `Skyress.Application\Invoices\Queries\GetInvoiceById\GetInvoiceByIdQuery.cs`:
  Change return type to `InvoiceResponse`, add logger and `InvoiceResponse.FromDomain(...)`.

- [X] T082 [P] [US2] Update `Skyress.Application\Invoices\Queries\GetInvoicesByCustomer\GetInvoicesByCustomerQuery.cs`:
  Add logger and two log calls.

- [X] T083 [P] [US2] Update `Skyress.Application\Invoices\Queries\GetInvoicesByState\GetInvoicesByStateQuery.cs`:
  Add logger and two log calls.

- [X] T084 [P] [US2] Update `Skyress.Application\Invoices\Queries\GetInvoiceWithPayments\GetInvoiceWithPaymentsQuery.cs`:
  Add logger and two log calls.

---

### Payments Feature

- [X] T085 [P] [US2] Update `Skyress.Application\Payments\Commands\CreatePayment\CreatePaymentCommand.cs`:
  Change return type to `PaymentResponse`, add logger, return `PaymentResponse.FromDomain(...)`.
  Add `using Skyress.Application.Payments.Responses;`

- [X] T086 [P] [US2] Create `Skyress.Application\Payments\Commands\CreatePayment\CreatePaymentCommandValidator.cs`:

  ```csharp
  namespace Skyress.Application.Payments.Commands.CreatePayment;

  using FluentValidation;

  public sealed class CreatePaymentCommandValidator : AbstractValidator<CreatePaymentCommand>
  {
      public CreatePaymentCommandValidator()
      {
          RuleFor(x => x.InvoiceId).GreaterThan(0).WithMessage("Invoice ID must be valid.");
          RuleFor(x => x.TotalDue).GreaterThan(0).WithMessage("Total due must be greater than zero.");
      }
  }
  ```

- [X] T087 [P] [US2] Update `Skyress.Application\Payments\Commands\CompleteCashPayment\CompleteCashPaymentCommand.cs`:
  Add logger and two log calls.

- [X] T088 [P] [US2] Create `Skyress.Application\Payments\Commands\CompleteCashPayment\CompleteCashPaymentCommandValidator.cs`:

  ```csharp
  namespace Skyress.Application.Payments.Commands.CompleteCashPayment;

  using FluentValidation;

  public sealed class CompleteCashPaymentCommandValidator : AbstractValidator<CompleteCashPaymentCommand>
  {
      public CompleteCashPaymentCommandValidator()
      {
          RuleFor(x => x.PaymentId).GreaterThan(0).WithMessage("Payment ID must be valid.");
          RuleFor(x => x.AmountPaid).GreaterThan(0).WithMessage("Amount paid must be greater than zero.");
      }
  }
  ```

  *(If `AmountPaid` is not a property on the command, check the actual command fields and adjust the rule accordingly.)*

- [X] T089 [P] [US2] Update `Skyress.Application\Payments\Queries\GetAllPayments\GetAllPaymentsQuery.cs`:
  Add logger and two log calls.

- [X] T090 [P] [US2] Update `Skyress.Application\Payments\Queries\GetPaymentById\GetPaymentByIdQuery.cs`:
  Change return type to `PaymentResponse`, add logger and `PaymentResponse.FromDomain(...)`.

- [X] T091 [P] [US2] Update `Skyress.Application\Payments\Queries\GetPaymentsByInvoice\GetPaymentsByInvoiceQuery.cs`:
  Add logger and two log calls.

---

### Todos Feature

- [X] T092 [P] [US2] Update `Skyress.Application\Todos\Commands\CreateTodo\CreateTodoCommand.cs`:
  Change return type to `TodoResponse`, add logger, return `TodoResponse.FromDomain(...)`.
  Add `using Skyress.Application.Todos.Responses;`

- [X] T093 [P] [US2] Create `Skyress.Application\Todos\Commands\CreateTodo\CreateTodoCommandValidator.cs`:

  ```csharp
  namespace Skyress.Application.Todos.Commands.CreateTodo;

  using FluentValidation;

  public sealed class CreateTodoCommandValidator : AbstractValidator<CreateTodoCommand>
  {
      public CreateTodoCommandValidator()
      {
          RuleFor(x => x.Context).NotEmpty().WithMessage("Context is required.");
      }
  }
  ```

- [X] T094 [P] [US2] Update `Skyress.Application\Todos\Commands\DeleteTodo\DeleteTodoCommand.cs`:
  Add logger and two log calls.

- [X] T095 [P] [US2] Create `Skyress.Application\Todos\Commands\DeleteTodo\DeleteTodoCommandValidator.cs`:

  ```csharp
  namespace Skyress.Application.Todos.Commands.DeleteTodo;

  using FluentValidation;

  public sealed class DeleteTodoCommandValidator : AbstractValidator<DeleteTodoCommand>
  {
      public DeleteTodoCommandValidator()
      {
          RuleFor(x => x.Id).GreaterThan(0).WithMessage("Todo ID must be valid.");
      }
  }
  ```

- [X] T096 [P] [US2] Update `Skyress.Application\Todos\Commands\UpdateTodoState\UpdateTodoStateCommand.cs`:
  Add logger and two log calls.

- [X] T097 [P] [US2] Create `Skyress.Application\Todos\Commands\UpdateTodoState\UpdateTodoStateCommandValidator.cs`:

  ```csharp
  namespace Skyress.Application.Todos.Commands.UpdateTodoState;

  using FluentValidation;

  public sealed class UpdateTodoStateCommandValidator : AbstractValidator<UpdateTodoStateCommand>
  {
      public UpdateTodoStateCommandValidator()
      {
          RuleFor(x => x.Id).GreaterThan(0).WithMessage("Todo ID must be valid.");
          RuleFor(x => x.State).IsInEnum().WithMessage("State must be a valid value.");
      }
  }
  ```

- [X] T098 [P] [US2] Update `Skyress.Application\Todos\Commands\UpdateTodoContext\UpdateTodoContextCommand.cs`:
  Add logger and two log calls.

- [X] T099 [P] [US2] Create `Skyress.Application\Todos\Commands\UpdateTodoContext\UpdateTodoContextCommandValidator.cs`:

  ```csharp
  namespace Skyress.Application.Todos.Commands.UpdateTodoContext;

  using FluentValidation;

  public sealed class UpdateTodoContextCommandValidator : AbstractValidator<UpdateTodoContextCommand>
  {
      public UpdateTodoContextCommandValidator()
      {
          RuleFor(x => x.Id).GreaterThan(0).WithMessage("Todo ID must be valid.");
          RuleFor(x => x.Context).NotEmpty().WithMessage("Context is required.");
      }
  }
  ```

- [X] T100 [P] [US2] Update `Skyress.Application\Todos\Queries\GetAllTodos\GetAllTodosQuery.cs`:
  Add logger and two log calls.

- [X] T101 [P] [US2] Update `Skyress.Application\Todos\Queries\GetTodoById\GetTodoByIdQuery.cs`:
  Change return type to `TodoResponse`, add logger and `TodoResponse.FromDomain(...)`.

- [X] T102 [P] [US2] Update `Skyress.Application\Todos\Queries\GetTodosByState\GetTodosByStateQuery.cs`:
  Add logger and two log calls.

---

### Tags Feature

- [X] T103 [P] [US2] Update `Skyress.Application\Tags\Commands\CreateTag\CreateTagCommand.cs`:
  Change return type to `TagResponse`, add logger, return `TagResponse.FromDomain(...)`.
  Add `using Skyress.Application.Tags.Responses;`

- [X] T104 [P] [US2] Create `Skyress.Application\Tags\Commands\CreateTag\CreateTagCommandValidator.cs`:

  ```csharp
  namespace Skyress.Application.Tags.Commands.CreateTag;

  using FluentValidation;

  public sealed class CreateTagCommandValidator : AbstractValidator<CreateTagCommand>
  {
      public CreateTagCommandValidator()
      {
          RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
          RuleFor(x => x.Type).IsInEnum().WithMessage("Type must be a valid value.");
      }
  }
  ```

- [X] T105 [P] [US2] Update `Skyress.Application\Tags\Commands\DeleteTag\DeleteTagCommand.cs`:
  Add logger and two log calls.

- [X] T106 [P] [US2] Create `Skyress.Application\Tags\Commands\DeleteTag\DeleteTagCommandValidator.cs`:

  ```csharp
  namespace Skyress.Application.Tags.Commands.DeleteTag;

  using FluentValidation;

  public sealed class DeleteTagCommandValidator : AbstractValidator<DeleteTagCommand>
  {
      public DeleteTagCommandValidator()
      {
          RuleFor(x => x.Id).GreaterThan(0).WithMessage("Tag ID must be valid.");
      }
  }
  ```

- [X] T107 [P] [US2] Update `Skyress.Application\Tags\Commands\UpdateTagName\UpdateTagNameCommand.cs`:
  Add logger and two log calls.

- [X] T108 [P] [US2] Create `Skyress.Application\Tags\Commands\UpdateTagName\UpdateTagNameCommandValidator.cs`:

  ```csharp
  namespace Skyress.Application.Tags.Commands.UpdateTagName;

  using FluentValidation;

  public sealed class UpdateTagNameCommandValidator : AbstractValidator<UpdateTagNameCommand>
  {
      public UpdateTagNameCommandValidator()
      {
          RuleFor(x => x.Id).GreaterThan(0).WithMessage("Tag ID must be valid.");
          RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
      }
  }
  ```

- [X] T109 [P] [US2] Update `Skyress.Application\Tags\Commands\UpdateTagType\UpdateTagTypeCommand.cs`:
  Add logger and two log calls.

- [X] T110 [P] [US2] Create `Skyress.Application\Tags\Commands\UpdateTagType\UpdateTagTypeCommandValidator.cs`:

  ```csharp
  namespace Skyress.Application.Tags.Commands.UpdateTagType;

  using FluentValidation;

  public sealed class UpdateTagTypeCommandValidator : AbstractValidator<UpdateTagTypeCommand>
  {
      public UpdateTagTypeCommandValidator()
      {
          RuleFor(x => x.Id).GreaterThan(0).WithMessage("Tag ID must be valid.");
          RuleFor(x => x.Type).IsInEnum().WithMessage("Type must be a valid value.");
      }
  }
  ```

- [X] T111 [P] [US2] Update `Skyress.Application\Tags\Queries\GetAllTags\GetAllTagsQuery.cs`:
  Add logger and two log calls.

- [X] T112 [P] [US2] Update `Skyress.Application\Tags\Queries\GetTagById\GetTagByIdQuery.cs`:
  Change return type to `TagResponse`, add logger and `TagResponse.FromDomain(...)`.

- [X] T113 [P] [US2] Update `Skyress.Application\Tags\Queries\GetTagsByType\GetTagsByTypeQuery.cs`:
  Add logger and two log calls.

---

### TagAssignments Feature

- [X] T114 [P] [US2] Update `Skyress.Application\TagAssignments\Commands\CreateTagAssignment\CreateTagAssignmentCommand.cs`:
  Change return type to `TagAssignmentResponse`, add logger, return `TagAssignmentResponse.FromDomain(...)`.
  Add `using Skyress.Application.TagAssignments.Responses;`

- [X] T115 [P] [US2] Create `Skyress.Application\TagAssignments\Commands\CreateTagAssignment\CreateTagAssignmentCommandValidator.cs`:

  ```csharp
  namespace Skyress.Application.TagAssignments.Commands.CreateTagAssignment;

  using FluentValidation;

  public sealed class CreateTagAssignmentCommandValidator : AbstractValidator<CreateTagAssignmentCommand>
  {
      public CreateTagAssignmentCommandValidator()
      {
          RuleFor(x => x.TagId).GreaterThan(0).WithMessage("Tag ID must be valid.");
          RuleFor(x => x.ItemId).GreaterThan(0).WithMessage("Item ID must be valid.");
      }
  }
  ```

- [X] T116 [P] [US2] Update `Skyress.Application\TagAssignments\Commands\DeleteTagAssignment\DeleteTagAssignmentCommand.cs`:
  Add logger and two log calls.

- [X] T117 [P] [US2] Create `Skyress.Application\TagAssignments\Commands\DeleteTagAssignment\DeleteTagAssignmentCommandValidator.cs`:

  ```csharp
  namespace Skyress.Application.TagAssignments.Commands.DeleteTagAssignment;

  using FluentValidation;

  public sealed class DeleteTagAssignmentCommandValidator : AbstractValidator<DeleteTagAssignmentCommand>
  {
      public DeleteTagAssignmentCommandValidator()
      {
          RuleFor(x => x.Id).GreaterThan(0).WithMessage("TagAssignment ID must be valid.");
      }
  }
  ```

- [X] T118 [P] [US2] Update `Skyress.Application\TagAssignments\Queries\GetTagAssignmentsByItem\GetTagAssignmentsByItemQuery.cs`:
  Add logger and two log calls.

- [X] T119 [P] [US2] Update `Skyress.Application\TagAssignments\Queries\GetTagAssignmentsByTag\GetTagAssignmentsByTagQuery.cs`:
  Add logger and two log calls.

---

### Baskets Feature

- [X] T120 [P] [US2] Update `Skyress.Application\Baskets\Commands\CreateBasket\CreateBasketCommand.cs`:
  Change return type to `BasketResponse` if it returns a `Basket`, add logger, return `BasketResponse.FromDomain(...)`.
  Add `using Skyress.Application.Baskets.Responses;`

- [X] T121 [P] [US2] Create `Skyress.Application\Baskets\Commands\CreateBasket\CreateBasketCommandValidator.cs`:

  ```csharp
  namespace Skyress.Application.Baskets.Commands.CreateBasket;

  using FluentValidation;

  public sealed class CreateBasketCommandValidator : AbstractValidator<CreateBasketCommand>
  {
      public CreateBasketCommandValidator()
      {
          // UserId is optional (nullable) — no validation required
      }
  }
  ```

- [X] T122 [P] [US2] Update `Skyress.Application\Baskets\Commands\AddItemToBasket\AddItemToBasketCommand.cs`:
  Add logger and two log calls.

- [X] T123 [P] [US2] Create `Skyress.Application\Baskets\Commands\AddItemToBasket\AddItemToBasketCommandValidator.cs`:

  ```csharp
  namespace Skyress.Application.Baskets.Commands.AddItemToBasket;

  using FluentValidation;

  public sealed class AddItemToBasketCommandValidator : AbstractValidator<AddItemToBasketCommand>
  {
      public AddItemToBasketCommandValidator()
      {
          RuleFor(x => x.BasketId).GreaterThan(0).WithMessage("Basket ID must be valid.");
          RuleFor(x => x.ItemId).GreaterThan(0).WithMessage("Item ID must be valid.");
          RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("Quantity must be greater than zero.");
      }
  }
  ```

- [X] T124 [P] [US2] Update `Skyress.Application\Baskets\Commands\RemoveItemFromBasket\RemoveItemFromBasketCommandHandler.cs`:
  Add logger injection and two log calls. (This is a separate handler file from the command.)

- [X] T125 [P] [US2] Create `Skyress.Application\Baskets\Commands\RemoveItemFromBasket\RemoveItemFromBasketCommandValidator.cs`:

  ```csharp
  namespace Skyress.Application.Baskets.Commands.RemoveItemFromBasket;

  using FluentValidation;

  public sealed class RemoveItemFromBasketCommandValidator : AbstractValidator<RemoveItemFromBasketCommand>
  {
      public RemoveItemFromBasketCommandValidator()
      {
          RuleFor(x => x.BasketId).GreaterThan(0).WithMessage("Basket ID must be valid.");
          RuleFor(x => x.ItemId).GreaterThan(0).WithMessage("Item ID must be valid.");
      }
  }
  ```

- [X] T126 [P] [US2] Update `Skyress.Application\Baskets\Commands\ClearBasket\ClearBasketCommandHandler.cs`:
  Add logger injection and two log calls.

- [X] T127 [P] [US2] Create `Skyress.Application\Baskets\Commands\ClearBasket\ClearBasketCommandValidator.cs`:

  ```csharp
  namespace Skyress.Application.Baskets.Commands.ClearBasket;

  using FluentValidation;

  public sealed class ClearBasketCommandValidator : AbstractValidator<ClearBasketCommand>
  {
      public ClearBasketCommandValidator()
      {
          RuleFor(x => x.BasketId).GreaterThan(0).WithMessage("Basket ID must be valid.");
      }
  }
  ```

- [X] T128 [P] [US2] Update `Skyress.Application\Baskets\Commands\DeleteBasketCommand\DeleteBasketCommandHandler.cs`:
  Add logger injection and two log calls.

- [X] T129 [P] [US2] Create `Skyress.Application\Baskets\Commands\DeleteBasketCommand\DeleteBasketCommandValidator.cs`:

  ```csharp
  namespace Skyress.Application.Baskets.Commands.DeleteBasketCommand;

  using FluentValidation;

  public sealed class DeleteBasketCommandValidator : AbstractValidator<DeleteBasketCommand>
  {
      public DeleteBasketCommandValidator()
      {
          RuleFor(x => x.Id).GreaterThan(0).WithMessage("Basket ID must be valid.");
      }
  }
  ```

- [X] T130 [P] [US2] Update `Skyress.Application\Baskets\Commands\CancelBasketReservation\CancelBasketReservationCommandHandler.cs`:
  Add logger injection and two log calls.

- [X] T131 [P] [US2] Create `Skyress.Application\Baskets\Commands\CancelBasketReservation\CancelBasketReservationCommandValidator.cs`:

  ```csharp
  namespace Skyress.Application.Baskets.Commands.CancelBasketReservation;

  using FluentValidation;

  public sealed class CancelBasketReservationCommandValidator : AbstractValidator<CancelBasketReservationCommand>
  {
      public CancelBasketReservationCommandValidator()
      {
          RuleFor(x => x.BasketId).GreaterThan(0).WithMessage("Basket ID must be valid.");
      }
  }
  ```

- [X] T132 [P] [US2] Update `Skyress.Application\Baskets\Queries\GetBasketById\GetBasketByIdQuery.cs`:
  Change return type to `BasketResponse`, add logger and `BasketResponse.FromDomain(...)`.

- [X] T133 [P] [US2] Update `Skyress.Application\Baskets\Queries\GetBasketsByState\GetBasketsByStateQuery.cs`:
  Add logger and two log calls.

- [X] T134 [P] [US2] Update `Skyress.Application\Baskets\Queries\GetBasketsByCustomer\GetBasketsByCustomerQuery.cs`:
  Add logger and two log calls.

**Checkpoint**: Run `dotnet build Skyress.Application\Skyress.Application.csproj` — zero errors.
Grep for `LogInformation` in all handler files — every handler must have at least one match.

---

## Phase 5: User Story 3 — Presentation Layer Compliance (Priority: P2)

**Goal**: Add `ExceptionHandlingMiddleware` and update all endpoint error returns to use `ProblemDetails`.

**Independent Test**: Build the presentation project. Hit a non-existent endpoint — should return
`ProblemDetails` JSON, not a stack trace. `BadRequest<string>` must not appear in any endpoint file.

**Depends on**: US1 and US2 complete (handlers must return DTOs before endpoints can use them).

- [X] T135 [US3] Create new file `Skyress\Middleware\ExceptionHandlingMiddleware.cs` with this exact content:

```csharp
namespace Skyress.API.Middleware;

using Microsoft.AspNetCore.Mvc;
using Skyress.Domain.Exceptions;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Domain rule violation: {Message}", ex.Message);
            context.Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
            await context.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Title = "Domain Rule Violation",
                Detail = ex.Message,
                Status = StatusCodes.Status422UnprocessableEntity
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Title = "Internal Server Error",
                Status = StatusCodes.Status500InternalServerError
            });
        }
    }
}
```

*(Note: If `DomainException` is in a different namespace, check `Skyress.Domain\Exceptions\DomainException.cs` and use the correct `using` directive.)*

- [X] T136 [US3] Register `ExceptionHandlingMiddleware` in `Skyress\Program.cs` (or `Startup.cs`):
  Add `app.UseMiddleware<ExceptionHandlingMiddleware>();` as the **first** middleware call,
  before `app.UseRouting()`, `app.UseAuthentication()`, and `app.UseAuthorization()`.
  Add `using Skyress.API.Middleware;` at the top of the file.

- [X] T137 [P] [US3] Update `Skyress\Endpoints\Items\CreateItemEndpoint.cs`:
  Change the return type from `Results<Ok<Item>, BadRequest<string>>` to
  `Results<Ok<ItemResponse>, UnprocessableEntity<ProblemDetails>>`.
  Change `TypedResults.BadRequest(result.Error.Message)` to:
  ```csharp
  TypedResults.UnprocessableEntity(new ProblemDetails
  {
      Title = "Validation Error",
      Detail = result.Error.Message,
      Status = StatusCodes.Status422UnprocessableEntity
  })
  ```
  Add `using Microsoft.AspNetCore.Mvc;` and `using Skyress.Application.Items.Responses;`

- [X] T138 [P] [US3] Update all other Items endpoints in `Skyress\Endpoints\Items\`:
  For each endpoint file that returns `BadRequest<string>` or a raw domain entity:
  - Replace `BadRequest<string>` with `UnprocessableEntity<ProblemDetails>`
  - Replace domain entity return types with response DTO types
  - Add `using Microsoft.AspNetCore.Mvc;` if not present
  Files to check: `DeleteItemEndpoint.cs`, `GetAllItemsEndpoint.cs`, `GetItemByIdEndpoint.cs`,
  `GetItemPricingHistoryEndpoint.cs`, `UpdateItemEndpoints.cs`

- [X] T139 [P] [US3] Update all Customers endpoints in `Skyress\Endpoints\Customers\`:
  Same pattern as T138. Replace raw error strings and domain entity returns with DTOs + ProblemDetails.
  Files: `CreateCustomerEndpoint.cs`, `DeleteCustomerEndpoint.cs`, `GetAllCustomersEndpoint.cs`,
  `GetCustomerEndpoint.cs`, `UpdateCustomerEndpoints.cs`

- [X] T140 [P] [US3] Update all Invoices endpoints in `Skyress\Endpoints\Invoices\`:
  Same pattern. Files: `DeleteInvoiceEndpoint.cs`, `GetAllInvoicesEndpoint.cs`,
  `GetInvoiceEndpoint.cs`, `GetInvoiceWithPaymentsEndpoint.cs`, `GetInvoicesByCustomerEndpoint.cs`,
  `GetInvoicesByStateEndpoint.cs`, `UpdateInvoiceEndpoints.cs`

- [X] T141 [P] [US3] Update all Payments endpoints in `Skyress\Endpoints\Payments\`:
  Files: `CompleteCashPaymentEndpoint.cs`, `GetAllPaymentsEndpoint.cs`, `GetPaymentEndpoint.cs`,
  `GetPaymentsByInvoiceEndpoint.cs`

- [X] T142 [P] [US3] Update all Todos endpoints in `Skyress\Endpoints\Todos\`:
  Files: `CreateTodoEndpoint.cs`, `DeleteTodoEndpoint.cs`, `GetAllTodosEndpoint.cs`,
  `GetTodoEndpoint.cs`, `GetTodosByStateEndpoint.cs`, `UpdateTodoEndpoints.cs`

- [X] T143 [P] [US3] Update all Tags endpoints in `Skyress\Endpoints\Tags\`:
  Files: `CreateTagEndpoint.cs`, `DeleteTagEndpoint.cs`, `GetAllTagsEndpoint.cs`,
  `GetTagEndpoint.cs`, `GetTagsByTypeEndpoint.cs`, `UpdateTagEndpoints.cs`

- [X] T144 [P] [US3] Update all TagAssignments endpoints in `Skyress\Endpoints\TagAssignments\`:
  Files: `CreateTagAssignmentEndpoint.cs`, `DeleteTagAssignmentEndpoint.cs`,
  `GetTagAssignmentsByItemEndpoint.cs`, `GetTagAssignmentsByTagEndpoint.cs`

- [X] T145 [P] [US3] Update all Baskets endpoints in `Skyress\Endpoints\Baskets\`:
  Files: `CreateBasketEndpoint.cs`, `AddItemToBasketEndpoint.cs`, `RemoveItemFromBasketEndpoint.cs`,
  `ClearBasketEndpoint.cs`, `DeleteBasketEndpoint.cs`, `CancelBasketReservationEndpoint.cs`,
  `GetBasketByIdEndpoint.cs`, `GetBasketsByStateEndpoint.cs`, `GetBasketsByCustomerEndpoint.cs`,
  `InitiateCheckoutBasketEndpoint.cs`

**Checkpoint**: `dotnet build skyress.sln` — zero errors.

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Final validation and cleanup.

- [X] T146 Run `dotnet build skyress.sln` from repo root and confirm zero errors
- [X] T147 Run validation checks from `specs/002-constitution-refactor/quickstart.md`:
  - Verify no handler returns a domain entity
  - Verify all handlers contain `LogInformation`
  - Verify `ExceptionHandlingMiddleware` is registered in Program.cs
  - Verify no `BadRequest<string>` in endpoint files

---

## Dependencies & Execution Order

### Phase Dependencies

- **Phase 1 (Setup)**: No dependencies — run first
- **Phase 2 (Foundational)**: Depends on Phase 1 — BLOCKS all user story phases
- **Phase 3 (US1 Domain)**: Depends on Phase 2 — namespace and factory changes
- **Phase 4 (US2 Application)**: Depends on Phase 2 (DTOs must exist) and Phase 3 (factory method)
- **Phase 5 (US3 Presentation)**: Depends on Phase 4 (handlers must return DTOs)
- **Phase 6 (Polish)**: Depends on all previous phases

### Within Phase 4

- All `[P]` tasks within the same feature can run in parallel
- Tasks across different features (Items vs Customers vs Invoices etc.) can ALL run in parallel
- Validator creation tasks `[P]` can run concurrently with handler update tasks

### Parallel Opportunities

```
Phase 2: T002–T009 all run in parallel (8 different new files)
          T010 → T011 (sequential: interface before implementation)

Phase 3: T012–T036 all run in parallel (different files)
          T037 (sequential: single multi-file task)
          T038 (independent: can run in parallel with T012–T036)

Phase 4: All T039–T134 [P] tasks can run in parallel across features
          Exception: T124 depends on T058 (Customer.Create must exist before handler update)

Phase 5: T135 → T136 (sequential: create middleware before registering it)
          T137–T145 all run in parallel
```

### User Story Dependencies

- **US1 (P1)**: Starts after Phase 2 — no dependency on other stories
- **US2 (P1)**: Starts after Phase 2 AND US1 T038 (Customer.Create factory method)
- **US3 (P2)**: Starts after US2 is complete

---

## Implementation Strategy

### MVP First (US1 + US2 Only)

1. Complete Phase 1 (verify build)
2. Complete Phase 2 (create DTOs + update generic repo)
3. Complete Phase 3 US1 (domain layer: namespace fixes + factory method)
4. **CHECKPOINT**: Build Skyress.Domain — zero errors
5. Complete Phase 4 US2 (application layer: DTOs in handlers + validators + logging)
6. **CHECKPOINT**: Build Skyress.Application — zero errors
7. **STOP and VALIDATE**: Grep for `LogInformation` in all handler files

### Incremental Delivery

1. Phase 2 → Foundation ready
2. Phase 3 (US1) → Domain layer compliant
3. Phase 4 (US2) → Application layer compliant — already major value
4. Phase 5 (US3) → Presentation layer compliant — externally visible improvement

---

## Notes

- `[P]` tasks write to different files — safe to run simultaneously
- All validator files are NEW files — use Write tool, not Edit
- All response DTO files are NEW files — use Write tool, not Edit
- All handler updates are EDITS to existing files — use Read first, then Edit
- After Phase 3, the `primitives` folder on disk still says `primitives` (Windows is case-insensitive) — this is expected and fine
- The `TagAssignmnet` folder on disk still has the old name — only the namespace changes. A git rename can be done separately if desired
- Do NOT modify Auth command validators (Login, Register, Logout, RefreshToken) — they are already compliant
- Do NOT modify Saga consumers or the background job ProcessOutboxMessagesJob
