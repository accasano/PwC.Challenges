using MediatR;

namespace PwC.CarRental.Application.Commands.Rentals;

public record ModifyReservationCommand(
    Guid RentalId,
    DateTime? NewStartDate,
    DateTime? NewEndDate,
    Guid? NewCarId
) : IRequest<bool>;