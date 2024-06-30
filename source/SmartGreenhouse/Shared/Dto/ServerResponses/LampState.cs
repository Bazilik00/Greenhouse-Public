using Shared.Models;
using Shared.ValueModels;

namespace Shared.Dto.ServerResponses;

public class LampState
{
    public RgbState RgbState { get; set; }
    
    public IlluminationModel IlluminationData { get; set; }
    
}