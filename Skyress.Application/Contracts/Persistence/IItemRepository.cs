﻿using Skyress.Domain.Aggregates.Item;

namespace Skyress.Application.Contracts.Persistence
{
    public interface IItemRepository : IGenericRepository<Item>
    {
    }
}
