# Data Model: Constitution Compliance Refactor

**Branch**: `002-constitution-refactor` | **Date**: 2026-05-09

This document defines all new response DTO records to be created in the Application
project. No database schema changes are needed — these are pure in-memory transfer objects.

---

## Response DTOs

All response records go in `Skyress.Application\[Feature]\Responses\`.
All records use `init` properties and are `sealed`.

### ItemResponse

**File**: `Skyress.Application\Items\Responses\ItemResponse.cs`

```csharp
namespace Skyress.Application.Items.Responses;

using Skyress.Domain.Aggregates.Item;
using Skyress.Domain.Enums;

public sealed record ItemResponse(
    long Id,
    string Name,
    string? Description,
    decimal Price,
    decimal? CostPrice,
    int QuantityLeft,
    int QuantityReserved,
    int QuantitySold,
    string? QrCode,
    Unit Unit,
    bool IsDeleted,
    DateTime CreatedAt)
{
    public static ItemResponse FromDomain(Item item) => new(
        item.Id,
        item.Name,
        item.Description,
        item.Price,
        item.CostPrice,
        item.QuantityLeft,
        item.QuantityReserved,
        item.QuantitySold,
        item.QrCode,
        item.Unit,
        item.IsDeleted,
        item.CreatedAt);
}
```

### CustomerResponse

**File**: `Skyress.Application\Customers\Responses\CustomerResponse.cs`

```csharp
namespace Skyress.Application.Customers.Responses;

using Skyress.Domain.Aggregates.Customer;
using Skyress.Domain.Enums;

public sealed record CustomerResponse(
    long Id,
    string Name,
    string Notes,
    CustomerState State,
    DateTime CreatedAt)
{
    public static CustomerResponse FromDomain(Customer customer) => new(
        customer.Id,
        customer.Name,
        customer.Notes,
        customer.State,
        customer.CreatedAt);
}
```

### InvoiceResponse

**File**: `Skyress.Application\Invoices\Responses\InvoiceResponse.cs`

```csharp
namespace Skyress.Application.Invoices.Responses;

using Skyress.Domain.Aggregates.Invoice;
using Skyress.Domain.Enums;

public sealed record InvoiceResponse(
    long Id,
    decimal TotalAmount,
    long BasketId,
    long? CustomerId,
    long? PaymentId,
    InvoiceState State,
    DateTime CreatedAt)
{
    public static InvoiceResponse FromDomain(Invoice invoice) => new(
        invoice.Id,
        invoice.TotalAmount,
        invoice.BasketId,
        invoice.CustomerId,
        invoice.PaymentId,
        invoice.State,
        invoice.CreatedAt);
}
```

### PaymentResponse

**File**: `Skyress.Application\Payments\Responses\PaymentResponse.cs`

```csharp
namespace Skyress.Application.Payments.Responses;

using Skyress.Domain.Aggregates.Payment;
using Skyress.Domain.Enums;

public sealed record PaymentResponse(
    long Id,
    PaymentType PaymentType,
    PaymentState PaymentState,
    decimal TotalPaid,
    decimal TotalDue,
    long InvoiceId,
    DateTime CreatedAt)
{
    public static PaymentResponse FromDomain(Payment payment) => new(
        payment.Id,
        payment.PaymentType,
        payment.PaymentState,
        payment.TotalPaid,
        payment.TotalDue,
        payment.InvoiceId,
        payment.CreatedAt);
}
```

### TodoResponse

**File**: `Skyress.Application\Todos\Responses\TodoResponse.cs`

```csharp
namespace Skyress.Application.Todos.Responses;

using Skyress.Domain.Aggregates.Todo;
using Skyress.Domain.Enums;

public sealed record TodoResponse(
    long Id,
    string Context,
    TodoState State,
    DateTime CreatedAt)
{
    public static TodoResponse FromDomain(Todo todo) => new(
        todo.Id,
        todo.context,
        todo.State,
        todo.CreatedAt);
}
```

### TagResponse

**File**: `Skyress.Application\Tags\Responses\TagResponse.cs`

```csharp
namespace Skyress.Application.Tags.Responses;

using Skyress.Domain.Aggregates.Tag;
using Skyress.Domain.Enums;

public sealed record TagResponse(
    long Id,
    string Name,
    TagType Type)
{
    public static TagResponse FromDomain(Tag tag) => new(
        tag.Id,
        tag.Name,
        tag.Type);
}
```

### TagAssignmentResponse

**File**: `Skyress.Application\TagAssignments\Responses\TagAssignmentResponse.cs`

```csharp
namespace Skyress.Application.TagAssignments.Responses;

using Skyress.Domain.Aggregates.TagAssignment;

public sealed record TagAssignmentResponse(
    long Id,
    long TagId,
    long ItemId)
{
    public static TagAssignmentResponse FromDomain(TagAssignment assignment) => new(
        assignment.Id,
        assignment.TagId,
        assignment.ItemId);
}
```

### BasketResponse

**File**: `Skyress.Application\Baskets\Responses\BasketResponse.cs`

```csharp
namespace Skyress.Application.Baskets.Responses;

using Skyress.Application.Baskets.DTOs;
using Skyress.Domain.Aggregates.Basket;
using Skyress.Domain.Enums;

public sealed record BasketResponse(
    long Id,
    long? UserId,
    BasketState State,
    long? InvoiceId,
    string? CheckoutId,
    IReadOnlyCollection<BasketItemDto> Items)
{
    public static BasketResponse FromDomain(Basket basket) => new(
        basket.Id,
        basket.UserId,
        basket.State,
        basket.InvoiceId,
        basket.CheckoutId,
        basket.BasketItems
            .Select(bi => new BasketItemDto(bi.BasketId, bi.ItemId, bi.Quantity))
            .ToList()
            .AsReadOnly());
}
```

---

## New Domain Addition: `Customer.Create` Factory Method

The `Customer` aggregate needs a static factory method added to it.

**File**: `Skyress.Domain\Aggregates\Customer\Customer.cs`

Add the following method inside the `Customer` class body:

```csharp
public static Customer Create(string name, string notes, CustomerState state)
{
    ArgumentException.ThrowIfNullOrWhiteSpace(name);
    return new Customer
    {
        Name = name,
        Notes = notes,
        State = state
    };
}
```

---

## IGenericRepository Updates

**File**: `Skyress.Application\Contracts\Persistence\IGenericRepository.cs`

Change these signatures:

```csharp
// BEFORE
public Task<IReadOnlyList<T>> GetAllAsync();
public Task<T?> GetByIdAsync(long id);
public Task<T> CreateAsync(T entity);
public Task DeleteByIdAsync(long id);

// AFTER
public Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);
public Task<T?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
public Task<T> CreateAsync(T entity, CancellationToken cancellationToken = default);
public Task DeleteByIdAsync(long id, CancellationToken cancellationToken = default);
```

**File**: `Skyress.Infrastructure\Repository\GenericRepository.cs`

Match implementations to updated interface signatures, forwarding the token to EF Core calls.
