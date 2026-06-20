# Skyress

Skyress is a .NET 8 backend for managing a shop workflow: catalog items, customers, baskets, invoices, payments, authentication, and a message-driven checkout process.

The project follows a layered architecture with Domain, Application, Infrastructure, and API projects. Business behavior is kept in domain aggregates and application commands, while persistence, messaging, identity services, and saga configuration live in infrastructure.

## Features

- JWT authentication with register, login, refresh token, and logout endpoints.
- Admin-protected APIs for items, customers, baskets, tags, tag assignments, invoices, payments, and todos.
- Basket checkout workflow coordinated by a MassTransit saga.
- PostgreSQL persistence with EF Core repositories and migrations.
- RabbitMQ transport for checkout events and consumers.
- FluentValidation command validation and typed domain results/errors.
- Serilog request/application logging.
- Swagger/OpenAPI in development.
- Docker support for local and containerized runs.

## Architecture

```text
Skyress/                      ASP.NET Core minimal API host, endpoints, middleware, OpenAPI
Skyress.Domain/               Aggregates, entities, enums, domain exceptions, Result types
Skyress.Application/          Commands, queries, validators, contracts, saga consumers
Skyress.Infrastructure/       EF Core DbContext, repositories, migrations, services, saga setup
Skyress.Domain.Tests/         Domain tests
Skyress.Application.Tests/    Application behavior tests
Skyress.Infrastructure.Tests/ Persistence and integration tests
docs/                         Project documentation and integration guides
charts/                       Helm chart for Kubernetes deployment
local-ci/                     Local CI and deployment support files
```

The solution file is `skyress.sln`.

## Tech Stack

- .NET 8 / ASP.NET Core Minimal APIs
- PostgreSQL
- EF Core 8 with Npgsql
- MassTransit 8 with RabbitMQ and EF saga persistence
- MediatR
- FluentValidation
- Quartz
- Serilog
- xUnit

## Requirements

- .NET SDK 8.x
- PostgreSQL
- RabbitMQ
- Optional: Docker / Docker Compose

## Configuration

For local development, `Skyress/appsettings.Development.json` contains default development values:

```text
ConnectionStrings:SkyressDb = Host=localhost;port=5432;Database=Skyress;Username=Skyress;Password=Skyress;Include Error Detail=true
ConnectionStrings:RabbitMq  = rabbitmq://localhost
Jwt:SecretKey               = development-only value
DefaultAdmin:Email          = admin@skyress.local
DefaultAdmin:Password       = Admin@12345!
```

Use environment variables, user secrets, or your deployment secret store for real environments. At minimum, override `Jwt:SecretKey` and the default admin credentials outside local development.

## Run Locally

Start PostgreSQL and RabbitMQ, then build the solution:

```powershell
dotnet build skyress.sln
```

Apply migrations:

```powershell
dotnet ef database update --project Skyress.Infrastructure --startup-project Skyress
```

Run the API:

```powershell
dotnet run --project Skyress/Skyress.API.csproj
```

In development, Swagger UI is available at the application root:

```text
http://localhost:5000/
```

Run tests:

```powershell
dotnet test skyress.sln
```

## Docker

Build and run the API container:

```powershell
docker compose up --build
```

For a local development database, use:

```powershell
docker compose -f docker-compose.dev.yml up -d skyress.db
```

RabbitMQ is also required for checkout saga messaging. If you do not already have RabbitMQ running locally, start one with:

```powershell
docker run --name skyress.rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3-management
```

RabbitMQ management UI is then available at:

```text
http://localhost:15672/
```

Default RabbitMQ credentials are `guest` / `guest`.

## API Notes

- API versioning uses the `api-version` query string. Version `1.0` is the default.
- Authentication routes are under `/api/v1/auth`.
- Most business endpoints require an authenticated user; many write/admin operations require the `Admin` role.
- Checkout starts with `POST /api/baskets/initiate-checkout?basketId={id}`.
- The checkout saga reserves basket items, creates and builds an invoice, creates a payment, waits for payment completion, and finalizes the basket.
- Cash payment completion is handled through `POST /api/payments/{paymentId}/Pay` by an Admin-authorized caller.

## Documentation

- [Integration guide](docs/INTEGRATION.md)
- [Checkout flow](docs/CHECKOUT_FLOW_README.md)
- [Checkout saga](docs/CHECKOUT_SAGA_README.md)
- [Frontend integration](docs/FRONTEND_INTEGRATION.md)
- [Postman collection notes](docs/postman/README.md)
