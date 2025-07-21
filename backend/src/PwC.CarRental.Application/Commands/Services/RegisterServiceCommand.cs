using MediatR;

namespace PwC.CarRental.Application.Commands.Services;

public record RegisterServiceCommand(
    Guid CarId,
    DateTime Date,
    int DurationDays = 2
) : IRequest<Guid>;
