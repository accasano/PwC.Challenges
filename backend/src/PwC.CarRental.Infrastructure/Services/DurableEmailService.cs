using PwC.CarRental.Application.Common.Interfaces;
using PwC.CarRental.Infrastructure.Configurations;
using System.Net.Http.Json;

namespace PwC.CarRental.Infrastructure.Services;

public class DurableEmailService(HttpClient httpClient, EmailFunctionConfiguration emailFunction) : IEmailService
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly string _functionUrl = emailFunction.ReservationConfirmationUrl;

    public async Task SendReservationConfirmationAsync(string toEmail, string subject, string body, CancellationToken cancellationToken = default)
    {
        var payload = new
        {
            To = toEmail,
            Subject = subject,
            Body = body
        };
        await _httpClient.PostAsJsonAsync(_functionUrl, payload, cancellationToken);
    }
}