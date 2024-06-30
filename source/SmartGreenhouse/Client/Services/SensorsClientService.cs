using System.Net.Http.Json;
using MudBlazor;
using Shared.Dto.ServerRequests;
using Shared.Dto.ServerResponses;
using Shared.Models;
using Shared.ValueModels;

namespace Client.Services;

public class SensorsClientService(HttpClient client, ISnackbar snackbar)
{
    private ClientsHeadersDto? _clientsHeaders;

    public async Task<int[]> GetClientIds()
    {
        if (_clientsHeaders is not null) return _clientsHeaders.Ids;

        try
        {
            var response = await client.GetAsync("/InsideSensors/SensorsClientHeaders");
            _clientsHeaders = await HandleResponse<ClientsHeadersDto>(response);
            if (_clientsHeaders is not null && _clientsHeaders.Ids.Length != 0) 
                return _clientsHeaders.Ids;
            
            snackbar.Add("Термокамеры не найдены", Severity.Error);
            throw new Exception("Термокамеры не найдены");
        }
        catch (Exception e)
        {
            snackbar.Add("Ошибка получения данных по id термокамер " + e.Message, Severity.Error);
            throw;
        }
    }

    public async Task<LampState> GetLampState(int clientId)
    {
        try
        {
            var response = await client.GetAsync($"/InsideSensors/LampState?roomId={clientId}");
            return await HandleResponse<LampState>(response);
        }
        catch (Exception e)
        {
            snackbar.Add("Ошибка получения данных состояния фитолампы: " + e.Message, Severity.Error);
            throw;
        }
    }

    public async Task SetRgbState(int clientId, RgbState state)
    {
        var request = new SetRgbStateRequest
        {
            State = state
        };
        try
        {
            var message = await client.PostAsJsonAsync($"/InsideSensors/RgbState?roomId={clientId}", request);
            message.EnsureSuccessStatusCode();
        }
        catch (Exception e)
        {
            snackbar.Add("Ошибка установки состояния лампы" + e.Message, Severity.Error);
        }
    }
    
    public async Task<InsideMicroclimateState> GetMicroclimateState(int clientId)
    {
        try
        {
            var response = await client.GetAsync($"/InsideSensors/Microclimate?roomId={clientId}");
            return await HandleResponse<InsideMicroclimateState>(response);
        }
        catch (Exception e)
        {
            snackbar.Add("Ошибка получения данных состояния микроклимата: " + e.Message, Severity.Error);
            throw;
        }
    }
    
    public async Task<WateringState> GetWateringState(int clientId)
    {
        try
        {
            var response = await client.GetAsync($"/InsideSensors/WateringState?roomId={clientId}");
            return await HandleResponse<WateringState>(response);
        }
        catch (Exception e)
        {
            snackbar.Add("Ошибка получения данных состояния полива: " + e.Message, Severity.Error);
            throw;
        }
    }
    

    private static async Task<TResponse> HandleResponse<TResponse>(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            return (await response.Content.ReadFromJsonAsync<TResponse>()) ??
                   throw new NullReferenceException("Empty response");
        }

        var errorMessage = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Ошибка: {response.StatusCode} - {errorMessage[..100]}");
        throw new Exception(response.ReasonPhrase);
    }


    public async Task SetHumidifierState(int clientId, bool value)
    {
        var request = new SetPowerRequest
        {
            Power = value
        };
        try
        {
            var message = await client.PostAsJsonAsync($"/InsideSensors/HumidifierOn?roomId={clientId}", request);
            message.EnsureSuccessStatusCode();
        }
        catch (Exception e)
        {
            snackbar.Add("Ошибка установки состояния увлажнителя" + e.Message, Severity.Error);
        }
    }
    
    public async Task SetPumpState(int clientId, bool value)
    {
        var request = new SetPowerRequest
        {
            Power = value
        };
        try
        {
            var message = await client.PostAsJsonAsync($"/InsideSensors/WateringOn?roomId={clientId}", request);
            message.EnsureSuccessStatusCode();
        }
        catch (Exception e)
        {
            snackbar.Add("Ошибка установки состояния полива" + e.Message, Severity.Error);
        }
    }


    public async Task SetFanState(int clientId, bool value)
    {
        var request = new SetPowerRequest
        {
            Power = value
        };
        try
        {
            var message = await client.PostAsJsonAsync($"/InsideSensors/FanOn?roomId={clientId}", request);
            message.EnsureSuccessStatusCode();
        }
        catch (Exception e)
        {
            snackbar.Add("Ошибка установки состояния вентилятора" + e.Message, Severity.Error);
        }
    }

    public async Task SetWindowState(int clientId, bool value)
    {
        var request = new SetPowerRequest
        {
            Power = value
        };
        try
        {
            var message = await client.PostAsJsonAsync($"/InsideSensors/WindowOpen?roomId={clientId}", request);
            message.EnsureSuccessStatusCode();
        }
        catch (Exception e)
        {
            snackbar.Add("Ошибка установки состояния окна" + e.Message, Severity.Error);
        }
    }

    public async Task UpdateTemperatureLimits(int? clientId, TemperatureModel state)
    {
        var request = new SetMinMaxSettingsRequest()
        {
            Max = state.Max,
            Min = state.Min
        };
        try
        {
            var message = await client.PostAsJsonAsync($"/InsideSensors/MicroclimateTemperature?roomId={clientId}", request);
            message.EnsureSuccessStatusCode();
        }
        catch (Exception e)
        {
            snackbar.Add("Ошибка установки порогов температуры" + e.Message, Severity.Error);
        }
    }

    public async Task UpdateHumidityLimits(int? clientId, HumidityModel state)
    {
        var request = new SetMinMaxSettingsRequest()
        {
            Max = state.Max,
            Min = state.Min
        };
        try
        {
            var message = await client.PostAsJsonAsync($"/InsideSensors/MicroclimateHumidity?roomId={clientId}", request);
            message.EnsureSuccessStatusCode();
        }
        catch (Exception e)
        {
            snackbar.Add("Ошибка установки порогов влажности" + e.Message, Severity.Error);
        }
    }
    
    public async Task UpdateSoilHumidityLimits(int? clientId, HumidityModel state)
    {
        var request = new SetMinMaxSettingsRequest()
        {
            Max = state.Max,
            Min = state.Min
        };
        try
        {
            var message = await client.PostAsJsonAsync($"/InsideSensors/SoilHumidity?roomId={clientId}", request);
            message.EnsureSuccessStatusCode();
        }
        catch (Exception e)
        {
            snackbar.Add("Ошибка установки порогов влажности почвы" + e.Message, Severity.Error);
        }
    }

    public async Task UpdateIlluminationLimits(int? clientId, IlluminationModel state)
    {
        var request = new SetMinMaxSettingsRequest()
        {
            Max = state.Max,
            Min = state.Min
        };
        try
        {
            var message = await client.PostAsJsonAsync($"/InsideSensors/Illumination?roomId={clientId}", request);
            message.EnsureSuccessStatusCode();
        }
        catch (Exception e)
        {
            snackbar.Add("Ошибка установки порогов освещенности" + e.Message, Severity.Error);
        }
    }
}