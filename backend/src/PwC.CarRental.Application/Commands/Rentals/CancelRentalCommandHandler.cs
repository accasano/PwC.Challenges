using MediatR;
using PwC.CarRental.Domain.Aggregates;

namespace PwC.CarRental.Application.Commands.Rentals;

public class CancelRentalCommandHandler(RentalSystem rentalSystem) : IRequestHandler<CancelRentalCommand, bool>
{
    private readonly RentalSystem _rentalSystem = rentalSystem;

    public async Task<bool> Handle(CancelRentalCommand request, CancellationToken cancellationToken)
    {
        await _rentalSystem.CancelRentalAsync(
            request.RentalId,
            cancellationToken);

        return true;
    }
}