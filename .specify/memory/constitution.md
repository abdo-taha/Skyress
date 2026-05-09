<!--
Sync Impact Report
Version change: (template placeholders) → 1.0.0
Modified principles: All placeholder tokens replaced — template had PRINCIPLE_1–5;
  expanded to nine concrete principles (I through IX) per project requirements.
Added sections:
  - I.  Folder Structure
  - II. Naming Conventions
  - III. Code Patterns
  - IV. Error Handling
  - V.  Configuration
  - VI. Logging
  - VII. Async/Await
  - VIII. Anti-Patterns
  - IX. Spec Kit Integration
  - Anti-Patterns Reference (Section 2)
  - Spec Kit Workflow Reference (Section 3)
  - Governance
Removed sections: None (template had no concrete content)
Templates requiring updates:
  ✅ .specify/templates/plan-template.md — Constitution Check section present; no
     structural changes needed; .NET-specific gates now defined in Principle IX
  ✅ .specify/templates/spec-template.md — Generic structure; compatible with DDD/CQRS project
  ✅ .specify/templates/tasks-template.md — Generic structure; path conventions now
     defined in Principle I (Domain/Application/Infrastructure/Presentation layout)
  ✅ .specify/templates/commands/ — No command files found; nothing to update
Follow-up TODOs: None — all placeholders resolved
-->

# Skyress Constitution

## Core Principles

### I. Folder Structure

The solution MUST follow a Hybrid Multi-Layer architecture with four top-level projects:

- **Skyress.Domain** — Aggregates, value objects, domain events, domain exceptions,
  repository interfaces
- **Skyress.Application** — CQRS commands/queries/handlers, validators, DTOs,
  application service interfaces
- **Skyress.Infrastructure** — Repository implementations, EF Core DbContext,
  external service clients, configuration
- **Skyress.Presentation** — ASP.NET Core controllers, middleware, request/response
  models, DI composition root

Inside each project, organization MUST be feature-based (not type-based):

```
Skyress.Application/
├── Features/
│   ├── Auth/
│   │   ├── Commands/
│   │   │   ├── LoginCommand.cs
│   │   │   ├── LoginCommandHandler.cs
│   │   │   └── LoginCommandValidator.cs
│   │   └── Queries/
│   │       ├── GetCurrentUserQuery.cs
│   │       └── GetCurrentUserQueryHandler.cs
│   └── Orders/
│       ├── Commands/
│       └── Queries/
Skyress.Domain/
├── Orders/
│   ├── Order.cs              (aggregate root)
│   ├── OrderItem.cs          (entity)
│   ├── OrderStatus.cs        (enum)
│   └── IOrderRepository.cs   (repository interface)
└── Shared/
    └── ValueObjects/
        ├── UserId.cs
        └── Money.cs
```

Cross-cutting abstractions (`Result<T>`, `DomainException`, base classes) MUST live
in `Skyress.Domain/Shared/`.

### II. Naming Conventions

All identifiers MUST follow these conventions without exception.

**Classes**
- Aggregate roots: `Order`, `User`, `Product` (noun, no suffix)
- Value objects: `UserId`, `Money`, `EmailAddress` (noun, descriptive)
- Domain exceptions: `OrderNotFoundException`, `InsufficientStockException`
  (noun + `Exception`)

**CQRS**
- Queries (read, no side effects): `GetUserQuery`, `GetOrderByIdQuery`,
  `ListProductsQuery`
- Commands (write, causes state change): `CreateOrderCommand`,
  `UpdateUserProfileCommand`, `DeleteProductCommand`
- Handlers: `GetUserQueryHandler`, `CreateOrderCommandHandler`
  (mirrors query/command name + `Handler`)
- Responses/DTOs: `UserResponse`, `OrderSummaryDto`, `ProductListResponse`

**Repositories**
- Interfaces: `IUserRepository`, `IOrderRepository` (I + domain noun + Repository)
- Implementations: `UserRepository`, `OrderRepository` (no prefix, Infrastructure)

**Services**
- Interfaces: `IEmailService`, `IPaymentGateway`, `ITokenService`
- Implementations: `EmailService`, `StripePaymentGateway`, `JwtTokenService`

**Properties & Variables**
- Public properties: `PascalCase` (`OrderId`, `CreatedAt`, `TotalAmount`)
- Private fields: `_camelCase` (`_unitOfWork`, `_logger`, `_dbContext`)
- Local variables: `camelCase` (`order`, `userId`, `cancellationToken`)
- CancellationToken parameter: always named `cancellationToken`

**Enums**
- Type name: `PascalCase` singular (`OrderStatus`, `PaymentMethod`)
- Values: `PascalCase` (`Pending`, `Processing`, `Completed`, `Cancelled`)

### III. Code Patterns

The following patterns MUST be used throughout the codebase. Deviations require
a Complexity Tracking entry in the feature plan.

**Aggregate Root with Factory Method**
```csharp
public sealed class Order : AggregateRoot
{
    private readonly List<OrderItem> _items = new();

    private Order() { } // EF Core

    public static Order Create(UserId userId, Address shippingAddress)
    {
        ArgumentNullException.ThrowIfNull(userId);
        ArgumentNullException.ThrowIfNull(shippingAddress);

        var order = new Order
        {
            Id = OrderId.New(),
            UserId = userId,
            ShippingAddress = shippingAddress,
            Status = OrderStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        order.RaiseDomainEvent(new OrderCreatedEvent(order.Id));
        return order;
    }

    public void AddItem(ProductId productId, int quantity, Money unitPrice)
    {
        if (Status != OrderStatus.Pending)
            throw new DomainException("Cannot add items to a non-pending order.");

        _items.Add(OrderItem.Create(Id, productId, quantity, unitPrice));
    }
}
```

**Value Object**
```csharp
public sealed record UserId(Guid Value)
{
    public static UserId New() => new(Guid.NewGuid());
    public static UserId From(Guid value) => new(value);
    public override string ToString() => Value.ToString();
}
```

**CQRS Command Handler with Result&lt;T&gt;**
```csharp
public sealed class CreateOrderCommandHandler
    : IRequestHandler<CreateOrderCommand, Result<OrderResponse>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateOrderCommandHandler> _logger;

    public CreateOrderCommandHandler(
        IOrderRepository orderRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateOrderCommandHandler> logger)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<OrderResponse>> Handle(
        CreateOrderCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating order for user {UserId}", command.UserId);

        var order = Order.Create(
            UserId.From(command.UserId),
            Address.From(command.ShippingAddress));

        await _orderRepository.AddAsync(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Order {OrderId} created successfully", order.Id);
        return Result.Success(OrderResponse.FromDomain(order));
    }
}
```

**FluentValidation Validator**
```csharp
public sealed class CreateOrderCommandValidator
    : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required.");

        RuleFor(x => x.ShippingAddress)
            .NotNull()
            .WithMessage("Shipping address is required.");
    }
}
```

**Repository Implementation**
```csharp
public sealed class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _dbContext;

    public OrderRepository(AppDbContext dbContext)
        => _dbContext = dbContext;

    public async Task<Order?> GetByIdAsync(
        OrderId id,
        CancellationToken cancellationToken = default)
        => await _dbContext.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);

    public async Task AddAsync(
        Order order,
        CancellationToken cancellationToken = default)
        => await _dbContext.Orders.AddAsync(order, cancellationToken);
}
```

**EF Core DbContext Configuration**
```csharp
public sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.Id);

        builder.Property(o => o.Id)
            .HasConversion(id => id.Value, value => OrderId.From(value));

        builder.OwnsOne(o => o.ShippingAddress, addr =>
        {
            addr.Property(a => a.Street).HasMaxLength(200).IsRequired();
            addr.Property(a => a.City).HasMaxLength(100).IsRequired();
        });

        builder.HasMany(o => o.Items)
            .WithOne()
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
```

**Exception Handling Middleware**
```csharp
public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Domain rule violation: {Message}", ex.Message);
            context.Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
            await context.Response.WriteAsJsonAsync(
                new ProblemDetails { Title = "Domain Rule Violation", Detail = ex.Message });
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Resource not found: {Message}", ex.Message);
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            await context.Response.WriteAsJsonAsync(
                new ProblemDetails { Title = "Not Found", Detail = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(
                new ProblemDetails { Title = "Internal Server Error" });
        }
    }
}
```

### IV. Error Handling

**Application layer MUST return `Result<T>`. Domain layer MUST throw `DomainException`.
Middleware MUST map all exceptions to structured HTTP responses.**

The two-layer error contract:

- **Domain layer** (`Skyress.Domain`): throws `DomainException` (or subtypes) for business
  rule violations. MUST NOT return null for a broken invariant — throw.
- **Application layer** (`Skyress.Application`): catches `DomainException` and wraps it in
  `Result.Failure<T>`. Handlers MUST NOT allow domain exceptions to reach the
  Presentation layer.
- **Presentation layer** (`Skyress.Presentation`): `ExceptionHandlingMiddleware` catches any
  unhandled exception and maps it to a structured HTTP `ProblemDetails` response.

```
Domain              Application           Presentation
──────────────      ────────────────────  ─────────────────────
throws              catches & wraps       middleware maps
DomainException   → Result<T>.Failure  → HTTP 422
NotFoundException                      → HTTP 404
ValidationException                    → HTTP 400
(unhandled)                            → HTTP 500
```

`Result<T>` contract:
```csharp
public sealed class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public string? Error { get; }

    private Result(bool isSuccess, T? value, string? error)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }

    public static Result<T> Success(T value) => new(true, value, null);
    public static Result<T> Failure(string error) => new(false, default, error);
}
```

Controllers MUST inspect `Result<T>` and return the appropriate HTTP status:
```csharp
var result = await _mediator.Send(command, cancellationToken);
return result.IsSuccess
    ? Ok(result.Value)
    : UnprocessableEntity(new ProblemDetails { Detail = result.Error });
```

### V. Configuration

**Infrastructure settings MUST use `IOptions<T>`. Domain constants MUST use enums.**

```csharp
// appsettings.json → typed options class
public sealed class JwtSettings
{
    public string Secret { get; init; } = string.Empty;
    public int ExpiryMinutes { get; init; }
    public string Issuer { get; init; } = string.Empty;
    public string Audience { get; init; } = string.Empty;
}

// Registration in DI
services.Configure<JwtSettings>(configuration.GetSection("Jwt"));

// Consumption
public JwtTokenService(IOptions<JwtSettings> options)
    => _settings = options.Value;
```

Magic strings for domain constants are forbidden. Use enums:
```csharp
// ❌ FORBIDDEN
if (order.Status == "pending") { ... }

// ✅ REQUIRED
if (order.Status == OrderStatus.Pending) { ... }
```

Environment-specific values (connection strings, API keys, secrets) MUST come from
environment variables or the .NET Secrets Manager. Hardcoded secrets are FORBIDDEN.

### VI. Logging

**Log entry and exit in every handler. Log errors with structured context.
Log business rule violations at Warning level only.**

Non-negotiable rules:
- Handlers MUST log entry (`LogInformation`) with the primary ID or user context.
- Handlers MUST log success (`LogInformation`) with the resulting resource ID.
- Catches of domain exceptions MUST log at `LogWarning` — these are expected conditions.
- Unexpected exceptions MUST log at `LogError` with the full exception object.
- Sensitive data (passwords, tokens, PII) MUST NOT appear in any log output.
- MUST use structured logging placeholders — string interpolation is FORBIDDEN.

```csharp
// ✅ REQUIRED — structured placeholders
_logger.LogInformation(
    "Processing {CommandType} for user {UserId}",
    nameof(CreateOrderCommand), command.UserId);

// ❌ FORBIDDEN — breaks log aggregators and may leak data
_logger.LogInformation($"Processing order for user {command.UserId}");
```

### VII. Async/Await

**All I/O MUST be async. `CancellationToken` MUST flow through every async call.
`.Result`, `.Wait()`, and `.GetAwaiter().GetResult()` are FORBIDDEN.**

Non-negotiable rules:
- Every method performing I/O (database, HTTP, file) MUST be `async Task<T>`
  (not `void`, except ASP.NET Core event handler callbacks).
- `CancellationToken cancellationToken` MUST be the last parameter in every async
  public method.
- The token MUST be forwarded to all downstream async calls (EF Core, HttpClient, etc.).
- `async void` is FORBIDDEN outside framework event handler callbacks.

```csharp
// ✅ REQUIRED
public async Task<Order?> GetByIdAsync(
    OrderId id, CancellationToken cancellationToken)
    => await _dbContext.Orders.FindAsync([id.Value], cancellationToken);

// ❌ FORBIDDEN — blocks a thread pool thread
public Order? GetById(OrderId id)
    => _dbContext.Orders.FindAsync(id.Value).Result;
```

### VIII. Anti-Patterns

The following patterns are FORBIDDEN. Any PR introducing them MUST be blocked.

**1. Sync-over-async (thread pool starvation)**
```csharp
// ❌ FORBIDDEN
var order = _repository.GetByIdAsync(id).Result;
var result = someTask.GetAwaiter().GetResult();

// ✅ REQUIRED
var order = await _repository.GetByIdAsync(id, cancellationToken);
```

**2. Magic strings (undetectable at compile time)**
```csharp
// ❌ FORBIDDEN
if (status == "active") { }
_logger.LogInformation("Status is " + status);

// ✅ REQUIRED
if (status == UserStatus.Active) { }
_logger.LogInformation("Status is {Status}", status);
```

**3. Mixed concerns (violates Clean Architecture)**
```csharp
// ❌ FORBIDDEN — domain rule checked in handler; direct EF access
public async Task<Result<OrderResponse>> Handle(
    CreateOrderCommand cmd, CancellationToken ct)
{
    if (cmd.Items.Count == 0)
        return Result.Failure<OrderResponse>("No items.");
    var order = new Order { Status = "pending" };
    await _dbContext.Orders.AddAsync(order);
}

// ✅ REQUIRED — domain logic in aggregate; repository abstraction used
public async Task<Result<OrderResponse>> Handle(
    CreateOrderCommand cmd, CancellationToken ct)
{
    var order = Order.Create(UserId.From(cmd.UserId), cmd.ShippingAddress);
    await _orderRepository.AddAsync(order, ct);
    await _unitOfWork.SaveChangesAsync(ct);
    return Result.Success(OrderResponse.FromDomain(order));
}
```

**4. Unhandled exceptions leaking implementation details**
```csharp
// ❌ FORBIDDEN — raw exception reaches the HTTP response
[HttpPost]
public async Task<IActionResult> Create(CreateOrderRequest request)
{
    var order = await _mediator.Send(request.ToCommand());
    return Ok(order);
}

// ✅ REQUIRED — Result<T> handled explicitly
[HttpPost]
public async Task<IActionResult> Create(
    CreateOrderRequest request, CancellationToken cancellationToken)
{
    var result = await _mediator.Send(request.ToCommand(), cancellationToken);
    return result.IsSuccess
        ? Ok(result.Value)
        : UnprocessableEntity(result.Error);
}
```

**5. Injecting DbContext outside the Infrastructure layer**
```csharp
// ❌ FORBIDDEN — infrastructure concern leaks into Application
public class CreateOrderCommandHandler(AppDbContext dbContext) { ... }

// ✅ REQUIRED — depend on the repository abstraction
public class CreateOrderCommandHandler(IOrderRepository orderRepository) { ... }
```

### IX. Spec Kit Integration

**Every non-trivial feature MUST follow the Spec Kit workflow before any code is written.**

Mandatory workflow sequence:

1. `/speckit-specify` — write `spec.md` with user stories and acceptance criteria
2. `/speckit-clarify` — resolve ambiguities before design (recommended for complex features)
3. `/speckit-plan` — produce architecture design, data model, and API contracts
4. `/speckit-tasks` — generate dependency-ordered task list from design artifacts
5. `/speckit-implement` — execute tasks one-by-one, committing after each logical group
6. `/speckit-analyze` — validate consistency between spec, plan, and tasks

Constitution Check gates verified during `/speckit-plan`:
- Layer separation: domain logic not in handlers; no `DbContext` in Application layer
- Naming: all classes follow Principle II conventions
- Result&lt;T&gt;: all command/query handlers return `Result<T>`
- Async: all I/O methods are async with `CancellationToken`
- Validation: a FluentValidation validator exists for every command

When `/speckit-tasks` generates a task list, the following categories MUST always appear:
- Domain model tasks (aggregates, value objects, repository interfaces)
- Application tasks (handlers, validators, DTOs)
- Infrastructure tasks (repository implementations, EF Core configurations)
- Presentation tasks (controllers, middleware registration)
- Integration test tasks (when tests are requested in the spec)

Feature source paths MUST follow the Principle I structure:
```text
Skyress.Domain/[Feature]/
Skyress.Application/Features/[Feature]/
Skyress.Infrastructure/[Feature]/
Skyress.Presentation/[Feature]/
specs/[###-feature-name]/
```

## Anti-Patterns Reference

The five forbidden patterns listed in Principle VIII MUST be verified during every code review.
Any PR introducing a forbidden pattern MUST be blocked until the pattern is corrected.
The Complexity Tracking table in `plan.md` MUST document any deviation from these patterns
with a specific technical justification. "Legacy code" and "time pressure" are not valid
justifications.

## Spec Kit Workflow Reference

Feature artifacts are stored under `specs/[###-feature-name]/`. The active feature is the
highest-numbered directory under `specs/`. Run `/speckit-analyze` after implementation to
verify cross-artifact consistency. When prompting an AI assistant to generate handlers,
validators, or repository code, reference `.specify/memory/constitution.md` explicitly to
ensure pattern compliance.

## Governance

This constitution supersedes all other coding practices, style guides, and verbal agreements
in this repository. Amendments MUST be proposed by updating this document, incrementing the
version according to semantic versioning, and committing with a message of the form:

```
docs: amend constitution to vX.Y.Z (<summary of change>)
```

Amendment procedure:
1. Propose the change in `constitution.md` with tracked rationale
2. Increment `CONSTITUTION_VERSION` (MAJOR: principle removals/redefinitions;
   MINOR: new principles or material expansion; PATCH: clarifications and wording)
3. Update `LAST_AMENDED_DATE` to today's ISO date
4. Propagate changes to affected templates (`plan-template.md`, `spec-template.md`,
   `tasks-template.md`)
5. Commit with the standard amendment message

All pull requests MUST include a Constitution Check confirming no principle has been violated.
Complexity violations MUST be documented in the feature plan's Complexity Tracking table.
For runtime development guidance, see `CLAUDE.md` and the active feature spec under `specs/`.

**Version**: 1.0.0 | **Ratified**: 2026-05-09 | **Last Amended**: 2026-05-09
