using MediatR;

namespace PwC.CarRental.Application.Queries.Statistics;

public record GetServiceScheduleQuery(
    DateTime FromDate,
    DateTime ToDate,
    Guid? LocationId
) : IRequest<IReadOnlyList<CarServiceScheduleDto>>;