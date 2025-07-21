namespace PwC.CarRental.Application.Common.Interfaces;

public interface IEmailService
{
    Task SendReservationConfirmationAsync(string toEmail, string subject, string body, CancellationToken cancellationToken = default);
}