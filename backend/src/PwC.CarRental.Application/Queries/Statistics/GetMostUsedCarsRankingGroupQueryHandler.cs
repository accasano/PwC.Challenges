using MediatR;
using Microsoft.EntityFrameworkCore;
using PwC.CarRental.Domain.Interfaces;

namespace PwC.CarRental.Application.Queries.Statistics;

public class GetMostUsedCarsRankingGroupQueryHandler(ICarRentalDbContext dbContext) : IRequestHandler<GetMostUsedCarsRankingGroupQuery, IReadOnlyList<MostUsedCarGroupRankingDto>>
{
    private readonly ICarRentalDbContext _dbContext = dbContext;

    public async Task<IReadOnlyList<MostUsedCarGroupRankingDto>> Handle(GetMostUsedCarsRankingGroupQuery request, CancellationToken cancellationToken)
    {
        var rentals = await _dbContext.Rentals
            .Include(r => r.Car)
            .ToListAsync(cancellationToken);

        var carUsage = rentals
            .GroupBy(r => r.Car.Id)
            .Select(g => new
            {
                g.First().Car,
                TotalRentalDays = g.Sum(r => (r.EndDate - r.StartDate).Days)
            })
            .OrderByDescending(x => x.TotalRentalDays)
            .Take(10)
            .ToList();

        var result = carUsage
            .Select((x, index) => new MostUsedCarGroupRankingDto
            {
                Ranking = index + 1,
                Model = x.Car.Model,
                Type = x.Car.Type
            })
            .ToList();

        return result;
    }
}