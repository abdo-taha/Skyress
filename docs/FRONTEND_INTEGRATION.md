# Skyress API — Frontend integration guide

This document describes how a web or mobile client should integrate with the Skyress backend. It reflects the **current** HTTP API and domain behavior.

**Payment:** The only supported payment flow exposed to clients is **cash**. The checkout saga creates a **cash** payment in `Initiated` state; the client must **confirm** it by posting the exact amount due (see [Complete cash payment](#5-complete-cash-payment)).

---

## Table of contents

1. [Base URL, versioning, and discovery](#1-base-url-versioning-and-discovery)
2. [JSON conventions](#2-json-conventions)
3. [Catalog and customers](#3-catalog-and-customers)
4. [Basket lifecycle](#4-basket-lifecycle)
5. [Checkout saga and cash payment](#5-checkout-saga-and-cash-payment)
6. [Invoices and order history](#6-invoices-and-order-history)
7. [Reference — main endpoints](#7-reference--main-endpoints)
8. [Operational requirements](#8-operational-requirements)
9. [Known API quirks](#9-known-api-quirks)

---

## 1. Base URL, versioning, and discovery

- **Default local URLs** (from launch settings): `http://localhost:5000` and `https://localhost:5001` (HTTPS may be disabled in code; prefer the URL your environment uses).
- **OpenAPI / Swagger** (Development): served at the application root (`/`). Use it to inspect schemas and try requests.

### API versioning

Versioning uses the **`api-version` query parameter** (see `QueryStringApiVersionReader` in the API host).

- **Default API version is `1.0`.** Many clients can call routes without `api-version` and still hit v1.
- If you receive version-related errors, append `?api-version=1.0` (or `2.0` only for the specific items route documented below).

Example:

```http
GET /api/items?api-version=1.0
```

### CORS

The API project does **not** currently register CORS middleware. If your frontend runs on another origin (e.g. `http://localhost:3000`), you will typically need either:

- a **dev proxy** (Vite, Next.js, etc.) that forwards `/api` to the backend, or  
- **CORS** to be added on the server for your deployment.

---

## 2. JSON conventions

- Request and response bodies use **JSON**.
- ASP.NET Core defaults to **camelCase** property names for JSON (e.g. `customerId`, `totalAmount`).
- **Enums** are serialized as **numeric** values unless you configure string enums on the server. The tables below list the important values.

### Enums used in checkout and payments

| Enum | Value | Meaning |
|------|------|---------|
| `BasketState` | `0` Active | Normal shopping |
| | `1` Reserved | Checkout started; stock reserved for this basket |
| | `2` CheckedOut | Completed |
| | `3` Draft | |
| | `4` Cancelled | |
| `InvoiceState` | `0` Draft | |
| | `1` Issued | |
| | `2` Paid | |
| | `3` PartiallyPaid | |
| | `4` Overdue | |
| | `5` Cancelled | |
| `PaymentState` | `0` Initiated | Awaiting cash confirmation via API |
| | `1` Paid | Cash recorded |
| | `2` Refunded | |
| | `3` PartiallyPaid | |
| | `4` Overdue | |
| `PaymentType` | `0` **Cash** | Only flow documented here |
| | `1` Installment | Not used by the current checkout saga path |
| `Unit` (items) | `0` Item | |
| | `1` CentiMeter | |
| | `2` Gram | |
| `CustomerState` | `0` Active | |
| | `1` Inactive | |
| | `2` OnHold | |
| | `3` Premium | |

---

## 3. Catalog and customers

### Items (catalog)

- **List items:** `GET /api/items`
- **Item by id (v1):** `GET /api/items/{id}`
- **Item by id (v2):** `GET /api/items/{id}?api-version=2.0` (v2 group only exposes this read).

Responses use the domain `Item` aggregate shape (e.g. `id`, `name`, `description`, `price`, `quantityLeft`, `unit`, etc.).

### Customers

- **Create:** `POST /api/customers`  
  Body (example): `{ "name": "...", "notes": "...", "state": 0 }`  
  Success: **201 Created** with `Location` pointing to the new customer; body includes the created `Customer` (with `id`).
- **Get by id:** `GET /api/customers/{id}`
- **List:** `GET /api/customers`
- **Update state:** `PATCH /api/customers/state` — body includes customer `id` and `state` (see `UpdateCustomerStateRequest`).
- **Update notes:** `PATCH /api/customers/notes` — body includes customer `id` and `notes`.

---

## 4. Basket lifecycle

Baskets link to a shopper via **`userId`** in the API model (set from `customerId` when creating a basket).

### Create basket

```http
POST /api/baskets
Content-Type: application/json

{ "customerId": 123 }
```

`customerId` may be `null` for anonymous carts (if your product allows it).

Success returns the created **`Basket`** entity (includes `id`, `userId`, `state`, etc.).

### Add line items

```http
POST /api/baskets/{id}/items
Content-Type: application/json

{ "itemId": 1, "quantity": 2 }
```

### Read basket (recommended for cart UI)

```http
GET /api/baskets/{id}
```

Returns a **`BasketDto`**: `id`, `userId`, `state`, and `items` with line-level `price`, `name`, `unit`, `quantity`, `itemId` (resolved from current item data).

### Other basket operations

| Action | Method and path |
|--------|------------------|
| Clear lines | `PATCH /api/baskets/{id}/clear` |
| Remove one item | `DELETE /api/baskets/{id}/items` with body `{ "itemId": 1 }` |
| Delete basket | `DELETE /api/baskets/{id}` |
| List by customer | `GET /api/baskets/customer/{customerId}` (`customerId` optional per OpenAPI) |
| List by state | `GET /api/baskets/state/{state}` |
| Cancel reservation | `POST /api/baskets/{id}/cancel-reservation` (use when abandoning checkout after reserve) |

---

## 5. Checkout saga and cash payment

Checkout is **asynchronous**: calling the API starts a **MassTransit saga** (reserve stock → invoice → **create cash payment** → wait for payment completion → finalize). The HTTP API does **not** stream saga progress; the UI should **poll** read models (invoice / payment) as described below.

### Step A — Start checkout

```http
POST /api/baskets/initiate-checkout?basketId={basketId}
```

There is no JSON body on this handler in the current code: **`basketId` is a query parameter.**

Effects:

- Basket moves toward checkout (domain: `InitiateCheckout`; basket `state` becomes **Reserved** when successful).
- A checkout correlation id is stored on the basket server-side (not returned in `BasketDto` today).

**Errors:** Failures may surface as **500** or **400** depending on validation and infrastructure; treat non-success as “checkout did not start.”

### Step B — Wait until a payment exists (polling)

The saga creates an **invoice** and then a **cash** `Payment` with `paymentState: 0` (`Initiated`) and `totalDue` equal to the invoice total.

The public **`BasketDto` does not include `invoiceId` or `paymentId`**, so clients should discover them indirectly:

1. **Poll** `GET /api/invoices/customer/{customerId}` (when the basket was created with that customer).
2. Find the invoice whose **`basketId`** matches the basket you checked out (the `Invoice` entity includes `basketId`).
3. Optionally use `GET /api/invoices/{invoiceId}/withPayments` to load **`payments`** in one call.

Alternatively:

- `GET /api/payments/invoice/{invoiceId}` returns a list of `Payment` rows for that invoice.

**Polling guidance:**

- Interval: e.g. **500 ms – 2 s**, with backoff and a maximum wait.
- Stop when you find a payment with `paymentType: 0` (Cash) and `paymentState: 0` (Initiated), or when the invoice/payment indicates the flow has moved on / failed (infer from `state` and HTTP errors).

### Step C — Complete cash payment (only supported payment action)

When you have the **`paymentId`** (from `invoice.paymentId`, from `GET /api/invoices/{id}/withPayments`, or from the payments list):

```http
POST /api/payments/{paymentId}/Pay
Content-Type: application/json

{ "amount": 99.99 }
```

**Rules (server-enforced):**

- `amount` must be **exactly** equal to the payment’s **`totalDue`** (same scale as invoice total). Mismatch returns **400 Bad Request**.
- Payment must be type **Cash** and state **Initiated**.

On success, the API returns **200 OK** and publishes internal events so the saga can finish checkout.

### Step D — Confirm completion (optional polling)

After cash is recorded:

- Payment `paymentState` becomes **Paid** (`1`).
- Invoice and basket states progress via saga; you can refresh:

`GET /api/baskets/{id}` — expect `state` **CheckedOut** (`2`) when the flow completes.

`GET /api/invoices/{invoiceId}` — inspect `state` and `paymentId`.

---

## 6. Invoices and order history

| Endpoint | Purpose |
|----------|---------|
| `GET /api/invoices` | All invoices |
| `GET /api/invoices/{id}` | Invoice header + sold lines (`soldItems`) |
| `GET /api/invoices/{id}/withPayments` | Invoice + `payments[]` with `totalDue`, `paymentState`, `paymentType` |
| `GET /api/invoices/customer/{customerId}` | Customer’s invoices (filter client-side by `basketId` / recency if needed) |
| `GET /api/invoices/state/{state}` | By `InvoiceState` numeric value |
| `PATCH /api/invoices/{id}/state` | Admin-style updates (body per `UpdateInvoiceStateRequest`) |
| `DELETE /api/invoices/{id}` | Delete |

---

## 7. Reference — main endpoints

Prefixes below omit `?api-version=1.0` for brevity.

### Baskets (`/api/baskets`)

| Method | Path | Notes |
|--------|------|--------|
| POST | `/` | Create; body `{ "customerId": long? }` |
| GET | `/{id}` | Cart DTO with line names/prices |
| POST | `/{id}/items` | Add line |
| PATCH | `/{id}/clear` | Clear lines |
| DELETE | `/{id}/items` | Remove line; body with `itemId` |
| DELETE | `/{id}` | Delete basket |
| GET | `/customer/{customerId?}` | List baskets |
| GET | `/state/{state}` | By basket state |
| POST | `/{id}/cancel-reservation` | Cancel reservation |
| POST | `/initiate-checkout?basketId={id}` | Start saga |

### Payments (`/api/payments`)

| Method | Path | Notes |
|--------|------|--------|
| GET | `/` | List payments |
| GET | `/{id}` | Payment by id |
| GET | `/invoice/{invoiceId}` | Payments for invoice |
| POST | `/{id}/Pay` | **Cash completion**; body `{ "amount": decimal }` |

### Items, customers, tags

See Swagger for full admin/catalog operations (`/api/items`, `/api/customers`, `/api/tags`, `/api/tagAssignments`, `/api/todos`).

**Note:** v1 items include a pricing-history route registered as `/api/items/pricingHistory{id}` (no slash before `{id}`), so the path literally looks like `.../pricingHistory42`.

---

## 8. Operational requirements

For checkout to **complete**, the API host must be running with:

- **PostgreSQL** (EF Core + saga persistence)
- **RabbitMQ** (MassTransit)

If the broker or consumers are down, checkout can remain stuck after `initiate-checkout` (e.g. reservation or later steps never complete). The frontend should surface timeouts and allow support flows (e.g. cancel reservation when appropriate).

---

## 9. Known API quirks

- **`initiate-checkout`** uses query string `basketId`, not the existing `InitiateCheckoutRequest` DTO (that type is unused by the endpoint today).
- **Basket DTO** omits `checkoutId` / `invoiceId` / `paymentId`; invoice discovery by **`basketId` + customer** (or global polling) is required.
- **No authentication** is described in the current host setup; treat as a trusted network or add auth before production.
- **CORS** is not configured; browser SPAs need a proxy or server CORS.
- **Cash** is the only payment path wired through the checkout saga; `Installment` exists in the enum but is not the saga’s automatic payment type today.

---

## Minimal integration checklist (cash checkout)

1. Create or select **customer** → obtain `customerId`.
2. **POST /api/baskets** with `customerId` → store `basketId`.
3. **POST /api/baskets/{basketId}/items** for each line.
4. **POST /api/baskets/initiate-checkout?basketId={basketId}`**.
5. Poll **GET /api/invoices/customer/{customerId}** until an invoice with matching **`basketId`** appears; read **`paymentId`** / payments.
6. Read **`totalDue`** from payment (or from invoice-with-payments).
7. **POST /api/payments/{paymentId}/Pay** with `{ "amount": <totalDue> }`.
8. Optionally verify **GET /api/baskets/{basketId}** → `state` **CheckedOut**.

This matches the backend behavior as implemented in the Skyress solution.
