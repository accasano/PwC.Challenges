namespace PwC.CarRental.Application.Queries.Statistics;

public class CarServiceScheduleDto
{
    public Guid CarId { get; set; }
    public string Model { get; set; } = default!;
    public string Type { get; set; } = default!;
    public DateTime ServiceDate { get; set; }
    public string LocationName { get; set; } = default!;
}