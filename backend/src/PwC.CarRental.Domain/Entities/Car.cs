namespace PwC.CarRental.Domain.Entities;

public class Car
{
    public Guid Id { get; set; }
    public string Type { get; set; } = default!;
    public string Model { get; set; } = default!;
    public Guid LocationId { get; set; }
    public Location Location { get; set; } = default!;
    public ICollection<Service> Services { get; set; } = [];
    public ICollection<Rental> Rentals { get; set; } = [];
}