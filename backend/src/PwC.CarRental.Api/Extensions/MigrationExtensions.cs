using Microsoft.EntityFrameworkCore;
using PwC.CarRental.Infrastructure.Persistence;

namespace PwC.CarRental.Api.Extensions;

public static class MigrationExtensions
{
    public static void MigrateDatabase(this IHost app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CarRentalDbContext>();
        db.Database.Migrate();
    }
}