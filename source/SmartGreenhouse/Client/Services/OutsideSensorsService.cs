using System.Net.Http.Json;
using MudBlazor;
using Shared.Dto.ServerResponses;

namespace Client.Services;

public class OutsideSensorsService(HttpClient client, ISnackbar snackbar)
{
    public async Task<OutsideMicroclimateState> GetState()
    {
        try
        {
            var response = await client.GetAsync("/OutsideSensors");
            return await HandleResponse(response);
        }
        catch (Exception e)
        {
            snackbar.Add("Ошибка получения данных по внешним датчикам: " + e.Message, Severity.Error);
            throw;
        }
    }

    private static async Task<OutsideMicroclimateState> HandleResponse(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            return (await response.Content.ReadFromJsonAsync<OutsideMicroclimateState>()) ??
                   throw new NullReferenceException("Empty response");
        }

        var errorMessage = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Ошибка: {response.StatusCode} - {errorMessage[..100]}");
        throw new Exception(response.ReasonPhrase);
    }
}