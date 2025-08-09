using Skyress.Application.Abstractions.Messaging;

namespace Skyress.Application.Baskets.Commands.CheckOutBasket;

public sealed record CheckOutBasketCommand(long BasketId, Guid IdempotencyKey) : ICommand;
