using PwC.CarRental.Domain.Entities;

namespace PwC.CarRental.Domain.Interfaces;

public interface IRentalRepository
{
    Task<Rental> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task AddAsync(Rental rental, CancellationToken cancellationToken);
    Task<IEnumerable<Rental>> GetRentalsForCarAsync(Guid carId, CancellationToken cancellationToken);
    Task<IEnumerable<Rental>> GetAllAsync(CancellationToken cancellationToken);
    Task UpdateAsync(Rental rental, CancellationToken cancellationToken);
}
