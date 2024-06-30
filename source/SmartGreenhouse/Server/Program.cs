using Infrastructure;
using Infrastructure.Esp;
using Server;
using Server.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// var espConfigs = builder.Configuration
//                      .GetSection("EspConfigs")
//                      .Get<ModbusEspOptions[]>()
//                  ?? throw new Exception("Infrastructure settings not configured");

// builder.Services.Configure<ModbusEspOptions[]>(builder.Configuration.GetSection("EspConfigs")).Validate;

builder.Services.AddOptions<EspClientsOptions>()
    .Bind(builder.Configuration.GetSection("EspConfig"))
    .ValidateDataAnnotations();

builder.Services.AddSingleton<EspClientLocator>();


builder.Services.AddScoped<InsideSensorsService>();
builder.Services.AddScoped<OutsideSensorsService>();

builder.Services.AddHostedService<BackgroundWorker>();

builder.Services.AddDataAccess(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



builder.Services.AddCors(o => o.AddPolicy("DevCorsPolicy", b =>
{
    b.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
}));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("DevCorsPolicy");

app.UseAuthorization();

app.MapControllers();

app.MigrateDatabase();


// await InitEspSettingsFromDatabase(app);


app.Run();
return;


async Task InitEspSettingsFromDatabase(WebApplication webApplication)
{
    try
    {
        using var scope = webApplication.Services.CreateScope();
        var insideSensorsService = scope.ServiceProvider
            .GetRequiredService<InsideSensorsService>();
        
            

        await insideSensorsService.InitSettingsFromDatabase();
    }
    catch (Exception e)
    {
        webApplication.Logger.LogError(e, "Error while initializing esp settings from database");
    }

}