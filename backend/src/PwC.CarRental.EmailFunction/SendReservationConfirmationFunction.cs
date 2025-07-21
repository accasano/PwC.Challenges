using MailKit.Net.Smtp;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using PwC.CarRental.EmailFunction.Configurations;
using System.Net;

namespace PwC.CarRental.EmailFunction;

public class SendReservationConfirmationFunction(IOptions<MailConfiguration> configuration, ILoggerFactory loggerFactory)
{
    private readonly MailConfiguration _configuration = configuration.Value;
    private readonly ILogger _logger = loggerFactory.CreateLogger<SendReservationConfirmationFunction>();

    [Function("SendReservationConfirmation")]
    public async Task<HttpResponseData> HttpStart(
       [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req,
       [DurableClient] DurableTaskClient client)
    {
        var payload = await req.ReadFromJsonAsync<ReservationEmailPayload>();
        if (payload is null)
        {
            var badRequest = req.CreateResponse(HttpStatusCode.BadRequest);
            await badRequest.WriteStringAsync("Invalid payload.");
            return badRequest;
        }

        var instanceId = await client.ScheduleNewOrchestrationInstanceAsync(nameof(RunOrchestrator), payload);
        _logger.LogInformation($"Started orchestration with ID = '{instanceId}'.");

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteStringAsync($"Orchestration started with ID: {instanceId}");
        return response;
    }

    [Function("RunOrchestrator")]
    public async Task RunOrchestrator([OrchestrationTrigger] TaskOrchestrationContext context)
    {
        var payload = context.GetInput<ReservationEmailPayload>();
        if (payload is null)
            return;

        var output = await context.CallActivityAsync<string>(nameof(SendMail), payload);
        return;
    }

    [Function("SendMail")]
    public void SendMail([ActivityTrigger] ReservationEmailPayload payload)
    {
        var msg = new MimeMessage();
        msg.From.Add(new MailboxAddress(_configuration.FromName, _configuration.From));
        msg.Subject = payload.Subject;
        msg.Body = new TextPart(MimeKit.Text.TextFormat.Plain)
        {
            Text = payload.Body
        };
        msg.To.Add(new MailboxAddress(payload.To, payload.To));

        using var smtpClient = new SmtpClient();
        smtpClient.Connect(_configuration.Host, _configuration.Port, false);
        smtpClient.Authenticate(_configuration.User, _configuration.Password);
        smtpClient.Send(msg);
        smtpClient.Disconnect(true);
    }
}

public record ReservationEmailPayload(string To, string Subject, string Body);