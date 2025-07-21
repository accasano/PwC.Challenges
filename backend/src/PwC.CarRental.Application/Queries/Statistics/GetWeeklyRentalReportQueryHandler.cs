using MediatR;
using Microsoft.EntityFrameworkCore;
using PwC.CarRental.Domain.Interfaces;

namespace PwC.CarRental.Application.Queries.Statistics;

public class GetWeeklyRentalReportQueryHandler(ICarRentalDbContext dbContext) : IRequestHandler<GetWeeklyRentalStatisticsQuery, WeeklyRentalStatisticsDto>
{
    private readonly ICarRentalDbContext _dbContext = dbContext;

    public async Task<WeeklyRentalStatisticsDto> Handle(GetWeeklyRentalStatisticsQuery request, CancellationToken cancellationToken)
    {
        var sevenDaysAgo = DateTime.UtcNow.Date.AddDays(-7);
        var today = DateTime.UtcNow.Date.AddDays(1);

        var rentalsLast7Days = await _dbContext.Rentals
            .Include(r => r.Car)
            .Where(r => r.StartDate >= sevenDaysAgo && r.StartDate < today)
            .ToListAsync(cancellationToken);

        var cancellations = rentalsLast7Days.Count(r => r.IsCancelled);

        var rentals = rentalsLast7Days.Count(r => !r.IsCancelled);

        var allCarIds = await _dbContext.Cars.Select(c => c.Id).ToListAsync(cancellationToken);

        var rentedCarIds = rentalsLast7Days.Select(r => r.Car.Id).Distinct().ToList();

        var unusedCars = allCarIds.Except(rentedCarIds).Count();

        return new WeeklyRentalStatisticsDto
        {
            Cancellations = cancellations,
            Rentals = rentals,
            UnusedCars = unusedCars
        };
    }
}