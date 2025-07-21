using PwC.CarRental.Domain.Entities;

namespace PwC.CarRental.Domain.Interfaces;

public interface ICarRepository
{
    Task<Car> GetByIdAsync(Guid carId, CancellationToken cancellationToken);
    Task<IEnumerable<Car>> GetAllAsync(CancellationToken cancellationToken);
    Task<Car> FindAsync(string type, string model, Guid? locationId, CancellationToken cancellationToken);
}