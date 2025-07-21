using PwC.CarRental.Api.Middleware;

namespace PwC.CarRental.Api.Extensions;

public static class MiddlewareExtensions
{
    public static void UseCustomMiddlewares(this IApplicationBuilder app)
    {
        app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
    }
}