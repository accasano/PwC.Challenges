namespace PwC.CarRental.Domain.Entities;

public class Location
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public ICollection<Car> Cars { get; set; } = [];
}