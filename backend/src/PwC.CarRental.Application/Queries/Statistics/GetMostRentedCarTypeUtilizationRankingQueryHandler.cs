using MediatR;
using Microsoft.EntityFrameworkCore;
using PwC.CarRental.Domain.Interfaces;

namespace PwC.CarRental.Application.Queries.Statistics;

public class GetMostRentedCarTypeUtilizationRankingQueryHandler(ICarRentalDbContext dbContext) : IRequestHandler<GetMostRentedCarTypeUtilizationRankingQuery, IReadOnlyList<MostRentedCarTypeUtilizationRankingDto>>
{
    private readonly ICarRentalDbContext _dbContext = dbContext;

    public async Task<IReadOnlyList<MostRentedCarTypeUtilizationRankingDto>> Handle(GetMostRentedCarTypeUtilizationRankingQuery request, CancellationToken cancellationToken)
    {
        if (!_dbContext.Rentals.Any())
            return [];

        var rentalsInPeriod = await _dbContext.Rentals
            .Include(r => r.Car)
            .Where(r => r.StartDate >= request.StartDate && r.StartDate <= request.EndDate)
            .ToListAsync(cancellationToken);

        var periodDays = (request.EndDate - request.StartDate).Days + 1;

        var results = rentalsInPeriod
            .GroupBy(r => r.Car.Type)
            .Select(g =>
            {
                var carType = g.Key;
                var rentalCount = g.Count();
                var totalDays = g.Sum(r => (r.EndDate - r.StartDate).Days);
                var totalCarsOfType = _dbContext.Cars.Count(c => c.Type == carType);
                var utilization = totalCarsOfType > 0
                    ? (double)totalDays / (totalCarsOfType * periodDays) * 100
                    : 0;

                return new MostRentedCarTypeUtilizationRankingDto
                {
                    CarType = carType,
                    RentalCount = rentalCount,
                    UtilizationPercentage = Math.Round(utilization, 2)
                };
            })
            .OrderByDescending(x => x.RentalCount)
            .Take(3)
            .Select((x, i) =>
            {
                x.Ranking = i + 1;
                return x;
            })
            .ToList();

        return results;
    }
}
