# Skyress Integration Guide

This guide is for developers integrating another service, frontend, or operations environment with Skyress.

For endpoint-by-endpoint client examples, see `docs/FRONTEND_INTEGRATION.md`. This document focuses on runtime dependencies, authentication, checkout integration, error behavior, and operational expectations.

## Runtime Dependencies

Skyress needs:

- PostgreSQL for application data, saga persistence, users, tokens, invoices, payments, baskets, and inventory.
- RabbitMQ for MassTransit checkout saga messages.
- A JWT secret configured outside source control.

Development defaults live in `Skyress/appsettings.Development.json`:

```text
SkyressDb = Host=localhost;port=5432;Database=Skyress;Username=Skyress;Password=Skyress;Include Error Detail=true
RabbitMq  = rabbitmq://localhost
Jwt:SecretKey = development-only value for local token generation
DefaultAdmin = development-only admin credentials for protected admin endpoints
```

MassTransit currently connects to RabbitMQ with `guest` / `guest` in `Skyress.Infrastructure/Saga/SagaConfigurator.cs`. Change this before production.

## Authentication And Authorization

Auth endpoints:

```text
POST /api/v1/auth/register
POST /api/v1/auth/login
POST /api/v1/auth/refresh
POST /api/v1/auth/logout
```

These endpoints allow anonymous access. Other endpoints are affected by the API host's authenticated fallback policy. Invoice and payment endpoints also require the `Admin` role in the current route registration.

Integrator expectations:

- Include `Authorization: Bearer <accessToken>` for protected business APIs.
- Use an Admin role token for invoice and payment reads, invoice updates, and cash payment completion.
- Store refresh tokens safely client-side. The hardened backend stores refresh-token hashes at rest.

## API Versioning

Most routes are registered as API version `1.0`, with default versioning enabled through the `api-version` query string.

Example:

```http
GET /api/baskets/10?api-version=1.0
Authorization: Bearer <token>
```

## Checkout Integration Contract

Checkout is asynchronous. The API call starts the workflow, then RabbitMQ and the saga complete the remaining steps.

1. Create or select a customer.
2. Create a basket.
3. Add basket items.
4. Start checkout:

   ```http
   POST /api/baskets/initiate-checkout?basketId={basketId}
   Authorization: Bearer <token>
   ```

5. Poll invoice/payment read endpoints with an Admin token, or provide a backend-for-frontend/service layer that does this safely.
6. Complete the generated cash payment:

   ```http
   POST /api/payments/{paymentId}/Pay
   Authorization: Bearer <adminToken>
   Content-Type: application/json

   { "amount": 99.99 }
   ```

7. Poll the basket until it reaches `CheckedOut`.

The checkout saga is described in detail in `docs/CHECKOUT_SAGA_README.md`.

## Error Behavior

The DDD hardening work standardizes expected business failures:

- Domain methods throw typed domain exceptions for expected rule violations.
- Application handlers convert typed domain exceptions into `Result` failures.
- Targeted API result mapping returns HTTP 422 with stable business error codes/messages.
- Unexpected failures remain generic server errors.

Not every legacy endpoint has been fully converted yet. Check `specs/004-ddd-pattern-hardening/tasks.md` before relying on a specific endpoint's final hardened behavior.

## Idempotency And Retry Behavior

Designed guarantees:

- Duplicate checkout initiation for a reserved basket republishes the existing checkout correlation id.
- Duplicate invoice creation for the same basket should return the existing invoice.
- Duplicate payment creation for the same invoice should return the existing payment.
- Duplicate sold-item creation for the same invoice/item pair should return the existing sold item.
- Migration preconditions should stop if historical duplicates would violate new unique constraints.

Current status:

- EF unique indexes and migration precondition work exist.
- Some repository conflict handling and integration tests are still open in the active Spec Kit task list.

For external retry logic, use bounded retries with backoff and treat duplicated checkout/payment operations as eventually consistent until the remaining US3 tasks are complete.

## Operations Checklist

- PostgreSQL is reachable before the API starts.
- RabbitMQ is reachable before checkout traffic is accepted.
- Migrations are applied and checked for duplicate-precondition failures.
- `Jwt:SecretKey` is configured with a strong secret.
- RabbitMQ default credentials are replaced outside development.
- Logs are captured from Serilog console/file output.
- The outbox background job and MassTransit consumers are running with the API host.
