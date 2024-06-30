using Shared.Models;
using Shared.ValueModels;

namespace Shared.Dto.ServerRequests;

public class SetRgbStateRequest
{
    public RgbState State { get; set; }
}