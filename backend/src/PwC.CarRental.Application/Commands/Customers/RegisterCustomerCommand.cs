using MediatR;

namespace PwC.CarRental.Application.Commands.Customers;

public record RegisterCustomerCommand(
    string FullName,
    string Address
) : IRequest<Guid>;
