using Microsoft.Extensions.Options;
using PwC.CarRental.Infrastructure.Configurations;

namespace PwC.CarRental.Api.Extensions;

public static class ConfigurationExtensions
{
    public static IServiceCollection AddCustomConfigurations(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<EmailFunctionConfiguration>(configuration.GetSection(nameof(EmailFunctionConfiguration)));
        services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<EmailFunctionConfiguration>>().Value);

        services.Configure<JwtConfiguration>(configuration.GetSection(nameof(JwtConfiguration)));
        services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<JwtConfiguration>>().Value);

        return services;
    }
}