using MediatR;

namespace PwC.CarRental.Application.Queries.Statistics;

public class GetMostRentedCarTypeUtilizationQuery : IRequest<MostRentedCarTypeUtilizationDto> { }

public class GetMostRentedCarTypeUtilizationRankingQuery(DateTime startDate, DateTime endDate) : IRequest<IReadOnlyList<MostRentedCarTypeUtilizationRankingDto>>
{
    public DateTime StartDate { get; set; } = startDate;
    public DateTime EndDate { get; set; } = endDate;
}

public class GetMostUsedCarsRankingQuery : IRequest<IReadOnlyList<MostUsedCarRankingDto>> { }

public class GetMostUsedCarsRankingGroupQuery : IRequest<IReadOnlyList<MostUsedCarGroupRankingDto>> { }

public class GetWeeklyRentalStatisticsQuery : IRequest<WeeklyRentalStatisticsDto> { }