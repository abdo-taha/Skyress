using Skyress.Application.Abstractions.Messaging;

namespace Skyress.Application.Baskets.Commands.ClearBasket;

public record ClearBasketCommand(long BasketId) : ICommand;