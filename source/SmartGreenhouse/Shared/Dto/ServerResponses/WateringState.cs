using Shared.Models;
using Shared.ValueModels;

namespace Shared.Dto.ServerResponses;

public class WateringState
{
    public HumidityModel SoilHumidityData { get; set; }
    
    public bool HasPlant { get; set; }
    
    public bool HasWater { get; set; }
    
    public bool PumpOn{ get; set; }
}