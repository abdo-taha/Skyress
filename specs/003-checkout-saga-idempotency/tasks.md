# Tasks: Checkout Saga Idempotency & Fault Tolerance

**Input**: Design documents from `specs/003-checkout-saga-idempotency/`
**Feature branch**: `003-checkout-saga-idempotency`
**Plan reference**: `specs/003-checkout-saga-idempotency/plan.md`

> **Haiku implementation note**: Every task below includes the exact code to write. Read the file first, make the targeted edit described, then move on. Do not refactor anything not listed. Do not change tests or other command files not mentioned.

---

## Phase 1: Setup

**Purpose**: No new project or folder structure needed — this feature modifies existing files only.

- [x] T001 Read `specs/003-checkout-saga-idempotency/plan.md` and `specs/003-checkout-saga-idempotency/data-model.md` to confirm scope before starting

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Domain flag, repository interface additions, and repository implementations. These are required by all subsequent handler changes. Complete this entire phase before moving to Phase 3.

**⚠️ CRITICAL**: All Phase 3–6 tasks depend on this phase being complete.

- [x] T002 Add `IsReserved` and `IsSold` flags to `Skyress.Domain/Aggregates/Basket/BasketItem.cs`

  Open the file. The class currently has `BasketId`, `ItemId`, `Quantity`, and `AddQuantity()`. Add these four members **inside the class body**, after the `AddQuantity` method:

  ```csharp
  public bool IsReserved { get; private set; }
  public bool IsSold { get; private set; }

  public void MarkAsReserved() => IsReserved = true;
  public void MarkAsSold() => IsSold = true;
  ```

  Do not change the constructor or any existing members.

- [x] T003 Add two new methods to `Skyress.Application/Contracts/Persistence/IInvoiceRepository.cs`

  Open the file. The interface currently has one method: `Task<Invoice?> GetByPaymentId(long paymentId);`. Add these two lines after it:

  ```csharp
  Task<Invoice?> GetByBasketIdAsync(long basketId, CancellationToken cancellationToken = default);
  Task<Invoice?> GetByIdWithSoldItemsAsync(long invoiceId, CancellationToken cancellationToken = default);
  ```

- [x] T004 Add one new method to `Skyress.Application/Contracts/Persistence/IPaymentRepository.cs`

  Open the file. The interface currently has no methods (empty body). Add:

  ```csharp
  Task<Payment?> GetByInvoiceIdAsync(long invoiceId, CancellationToken cancellationToken = default);
  ```

- [x] T005 Implement `GetByBasketIdAsync` and `GetByIdWithSoldItemsAsync` in `Skyress.Infrastructure/Repository/InvoiceRepository.cs`

  Open the file. The class already has `GetByPaymentId`. Add these two methods after it. You will need a reference to the DbContext — the base class `GenericRepository<T>` exposes `_skyressDbContext` (check `GenericRepository.cs` to confirm the field name):

  ```csharp
  public async Task<Invoice?> GetByBasketIdAsync(long basketId, CancellationToken cancellationToken = default)
  {
      return await GetAsync(i => i.BasketId == basketId).FirstOrDefaultAsync(cancellationToken);
  }

  public async Task<Invoice?> GetByIdWithSoldItemsAsync(long invoiceId, CancellationToken cancellationToken = default)
  {
      return await _skyressDbContext.Invoices
          .Include(i => i.SoldItems)
          .FirstOrDefaultAsync(i => i.Id == invoiceId, cancellationToken);
  }
  ```

  If `using Microsoft.EntityFrameworkCore;` is not already at the top of the file, add it.

- [x] T006 Implement `GetByInvoiceIdAsync` in `Skyress.Infrastructure/Repository/PaymentRepository.cs`

  Open the file. The class currently has no methods. Add:

  ```csharp
  public async Task<Payment?> GetByInvoiceIdAsync(long invoiceId, CancellationToken cancellationToken = default)
  {
      return await GetAsync(p => p.InvoiceId == invoiceId).FirstOrDefaultAsync(cancellationToken);
  }
  ```

  If `using Microsoft.EntityFrameworkCore;` is not at the top, add it.

- [x] T007 Verify the project builds with no errors after T002–T006

  Run from repo root:
  ```powershell
  dotnet build
  ```
  Fix any compile errors before continuing. Common issue: if `_skyressDbContext` field name is wrong in T005, read `Skyress.Infrastructure/Repository/GenericRepository.cs` and use the correct name.

**Checkpoint**: Foundation complete — interface contracts defined, domain flags added, repositories implemented.

---

## Phase 3: User Story 1 — Duplicate Checkout Request Is Ignored (Priority: P1) 🎯 MVP

**Goal**: Calling `InitiateCheckout` twice on the same basket produces exactly one saga instance and one `CheckoutInitiated` event.

**Independent Test**: POST the checkout endpoint for the same basket twice. Query `CheckoutSagaData` — exactly one row exists. Query `Invoices` — exactly one invoice exists for the basket.

- [x] T008 [US1] Make `InitiateCheckoutCommandHandler.Handle` idempotent in `Skyress.Application/Baskets/Commands/InitiateCheckout/InitiateCheckoutCommandHandler.cs`

  Open the file. Replace the **entire body** of the `Handle` method with the following. Keep all existing `using` statements and the class/constructor unchanged:

  ```csharp
  public async Task<Result> Handle(InitiateCheckoutCommand request, CancellationToken cancellationToken)
  {
      var basket = await _basketRepository.GetBasketWithItemsAsync(request.BasketId);
      if (basket is null)
          throw new Exception();

      // Already fully checked out — reject
      if (basket.State == BasketState.CheckedOut)
          return Result.Failure(new Error("Basket.AlreadyCheckedOut", "Basket has already been checked out."));

      // Already in progress — republish with existing correlation ID (idempotent re-entry)
      if (basket.State == BasketState.Reserved
          && !string.IsNullOrEmpty(basket.CheckoutId)
          && Guid.TryParse(basket.CheckoutId, out var existingId))
      {
          await _publisher.Publish(new CheckoutInitiated(existingId, request.BasketId));
          return Result.Success();
      }

      if (basket.InitiateCheckout().IsFailure)
          throw new Exception();

      Guid correlationId = UpdateCheckoutId(basket);
      await _basketRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
      await _publisher.Publish(new CheckoutInitiated(correlationId, request.BasketId));
      return Result.Success();
  }
  ```

  Add `using Skyress.Domain.Enums;` at the top if not already there. The `UpdateCheckoutId` private method and all other members stay unchanged.

**Checkpoint**: User Story 1 complete. Duplicate checkout requests now return the existing correlation ID.

---

## Phase 4: User Story 2 — Each Checkout Step Is Safe to Retry (Priority: P2)

**Goal**: Retrying any consumer step (due to transient failure) produces the same outcome as the first execution — no duplicate data.

**Independent Test**: For each handler below, verify that calling the underlying command twice with the same inputs produces the same database state as calling it once.

- [x] T009 [US2] Make `ReserveItemsCommandHandler.Handle` idempotent in `Skyress.Application/Baskets/Commands/ReserveItems/ReserveItemsCommand.cs`

  Open the file. Replace the **entire body** of the `Handle` method. Keep the `GetItems` private method and all other members unchanged:

  ```csharp
  public async Task<Result> Handle(ReserveItemsCommand request, CancellationToken cancellationToken)
  {
      Basket? basket = await _basketRepository.GetBasketWithItemsAsync(request.BasketId);
      if (basket == null)
          throw new NullReferenceException();

      Dictionary<long, Item> items = await GetItems(basket);
      bool anyReserved = false;

      foreach (BasketItem basketItem in basket.BasketItems)
      {
          if (basketItem.IsReserved)
              continue; // already done — skip

          Item item = items[basketItem.ItemId];
          Result reserveResult = item.ReserveQuantity(basketItem.Quantity);
          if (reserveResult.IsFailure)
              throw new InvalidOperationException(reserveResult.Error.Message);

          basketItem.MarkAsReserved();
          anyReserved = true;
      }

      if (anyReserved)
          await _itemRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

      return Result.Success();
  }
  ```

- [x] T010 [P] [US2] Make `CreateInvoiceCommandHandler.Handle` idempotent in `Skyress.Application/Invoices/Commands/CreateInvoice/CreateInvoiceCommand.cs`

  Open the file. Add an idempotency check **at the top** of the `Handle` method body, before the `var basket = ...` line:

  ```csharp
  // Idempotency: return existing invoice if one already exists for this basket
  var existing = await _invoiceRepository.GetByBasketIdAsync(request.BasketId, cancellationToken);
  if (existing is not null)
  {
      _logger.LogInformation("{Command} skipped — invoice already exists. Id: {Id}", nameof(CreateInvoiceCommand), existing.Id);
      return Result.Success(InvoiceResponse.FromDomain(existing));
  }
  ```

  Everything after this block stays exactly as it is.

- [x] T011 [P] [US2] Make `AddSoldItemToInvoiceCommandHandler.Handle` idempotent in `Skyress.Application/Invoices/Commands/AddSoldItemToInvoice/AddSoldItemToInvoiceCommand.cs`

  Open the file. Replace the **entire body** of the `Handle` method. Keep all `using` statements and class members unchanged:

  ```csharp
  public async Task<Result<SoldItem>> Handle(AddSoldItemToInvoiceCommand request, CancellationToken cancellationToken)
  {
      _logger.LogInformation("Handling {Command}", nameof(AddSoldItemToInvoiceCommand));

      // Load invoice WITH its sold items so we can check for duplicates
      var invoice = await _invoiceRepository.GetByIdWithSoldItemsAsync(request.InvoiceId, cancellationToken);

      if (invoice is null)
          return Result<SoldItem>.Failure(new Error("Invoice.NotFound", "Invoice not found"));

      // Idempotency: skip if a sold item for this item already exists in the invoice
      var existingSoldItem = invoice.SoldItems.FirstOrDefault(si => si.ItemId == request.ItemId);
      if (existingSoldItem is not null)
      {
          _logger.LogInformation("{Command} skipped — SoldItem already exists for ItemId {ItemId}",
              nameof(AddSoldItemToInvoiceCommand), request.ItemId);
          return Result.Success(existingSoldItem);
      }

      var item = await _itemRepository.GetByIdAsync(request.ItemId, cancellationToken);
      if (item is null)
          return Result<SoldItem>.Failure(Error.Dummy);

      var soldItem = new SoldItem
      {
          Name = item.Name,
          Price = item.Price,
          Quantity = request.Quantity,
          TransactionType = request.TransactionType,
          SellingTime = DateTime.UtcNow,
          InvoiceId = request.InvoiceId,
          ItemId = request.ItemId,
          ItemCost = item.CostPrice,
      };

      invoice.AddSoldItem(soldItem);
      await _invoiceRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
      _logger.LogInformation("{Command} completed", nameof(AddSoldItemToInvoiceCommand));
      return Result.Success(soldItem);
  }
  ```

- [x] T012 [P] [US2] Make `BuildInvoiceFromBasketCommandHandler.UpdateInvoiceStatus` idempotent in `Skyress.Application/Invoices/Commands/BuildInvoiceFromBasketCommand/BuildInvoiceFromBasketCommand.cs`

  Open the file. Find the private method `UpdateInvoiceStatus`. Replace its **entire body** with:

  ```csharp
  private async Task UpdateInvoiceStatus(long invoiceId)
  {
      Invoice? invoice = await _invoiceRepository.GetByIdAsync(invoiceId);
      if (invoice == null)
          throw new Exception();

      // Idempotency: skip if already issued or beyond
      if (invoice.State >= InvoiceState.Issued)
          return;

      invoice.State = InvoiceState.Issued;
      await _invoiceRepository.UnitOfWork.SaveChangesAsync();
  }
  ```

  Add `using Skyress.Domain.Enums;` at the top if not already there. The `Handle` method and `AddSoldItemToInvoice` private method are unchanged (idempotency for item addition is handled by T011).

- [x] T013 [P] [US2] Make `CreatePaymentCommandHandler.Handle` idempotent in `Skyress.Application/Payments/Commands/CreatePayment/CreatePaymentCommand.cs`

  Open the file. After the existing null check for `invoice`, add the following idempotency block **before** the `var payment = new Payment` line:

  ```csharp
  // Idempotency: return existing payment if one already exists for this invoice
  var existingPayment = await _paymentRepository.GetByInvoiceIdAsync(request.InvoiceId, cancellationToken);
  if (existingPayment is not null)
  {
      _logger.LogInformation("{Command} skipped — payment already exists. Id: {Id}",
          nameof(CreatePaymentCommand), existingPayment.Id);
      return Result.Success(PaymentResponse.FromDomain(existingPayment));
  }
  ```

  Everything before (invoice null-check) and after (payment creation) stays unchanged.

- [x] T014 [P] [US2] Make `UpdateInvoiceStateCommandHandler.Handle` idempotent in `Skyress.Application/Invoices/Commands/UpdateInvoiceState/UpdateInvoiceStateCommand.cs`

  Open the file. After the null check for `invoice`, add the following block **before** the `invoice.State = request.State;` line:

  ```csharp
  // Idempotency: no-op if invoice is already in the target state
  if (invoice.State == request.State)
  {
      _logger.LogInformation("{Command} skipped — invoice already in state {State}",
          nameof(UpdateInvoiceStateCommand), request.State);
      return Result.Success(InvoiceResponse.FromDomain(invoice));
  }
  ```

  The rest of the method (setting state, saving, returning) stays unchanged.

- [x] T015 [P] [US2] Make `CompleteCheckoutCommandHandler.Handle` idempotent in `Skyress.Application/Baskets/Commands/CompleteCheckout/CompleteCheckoutCommand.cs`

  Open the file. Replace the **entire body** of the `Handle` method:

  ```csharp
  public async Task<Result> Handle(CompleteCheckoutCommand request, CancellationToken cancellationToken)
  {
      Result<Basket> result = await _basketRepository.GetBasketWithItemsAsync(request.BasketId);

      // Idempotency: no-op if already checked out
      if (result.Value.State == BasketState.CheckedOut)
          return Result.Success();

      result.Value.CompleteCheckout();
      await _basketRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
      return Result.Success();
  }
  ```

  Add `using Skyress.Domain.Enums;` at the top if not already there.

- [x] T016 [US2] Make `MarkItemsAsSoldCommandHandler.Handle` idempotent in `Skyress.Application/Items/Commands/MarkItemsAsSold/MarkItemsAsSoldCommand.cs`

  Open the file. Replace the **entire body** of the `Handle` method. Keep the class constructor and all members unchanged:

  ```csharp
  public async Task<Result> Handle(MarkItemsAsSoldCommand request, CancellationToken cancellationToken)
  {
      var basket = await _basketRepository.GetBasketWithItemsAsync(request.BasketId);
      if (basket == null)
          return Result.Failure(Error.Dummy);

      var itemIds = basket.BasketItems.Select(i => i.ItemId).ToList();
      var items = (await _itemRepository.GetByIdsAsync(itemIds)).ToDictionary(item => item.Id);
      bool anyMarked = false;

      foreach (var soldItem in basket.BasketItems)
      {
          if (soldItem.IsSold)
              continue; // already done — skip

          if (!items.TryGetValue(soldItem.ItemId, out var item))
              return Result.Failure(new Error("Item.NotFound", $"Item with ID {soldItem.ItemId} not found"));

          Result result = item.MarkAsSold(soldItem.Quantity);
          if (result.IsFailure)
              return Result.Failure(result.Error);

          soldItem.MarkAsSold();
          anyMarked = true;
      }

      if (anyMarked)
          await _itemRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

      return Result.Success();
  }
  ```

- [x] T017 [US2] Verify the project builds after T009–T016

  ```powershell
  dotnet build
  ```

**Checkpoint**: User Story 2 complete. Every consumer step is now safe to retry.

---

## Phase 5: User Story 3 — Checkout Resumes After System Fault (Priority: P3)

**Goal**: A saga interrupted mid-flight continues to completion after restart without re-running already-completed steps.

**Independent Test**: Advance a checkout to any mid-saga state. Restart the application. Deliver the next expected event. The saga continues from its last persisted state.

- [x] T018 [US3] Add `UseMessageRetry` to `Skyress.Infrastructure/Saga/SagaConfigurator.cs`

  Open the file. Find the `configurator.UsingRabbitMq(...)` callback. Add `cfg.UseMessageRetry(...)` **after** the existing `cfg.UseInMemoryOutbox(context);` line and **before** `cfg.ConfigureEndpoints(context);`:

  ```csharp
  cfg.UseInMemoryOutbox(context);

  // Retry transient failures up to 3 times with exponential back-off
  cfg.UseMessageRetry(r => r.Exponential(3,
      TimeSpan.FromSeconds(1),
      TimeSpan.FromSeconds(10),
      TimeSpan.FromSeconds(2)));

  cfg.ConfigureEndpoints(context);
  ```

  Only add the retry block. Do not change the host, username, password, outbox, or endpoint configuration.

**Checkpoint**: User Story 3 complete. Transient failures are retried automatically; saga state survives restarts via EF Core persistence.

---

## Phase 6: User Story 4 — Out-of-Order or Late Events Are Handled Safely (Priority: P4)

**Goal**: Events that arrive in a saga state where they are not expected are silently discarded without crashing or advancing state incorrectly.

**Independent Test**: Deliver a `PaymentCompleted` event to a saga in `ReservingItems` state. The saga stays in `ReservingItems` with no crash and no duplicate state transition.

- [x] T019 [US4] Add `Ignore()` declarations to `Skyress.Application/Checkout/Sagas/CheckoutStateMachine.cs`

  Open the file. Find the `SetCompletedWhenFinalized();` call at the very end of the constructor. Add the following **after** `SetCompletedWhenFinalized();`:

  ```csharp
  // Discard duplicate CheckoutInitiated events when saga is already active
  During(ReservingItems, InitiatingInvoice, BuildingInvoice, InitiatingPayment, PaymentPending, Finalizing,
      Ignore(CheckoutInitiated));

  // Discard ItemsReserved when already past that step
  During(InitiatingInvoice, BuildingInvoice, InitiatingPayment, PaymentPending, Finalizing,
      Ignore(ItemsReservedEvent));

  // Discard InvoiceInitiated when already past that step
  During(BuildingInvoice, InitiatingPayment, PaymentPending, Finalizing,
      Ignore(InvoiceInitiatedEvent));

  // Discard InvoiceCreated when already past that step
  During(InitiatingPayment, PaymentPending, Finalizing,
      Ignore(InvoiceCreatedEvent));

  // Discard PaymentInitiated when already past that step
  During(PaymentPending, Finalizing,
      Ignore(PaymentInitiatedEvent));

  // Discard PaymentCompleted when saga has already advanced past it (late delivery)
  During(Finalizing,
      Ignore(PaymentCompletedEvent));
  ```

  Do not change any existing `During` or `Initially` blocks above.

**Checkpoint**: User Story 4 complete. Late and duplicate events are explicitly discarded in all states.

---

## Phase 7: Polish & Cross-Cutting Concerns

- [x] T020 Run a full build to confirm no compile errors across all four projects

  ```powershell
  dotnet build
  ```

- [x] T021 Generate the EF Core migration for the two new `BasketItem` columns

  Run from repo root:
  ```powershell
  dotnet ef migrations add AddBasketItemIdempotencyFlags `
      --project Skyress.Infrastructure `
      --startup-project Skyress.Presentation
  ```

  After the command succeeds, open the generated migration file in `Skyress.Infrastructure/Migrations/` and confirm it contains `AddColumn` for both `IsReserved` and `IsSold` on `BasketItems`, with `defaultValue: false`.

- [ ] T022 Apply the migration to the development database

  ```powershell
  dotnet ef database update `
      --project Skyress.Infrastructure `
      --startup-project Skyress.Presentation
  ```

- [ ] T023 Run the smoke tests from `specs/003-checkout-saga-idempotency/quickstart.md`

  Follow the three manual test scenarios in `quickstart.md`:
  1. Idempotent checkout initiation — submit checkout twice, verify one saga row, one invoice row
  2. Consumer retry safety — verify `IsReserved = true` and `QuantityReserved` not doubled
  3. Saga resumability — verify `CurrentState` persists across restart

---

## Dependencies & Execution Order

### Phase Dependencies

- **Phase 1 (Setup)**: No dependencies — start immediately
- **Phase 2 (Foundational)**: Depends on Phase 1 — **BLOCKS all user story phases**
- **Phase 3 (US1)**: Depends on Phase 2 completion
- **Phase 4 (US2)**: Depends on Phase 2 completion (T009–T016 can run in parallel after Phase 2)
- **Phase 5 (US3)**: Depends on Phase 2 completion (independent of Phase 3 and 4)
- **Phase 6 (US4)**: Depends on Phase 2 completion (independent of Phase 3, 4, 5)
- **Phase 7 (Polish)**: Depends on Phases 3–6 completion

### User Story Dependencies

- **US1 (Phase 3)**: Independent after Phase 2
- **US2 (Phase 4)**: Independent after Phase 2; T009 and T016 depend on T002 (BasketItem flags)
- **US3 (Phase 5)**: Independent after Phase 2
- **US4 (Phase 6)**: Independent after Phase 2

### Parallel Opportunities Within Phase 4 (US2)

Tasks T010–T015 touch different files with no cross-dependencies. They can be executed in parallel:

```
T009 (ReserveItemsCommandHandler)     — must be first in US2: depends on BasketItem.IsReserved
T016 (MarkItemsAsSoldCommandHandler)  — must be last in US2: depends on BasketItem.IsSold

Parallel group (after T009):
  T010 CreateInvoiceCommandHandler
  T011 AddSoldItemToInvoiceCommandHandler
  T012 BuildInvoiceFromBasketCommandHandler
  T013 CreatePaymentCommandHandler
  T014 UpdateInvoiceStateCommandHandler
  T015 CompleteCheckoutCommandHandler
```

---

## Parallel Example: Phase 4 (US2)

```
Start T009 first (depends on BasketItem.IsReserved from T002)
Then launch in parallel:
  Task T010: CreateInvoiceCommandHandler idempotency
  Task T011: AddSoldItemToInvoiceCommandHandler idempotency
  Task T012: BuildInvoiceFromBasketCommandHandler idempotency
  Task T013: CreatePaymentCommandHandler idempotency
  Task T014: UpdateInvoiceStateCommandHandler idempotency
  Task T015: CompleteCheckoutCommandHandler idempotency
Then T016 (MarkItemsAsSoldCommandHandler — depends on BasketItem.IsSold from T002)
```

---

## Implementation Strategy

### MVP First (US1 Only)

1. Complete Phase 2: Foundational (T002–T007)
2. Complete Phase 3: US1 (T008)
3. **VALIDATE**: Submit checkout twice → confirm single saga instance
4. This alone eliminates the most visible failure: duplicate checkout initiation

### Incremental Delivery

1. Phase 2 → Phase 3 (US1): Stop duplicate saga creation
2. Phase 4 (US2): Make every step retry-safe
3. Phase 5 (US3): Add retry policy for automatic recovery
4. Phase 6 (US4): Suppress late-event warnings
5. Phase 7: Migration + smoke tests

### File Change Summary

| Task | File |
|------|------|
| T002 | `Skyress.Domain/Aggregates/Basket/BasketItem.cs` |
| T003 | `Skyress.Application/Contracts/Persistence/IInvoiceRepository.cs` |
| T004 | `Skyress.Application/Contracts/Persistence/IPaymentRepository.cs` |
| T005 | `Skyress.Infrastructure/Repository/InvoiceRepository.cs` |
| T006 | `Skyress.Infrastructure/Repository/PaymentRepository.cs` |
| T008 | `Skyress.Application/Baskets/Commands/InitiateCheckout/InitiateCheckoutCommandHandler.cs` |
| T009 | `Skyress.Application/Baskets/Commands/ReserveItems/ReserveItemsCommand.cs` |
| T010 | `Skyress.Application/Invoices/Commands/CreateInvoice/CreateInvoiceCommand.cs` |
| T011 | `Skyress.Application/Invoices/Commands/AddSoldItemToInvoice/AddSoldItemToInvoiceCommand.cs` |
| T012 | `Skyress.Application/Invoices/Commands/BuildInvoiceFromBasketCommand/BuildInvoiceFromBasketCommand.cs` |
| T013 | `Skyress.Application/Payments/Commands/CreatePayment/CreatePaymentCommand.cs` |
| T014 | `Skyress.Application/Invoices/Commands/UpdateInvoiceState/UpdateInvoiceStateCommand.cs` |
| T015 | `Skyress.Application/Baskets/Commands/CompleteCheckout/CompleteCheckoutCommand.cs` |
| T016 | `Skyress.Application/Items/Commands/MarkItemsAsSold/MarkItemsAsSoldCommand.cs` |
| T018 | `Skyress.Infrastructure/Saga/SagaConfigurator.cs` |
| T019 | `Skyress.Application/Checkout/Sagas/CheckoutStateMachine.cs` |
| T021–T022 | EF Core migration (shell commands) |

---

## Notes

- `[P]` = can run in parallel (different files, no shared in-flight state)
- `[USn]` = belongs to user story n from `specs/003-checkout-saga-idempotency/spec.md`
- Each task includes the exact code to write — do not add extra logic beyond what is shown
- Do not modify FluentValidation validators — no new commands are introduced
- Do not modify controllers or middleware — all changes are Application/Domain/Infrastructure
- Commit after each phase completes at minimum
