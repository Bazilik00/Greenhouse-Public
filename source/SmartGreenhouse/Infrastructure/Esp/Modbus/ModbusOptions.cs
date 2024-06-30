namespace Infrastructure.Esp.Modbus;

public class ModbusOptions
{
    public required string IpAddress { get; set; } = "127.0.0.1";
    public required int Port { get; set; } = 502;
    public required int CommandTimeout { get; set; } = 2000;
    public required int RequestAttemptsCount { get; set; } = 50;
    public required int WaitBeforeCommand { get; set; } = 100;
}