# Feature Specification: Checkout Saga Idempotency & Fault Tolerance

**Feature Branch**: `003-checkout-saga-idempotency`
**Created**: 2026-05-09
**Status**: Draft
**Input**: User description: "handle idempotency in checkout saga and make sure the request can be resumed and fault tolerant"

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Duplicate Checkout Request Is Ignored (Priority: P1)

A customer clicks "Checkout" but their browser submits the request twice due to a slow network response. The second submission arrives while the first is still being processed. Only one checkout flow runs — the second request is silently discarded rather than creating a duplicate order, duplicate invoice, or charging the customer twice.

**Why this priority**: Duplicate processing is the most direct and visible failure mode. It results in data corruption (double-reserved inventory, double-charged payments) and is the core of what "idempotency" means in this context. All other stories build on this guarantee.

**Independent Test**: Can be fully tested by submitting two `InitiateCheckout` requests for the same basket in rapid succession and confirming that exactly one saga instance runs to completion, exactly one invoice is created, and exactly one payment is initiated.

**Acceptance Scenarios**:

1. **Given** a basket with an in-progress checkout (saga active), **When** a second `InitiateCheckout` request arrives for the same basket, **Then** the second request returns the same checkout identifier and no new saga is started.
2. **Given** a checkout that has already completed, **When** `InitiateCheckout` is called again for the same basket, **Then** the request is rejected with a clear indication that the basket is already checked out.
3. **Given** a checkout correlation ID that already exists in the saga store, **When** a duplicate `CheckoutInitiated` event is published with the same ID, **Then** the saga ignores the duplicate and remains in its current state.

---

### User Story 2 - Each Checkout Step Is Safe to Retry (Priority: P2)

During checkout, any individual step (reserving inventory, creating an invoice, initiating payment, finalizing the order) may be retried by the messaging infrastructure due to transient failures. Retrying a step that already succeeded must produce the same outcome as the first execution — no duplicate reservations, no duplicate invoices, no double payments.

**Why this priority**: MassTransit automatically retries failed consumers. Without per-step idempotency, a single transient database error at any point in the saga could corrupt the order by duplicating a side effect that already succeeded.

**Independent Test**: Can be tested for each consumer individually by triggering its event twice with the same correlation ID and verifying that the resulting data state is identical to a single execution (e.g., exactly one invoice row, inventory reserved once, payment record created once).

**Acceptance Scenarios**:

1. **Given** the `ReserveItemsRequested` event has already been processed successfully for a basket, **When** the same event is delivered a second time, **Then** inventory counts remain unchanged and the saga advances normally.
2. **Given** an invoice has already been created for a correlation ID, **When** `InitiateInvoiceRequested` or `BuildInvoiceRequested` is consumed again, **Then** no duplicate invoice is created and the existing invoice ID is returned.
3. **Given** a payment has already been initiated for a correlation ID, **When** `CreatePaymentRequested` is consumed again, **Then** no duplicate payment record is created and the existing payment ID is returned.
4. **Given** all finalization steps (invoice paid, basket closed, items sold) have already run, **When** `FinalizeCheckoutRequested` is consumed again, **Then** the state remains consistent and no data is duplicated.

---

### User Story 3 - Checkout Resumes After System Fault (Priority: P3)

The system experiences a fault (process restart, database hiccup, message broker reconnect) while a checkout is mid-flight — for example, after items are reserved but before the invoice is created. When the system recovers, the checkout saga resumes from its last confirmed state and continues to completion without restarting steps that already succeeded.

**Why this priority**: Without resumability, any infrastructure interruption mid-checkout leaves the customer in a broken state (inventory reserved, basket locked, no order placed) with no automatic recovery path. Manual intervention would be required for every interrupted checkout.

**Independent Test**: Can be tested by processing a checkout up to a specific state (e.g., `ReservingItems`), forcibly restarting the application, then delivering the next expected event and verifying the saga continues from `InitiatingInvoice` rather than restarting from the beginning.

**Acceptance Scenarios**:

1. **Given** a saga has persisted its state as `ReservingItems`, **When** the application restarts and the `ItemsReserved` event is delivered, **Then** the saga transitions to `InitiatingInvoice` and continues normally.
2. **Given** a saga has persisted its state as `PaymentPending`, **When** the application restarts and the `PaymentCompleted` event is delivered, **Then** the saga transitions to `Finalizing` and completes successfully.
3. **Given** a saga in any intermediate state, **When** the same triggering event is redelivered after recovery, **Then** the saga processes it exactly once and advances to the next state.

---

### User Story 4 - Out-of-Order or Late Events Are Handled Safely (Priority: P4)

A result event (e.g., `PaymentCompleted`) arrives before the saga has reached the expected waiting state, or arrives after the saga has already advanced past that state. The system handles these race conditions without getting stuck or processing the event twice.

**Why this priority**: Distributed messaging does not guarantee strict ordering. Without handling late or early events, the saga can silently drop completions or re-process steps, leaving checkouts permanently stuck.

**Independent Test**: Can be tested by delivering a result event (e.g., `ItemsReserved`) when the saga is not in the `ReservingItems` state, and verifying that the saga neither crashes nor incorrectly advances its state.

**Acceptance Scenarios**:

1. **Given** a saga in the `Completed` state, **When** a `PaymentCompleted` event arrives (late delivery), **Then** the event is discarded without error and no side effect is triggered.
2. **Given** a saga in the `ReservingItems` state, **When** a `PaymentCompleted` event arrives early, **Then** the event is held or discarded without corrupting the saga state.
3. **Given** a saga that receives the same result event twice within a short window (e.g., messaging broker retries at the broker level), **Then** only one state transition occurs and no duplicate downstream events are published.

---

### Edge Cases

- What happens if `InitiateCheckout` is called for a basket that is currently in the `Finalizing` state — nearly complete but not yet marked done?
- How does the system behave if the saga store (database) is unavailable when a result event arrives?
- What if inventory was reserved but the invoice creation repeatedly fails — does inventory remain locked indefinitely?
- What happens if a `PaymentCompleted` event arrives for an unknown or already-completed correlation ID?
- Can a checkout be cancelled mid-saga (e.g., basket deleted) and what happens to in-progress steps?

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: System MUST guarantee that initiating checkout for a basket with an active or completed saga returns the existing checkout identifier rather than starting a new saga.
- **FR-002**: System MUST store and persist saga state after each successful state transition so that recovery after a fault does not restart already-completed steps.
- **FR-003**: The `ReserveItemsCommand` MUST be idempotent — re-executing it for a basket whose items are already reserved must produce no change in inventory state.
- **FR-004**: The invoice creation step MUST be idempotent — a second call with the same basket and correlation context must return the existing invoice rather than creating a duplicate.
- **FR-005**: The payment initiation step MUST be idempotent — a second call with the same invoice reference must return the existing payment rather than creating a duplicate charge.
- **FR-006**: The finalization step MUST be idempotent — re-running basket completion, invoice state update, and item sale marking must result in a no-op if those transitions have already occurred.
- **FR-007**: The saga MUST ignore events that arrive in a state where they are not expected (wrong current state), without crashing or corrupting saga data.
- **FR-008**: The saga MUST ignore duplicate events that carry the same correlation ID and arrive after the saga has already transitioned past the state that event triggers.
- **FR-009**: The `InitiateCheckout` command MUST be idempotent — multiple identical submissions for the same basket MUST NOT create multiple saga instances.
- **FR-010**: System MUST provide a way to detect that a checkout is already in progress for a given basket and communicate this clearly to the caller.

### Key Entities

- **CheckoutSagaData**: The persisted state of a single checkout flow, identified by a stable correlation ID tied to the basket. Records the current state and all resource IDs produced during the flow (InvoiceId, PaymentId).
- **Basket**: The shopping cart being checked out; holds the stable `CheckoutId` used as the saga correlation ID to prevent duplicate saga creation.
- **Invoice**: The billing record produced during checkout; must be created at most once per checkout correlation.
- **Payment**: The payment record initiated during checkout; must be created at most once per invoice.
- **CheckoutIdempotencyKey**: A stable identifier (the basket's `CheckoutId`) that ties a basket to exactly one saga instance across all retries and recoveries.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Submitting `InitiateCheckout` twice for the same basket results in exactly one completed order — zero duplicate invoices, zero duplicate payments, 100% of the time.
- **SC-002**: A checkout saga interrupted at any state completes successfully after system recovery with zero manual intervention required.
- **SC-003**: Any individual consumer step can be retried up to 5 times without producing more than one side effect.
- **SC-004**: Duplicate events delivered to the saga (same correlation ID, same event type) are silently discarded — 0% result in an extra state transition or downstream publish.
- **SC-005**: A checkout that has been interrupted and resumed completes within the same total time budget as an uninterrupted checkout (no additional user-visible delay beyond normal processing time).
- **SC-006**: 100% of active checkout sagas survive an application restart and continue to completion when the next expected event is delivered.

## Assumptions

- The saga persistence backend (EF Core + database) is already in place via `CheckoutSagaDbConfiguration`; this feature extends its idempotency guarantees, not its storage mechanism.
- MassTransit's built-in saga concurrency control (optimistic or pessimistic locking on the saga store) is the primary guard against concurrent event processing for the same correlation ID; this feature ensures the business logic above that layer is also idempotent.
- Compensation (rollback of already-completed steps on failure) is out of scope for this feature; the focus is forward-progress idempotency and resumability.
- The `CheckoutId` field already present on the `Basket` aggregate is the stable idempotency key and serves as the saga correlation ID — no new identifier is introduced.
- Items are considered "already reserved" if their status in the domain reflects a reserved state; the `ReserveItemsCommand` handler is responsible for detecting and skipping this.
- Out-of-scope: idempotency for the external payment gateway (assumed handled by the gateway itself or a separate payment integration feature).
- The messaging broker (MassTransit with its configured transport) guarantees at-least-once delivery; this feature ensures the consumers tolerate duplicate delivery.
- A single user may only have one active checkout per basket at a time.
