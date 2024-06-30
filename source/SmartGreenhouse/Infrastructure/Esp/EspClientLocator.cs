using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Esp;

public class EspClientLocator : IDisposable
{
    private readonly EspClient[] _clients;
    public EspClientLocator(IOptionsMonitor<EspClientsOptions> clientsIdentifiers, ILoggerFactory loggerFactory)
    {
        _clients = clientsIdentifiers.CurrentValue.EspConfigs
            .Select(x => new EspClient(x,loggerFactory.CreateLogger<EspClient>()))
            .ToArray();
    }
    
    public EspClient GetClient(ModbusEspOptions options)
    {
        return _clients
            .FirstOrDefault(x => x.Options.Type == options.Type && x.Options.RoomId == options.RoomId)
               ?? throw new Exception("Client not found");
    }
    
    public EspClient GetClient(ModbusEspOptions.EspType type, int id)
    {
        return _clients.FirstOrDefault(x => x.Options.Type == type && x.Options.RoomId == id) 
               ?? throw new Exception("Client not found");
    }
    
    public EspClient GetInsideClient(int id)
    {
        return _clients.FirstOrDefault(x => x.Options.Type == ModbusEspOptions.EspType.Inside && x.Options.RoomId == id) 
               ?? throw new Exception("Client not found");
    }

    public EspClient[] GetInsideClients()
    {
        return _clients.Where(x => x.Options.Type == ModbusEspOptions.EspType.Inside).ToArray();
    }
    

    public void Dispose()
    {
        foreach (var client in _clients)
        {
            client.Dispose();
        }
    }
}