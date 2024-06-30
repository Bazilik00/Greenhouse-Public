using System.ComponentModel;
using Infrastructure.Database;
using Infrastructure.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Server.Services;
using Shared.Dto;
using Shared.Dto.ServerRequests;
using Shared.Dto.ServerResponses;
using Shared.Models;

namespace Server;

public class BackgroundWorker(
    ILogger<BackgroundWorker> logger,
    IServiceScopeFactory serviceScopeFactory
) : BackgroundService
{
    private readonly PeriodicTimer _timer = new PeriodicTimer(TimeSpan.FromSeconds(10));

    private DateTime _lastUpdate = DateTime.Now;
    private DateTime _lastDrop = DateTime.Now;


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (await _timer.WaitForNextTickAsync(stoppingToken))
        {
            using var scope = serviceScopeFactory.CreateScope();

            var insideSensorsService =
                scope.ServiceProvider.GetRequiredService<InsideSensorsService>();

            var outsideSensorsService =
                scope.ServiceProvider.GetRequiredService<OutsideSensorsService>();

            var dbContext =
                scope.ServiceProvider.GetRequiredService<AppDbContext>();


            await WorkProsessing(outsideSensorsService, insideSensorsService, dbContext, stoppingToken);

            if (DateTime.Now -_lastUpdate  > TimeSpan.FromMinutes(1))
            {
                logger.LogInformation("Updating charts");
                await UpdateCharts(stoppingToken, outsideSensorsService, insideSensorsService, dbContext);
                _lastUpdate = DateTime.Now;
            }

            if (DateTime.Now - _lastDrop  > TimeSpan.FromMinutes(10))
                logger.LogInformation("Drop old values");
            {
                DropOld(dbContext);
                _lastDrop = DateTime.Now;
            }


            await dbContext.SaveChangesAsync(cancellationToken: stoppingToken);
        }
    }

    private async Task WorkProsessing(OutsideSensorsService outsideSensorsService,
        InsideSensorsService insideSensorsService,
        AppDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var insideClients = insideSensorsService.GetClientsHeaders();

        var outsideSensors = await outsideSensorsService.GetSensorsData();

        foreach (var clientId in insideClients.Ids)
        {
            var settings = await dbContext.EspClientSettings
                .FirstOrDefaultAsync(x => x.Id == clientId, cancellationToken: cancellationToken);

            await insideSensorsService.SetSyncHasWater(outsideSensors.HasWater, clientId);
            
            if (settings == null) continue;
            
            var sensors = await insideSensorsService.GetSensorsData(clientId);
            
            await TemperatureHandle(insideSensorsService, sensors, settings, clientId);
            await HumidityHandle(insideSensorsService, sensors, settings, clientId);
            await IlluminationHandle(insideSensorsService, sensors, settings, clientId);
        }
    }

    private async Task TemperatureHandle(InsideSensorsService insideSensorsService, InsideSensorsDto sensors,
        EspClientSetting settings, int clientId)
    {
        if (sensors.Temperature >= settings.MaxTemperature && sensors is not { WindowOpen: true, FanOn: true })
        {
            logger.LogInformation("Open window and fan");

            await insideSensorsService.SetFanOn(new SetPowerRequest()
            {
                Power = true
            }, clientId);

            await insideSensorsService.SetWindowOpen(new SetPowerRequest()
            {
                Power = true
            }, clientId);
        }


        if (sensors.Temperature <= settings.MinTemperature && sensors is not { WindowOpen: false, FanOn: false })
        {
            logger.LogInformation("Close window and fan");
            await insideSensorsService.SetFanOn(new SetPowerRequest()
            {
                Power = false
            }, clientId);

            await insideSensorsService.SetWindowOpen(new SetPowerRequest()
            {
                Power = false
            }, clientId);
        }
    }
    
    private async Task HumidityHandle(InsideSensorsService insideSensorsService, InsideSensorsDto sensors,
        EspClientSetting settings, int clientId)
    {
        if (sensors.Humidity >= settings.MaxHumidity && sensors is { HumidifierOn: true})
        {
            logger.LogInformation("Close humidifier");

            await insideSensorsService.SetHumidifierOn(new SetPowerRequest()
            {
                Power = false
            }, clientId);
        }

        if (sensors.Humidity < settings.MinHumidity && sensors is { HumidifierOn: false })
        {
            logger.LogInformation("Open humidifier");

            await insideSensorsService.SetHumidifierOn(new SetPowerRequest()
            {
                Power = true
            }, clientId);
        }
        
    }

    private async Task IlluminationHandle(InsideSensorsService insideSensorsService, InsideSensorsDto sensors,
        EspClientSetting settings, int clientId)
    {
        if (sensors.Illumination >= settings.MaxIllumination && sensors.Rgb.Power)
        {
            logger.LogInformation("Close rgb");

            sensors.Rgb.Power = false;
            
            await insideSensorsService.SetRgbState(new SetRgbStateRequest()
            {
                State = sensors.Rgb
            }, clientId);
        }

        if (sensors.Illumination < settings.MinIllumination && !sensors.Rgb.Power)
        {
            logger.LogInformation("Open humidifier");

            sensors.Rgb.Power = true;
            
            await insideSensorsService.SetRgbState(new SetRgbStateRequest()
            {
                State = sensors.Rgb
            }, clientId);
        }
        
    }


    private async Task UpdateCharts(CancellationToken stoppingToken, OutsideSensorsService outsideSensorsService,
        InsideSensorsService insideSensorsService, AppDbContext dbContext)
    {
        var insideClients = insideSensorsService.GetClientsHeaders();
        var outsideSensors = await outsideSensorsService.GetSensorsData();
        var insideSensors =
            insideClients.Ids.ToDictionary(x => x, async x => await insideSensorsService.GetSensorsData(x));

        await AddOutsideValues(stoppingToken, outsideSensors, dbContext);


        foreach (var insideSensor in insideSensors)
        {
            var sensors = await insideSensor.Value;
            await dbContext.TemperatureChartsData.AddAsync(new TemperatureChartsData()
            {
                Source = insideSensor.Key.ToString(),
                Value = sensors.Temperature,
                DateTime = DateTime.Now
            }, stoppingToken);

            await dbContext.HumidityChartsData.AddAsync(new HumidityChartsData()
            {
                Source = insideSensor.Key.ToString(),
                Value = sensors.Humidity,
                DateTime = DateTime.Now
            }, stoppingToken);

            await dbContext.SoilHumidityChartsData.AddAsync(new SoilHumidityChartsData()
            {
                Source = insideSensor.Key.ToString(),
                Value = sensors.SoilHumidity,
                DateTime = DateTime.Now
            }, stoppingToken);

            await dbContext.IlluminationChartsData.AddAsync(new IlluminationChartsData()
            {
                Source = insideSensor.Key.ToString(),
                Value = sensors.Illumination,
                DateTime = DateTime.Now
            }, stoppingToken);
        }
    }

    private static void DropOld(AppDbContext dbContext)
    {
        var oldIllumination = dbContext.IlluminationChartsData
            .Where(log => log.DateTime < DateTime.Now.AddDays(-2));

        var oldHumidity = dbContext.HumidityChartsData
            .Where(log => log.DateTime < DateTime.Now.AddDays(-2));

        var oldTemperature = dbContext.TemperatureChartsData
            .Where(log => log.DateTime < DateTime.Now.AddDays(-2));

        var oldSoilHumidity = dbContext.SoilHumidityChartsData
            .Where(log => log.DateTime < DateTime.Now.AddDays(-2));

        dbContext.HumidityChartsData.RemoveRange(oldHumidity);
        dbContext.TemperatureChartsData.RemoveRange(oldTemperature);
        dbContext.SoilHumidityChartsData.RemoveRange(oldSoilHumidity);
        dbContext.IlluminationChartsData.RemoveRange(oldIllumination);
    }

    private async Task AddOutsideValues(CancellationToken stoppingToken, OutsideMicroclimateState outsideSensors,
        AppDbContext dbContext)
    {
        await dbContext.IlluminationChartsData.AddAsync(new IlluminationChartsData()
        {
            Source = "outside",
            Value = outsideSensors.Illumination,
            DateTime = DateTime.Now
        }, stoppingToken);

        await dbContext.HumidityChartsData.AddAsync(new HumidityChartsData()
        {
            Source = "outside",
            Value = outsideSensors.Humidity,
            DateTime = DateTime.Now
        }, stoppingToken);

        await dbContext.TemperatureChartsData.AddAsync(new TemperatureChartsData()
        {
            Source = "outside",
            Value = outsideSensors.Temperature,
            DateTime = DateTime.Now
        }, stoppingToken);
    }
}