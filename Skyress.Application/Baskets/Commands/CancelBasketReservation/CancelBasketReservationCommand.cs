using Skyress.Application.Abstractions.Messaging;

namespace Skyress.Application.Baskets.Commands.CancelBasketReservation;

public sealed record CancelBasketReservationCommand(long BasketId) : ICommand; 