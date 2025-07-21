using MediatR;
using PwC.CarRental.Application.Events.Rentals;
using PwC.CarRental.Domain.Aggregates;
using PwC.CarRental.Domain.Interfaces;

namespace PwC.CarRental.Application.Commands.Rentals;

public class RegisterRentalCommandHandler(IMediator mediator, RentalSystem rentalSystem, ICustomerRepository customerRepository, ICarRepository carRepository) : IRequestHandler<RegisterRentalCommand, Guid>
{
    private readonly IMediator _mediator = mediator;
    private readonly RentalSystem _rentalSystem = rentalSystem;
    private readonly ICustomerRepository _customerRepository = customerRepository;
    private readonly ICarRepository _carRepository = carRepository;

    public async Task<Guid> Handle(RegisterRentalCommand request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(request.CustomerId, cancellationToken)
            ?? throw new InvalidOperationException("Customer not found.");

        var car = await _carRepository.GetByIdAsync(request.CarId, cancellationToken)
            ?? throw new InvalidOperationException("Car not found.");

        var rental = await _rentalSystem.RegisterRentalAsync(
            customer,
            car,
            request.StartDate,
            request.EndDate,
            cancellationToken);

        await _mediator.Publish(new RentalRegisteredEvent(rental.Id, request.CustomerId, request.UserEmail), cancellationToken);

        return rental.Id;
    }
}