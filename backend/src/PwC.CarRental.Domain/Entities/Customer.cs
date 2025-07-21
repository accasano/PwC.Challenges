namespace PwC.CarRental.Domain.Entities;

public class Customer(Guid id, string fullName, string address)
{
    public Guid Id { get; private set; } = id;
    public string FullName { get; private set; } = fullName;
    public string Address { get; private set; } = address;
}