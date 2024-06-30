using Infrastructure.Esp;
using Shared.Dto;
using Shared.Dto.ServerResponses;

namespace Server.Services;

public class OutsideSensorsService(EspClientLocator espClientLocator)
{
    private readonly EspClient _espClient = 
        espClientLocator.GetClient(ModbusEspOptions.EspType.Outside, 1) ?? throw new Exception();

    public async Task<OutsideMicroclimateState> GetSensorsData()
    {
        var state = await _espClient.GetOutsideEspState();
        
        return new OutsideMicroclimateState()
        {
            Temperature = state.Temperature,
            Humidity = state.Humidity,
            Illumination = state.Illumination,
            HasWater = state.HasWater
        };
    }
}