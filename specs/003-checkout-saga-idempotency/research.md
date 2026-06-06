# Research: Checkout Saga Idempotency & Fault Tolerance

## Decision 1: Idempotency Pattern Per Step

**Decision**: "Check-before-execute" pattern — each handler queries existing state before writing.

**Rationale**: The saga persists state after every transition (EF Core saga store). Each consumer receives a result event and re-publishes only after the command succeeds. So the natural place to detect duplicates is inside the command handler itself, before doing any mutation. This requires no new infrastructure and works cleanly with the existing `Result<T>` pattern.

**Alternatives considered**:
- Outbox deduplication table — more infrastructure, unnecessary given the existing in-memory outbox
- Saga data flags (e.g., `ItemsReserved: bool`) — requires schema change on `CheckoutSagaData` and couples saga data to business state; rejected because handlers already have access to domain state

---

## Decision 2: `BasketItem.IsReserved` and `BasketItem.IsSold` Flags

**Decision**: Add `IsReserved` and `IsSold` boolean flags to `BasketItem`.

**Rationale**: `Item.QuantityReserved` is a global counter across all baskets. There's no existing per-basket-item tracking of whether reservation or sale has been applied for this specific checkout. Adding flags to `BasketItem` is the minimal domain change: it localises the "was this step done for this basket item?" check without touching the global `Item` aggregate.

**Alternatives considered**:
- Check `Item.QuantityReserved >= basketItem.Quantity` — fails when other baskets also hold reservations for the same item; not reliably basket-specific
- Transaction log / event store — overengineered for this scope

---

## Decision 3: Duplicate Saga Initiation

**Decision**: In `InitiateCheckoutCommandHandler`, if the basket is already in `Reserved` state with a non-empty `CheckoutId`, skip the domain transition and just republish `CheckoutInitiated` with the existing correlation ID.

**Rationale**: The basket domain method `InitiateCheckout()` rejects re-entry for `Reserved` baskets. The existing `UpdateCheckoutId` helper already preserves the correlation ID. The only missing piece is skipping the domain transition call (and DB save) when the basket is already reserved — and still republishing so the saga (which may have lost its first event) can resume.

**Alternatives considered**:
- Reject the duplicate with an error — creates a bad UX when the client legitimately retries after a timeout
- Let the saga handle it via `Ignore()` alone — doesn't help if the first `CheckoutInitiated` was never received by the saga

---

## Decision 4: Duplicate Events in the Saga State Machine

**Decision**: Add explicit `Ignore()` handlers in MassTransit `During()` blocks for events that arrive in wrong states.

**Rationale**: MassTransit 8 does not crash on unhandled events in non-matching states — it discards them silently but may log a warning. Explicit `.Ignore()` in `During()` blocks documents intent, suppresses spurious warning log entries, and prevents future breakage if the default behavior changes.

**Alternatives considered**:
- `DuringAny(Ignore(event))` — conflicts with `Initially(When(event))` because `DuringAny` includes the Initial state
- No change (rely on MassTransit default) — works today but not self-documenting; also `CheckoutInitiated` TODO in the state machine itself shows the team expected to add this

---

## Decision 5: Retry Policy

**Decision**: Add `UseMessageRetry` with a 3-retry exponential back-off in `SagaConfigurator`.

**Rationale**: The existing `UseInMemoryOutbox` already guarantees that published messages are held until the consumer's DB save succeeds (publish-after-commit). Adding a retry policy completes the fault tolerance story: transient DB or network errors will be retried automatically before the message is moved to the dead-letter queue.

**Alternatives considered**:
- Per-consumer retry configuration — more granular but requires touching every consumer class; bus-level retry covers all consumers uniformly

---

## Decision 6: `BuildInvoiceFromBasket` Idempotency

**Decision**: Load the invoice with its `SoldItems` collection before the loop in `AddSoldItemToInvoiceCommandHandler`. If a `SoldItem` for the same `ItemId` already exists in the invoice, skip that item.

**Rationale**: `AddSoldItemToInvoice` is called once per basket item. If the handler is retried, it will try to add the same sold items again — duplicating `SoldItem` rows and inflating `Invoice.TotalAmount`. Checking the already-persisted `SoldItems` before inserting is the correct check-before-execute guard here.

**Alternatives considered**:
- Check `Invoice.SoldItems.Count > 0` and skip the whole build step — loses partial retry safety (if 2 of 5 items were added before crash, the remaining 3 would not be added on retry)
- Unique constraint on `(InvoiceId, ItemId)` in the DB — valid defensive layer but doesn't change the handler behavior; it would throw instead of skipping cleanly

---

## MassTransit Configuration Already in Place

- `UseInMemoryOutbox` ✅ — messages published inside consumers are held until DB commit
- EF Core saga repository ✅ — saga state persists across restarts
- PostgreSQL with optimistic concurrency ✅ — prevents two concurrent events from double-processing the same saga instance
- `SetKebabCaseEndpointNameFormatter` ✅ — consistent queue naming
