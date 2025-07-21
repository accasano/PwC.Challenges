using Microsoft.EntityFrameworkCore;
using PwC.CarRental.Domain.Entities;
using PwC.CarRental.Domain.Interfaces;

namespace PwC.CarRental.Infrastructure.Repositories;

public class CarRepository(ICarRentalDbContext dbContext) : ICarRepository
{
    private readonly ICarRentalDbContext _dbContext = dbContext;

    public async Task<Car> GetByIdAsync(Guid carId, CancellationToken cancellationToken)
    {
        return await _dbContext.Cars
            .Include(c => c.Services)
            .Include(c => c.Rentals)
            .FirstOrDefaultAsync(c => c.Id == carId, cancellationToken);
    }

    public async Task<IEnumerable<Car>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Cars
            .Include(c => c.Services)
            .Include(c => c.Rentals)
            .ToListAsync(cancellationToken);
    }

    public async Task<Car> FindAsync(string type, string model, Guid? locationId, CancellationToken cancellationToken)
    {
        var query = _dbContext.Cars
            .Include(c => c.Location)
            .Include(c => c.Rentals)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(type))
            query = query.Where(c => c.Type == type);

        if (!string.IsNullOrWhiteSpace(model))
            query = query.Where(c => c.Model == model);

        if (locationId.HasValue)
            query = query.Where(c => c.LocationId == locationId.Value);

        return await query.FirstOrDefaultAsync(cancellationToken);
    }
}