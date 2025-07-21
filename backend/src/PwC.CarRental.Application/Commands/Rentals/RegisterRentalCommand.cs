using MediatR;

namespace PwC.CarRental.Application.Commands.Rentals;

public record RegisterRentalCommand(
    Guid CustomerId,
    Guid CarId,
    DateTime StartDate,
    DateTime EndDate,
    string UserEmail
) : IRequest<Guid>;