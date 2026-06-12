# Checkout Flow README

This document explains the business checkout flow from the caller's point of view.

## Participants

- Customer: owns or uses a basket.
- Basket: contains the intended item lines.
- Item: inventory that can be reserved and sold.
- Invoice: bill created from the basket.
- Payment: cash payment created for the invoice.
- Checkout saga: background process that coordinates the multi-step workflow.

## Happy Path

1. A customer has an active basket.
2. The caller adds one or more items to the basket.
3. The caller starts checkout with the basket id.
4. Skyress reserves the basket items.
5. Skyress creates an invoice for the basket.
6. Skyress builds invoice sold-item lines from the basket.
7. Skyress creates a cash payment in `Initiated` state.
8. The caller completes the cash payment with the exact amount due.
9. Skyress marks the invoice paid.
10. Skyress completes the basket checkout.
11. Skyress marks the reserved items as sold.
12. The basket reaches `CheckedOut`.

## Step Details

### 1. Create Basket

```http
POST /api/baskets
Content-Type: application/json

{ "customerId": 123 }
```

The basket starts as the customer's active cart.

### 2. Add Items

```http
POST /api/baskets/{basketId}/items
Content-Type: application/json

{ "itemId": 10, "quantity": 2 }
```

The item remains normal inventory until checkout reservation runs.

### 3. Start Checkout

```http
POST /api/baskets/initiate-checkout?basketId={basketId}
```

What happens:

- The basket domain model checks that checkout can begin.
- A checkout correlation id is stored on the basket.
- A `CheckoutInitiated` event is published.
- If checkout is already in progress for the basket, the existing correlation id is reused.

### 4. Reserve Items

The saga asks the application to reserve all basket items.

What happens:

- Stock rules are checked in the domain model.
- Invalid reservations fail before stock is changed.
- Successful reservations move quantities from available to reserved.
- The saga advances only after `ItemsReserved` is published.

### 5. Create Invoice

The saga creates an invoice for the basket.

What happens:

- The invoice is linked to the basket.
- The hardening design expects one checkout invoice per basket.
- Duplicate invoice creation should resolve to the existing invoice once the remaining idempotency tasks are complete.

### 6. Build Invoice Lines

The saga builds invoice sold-item lines from basket contents.

What happens:

- Basket item quantities and prices are copied into invoice sold items.
- The invoice total is calculated from those lines.
- Duplicate sold-item work is intended to be protected by invoice/item uniqueness.

### 7. Create Cash Payment

The saga creates a payment for the invoice.

What happens:

- The payment type is `Cash`.
- The payment state starts as `Initiated`.
- The total due matches the invoice total.
- The saga moves to `PaymentPending`.

### 8. Complete Cash Payment

```http
POST /api/payments/{paymentId}/Pay
Content-Type: application/json

{ "amount": 99.99 }
```

What happens:

- The payment must exist.
- The payment must be cash.
- The payment must still be initiated.
- The submitted amount must exactly equal `totalDue`.
- On success, payment state becomes `Paid`.
- A `PaymentCompletedEvent` is published.

### 9. Finalize Checkout

After payment completion, the saga finalizes the workflow.

What happens:

- The invoice is marked `Paid`.
- The basket is marked `CheckedOut`.
- Reserved item quantities are marked sold.
- A `FinalizedCheckout` event completes the saga.

## Failure And Retry Notes

- If a basket is already checked out, checkout initiation fails.
- If a basket is already reserved and has a checkout id, checkout initiation republishes the existing saga event.
- If RabbitMQ is unavailable, checkout can start but fail to advance.
- If payment is not completed, the saga remains in `PaymentPending`.
- If invoice/payment/sold-item duplicates race, the target design is to return existing successful results. Some repository conflict handling tasks are still open.

## States To Watch

Basket:

```text
Active -> Reserved -> CheckedOut
```

Invoice:

```text
Draft/Issued -> Paid
```

Payment:

```text
Initiated -> Paid
```
