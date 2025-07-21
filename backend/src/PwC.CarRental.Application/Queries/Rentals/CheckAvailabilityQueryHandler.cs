using MediatR;
using Microsoft.Extensions.Caching.Memory;
using PwC.CarRental.Domain.Aggregates;
using PwC.CarRental.Domain.Interfaces;

namespace PwC.CarRental.Application.Queries.Rentals;

public class CheckAvailabilityQueryHandler(ICarRepository carRepository, RentalSystem rentalSystem, IMemoryCache cache) : IRequestHandler<CheckAvailabilityQuery, bool>//IReadOnlyList<CarAvailabilityDto>>
{
    private readonly RentalSystem _rentalSystem = rentalSystem;
    private readonly ICarRepository _carRepository = carRepository;
    private readonly IMemoryCache _cache = cache;

    public async Task<bool> Handle(CheckAvailabilityQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"availability:{request.StartDate:yyyyMMdd}:{request.EndDate:yyyyMMdd}:{request.CarType}:{request.Model}:{request.LocationId}";
        if (_cache.TryGetValue<bool>(cacheKey, out var cachedResult))
        {
            return cachedResult;
        }

        // Fetch the car entity
        var car = await _carRepository.FindAsync(request.CarType, request.Model, request.LocationId, cancellationToken)
            ?? throw new InvalidOperationException("Car not found.");

        // Use the domain aggregate to check availability
        var isAvailable = await _rentalSystem.CheckCarAvailabilityAsync(
            car,
            request.StartDate,
            request.EndDate,
            cancellationToken);

        _cache.Set(cacheKey, isAvailable, TimeSpan.FromMinutes(1));
        return isAvailable;
    }
}