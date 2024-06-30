using Microsoft.AspNetCore.Mvc;
using Server.Services;

namespace Server.Controllers;

[ApiController]
[Route("[controller]")]
public class OutsideSensorsController(OutsideSensorsService sensorsService, ILogger<OutsideSensorsService> logger) : ControllerBase
{
    [HttpGet]
    public async Task<IResult> Get()
    {
        try
        {
            return Results.Json(await sensorsService.GetSensorsData());
        }
        catch(Exception ex)
        {
            logger.LogError("{Error}", ex);
            return Results.Problem("Ошибка получения данных с ESP");
        }
    }
}