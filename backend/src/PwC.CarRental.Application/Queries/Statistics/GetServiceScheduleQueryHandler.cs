using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using PwC.CarRental.Domain.Interfaces;

namespace PwC.CarRental.Application.Queries.Statistics;

public class GetServiceScheduleQueryHandler(ICarRentalDbContext dbContext, IMemoryCache cache) : IRequestHandler<GetServiceScheduleQuery, IReadOnlyList<CarServiceScheduleDto>>
{
    private readonly ICarRentalDbContext _dbContext = dbContext;
    private readonly IMemoryCache _cache = cache;

    public async Task<IReadOnlyList<CarServiceScheduleDto>> Handle(GetServiceScheduleQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"serviceSchedule:{request.FromDate:yyyyMMdd}:{request.ToDate:yyyyMMdd}:{request.LocationId}";
        if (_cache.TryGetValue<IReadOnlyList<CarServiceScheduleDto>>(cacheKey, out var cachedResult))
        {
            return cachedResult;
        }

        var query = _dbContext.Services
            .Include(s => s.Car)
            .ThenInclude(c => c.Location)
            .Where(s => s.Date >= request.FromDate && s.Date <= request.ToDate);

        if (request.LocationId.HasValue)
            query = query.Where(s => s.Car.LocationId == request.LocationId);

        var result = await query
            .Select(s => new CarServiceScheduleDto
            {
                CarId = s.CarId,
                Model = s.Car.Model,
                Type = s.Car.Type,
                ServiceDate = s.Date,
                LocationName = s.Car.Location.Name
            })
            .ToListAsync(cancellationToken);

        _cache.Set(cacheKey, result, TimeSpan.FromMinutes(5));
        return result;
    }
}