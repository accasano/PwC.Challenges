using MediatR;
using Microsoft.EntityFrameworkCore;
using PwC.CarRental.Domain.Interfaces;

namespace PwC.CarRental.Application.Queries.Statistics;

public class GetMostRentedCarTypeUtilizationQueryHandler(ICarRentalDbContext dbContext) : IRequestHandler<GetMostRentedCarTypeUtilizationQuery, MostRentedCarTypeUtilizationDto>
{
    private readonly ICarRentalDbContext _dbContext = dbContext;

    public async Task<MostRentedCarTypeUtilizationDto> Handle(GetMostRentedCarTypeUtilizationQuery request, CancellationToken cancellationToken)
    {
        if (!_dbContext.Rentals.Any())
            return default;

        var rentalCounts = await _dbContext.Rentals
            .Include(r => r.Car)
            .GroupBy(r => r.Car.Type)
            .Select(g => new
            {
                CarType = g.Key,
                RentalCount = g.Count()
            })
            .OrderByDescending(x => x.RentalCount)
            .ToListAsync(cancellationToken);

        if (rentalCounts.Count == 0)
        {
            return new MostRentedCarTypeUtilizationDto
            {
                CarType = string.Empty,
                RentalCount = 0,
                UtilizationPercentage = 0
            };
        }

        var mostRented = rentalCounts.First();

        var totalCarsOfType = await _dbContext.Cars.CountAsync(c => c.Type == mostRented.CarType, cancellationToken);

        var rentalsOfType = await _dbContext.Rentals
            .Where(r => r.Car.Type == mostRented.CarType)
            .ToListAsync(cancellationToken);

        var totalDays = rentalsOfType.Sum(r => (r.EndDate - r.StartDate).Days);

        var periodDays = 365;

        var utilization = totalCarsOfType > 0
            ? (double)totalDays / (totalCarsOfType * periodDays) * 100
            : 0;

        return new MostRentedCarTypeUtilizationDto
        {
            CarType = mostRented.CarType,
            RentalCount = mostRented.RentalCount,
            UtilizationPercentage = Math.Round(utilization, 2)
        };
    }
}
