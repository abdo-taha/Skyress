﻿namespace Skyress.Domain.Aggregates.Customer;

using Skyress.Domain.Enums;
using Skyress.Domain.primitives;

public sealed class Customer : AggregateRoot, IAuditable, ISoftDeletable
{
    public required string Name { get; set; }

    public CustomerState State { get; set; }

    public required string Notes { get; set; }

    public string? LastEditBy { get; set; }

    public DateTime LastEditDate { get; set; }

    public DateTime CreatedAt { get; set; }
    public bool IsDeleted { get; private set; }

    public void SoftDelete()
    {
        IsDeleted = true;
    }

    public void UnDelete()
    {
        IsDeleted = false;
    }
}
