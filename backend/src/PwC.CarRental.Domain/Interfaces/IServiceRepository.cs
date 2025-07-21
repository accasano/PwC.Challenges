using PwC.CarRental.Domain.Entities;

namespace PwC.CarRental.Domain.Interfaces;

public interface IServiceRepository
{
    Task AddAsync(Service service, CancellationToken cancellationToken);
    Task<IReadOnlyList<Service>> GetServicesForCarAsync(Guid carId, DateTime start, DateTime end, CancellationToken cancellationToken);
    Task<Service> GetLastServiceForCarAsync(Guid carId, CancellationToken cancellationToken);
}
