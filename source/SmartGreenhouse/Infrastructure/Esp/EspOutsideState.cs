namespace Infrastructure.Esp;

public class EspOutsideState
{
	public required double Temperature { get; set; }

	public required double Humidity { get; set; }

	public required int Illumination { get; set; }
	
	public required bool HasWater { get; set; }

}