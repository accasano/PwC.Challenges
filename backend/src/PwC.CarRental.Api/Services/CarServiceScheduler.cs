using MediatR;
using PwC.CarRental.Application.Commands.Services;
using PwC.CarRental.Domain.Interfaces;

namespace PwC.CarRental.Api.Services;

// A background service has been launched to schedule maintenance tasks for the vehicles.
public class CarServiceScheduler(IServiceProvider serviceProvider) : BackgroundService
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _serviceProvider.CreateScope();
            var _mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var _serviceRepository = scope.ServiceProvider.GetRequiredService<IServiceRepository>();
            var _carRepository = scope.ServiceProvider.GetRequiredService<ICarRepository>();

            var cars = await _carRepository.GetAllAsync(stoppingToken);

            foreach (var car in cars)
            {
                var lastService = await _serviceRepository.GetLastServiceForCarAsync(car.Id, stoppingToken);
                var now = DateTime.UtcNow;

                if (lastService == null || lastService.Date.AddMonths(2) <= now)
                    await _mediator.Send(new RegisterServiceCommand(car.Id, now), stoppingToken);
            }

            await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
        }
    }
}
