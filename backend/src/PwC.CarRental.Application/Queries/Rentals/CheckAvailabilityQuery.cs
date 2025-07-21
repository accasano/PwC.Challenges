using MediatR;

namespace PwC.CarRental.Application.Queries.Rentals;

public record CheckAvailabilityQuery(
    DateTime StartDate,
    DateTime EndDate,
    string CarType,
    string Model,
    Guid? LocationId
) : IRequest<bool>;