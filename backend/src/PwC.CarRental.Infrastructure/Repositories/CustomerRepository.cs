using Microsoft.EntityFrameworkCore;
using PwC.CarRental.Domain.Entities;
using PwC.CarRental.Domain.Interfaces;

namespace PwC.CarRental.Infrastructure.Repositories;

public class CustomerRepository(ICarRentalDbContext dbContext) : ICustomerRepository
{
    private readonly ICarRentalDbContext _dbContext = dbContext;

    public async Task AddAsync(Customer customer, CancellationToken cancellationToken)
    {
        _dbContext.Customers.Add(customer);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<Customer> GetByIdAsync(Guid customerId, CancellationToken cancellationToken)
    {
        return await _dbContext.Customers
            .FirstOrDefaultAsync(c => c.Id == customerId, cancellationToken);
    }

    public async Task<IEnumerable<Customer>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Customers
            .ToListAsync(cancellationToken);
    }
}