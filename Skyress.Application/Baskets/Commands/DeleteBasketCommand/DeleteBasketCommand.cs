using Skyress.Application.Abstractions.Messaging;

namespace Skyress.Application.Baskets.Commands.DeleteBasketCommand;

public record DeleteBasketCommand(long BasketId) : ICommand;