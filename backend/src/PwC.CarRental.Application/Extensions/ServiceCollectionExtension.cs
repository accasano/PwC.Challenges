using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using PwC.CarRental.Application.Behaviors;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace PwC.CarRental.Application.Extensions;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtension
{
    public static IServiceCollection AddApplicationBehaviors(this IServiceCollection services)
    {
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }
}
