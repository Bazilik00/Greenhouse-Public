using Shared.Models;
using Shared.ValueModels;

namespace Shared.Dto.ServerResponses;

public class InsideMicroclimateState
{
    public HumidityModel HumidityData { get; set; }
    public TemperatureModel TemperatureData { get; set; }
    public bool FanOn { get; set; }
    
    public bool WindowOn { get; set; }
    
    public bool HumidifierOn { get; set; }
    public bool HasWater { get; set; }
}