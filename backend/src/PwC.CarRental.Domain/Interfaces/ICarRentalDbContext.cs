using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PwC.CarRental.Domain.Entities;

namespace PwC.CarRental.Domain.Interfaces;

public interface ICarRentalDbContext
{
    DbSet<Location> Locations { get; set; }
    DbSet<Car> Cars { get; set; }
    DbSet<Customer> Customers { get; set; }
    DbSet<Rental> Rentals { get; set; }
    DbSet<Service> Services { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    EntityEntry Entry(object entity);
}