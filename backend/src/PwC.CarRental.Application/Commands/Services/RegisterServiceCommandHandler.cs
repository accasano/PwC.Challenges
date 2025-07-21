using MediatR;
using PwC.CarRental.Domain.Entities;
using PwC.CarRental.Domain.Interfaces;

namespace PwC.CarRental.Application.Commands.Services;

public class RegisterServiceCommandHandler(IServiceRepository serviceRepository) : IRequestHandler<RegisterServiceCommand, Guid>
{
    private readonly IServiceRepository _serviceRepository = serviceRepository;

    public async Task<Guid> Handle(RegisterServiceCommand request, CancellationToken cancellationToken)
    {
        var lastService = await _serviceRepository.GetLastServiceForCarAsync(request.CarId, cancellationToken);

        var durationMonths = 2;
        if (lastService != null && lastService.Date.AddMonths(durationMonths) > request.Date)
            throw new InvalidOperationException($"Service can only be registered every {durationMonths} month(s).");

        var service = new Service
        {
            Id = Guid.NewGuid(),
            CarId = request.CarId,
            Date = request.Date,
            DurationDays = request.DurationDays
        };

        await _serviceRepository.AddAsync(service, cancellationToken);

        return service.Id;
    }
}