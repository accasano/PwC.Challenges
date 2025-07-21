using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using PwC.CarRental.Api.Auth.Commands;
using PwC.CarRental.Api.Services;
using PwC.CarRental.Application.Behaviors;
using PwC.CarRental.Application.Commands.Rentals;
using PwC.CarRental.Application.Common.Interfaces;
using PwC.CarRental.Application.Extensions;
using PwC.CarRental.Domain.Aggregates;
using PwC.CarRental.Domain.Interfaces;
using PwC.CarRental.Infrastructure.Persistence;
using PwC.CarRental.Infrastructure.Repositories;
using PwC.CarRental.Infrastructure.Services;
using System.Reflection;
using System.Text.Json.Serialization;

namespace PwC.CarRental.Api.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddCustomServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(RegisterUserCommand).Assembly));
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(RegisterRentalCommand).Assembly));
        services.AddBehaviors();
        services.AddHostedService<CarServiceScheduler>();

        services.AddDbContext(configuration);
        services.AddScoped<RentalSystem>();

        services.AddMemoryCache();
        services.AddHttpClient<IEmailService, DurableEmailService>();
        services.Configure<JsonOptions>(options =>
        {
            options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        });

        return services;
    }

    private static void AddDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<CarRentalDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        services.AddScoped<ICarRentalDbContext>(provider => provider.GetRequiredService<CarRentalDbContext>());

        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<ICarRepository, CarRepository>();
        services.AddScoped<IRentalRepository, RentalRepository>();
        services.AddScoped<IServiceRepository, ServiceRepository>();
    }

    public static IServiceCollection AddBehaviors(this IServiceCollection services)
    {
        services.AddApplicationBehaviors();
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }
}