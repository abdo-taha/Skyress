# Quickstart: Checkout Saga Idempotency & Fault Tolerance

## Prerequisites

- PostgreSQL running (same as normal dev setup)
- RabbitMQ running (same as normal dev setup)
- .NET SDK installed

## Apply the Database Migration

After implementation, apply the EF migration to add the two new `BasketItem` columns:

```powershell
dotnet ef migrations add AddBasketItemIdempotencyFlags --project Skyress.Infrastructure --startup-project Skyress.Presentation
dotnet ef database update --project Skyress.Infrastructure --startup-project Skyress.Presentation
```

## Run the Application

```powershell
dotnet run --project Skyress.Presentation
```

## Manual Smoke Test: Idempotent Checkout Initiation

1. Create a basket and add items (use existing API).
2. Call `POST /baskets/{id}/checkout` twice in rapid succession (or twice in a row).
3. **Expected**: Both calls return success. Exactly **one** saga instance exists in the `CheckoutSagaData` table. Exactly **one** invoice exists for the basket.

```sql
-- Verify single saga instance:
SELECT * FROM "CheckoutSagaData" WHERE "BasketId" = <your_basket_id>;

-- Verify single invoice:
SELECT * FROM "Invoices" WHERE "BasketId" = <your_basket_id>;
```

## Manual Smoke Test: Consumer Retry Safety

1. Start a checkout.
2. Temporarily break the `ReserveItemsCommand` (e.g., add a throw after the reservation loop but before `SaveChanges`).
3. Let MassTransit retry — the retry will hit the same handler.
4. After removing the break, observe that items have `IsReserved = true` and `QuantityReserved` was only incremented once.

```sql
-- Verify reservation applied once per item:
SELECT "IsReserved", "IsSold" FROM "BasketItems" WHERE "BasketId" = <your_basket_id>;

-- Verify QuantityReserved not doubled:
SELECT "Name", "QuantityReserved" FROM "Items" WHERE "Id" IN (
    SELECT "ItemId" FROM "BasketItems" WHERE "BasketId" = <your_basket_id>
);
```

## Manual Smoke Test: Saga Resumability

1. Start a checkout. Let it advance to at least `ReservingItems`.
2. Stop the application mid-saga.
3. Restart the application.
4. Deliver the next expected event (e.g., if the saga was waiting on `ItemsReserved`, the consumer will republish it on the next processing cycle via MassTransit retry).
5. **Expected**: The saga continues from `InitiatingInvoice` without re-running the reservation step.

```sql
-- Verify saga state survived restart:
SELECT "CurrentState", "InvoiceId", "PaymentId" FROM "CheckoutSagaData" WHERE "CorrelationId" = '<correlation-id>';
```

## Key Files Changed by This Feature

```
Skyress.Domain/Aggregates/Basket/BasketItem.cs
Skyress.Application/Contracts/Persistence/IInvoiceRepository.cs
Skyress.Application/Contracts/Persistence/IPaymentRepository.cs
Skyress.Application/Baskets/Commands/InitiateCheckout/InitiateCheckoutCommandHandler.cs
Skyress.Application/Baskets/Commands/ReserveItems/ReserveItemsCommand.cs
Skyress.Application/Baskets/Commands/CompleteCheckout/CompleteCheckoutCommand.cs
Skyress.Application/Items/Commands/MarkItemsAsSold/MarkItemsAsSoldCommand.cs
Skyress.Application/Invoices/Commands/CreateInvoice/CreateInvoiceCommand.cs
Skyress.Application/Invoices/Commands/AddSoldItemToInvoice/AddSoldItemToInvoiceCommand.cs
Skyress.Application/Invoices/Commands/BuildInvoiceFromBasketCommand/BuildInvoiceFromBasketCommand.cs
Skyress.Application/Invoices/Commands/UpdateInvoiceState/UpdateInvoiceStateCommand.cs
Skyress.Application/Payments/Commands/CreatePayment/CreatePaymentCommand.cs
Skyress.Application/Checkout/Sagas/CheckoutStateMachine.cs
Skyress.Infrastructure/Repository/InvoiceRepository.cs
Skyress.Infrastructure/Repository/PaymentRepository.cs
Skyress.Infrastructure/Saga/SagaConfigurator.cs
Skyress.Infrastructure/Migrations/  (new migration)
```
