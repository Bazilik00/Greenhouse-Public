using Shared.Models;
using Shared.ValueModels;

namespace Shared.Dto.ServerResponses;

public class ChartsResponse
{
    public required ChartValue[] Charts { get; set; }
}

public class ChartValue
{
    public DateTime DateTime { get; set; }

    public double Value { get; set; }

    public required string Source { get; set; }
}