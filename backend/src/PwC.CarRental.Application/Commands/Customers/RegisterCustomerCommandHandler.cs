using MediatR;
using PwC.CarRental.Domain.Aggregates;

namespace PwC.CarRental.Application.Commands.Customers;

public class RegisterCustomerCommandHandler(RentalSystem rentalSystem) : IRequestHandler<RegisterCustomerCommand, Guid>
{
    private readonly RentalSystem _rentalSystem = rentalSystem;

    public async Task<Guid> Handle(RegisterCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = await _rentalSystem.CreateCustomer(request.FullName, request.Address, cancellationToken);

        return customer.Id;
    }
}