using PwC.CarRental.Domain.Entities;
using PwC.CarRental.Domain.Interfaces;

namespace PwC.CarRental.Domain.Aggregates;

public class RentalSystem(ICustomerRepository customerRepository, IRentalRepository rentalRepository, ICarRepository carRepository, IServiceRepository serviceRepository)
{
    private readonly ICustomerRepository _customerRepository = customerRepository;
    private readonly IRentalRepository _rentalRepository = rentalRepository;
    private readonly ICarRepository _carRepository = carRepository;
    private readonly IServiceRepository _serviceRepository = serviceRepository;

    public async Task<Customer> CreateCustomer(string fullName, string address, CancellationToken cancellationToken)
    {
        ValidateCustomerInput(fullName, address);

        var existingCustomers = await _customerRepository.GetAllAsync(cancellationToken);
        if (IsDuplicateCustomer(existingCustomers, fullName, address))
            throw new InvalidOperationException("Customer already registered.");

        var customer = new Customer(Guid.NewGuid(), fullName, address);
        await _customerRepository.AddAsync(customer, cancellationToken);
        return customer;
    }

    public async Task<bool> CheckCarAvailabilityAsync(Car car, DateTime rentalStart, DateTime rentalEnd, CancellationToken cancellationToken)
    {
        var rentals = await _rentalRepository.GetRentalsForCarAsync(car.Id, cancellationToken);

        if (IsCarRented(car, rentals, rentalStart, rentalEnd))
            return false;

        if (IsNextDayBlocked(rentals, rentalStart))
            return false;

        var services = await _serviceRepository.GetServicesForCarAsync(car.Id, rentalStart, rentalEnd, cancellationToken);
        if (services.Any())
            return false;

        return true;
    }

    public async Task<Rental> RegisterRentalAsync(Customer customer, Car car, DateTime start, DateTime end, CancellationToken cancellationToken)
    {
        ValidateRentalDates(start, end);

        if (!await CheckCarAvailabilityAsync(car, start, end, cancellationToken))
            throw new InvalidOperationException("Car is not available.");

        var rental = new Rental(customer, start, end, car);
        await _rentalRepository.AddAsync(rental, cancellationToken);
        return rental;
    }

    public async Task ModifyReservationAsync(Guid rentalId, DateTime? newStart, DateTime? newEnd, Guid? newCarId, CancellationToken cancellationToken)
    {
        if (NoChangesRequested(newStart, newEnd, newCarId))
            return;

        var rental = await _rentalRepository.GetByIdAsync(rentalId, cancellationToken)
            ?? throw new InvalidOperationException("Rental not found.");

        var startDate = newStart ?? rental.StartDate;
        var endDate = newEnd ?? rental.EndDate;
        var carId = newCarId ?? rental.Car.Id;

        ValidateRentalDates(startDate, endDate);

        var newCar = await GetCarIfChangedAsync(rental, carId, cancellationToken);

        if (!await CheckCarAvailabilityAsync(newCar, startDate, endDate, cancellationToken))
            throw new InvalidOperationException("Selected car is not available for the new dates.");

        UpdateRentalIfChanged(rental, newCar, carId, startDate, endDate);

        await _rentalRepository.UpdateAsync(rental, cancellationToken);
    }

    public async Task CancelRentalAsync(Guid rentalId, CancellationToken cancellationToken)
    {
        var rental = await _rentalRepository.GetByIdAsync(rentalId, cancellationToken)
            ?? throw new InvalidOperationException("Rental not found.");

        if (rental.StartDate <= DateTime.UtcNow)
            throw new InvalidOperationException("Cannot cancel a rental that has already started.");

        rental.Cancel();
        await _rentalRepository.UpdateAsync(rental, cancellationToken);
    }

    private static void ValidateCustomerInput(string fullName, string address)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Full name is required.", nameof(fullName));
        if (string.IsNullOrWhiteSpace(address))
            throw new ArgumentException("Address is required.", nameof(address));
    }

    private static bool IsDuplicateCustomer(IEnumerable<Customer> customers, string fullName, string address)
        => customers.Any(c => c.FullName == fullName && c.Address == address);

    private static void ValidateRentalDates(DateTime start, DateTime end)
    {
        if (start.Date <= DateTime.UtcNow.Date)
            throw new ArgumentException("Rental start date must be after today.", nameof(start));
        if (end.Date <= start.Date)
            throw new ArgumentException("Rental end date must be after start date.", nameof(end));
    }

    private static bool NoChangesRequested(DateTime? newStart, DateTime? newEnd, Guid? newCarId)
        => newStart is null && newEnd is null && newCarId is null;

    private static bool IsCarRented(Car car, IEnumerable<Rental> rentals, DateTime rentalStart, DateTime rentalEnd)
    {
        return rentals.Any(r =>
            r.Car.Id == car.Id &&
            (
                (rentalStart >= r.StartDate && rentalStart < r.EndDate) ||
                (rentalEnd > r.StartDate && rentalEnd <= r.EndDate)
            )
        );
    }

    private static bool IsNextDayBlocked(IEnumerable<Rental> rentals, DateTime rentalStart)
    {
        return rentals.Any(r =>
            !r.IsCancelled &&
            r.EndDate.AddDays(1).Date > rentalStart.Date &&
            r.EndDate.Date < rentalStart.Date
        );
    }

    private async Task<Car> GetCarIfChangedAsync(Rental rental, Guid carId, CancellationToken cancellationToken)
    {
        if (rental.Car.Id == carId)
            return rental.Car;

        var newCar = await _carRepository.GetByIdAsync(carId, cancellationToken)
            ?? throw new InvalidOperationException("Selected car not found.");
        return newCar;
    }

    private static void UpdateRentalIfChanged(Rental rental, Car newCar, Guid carId, DateTime startDate, DateTime endDate)
    {
        if (rental.Car.Id != carId)
            rental.UpdateCar(newCar);
        if (rental.StartDate != startDate || rental.EndDate != endDate)
            rental.UpdateDates(startDate, endDate);
    }
}
