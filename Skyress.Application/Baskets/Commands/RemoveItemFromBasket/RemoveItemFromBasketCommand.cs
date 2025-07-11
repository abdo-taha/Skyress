using Skyress.Application.Abstractions.Messaging;

namespace Skyress.Application.Baskets.Commands.RemoveItemFromBasket;

public record RemoveItemFromBasketCommand(long BasketId, long ItemId) : ICommand;