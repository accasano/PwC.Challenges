using Microsoft.EntityFrameworkCore;
using PwC.CarRental.Domain.Entities;
using PwC.CarRental.Domain.Interfaces;

namespace PwC.CarRental.Infrastructure.Repositories;

public class RentalRepository(ICarRentalDbContext dbContext) : IRentalRepository
{
    private readonly ICarRentalDbContext _dbContext = dbContext;

    public async Task<Rental> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.Rentals
            .Include(r => r.Customer)
            .Include(r => r.Car)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task AddAsync(Rental rental, CancellationToken cancellationToken)
    {
        _dbContext.Rentals.Add(rental);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<Rental>> GetRentalsForCarAsync(Guid carId, CancellationToken cancellationToken)
    {
        return await _dbContext.Rentals
            .Include(r => r.Customer)
            .Include(r => r.Car)
            .Where(r => r.Car.Id == carId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Rental>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Rentals
            .Include(r => r.Customer)
            .Include(r => r.Car)
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateAsync(Rental rental, CancellationToken cancellationToken)
    {
        _dbContext.Rentals.Update(rental);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}