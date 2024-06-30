using Client;
using Client.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddMudServices();

builder.Services.AddScoped<SensorsClientService>();
builder.Services.AddScoped<OutsideSensorsService>();

var serverUrl = builder.Configuration["Server:Url"];
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(serverUrl ?? throw new InvalidOperationException("Server:Url not found")) });

await builder.Build().RunAsync();
