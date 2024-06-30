using Infrastructure.Database;
using Infrastructure.Database.Entities;
using Infrastructure.Esp;
using Microsoft.EntityFrameworkCore;
using Shared.Dto;
using Shared.Dto.ServerRequests;
using Shared.Dto.ServerResponses;
using Shared.Models;
using Shared.ValueModels;

namespace Server.Services;

public class InsideSensorsService(EspClientLocator clientLocator, AppDbContext context)
{
    public async Task<InsideSensorsDto[]> GetSensorsData()
    {
        var state = await clientLocator.GetInsideClient(1).GetEspState();
        var state2 = await clientLocator.GetInsideClient(2).GetEspState();

        return
        [
            new InsideSensorsDto
            {
                Temperature = state.Temperature,
                Humidity = state.Humidity,
                WindowOpen = state.WindowOpen,
                SoilHumidity = state.SoilHumidity,
                Illumination = state.Illumination,
                FanOn = state.FanOn,
                HumidifierOn = state.HumidifierOn,
                ValveOn = state.ValveOn,
                HasPlant = state.HasPlant,
                Rgb = state.Rgb
            },
            new InsideSensorsDto
            {
                Temperature = state2.Temperature,
                Humidity = state2.Humidity,
                WindowOpen = state2.WindowOpen,
                SoilHumidity = state2.SoilHumidity,
                Illumination = state2.Illumination,
                FanOn = state2.FanOn,
                HumidifierOn = state2.HumidifierOn,
                ValveOn = state2.ValveOn,
                HasPlant = state2.HasPlant,
                Rgb = state2.Rgb
            }
        ];
    }

    public async Task<InsideSensorsDto> GetSensorsData(int clientId)
    {
        var state = await clientLocator.GetInsideClient(clientId).GetEspState();

        return new InsideSensorsDto
        {
            Temperature = state.Temperature,
            Humidity = state.Humidity,
            WindowOpen = state.WindowOpen,
            SoilHumidity = state.SoilHumidity,
            Illumination = state.Illumination,
            FanOn = state.FanOn,
            HumidifierOn = state.HumidifierOn,
            ValveOn = state.ValveOn,
            HasPlant = state.HasPlant,
            Rgb = state.Rgb
        };
    }

    public async Task<InsideMicroclimateState> GetMicroclimate(int clientId)
    {
        var client = clientLocator.GetInsideClient(clientId);
        var settings = await GetClientSettings(clientId);

        var state = await client.GetEspState();

        return new InsideMicroclimateState()
        {
            FanOn = state.FanOn,
            WindowOn = state.WindowOpen,
            HumidifierOn = state.HumidifierOn,
            HasWater = state.HasWater,

            HumidityData = new HumidityModel()
            {
                Current = state.Humidity,
                Max = (int)settings.MaxHumidity,
                Min = (int)settings.MinHumidity
            },
            TemperatureData = new TemperatureModel()
            {
                Current = state.Temperature,
                Max = (int)settings.MaxTemperature,
                Min = (int)settings.MinTemperature
            }
        };
    }

    public async Task<LampState> GetLampState(int clientId)
    {
        var client = clientLocator.GetInsideClient(clientId);
        var settings = await GetClientSettings(clientId);


        var state = await client.GetEspState();

        return new LampState
        {
            IlluminationData = new IlluminationModel()
            {
                Current = state.Illumination,
                Max = settings.MaxIllumination,
                Min = settings.MinIllumination
            },
            RgbState = state.Rgb
        };
    }

    public async Task<WateringState> GetWateringState(int clientId)
    {
        var client = clientLocator.GetInsideClient(clientId);
        var settings = await GetClientSettings(clientId);

        var state = await client.GetEspState();

        return new WateringState
        {
            HasPlant = state.HasPlant,
            HasWater = state.HasWater,
            PumpOn = state.ValveOn,
            SoilHumidityData = new HumidityModel()
            {
                Current = state.SoilHumidity,
                Max = settings.MaxSoilHumidity,
                Min = settings.MinSoilHumidity,
            }
        };
    }

    public async Task InitSettingsFromDatabase()
    {
        var clients = clientLocator.GetInsideClients();

        foreach (var client in clients)
        {
            var settings = await context.EspClientSettings
                .FirstOrDefaultAsync(x => x.Id == client.Options.RoomId);

            if (settings is null)
            {
                settings = context.EspClientSettings.Add(new EspClientSetting()
                {
                    Id = client.Options.RoomId,
                }).Entity;

                await context.SaveChangesAsync();
            }

            await client.SetFanOn(settings.FanOn);
            await client.SetRgbBrightness(settings.RgbBrightness);
            await client.SetRgbColor(settings.RgbColor);
            await client.SetRgbMode(settings.RgbMode);
            await client.SetRgbPower(settings.RgbPower);
        }
    }

    public ClientsHeadersDto GetClientsHeaders() => new ClientsHeadersDto()
    {
        Ids = clientLocator
            .GetInsideClients()
            .Select(x => x.Options.RoomId)
            .ToArray()
    };


    // public Graphics GetGraphics()//string and data
    // {
    //     var rnd = Random.Shared.Next(0, 2);
    //     var typeArea = "";
    //     typeArea = rnd switch
    //     {
    //         0 => "Box1",
    //         1 => "Box2",
    //         2 => "Outside",
    //         _ => "Error",
    //     };
    //     var state = new Graphics()
    //     {
    //         TypeArea = typeArea,
    //         CellID = Random.Shared.Next() % 10,
    //         From = DateTime.Now.AddHours(Random.Shared.Next(-24, 0)), //"2024-03-13T22:09Z",
    //         To = DateTime.Now, //"2024-03-13T22:09Z",
    //     };
    //
    //     return state;
    // }
    public async Task SetMicroclimateTemperature(SetMinMaxSettingsRequest request, int clientId)
    {
        var settings = await GetClientSettings(clientId);

        settings.MaxTemperature = request.Max;
        settings.MinTemperature = request.Min;

        await context.SaveChangesAsync();
    }

    public async Task SetMicroclimateHumidity(SetMinMaxSettingsRequest request, int clientId)
    {
        var settings = await GetClientSettings(clientId);

        settings.MaxHumidity = request.Max;
        settings.MinHumidity = request.Min;

        await context.SaveChangesAsync();
    }

    public async Task SetSoilHumidity(SetMinMaxSettingsRequest request, int clientId)
    {
        var settings = await GetClientSettings(clientId);

        settings.MaxSoilHumidity = request.Max;
        settings.MinSoilHumidity = request.Min;

        await context.SaveChangesAsync();
    }

    public async Task SetIllumination(SetMinMaxSettingsRequest request, int clientId)
    {
        var settings = await GetClientSettings(clientId);

        settings.MaxIllumination = request.Max;
        settings.MinIllumination = request.Min;

        await context.SaveChangesAsync();
    }

    private async Task<EspClientSetting> GetClientSettings(int clientId)
    {
        var settings = await context.EspClientSettings
            .FirstOrDefaultAsync(x => x.Id == clientId) ?? throw new Exception("Settings not found");
        return settings;
    }

    public async Task SetFanOn(SetPowerRequest request, int clientId)
    {
        var client = clientLocator.GetInsideClient(clientId);
        var settings = await GetClientSettings(clientId);

        settings.FanOn = request.Power;

        await client.SetFanOn(request.Power);
        await context.SaveChangesAsync();
    }

    public async Task SetWindowOpen(SetPowerRequest request, int clientId)
    {
        var client = clientLocator.GetInsideClient(clientId);
        var settings = await GetClientSettings(clientId);

        settings.WindowOpen = request.Power;

        await client.SetWindowOn(request.Power);
        await context.SaveChangesAsync();
    }

    public async Task SetRgbState(SetRgbStateRequest request, int clientId)
    {
        var client = clientLocator.GetInsideClient(clientId);
        var settings = await GetClientSettings(clientId);

        var newRgbState = request.State;

        settings.RgbBrightness = newRgbState.Brightness;
        settings.RgbColor = newRgbState.Color;
        settings.RgbMode = newRgbState.Mode;
        settings.RgbPower = newRgbState.Power;

        await client.SetRgbBrightness(newRgbState.Brightness);
        await client.SetRgbColor(newRgbState.Color);
        await client.SetRgbMode(newRgbState.Mode);
        await client.SetRgbPower(newRgbState.Power);

        await context.SaveChangesAsync();
    }

    public async Task SetHumidifierOn(SetPowerRequest request, int clientId)
    {
        var client = clientLocator.GetInsideClient(clientId);

        await client.SetHumidifierOn(request.Power);
    }

    public async Task SetValveOn(SetPowerRequest request, int clientId)
    {
        var client = clientLocator.GetInsideClient(clientId);

        await client.SetValveOn(request.Power);
    }

    public async Task SetSyncHasWater(bool value, int clientId)
    {
        var client = clientLocator.GetInsideClient(clientId);

        await client.SetSyncWater(value);
    }
    
    public async Task SetPumpTick(int value, int clientId)
    {
        var client = clientLocator.GetInsideClient(clientId);

        await client.SetPumpTicksWater(value);
    }
}