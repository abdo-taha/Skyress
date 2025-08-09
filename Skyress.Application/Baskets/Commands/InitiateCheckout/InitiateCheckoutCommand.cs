using Skyress.Application.Abstractions.Messaging;

namespace Skyress.Application.Baskets.Commands.InitiateCheckout;

public sealed record InitiateCheckoutCommand(long BasketId, Guid IdempotencyKey) : ICommand<long>;