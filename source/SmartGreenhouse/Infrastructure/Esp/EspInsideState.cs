using Shared.Models;
using Shared.ValueModels;

namespace Infrastructure.Esp;

public class EspInsideState
{
	public required double Temperature { get; set; }

	public required double Humidity { get; set; }

	public required bool WindowOpen { get; set; }

	public required int SoilHumidity { get; set; }

	public required RgbState Rgb { get; set; }

	public required int Illumination { get; set; }

	public required bool FanOn { get; set; }

	public required bool ValveOn { get; set; }

	public required bool HumidifierOn { get; set; }

	public required bool HasPlant { get; set; }
	
	public required bool HasWater { get; set; }
}
