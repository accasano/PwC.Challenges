using MediatR;
using Microsoft.EntityFrameworkCore;
using PwC.CarRental.Domain.Interfaces;

namespace PwC.CarRental.Application.Queries.Statistics;

public class GetMostUsedCarsRankingQueryHandler(ICarRentalDbContext dbContext) : IRequestHandler<GetMostUsedCarsRankingQuery, IReadOnlyList<MostUsedCarRankingDto>>
{
    private readonly ICarRentalDbContext _dbContext = dbContext;

    public async Task<IReadOnlyList<MostUsedCarRankingDto>> Handle(GetMostUsedCarsRankingQuery request, CancellationToken cancellationToken)
    {
        var cars = await _dbContext.Cars.ToListAsync(cancellationToken);

        var rentals = await _dbContext.Rentals
            .Include(r => r.Car)
            .ToListAsync(cancellationToken);

        if (rentals.Count == 0)
            return [];

        var minDate = rentals.Min(r => r.StartDate);
        var maxDate = rentals.Max(r => r.EndDate);
        var periodDays = (maxDate - minDate).Days;

        var results = cars
            .Select(car =>
            {
                var carRentals = rentals.Where(r => r.Car.Id == car.Id);
                var totalRentalDays = carRentals.Sum(r => (r.EndDate - r.StartDate).Days);
                return new MostUsedCarRankingDto
                {
                    CarId = car.Id,
                    Model = car.Model,
                    Type = car.Type,
                    RentalCount = carRentals.Count(),
                    TotalRentalDays = totalRentalDays,
                    UtilizationPercentage = Math.Round((double)totalRentalDays / periodDays * 100, 2)
                };
            })
            .OrderByDescending(x => x.TotalRentalDays)
            .Select((x, i) =>
            {
                x.Ranking = i + 1;
                return x;
            })
            .ToList();

        return results;
    }
}