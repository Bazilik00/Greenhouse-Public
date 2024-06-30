using System.ComponentModel.DataAnnotations;
using Shared.ValueModels;

namespace Infrastructure.Database.Entities;

public class EspClientSetting
{
    [Key]
    public int Id { get; set; }
    
    public double MinTemperature { get; set; } = 19.0;
    public double MaxTemperature { get; set; } = 27.0;

    public double MinHumidity { get; set; } = 15;
    public double MaxHumidity { get; set; } = 70;

    public int MinIllumination { get; set; } = 10;
    public int MaxIllumination { get; set; } = 60;

    public int MinSoilHumidity { get; set; } = 15;
    public int MaxSoilHumidity { get; set; } = 70;

    public RgbMode RgbMode { get; set; } = RgbMode.Color;
    public string RgbColor { get; set; } = "#0000FF";
    public bool RgbPower { get; set; } = true;
    public int RgbBrightness { get; set; } = 30;
    
    public bool WindowOpen { get; set; } = false;
    public bool FanOn { get; set; } = true;
}