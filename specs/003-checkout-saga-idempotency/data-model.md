# Data Model: Checkout Saga Idempotency & Fault Tolerance

## Summary of Changes

This feature adds two boolean tracking flags to an existing entity. No new tables or relationships are introduced.

---

## Modified Entity: `BasketItem`

**File**: `Skyress.Domain/Aggregates/Basket/BasketItem.cs`

| Field | Type | Default | Nullable | Purpose |
|-------|------|---------|----------|---------|
| `IsReserved` | `bool` | `false` | No | Tracks whether `ReserveQuantity` has been applied for this basket item in this checkout |
| `IsSold` | `bool` | `false` | No | Tracks whether `MarkAsSold` has been applied for this basket item in this checkout |

### Behaviour

- `IsReserved` is set to `true` by `BasketItem.MarkAsReserved()` inside `ReserveItemsCommandHandler` after `item.ReserveQuantity()` succeeds.
- `IsSold` is set to `true` by `BasketItem.MarkAsSold()` inside `MarkItemsAsSoldCommandHandler` after `item.MarkAsSold()` succeeds.
- Both flags start as `false` on creation.
- Both are write-once (no un-reserve or un-sell via these flags — those use domain methods on `Item`).

### Domain Addition (exact code)

```csharp
// Add to BasketItem.cs:
public bool IsReserved { get; private set; }
public bool IsSold { get; private set; }

public void MarkAsReserved() => IsReserved = true;
public void MarkAsSold() => IsSold = true;
```

---

## DB Schema Change (EF Core Migration)

**Table**: `BasketItems`

```sql
ALTER TABLE "BasketItems" ADD "IsReserved" boolean NOT NULL DEFAULT false;
ALTER TABLE "BasketItems" ADD "IsSold" boolean NOT NULL DEFAULT false;
```

Existing rows will receive `false` for both columns, which is the correct default (no ongoing checkouts at migration time is assumed; any interrupted saga will re-run its consumer and re-evaluate the flag).

**Migration name (suggested)**: `AddBasketItemIdempotencyFlags`

---

## No Changes to `CheckoutSagaData`

The saga state machine already persists `CorrelationId`, `CurrentState`, `BasketId`, `InvoiceId`, and `PaymentId`. No new columns are needed — all idempotency checks query domain entities directly.

---

## Repository Interface Additions

These are contract changes, not schema changes.

### `IInvoiceRepository`

```csharp
// Add to Skyress.Application/Contracts/Persistence/IInvoiceRepository.cs:
Task<Invoice?> GetByBasketIdAsync(long basketId, CancellationToken cancellationToken = default);
Task<Invoice?> GetByIdWithSoldItemsAsync(long invoiceId, CancellationToken cancellationToken = default);
```

### `IPaymentRepository`

```csharp
// Add to Skyress.Application/Contracts/Persistence/IPaymentRepository.cs:
Task<Payment?> GetByInvoiceIdAsync(long invoiceId, CancellationToken cancellationToken = default);
```
