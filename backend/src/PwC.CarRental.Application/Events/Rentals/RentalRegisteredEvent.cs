using MediatR;

namespace PwC.CarRental.Application.Events.Rentals;

public record RentalRegisteredEvent(Guid RentalId, Guid CustomerId, string UserEmail) : INotification;
