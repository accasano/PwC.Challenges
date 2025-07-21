using MediatR;
using Microsoft.AspNetCore.Mvc;
using PwC.CarRental.Api.Auth.Commands;
using PwC.CarRental.Api.Contracts.Rentals;
using PwC.CarRental.Application.Commands.Customers;
using PwC.CarRental.Application.Commands.Rentals;
using PwC.CarRental.Application.Queries.Rentals;
using PwC.CarRental.Application.Queries.Statistics;
using System.Security.Claims;

namespace PwC.CarRental.Api.Extensions;

public static class EndpointExtensions
{
    public static void MapApiEndpoints(this WebApplication app)
    {
        app.MapAuthApiEndpoints();

        // Register a customer
        app.MapPost("/api/customers", async (IMediator mediator, RegisterCustomerCommand command) =>
        {
            var customerId = await mediator.Send(command);
            return Results.Created($"/api/customers/{customerId}", customerId);
        }).RequireAuthorization();

        // Check car availability
        app.MapGet("/api/cars/availability", async (IMediator mediator, [AsParameters] CheckAvailabilityQuery query) =>
        {
            var result = await mediator.Send(query);
            return result ? Results.Ok() : Results.NotFound();
        }).RequireAuthorization(); ;

        // Register a rental
        app.MapPost("/api/rentals", async (IMediator mediator, RegisterRentalRequest request, ClaimsPrincipal user) =>
        {
            var email = user.FindFirst(ClaimTypes.Email)?.Value ?? user.FindFirst("email")?.Value;
            var command = new RegisterRentalCommand(
                request.CustomerId,
                request.CarId,
                request.StartDate,
                request.EndDate,
                email
            );
            var rentalId = await mediator.Send(command);
            return Results.Created($"/api/rentals/{rentalId}", rentalId);
        }).RequireAuthorization();

        // Modify a reservation
        app.MapPut("/api/rentals/{rentalId}", async (IMediator mediator, Guid rentalId, [FromBody] ModifyReservationCommand command) =>
        {
            command = command with { RentalId = rentalId };
            return await mediator.Send(command) ? Results.Ok() : Results.NotFound();
        }).RequireAuthorization();

        // Cancel a rental
        app.MapDelete("/api/rentals/{rentalId}", async (IMediator mediator, Guid rentalId) =>
        {
            var command = new CancelRentalCommand(rentalId);
            return await mediator.Send(command) ? Results.Ok() : Results.NotFound();
        }).RequireAuthorization();

        // Get services scheduled
        app.MapGet("/api/services", async (IMediator mediator, [AsParameters] GetServiceScheduleQuery query) =>
        {
            var result = await mediator.Send(query);
            return result.Any() ? Results.Ok(result) : Results.NotFound();
        }).RequireAuthorization();

        // Most rented car type
        app.MapGet("/api/statistics/car-types/most-rented", async (IMediator mediator) =>
        {
            var result = await mediator.Send(new GetMostRentedCarTypeUtilizationQuery());
            return result != null ? Results.Ok(result) : Results.NotFound();
        }).RequireAuthorization();

        // Ranking of most rented car types
        app.MapGet("/api/statistics/car-types/most-rented/ranking", async (IMediator mediator, [AsParameters] GetMostRentedCarTypeUtilizationRankingQuery query) =>
        {
            var result = await mediator.Send(query);
            return result.Any() ? Results.Ok(result) : Results.NotFound();
        }).RequireAuthorization();

        // Most used cars overall
        app.MapGet("/api/statistics/cars/most-used", async (IMediator mediator) =>
        {
            var result = await mediator.Send(new GetMostUsedCarsRankingQuery());
            return result.Any() ? Results.Ok(result) : Results.NotFound();
        }).RequireAuthorization();

        // Most used cars by brand, model, type
        app.MapGet("/api/statistics/cars/most-used/by-details", async (IMediator mediator) =>
        {
            var result = await mediator.Send(new GetMostUsedCarsRankingGroupQuery());
            return result.Any() ? Results.Ok(result) : Results.NotFound();
        }).RequireAuthorization();

        // Daily operational stats
        app.MapGet("/api/statistics/daily-activity", async (IMediator mediator) =>
        {
            var result = await mediator.Send(new GetWeeklyRentalStatisticsQuery());
            return result != null ? Results.Ok(result) : Results.NotFound();
        });
    }

    private static void MapAuthApiEndpoints(this WebApplication app)
    {
        app.MapPost("/api/auth/register", async (IMediator mediator, [AsParameters] RegisterUserCommand command) =>
        {
            var result = await mediator.Send(command);
            return result.Succeeded ? Results.Ok() : Results.BadRequest(result.Errors);
        });

        app.MapPost("/api/auth/login", async (IMediator mediator, [AsParameters] LoginUserCommand command) =>
        {
            var result = await mediator.Send(command);
            return result.Succeeded ? Results.Ok(new { token = result.Token }) : Results.Unauthorized();
        });
    }
}
