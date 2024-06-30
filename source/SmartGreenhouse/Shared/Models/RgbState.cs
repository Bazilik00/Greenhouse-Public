using Shared.ValueModels;

namespace Shared.Models;

public class RgbState
{
    public string Color { get; set; }

    public int Brightness { get; set; }

    public bool Power { get; set; }

    public RgbMode Mode { get; set; }
}