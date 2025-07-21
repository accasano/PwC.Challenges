using Microsoft.EntityFrameworkCore;
using PwC.CarRental.Domain.Entities;
using PwC.CarRental.Domain.Interfaces;

namespace PwC.CarRental.Infrastructure.Repositories;

public class ServiceRepository(ICarRentalDbContext dbContext) : IServiceRepository
{
    private readonly ICarRentalDbContext _dbContext = dbContext;

    public async Task AddAsync(Service service, CancellationToken cancellationToken)
    {
        _dbContext.Services.Add(service);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Service>> GetServicesForCarAsync(Guid carId, DateTime start, DateTime end, CancellationToken cancellationToken)
    {
        return await _dbContext.Services
            .Where(s => s.CarId == carId &&
                        ((start >= s.Date && start < s.Date.AddDays(s.DurationDays)) ||
                         (end > s.Date && end <= s.Date.AddDays(s.DurationDays))))
            .ToListAsync(cancellationToken);
    }

    public async Task<Service> GetLastServiceForCarAsync(Guid carId, CancellationToken cancellationToken)
    {
        return await _dbContext.Services
            .Where(s => s.CarId == carId)
            .OrderByDescending(s => s.Date)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
