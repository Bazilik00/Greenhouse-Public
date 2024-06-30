using Microsoft.AspNetCore.Mvc;
using Server.Services;
using Shared.Dto.ServerRequests;

namespace Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InsideSensorsController(InsideSensorsService sensorsService, ILogger<InsideSensorsController> logger) : ControllerBase
    {
        [HttpGet("SensorsClientHeaders")]
        public Task<IResult> GetSensorsClientHeaders()
        {
            return Task.FromResult(Results.Json(sensorsService.GetClientsHeaders()));
        }
        [HttpGet]
        public async Task<IResult> Get()
        {
            return Results.Json(await sensorsService.GetSensorsData());
        }

        [HttpGet("Microclimate")]
        public async Task<IResult> GetMicroclimate(int roomId)
        {
            return Results.Json(await sensorsService.GetMicroclimate(roomId));
        }

        [HttpGet("WateringState")]
        public async Task<IResult> GetWateringState(int roomId)
        {
            return Results.Json(await sensorsService.GetWateringState(roomId));
        }

        [HttpGet("LampState")]
        public async Task<IResult> GetLampState(int roomId)
        {
            var result = await sensorsService.GetLampState(roomId);
                
            return Results.Json(result);
        }

        [HttpPost("MicroclimateTemperature")]
        public async Task<IResult> SetMicroclimateTemperature(
            [FromBody] SetMinMaxSettingsRequest request,
            [FromQuery] int roomId
        )
        {
            await sensorsService.SetMicroclimateTemperature(request, roomId);
            return Results.Ok();
        }

        [HttpPost("MicroclimateHumidity")]
        public async Task<IResult> SetMicroclimateHumidity(
            [FromBody] SetMinMaxSettingsRequest request,
            [FromQuery] int roomId
        )
        {
            await sensorsService.SetMicroclimateHumidity(request, roomId);
            return Results.Ok();
        }
        
        
        [HttpPost("SoilHumidity")]
        public async Task<IResult> SetSoilHumidity(
            [FromBody] SetMinMaxSettingsRequest request,
            [FromQuery] int roomId
        )
        {
            await sensorsService.SetSoilHumidity(request, roomId);
            return Results.Ok();
        }
        
                
        [HttpPost("Illumination")]
        public async Task<IResult> SetIllumination(
            [FromBody] SetMinMaxSettingsRequest request,
            [FromQuery] int roomId
        )
        {
            await sensorsService.SetIllumination(request, roomId);
            return Results.Ok();
        }
        
        [HttpPost("FanOn")]
        public async Task<IResult> SetFanOn(
            [FromBody] SetPowerRequest request,
            [FromQuery] int roomId
        )
        {
            await sensorsService.SetFanOn(request, roomId);
            return Results.Ok();
        }
        
        [HttpPost("WindowOpen")]
        public async Task<IResult> SetWindowOpen(
            [FromBody] SetPowerRequest request,
            [FromQuery] int roomId
        )
        {
            await sensorsService.SetWindowOpen(request, roomId);
            return Results.Ok();
        }
        
        [HttpPost("HumidifierOn")]
        public async Task<IResult> SetHumidifierOn(
            [FromBody] SetPowerRequest request,
            [FromQuery] int roomId
        )
        {
            await sensorsService.SetHumidifierOn(request, roomId);
            return Results.Ok();
        }
        
        
        [HttpPost("WateringOn")]
        public async Task<IResult> SetWateringOn(
            [FromBody] SetPowerRequest request,
            [FromQuery] int roomId
        )
        {
            await sensorsService.SetValveOn(request, roomId); 
            return Results.Ok();
        }
        
        [HttpPost("RgbState")]
        public async Task<IResult> SetRgbState(
            [FromBody] SetRgbStateRequest request,
            [FromQuery] int roomId
        )
        {
            await sensorsService.SetRgbState(request, roomId);
            return Results.Ok();
        }
        
        [HttpPost("PumpTick")]
        public async Task<IResult> SetPumpTick(
            [FromBody] int request,
            [FromQuery] int roomId
        )
        {
            await sensorsService.SetPumpTick(request, roomId);
            return Results.Ok();
        }
        
    }
}