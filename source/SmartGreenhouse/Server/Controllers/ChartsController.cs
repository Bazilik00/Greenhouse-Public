using Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Services;
using Shared.Dto.ServerResponses;

namespace Server.Controllers;

[ApiController]
[Route("[controller]")]
public class ChartsController(AppDbContext context, ILogger<OutsideSensorsService> logger) : ControllerBase
{
    [HttpGet]
    public async Task<IResult> Get(string type, int hours)
    {
        try
        {
            switch (type)
            {
                case "temperature":
                {
                    var data = await context.TemperatureChartsData
                        .Where(x => x.DateTime > DateTime.Now.AddHours(-hours))
                        .ToListAsync();

                    var response = new ChartsResponse
                    {
                        Charts = data
                            .Select(x => new ChartValue()
                            {
                                Value = x.Value,
                                DateTime = x.DateTime,
                                Source = x.Source
                            }).ToArray()
                    };
                    return Results.Json(response);
            }

            case "humidity":
                {
                    var data = await context.HumidityChartsData
                        .Where(x => x.DateTime > DateTime.Now.AddHours(-hours))
                        .ToListAsync();

                    var response = new ChartsResponse
                    {
                        Charts = data
                            .Select(x => new ChartValue()
                            {
                                Value = x.Value,
                                DateTime = x.DateTime,
                                Source = x.Source
                            }).ToArray()
                    };
                    return Results.Json(response);
            }

            case "soilHumidity":
                {
                    var data = await context.SoilHumidityChartsData
                        .Where(x => x.DateTime > DateTime.Now.AddHours(-hours))
                        .ToListAsync();

                    var response = new ChartsResponse
                    {
                        Charts = data
                            .Select(x => new ChartValue()
                            {
                                Value = x.Value,
                                DateTime = x.DateTime,
                                Source = x.Source
                            }).ToArray()
                    };
                    return Results.Json(response);
            }

            case "illumination":
                {
                    var data = await context.IlluminationChartsData
                        .ToListAsync();

                    var response = new ChartsResponse
                    {
                        Charts = data
                            .Where(x => x.Source == type)
                            .Select(x => new ChartValue()
                            {
                                Value = x.Value,
                                DateTime = x.DateTime,
                                Source = x.Source
                            }).ToArray()
                    };
                    return Results.Json(response);
            }

            default:
                    return Results.NotFound();
            }
       
            
    
        }
        catch (Exception ex)
        {
            logger.LogError("{Error}", ex);
            return Results.Problem("Ошибка получения данных с ESP");
        }
    }
    
}