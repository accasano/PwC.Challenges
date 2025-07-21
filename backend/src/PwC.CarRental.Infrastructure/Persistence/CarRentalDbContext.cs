using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PwC.CarRental.Domain.Entities;
using PwC.CarRental.Domain.Interfaces;

namespace PwC.CarRental.Infrastructure.Persistence;

public class CarRentalDbContext(DbContextOptions<CarRentalDbContext> options) : IdentityDbContext<IdentityUser, IdentityRole, string>(options), ICarRentalDbContext
{
    public DbSet<Location> Locations { get; set; }
    public DbSet<Car> Cars { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Rental> Rentals { get; set; }
    public DbSet<Service> Services { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Customer
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.FullName).IsRequired();
            entity.Property(c => c.Address).IsRequired();
        });

        // Car
        modelBuilder.Entity<Car>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Type).IsRequired();
            entity.Property(c => c.Model).IsRequired();

            entity.HasMany(c => c.Services)
                  .WithOne(s => s.Car)
                  .HasForeignKey(s => s.CarId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Service
        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(s => s.Id);
            entity.Property(s => s.Date).IsRequired();
            entity.HasOne(s => s.Car)
                  .WithMany(c => c.Services)
                  .HasForeignKey(s => s.CarId);
        });

        // Rental
        modelBuilder.Entity<Rental>(entity =>
        {
            entity.HasKey(r => r.Id);
            entity.Property(r => r.StartDate).IsRequired();
            entity.Property(r => r.EndDate).IsRequired();

            entity.HasOne(r => r.Customer)
                  .WithMany()
                  .HasForeignKey("CustomerId")
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(r => r.Car)
                  .WithMany(c => c.Rentals)
                  .HasForeignKey("CarId")
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}