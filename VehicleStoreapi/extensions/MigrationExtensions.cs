using Microsoft.EntityFrameworkCore;
using VehicleStoreapi.Database;

namespace VehicleStoreapi.extensions;

public class MigrationExtensions
{
    public static void ApplyMigrations(IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();

        using AppDbContext context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        context.Database.Migrate();
    }
}