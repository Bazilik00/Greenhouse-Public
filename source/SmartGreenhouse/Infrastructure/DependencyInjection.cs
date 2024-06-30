using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


namespace Infrastructure;

public static class DependencyInjection
{
    public static void AddDataAccess(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database");

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new Exception("Connection string event db is not configured.");
        }

        services.AddDbContext<AppDbContext>(x => x.UseSqlite(connectionString));
    }

    public static void MigrateDatabase(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var dbContext = scope.ServiceProvider
            .GetRequiredService<AppDbContext>();

        dbContext.Database.Migrate();
    }
}