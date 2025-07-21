namespace PwC.CarRental.Domain.Entities;

public class Service
{
    public Guid Id { get; set; }
    public Guid CarId { get; set; }
    public Car Car { get; set; } = default!;
    public DateTime Date { get; set; }
    public int DurationDays { get; set; }
    public DateTime EndDate => Date.AddDays(DurationDays);
}