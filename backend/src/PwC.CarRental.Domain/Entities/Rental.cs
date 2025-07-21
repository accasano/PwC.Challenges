namespace PwC.CarRental.Domain.Entities;

public class Rental
{
    public Guid Id { get; private set; }
    public Customer Customer { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public Car Car { get; private set; }
    public bool IsCancelled { get; private set; }

    public Rental()
    {
    }

    public Rental(Customer customer, DateTime startDate, DateTime endDate, Car car)
    {
        Id = Guid.NewGuid();
        Customer = customer;
        StartDate = startDate;
        EndDate = endDate;
        Car = car;
    }

    internal void UpdateDates(DateTime newStart, DateTime newEnd)
    {
        StartDate = newStart;
        EndDate = newEnd;
    }

    internal void UpdateCar(Car newCar)
    {
        Car = newCar;
    }

    internal void Cancel()
    {
        IsCancelled = true;
    }
}