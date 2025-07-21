using Microsoft.EntityFrameworkCore;
using PwC.CarRental.Domain.Entities;
using PwC.CarRental.Domain.Interfaces;
using PwC.CarRental.Infrastructure.Persistence;

namespace PwC.CarRental.UnitTests.Fixtures;

public class CarRentalDbContextFixture : IDisposable
{
    public CarRentalDbContext DbContext { get; }

    public CarRentalDbContextFixture()
    {
        var options = new DbContextOptionsBuilder<CarRentalDbContext>()
            .UseInMemoryDatabase("CarRentalTestDb")
            .Options;
        DbContext = new CarRentalDbContext(options);

        SetData(DbContext);
    }

    public void Dispose()
    {
        DbContext.Dispose();
        GC.SuppressFinalize(this);
    }

    private static void SetData(ICarRentalDbContext dbContext)
    {
        var customerId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");
        var customerId2 = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd");
        var carId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        var locationId = Guid.Parse("11111111-1111-1111-1111-111111111111");

        // Seed data
        var location = new Location
        {
            Id = locationId,
            Name = "Airport"
        };
        var car = new Car
        {
            Id = carId,
            Type = "SUV",
            Model = "Toyota RAV4",
            LocationId = location.Id,
            Location = location
        };
        var customer1 = new Customer(customerId, "John Doe", "123 Main St");
        var customer2 = new Customer(customerId2, "Jane Smith", "456 Elm St");

        dbContext.Locations.Add(location);
        dbContext.Cars.Add(car);
        dbContext.Customers.Add(customer1);
        dbContext.Customers.Add(customer2);

        var reservedStart = DateTime.Today.AddDays(1);
        var reservedEnd = DateTime.Today.AddDays(10);

        var rental1 = new Rental(customer1, reservedStart, reservedEnd, car);
        dbContext.Rentals.Add(rental1);
        dbContext.SaveChangesAsync();
    }
}