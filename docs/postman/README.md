# Postman Collections

## Skyress Full API + Checkout Flow

Import `Skyress_Checkout_Flow.postman_collection.json` into Postman.

Before running:

1. Start the API, PostgreSQL, and RabbitMQ.
2. Set `baseUrl`, usually `http://localhost:5000`.
3. Run `Register Customer` or `Login Customer`.
4. Run `Login Admin (optional)` using the default development admin, or set `adminToken` manually.
5. Either run `Create Item (Admin)` or set `itemId` manually to an existing item with enough stock.

The collection includes folders for:

- Auth
- Items
- Customers
- Baskets
- Invoices
- Payments
- Tags
- Tag assignments
- Todos
- Checkout flow runner

Recommended checkout run order:

1. `00 - Auth`
2. `09 - Checkout Flow Runner`

The saga is asynchronous. After `Initiate Checkout`, retry these requests until the expected data appears:

- `6. Find Invoice By Customer (Admin, retry)`
- `7. Get Invoice With Payments (Admin, retry)`
- `9. Verify Basket Checked Out (retry)`

Notes:

- Invoice/payment endpoints require an Admin token in the current API route registration.
- The default development admin is `admin@skyress.local` / `Admin@12345!` when `DefaultAdmin` config is enabled.
- `BasketState.CheckedOut` is numeric value `2`.
- `PaymentType.Cash` is numeric value `0`.
- `PaymentState.Initiated` is numeric value `0`; `PaymentState.Paid` is numeric value `1`.
