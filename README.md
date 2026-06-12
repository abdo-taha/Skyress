# Skyress

Skyress is a .NET 8 backend for a shop/order workflow. It uses a layered architecture with Domain, Application, Infrastructure, and API projects, and it coordinates checkout with PostgreSQL, RabbitMQ, MassTransit sagas, MediatR commands, EF Core repositories, FluentValidation, JWT authentication, and typed domain errors.


## Project Structure

```text
Skyress.Domain/              Business entities, aggregates, enums, domain exceptions, Result types
Skyress.Application/         Commands, queries, validation, saga consumers, contracts
Skyress.Infrastructure/      EF Core DbContext, repositories, migrations, services, saga configuration
Skyress/                     Minimal API host, endpoints, middleware, auth, OpenAPI
Skyress.Domain.Tests/        Domain unit tests
Skyress.Application.Tests/   Application and API-boundary behavior tests
Skyress.Infrastructure.Tests/ Infrastructure, persistence, and integration tests
docs/                        Architecture and integration documentation
specs/                       Spec Kit feature specifications and implementation plans
```

## Main Capabilities

- Catalog, customer, basket, invoice, and payment APIs.
- JWT register/login/refresh/logout flow.
- Checkout saga: reserve basket items, create invoice, build invoice lines, create cash payment, wait for payment completion, finalize checkout.
- Domain exception mapping for stable business error codes/messages.
- HTTP 422 behavior for expected business failures in targeted hardened endpoints.
- EF Core migrations and repository abstractions for PostgreSQL.
- MassTransit saga persistence and RabbitMQ message transport.

## Requirements

- .NET SDK 8.x
- PostgreSQL
- RabbitMQ
- Optional: Docker for local infrastructure

Development settings expect:

```text
ConnectionStrings:SkyressDb = Host=localhost;port=5432;Database=Skyress;Username=Skyress;Password=Skyress;Include Error Detail=true
ConnectionStrings:RabbitMq  = rabbitmq://localhost
Jwt:SecretKey               = configured in appsettings.Development.json for local development only
DefaultAdmin                = configured in appsettings.Development.json for local development only
```

Set `Jwt:SecretKey` from environment variables, user secrets, or your deployment secret store before running anything beyond local development.
Set `DefaultAdmin:Email` and `DefaultAdmin:Password` from secure configuration if you want startup admin seeding outside development.

## Run Locally

Restore and build:

```powershell
dotnet build skyress.sln
```

Run tests:

```powershell
dotnet test skyress.sln
```

Run the API:

```powershell
dotnet run --project Skyress/Skyress.API.csproj
```

In Development, Swagger UI is available at the application root, usually:

```text
http://localhost:5000/
```

## API Notes

- API versioning uses the `api-version` query string. Version `1.0` is the default.
- Auth routes are under `/api/v1/auth` and allow anonymous access for register, login, refresh, and logout.
- Most business endpoints use the global authenticated fallback policy. Several invoice/payment query and command endpoints explicitly require the `Admin` role.
- Checkout starts with `POST /api/baskets/initiate-checkout?basketId={id}`.
- The saga creates a cash payment. The payment must then be completed through `POST /api/payments/{paymentId}/Pay` by an Admin-authorized caller.

## Documentation

- [Integration guide](docs/INTEGRATION.md)
- [Checkout flow README](docs/CHECKOUT_FLOW_README.md)
- [Checkout saga README](docs/CHECKOUT_SAGA_README.md)
- [Frontend integration guide](docs/FRONTEND_INTEGRATION.md)
- [Architecture review](docs/architecture-review/README.md)
- [DDD hardening plan](specs/004-ddd-pattern-hardening/plan.md)

## Current Implementation Status

The DDD hardening feature has completed the domain, validation, stable error, and several persistence-configuration tasks. Some durable idempotency and concurrency tasks remain open in `specs/004-ddd-pattern-hardening/tasks.md`, especially repository conflict handling, stock concurrency tests, and refresh-token replay concurrency.
