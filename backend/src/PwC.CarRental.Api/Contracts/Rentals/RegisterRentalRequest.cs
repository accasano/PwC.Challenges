namespace PwC.CarRental.Api.Contracts.Rentals;

public record RegisterRentalRequest(
    Guid CustomerId,
    Guid CarId,
    DateTime StartDate,
    DateTime EndDate
);