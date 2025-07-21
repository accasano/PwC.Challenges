using MediatR;

namespace PwC.CarRental.Application.Commands.Rentals;

public record CancelRentalCommand(
    Guid RentalId
) : IRequest<bool>;