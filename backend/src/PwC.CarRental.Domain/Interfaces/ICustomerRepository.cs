using PwC.CarRental.Domain.Entities;

namespace PwC.CarRental.Domain.Interfaces;

public interface ICustomerRepository
{
    Task AddAsync(Customer customer, CancellationToken cancellationToken);
    Task<Customer> GetByIdAsync(Guid customerId, CancellationToken cancellationToken);
    Task<IEnumerable<Customer>> GetAllAsync(CancellationToken cancellationToken);
}