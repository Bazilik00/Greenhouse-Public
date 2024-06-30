using Infrastructure.Esp.Modbus;

namespace Infrastructure.Esp;

public class EspClientsOptions
{
    public required ModbusEspOptions[] EspConfigs { get; set; }
}

public struct ModbusEspOptions
{
    public enum EspType
    {
        Outside,
        Inside
    }

    public required EspType Type { get; set; }
    
    public required int RoomId { get; set; }
    
    public required int ServerId { get; set; }
    
    public required ModbusOptions ModbusOptions { get; set; }
}