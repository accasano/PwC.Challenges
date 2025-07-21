namespace PwC.CarRental.Application.Queries.Statistics;

public class MostRentedCarTypeUtilizationRankingDto : MostRentedCarTypeUtilizationDto
{
    public int Ranking { get; set; }
}

public class MostRentedCarTypeUtilizationDto
{
    public string CarType { get; set; }
    public int RentalCount { get; set; }
    public double UtilizationPercentage { get; set; }
}

public class MostUsedCarRankingDto : MostUsedCarDto
{
    public Guid CarId { get; set; }
    public int Ranking { get; set; }
    public int RentalCount { get; set; }
    public int TotalRentalDays { get; set; }
    public double UtilizationPercentage { get; set; }
}

public class MostUsedCarDto
{
    public string Model { get; set; }
    public string Type { get; set; }
}

public class MostUsedCarGroupRankingDto : MostUsedCarDto
{
    public int Ranking { get; set; }
}

public class WeeklyRentalStatisticsDto
{
    public int Cancellations { get; set; }
    public int Rentals { get; set; }
    public int UnusedCars { get; set; }
}