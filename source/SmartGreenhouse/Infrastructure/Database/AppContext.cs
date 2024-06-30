using Infrastructure.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public class SoilHumidityChartsData : ChartsData { }
public class HumidityChartsData : ChartsData { }
public class TemperatureChartsData : ChartsData { }
public class IlluminationChartsData : ChartsData { }
public class AppDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Setting> Settings { get; set; } = default!;
    
    public DbSet<SoilHumidityChartsData> SoilHumidityChartsData { get; set; } = default!;
    public DbSet<HumidityChartsData> HumidityChartsData { get; set; } = default!;
    public DbSet<TemperatureChartsData> TemperatureChartsData { get; set; } = default!;
    public DbSet<IlluminationChartsData> IlluminationChartsData { get; set; } = default!;
    public DbSet<EspClientSetting> EspClientSettings { get; set; } = default!;
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DbContextOptions).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}