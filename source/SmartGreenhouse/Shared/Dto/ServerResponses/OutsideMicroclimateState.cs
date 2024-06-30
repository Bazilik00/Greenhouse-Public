namespace Shared.Dto.ServerResponses;

public class OutsideMicroclimateState
{
    public double Temperature { get; set; }
    public double Humidity { get; set; }
    public int Illumination { get; set; }
    public bool HasWater { get; set; }
}