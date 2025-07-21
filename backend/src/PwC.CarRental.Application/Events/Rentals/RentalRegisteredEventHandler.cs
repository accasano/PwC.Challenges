using MediatR;
using PwC.CarRental.Application.Common.Interfaces;
using PwC.CarRental.Domain.Interfaces;

namespace PwC.CarRental.Application.Events.Rentals;

public class RentalRegisteredEventHandler(ICarRentalDbContext context, IEmailService emailService) : INotificationHandler<RentalRegisteredEvent>
{
    public async Task Handle(RentalRegisteredEvent notification, CancellationToken cancellationToken)
    {
        var customer = await context.Customers.FindAsync([notification.CustomerId], cancellationToken);
        var rental = await context.Rentals.FindAsync([notification.RentalId], cancellationToken);
        var car = await context.Cars.FindAsync([rental.Car.Id], cancellationToken);

        if (customer != null && !string.IsNullOrEmpty(notification.UserEmail))
        {
            var subject = "Car Rental Reservation Confirmation";
            var body = $"""
                Dear {customer.FullName},

                Your car reservation has been confirmed.

                Reservation Details:
                - Car: {car.Model} ({car.Type})
                - Start Date: {rental.StartDate:yyyy-MM-dd}
                - End Date: {rental.EndDate:yyyy-MM-dd}
                - Location: {car.Location?.Name}

                Thank you for choosing our service!
            """;

            await emailService.SendReservationConfirmationAsync(notification.UserEmail, subject, body, cancellationToken);
        }
    }
}
