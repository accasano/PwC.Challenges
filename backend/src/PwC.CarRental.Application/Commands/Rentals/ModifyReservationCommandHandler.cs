using MediatR;
using PwC.CarRental.Domain.Aggregates;

namespace PwC.CarRental.Application.Commands.Rentals;

public class ModifyReservationCommandHandler(RentalSystem rentalSystem) : IRequestHandler<ModifyReservationCommand, bool>
{
    private readonly RentalSystem _rentalSystem = rentalSystem;

    public async Task<bool> Handle(ModifyReservationCommand request, CancellationToken cancellationToken)
    {
        await _rentalSystem.ModifyReservationAsync(
            request.RentalId,
            request.NewStartDate,
            request.NewEndDate,
            request.NewCarId,
            cancellationToken);

        return true;
    }
}