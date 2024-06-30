using System.Net;
using System.Net.Sockets;
using Infrastructure.Esp.Exceptions;
using Infrastructure.Esp.Extensions;
using Microsoft.Extensions.Logging;
using NModbus;

namespace Infrastructure.Esp.Modbus;

public class ModbusClientProxy : IModbusMaster
{
    private readonly ILogger<ModbusClientProxy> _logger;
    private readonly ModbusOptions _options;

    private IModbusMaster _modbusMaster = null!;

    private readonly object _locker = new();

    private readonly IPEndPoint _ipEndPoint;
    private TcpClient _tcpClient;


    protected ModbusClientProxy(
        ModbusOptions options,
        ILogger<ModbusClientProxy> logger)
    {
        _logger = logger;
        _options = options;

        _ipEndPoint = new IPEndPoint
            (IPAddress.Parse(_options.IpAddress), _options.Port);

        _tcpClient = new TcpClient();
        InitModbusMaster();
    }

    private void InitModbusMaster()
    {
        try
        {
            _logger.LogInformation("Attempt to connect to modbus gateway");
            _modbusMaster?.Dispose();
            _tcpClient?.Dispose();

            _tcpClient = new TcpClient();

            _tcpClient.Connect(_ipEndPoint, _options.CommandTimeout);

            var factory = new ModbusFactory();
            _modbusMaster = factory.CreateMaster(_tcpClient);

            _modbusMaster.Transport.ReadTimeout = _options.CommandTimeout;
            _modbusMaster.Transport.WriteTimeout = _options.CommandTimeout;
            
            _logger.LogInformation("Successfully connected to the modbus gateway");
        }
        catch (SocketException)
        {
            _logger.LogError(
                "Failed to connect to modbus host. {IpAddress}:{Port}",
                _options.IpAddress,
                _options.Port);
        }
    }

    public ushort[] ReadHoldingRegisters(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
    {
        // lock (_locker)
        // {
            if (!_tcpClient.Connected) InitModbusMaster();

            for (var i = 0; i <= _options.RequestAttemptsCount; i++)
            {
                try
                {
                    return _modbusMaster.ReadHoldingRegisters(slaveAddress, startAddress, numberOfPoints);
                }
                catch (Exception)
                {
                    _logger.LogWarning("Failed try of modbus request, try number:{Try}", i);
                    if (i == _options.RequestAttemptsCount)
                    {
                        _tcpClient.Close();
                        throw;
                    }

                    Thread.Sleep(_options.WaitBeforeCommand);
                }
            }

            throw new InvalidOperationException();
        // }
    }

    public async Task<ushort[]> ReadHoldingRegistersAsync(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
    {
            if (!_tcpClient.Connected) InitModbusMaster();


            for (var i = 0; i <= _options.RequestAttemptsCount; i++)
            {
                try
                {
                    return await _modbusMaster.ReadHoldingRegistersAsync(slaveAddress, startAddress, numberOfPoints);
                }
                catch (Exception)
                {
                    _logger.LogWarning("Failed try of modbus request, try number:{Try}", i);
                    InitModbusMaster();
                    if (i == _options.RequestAttemptsCount)
                    {
                        _tcpClient.Close();
                        throw;
                    }

                    await Task.Delay(_options.WaitBeforeCommand);
                }
            }

        throw new InvalidOperationException();
    }

    public async Task WriteSingleRegisterAsync(byte slaveAddress, ushort registerAddress, ushort value)
    {
            if (!_tcpClient.Connected) InitModbusMaster();

            for (var i = 0; i <= _options.RequestAttemptsCount; i++)
            {
                try
                {
                    await _modbusMaster.WriteSingleRegisterAsync(slaveAddress, registerAddress, value);
                    return;
                }
                catch (Exception)
                {
                    _logger.LogWarning("Failed try of modbus request, try number:{Try}", i);
                    InitModbusMaster();
                    if (i == _options.RequestAttemptsCount)
                    {
                        _tcpClient.Close();
                        throw;
                    }

                    await Task.Delay(_options.WaitBeforeCommand);
                }
            }
    }

    public void WriteSingleRegister(byte slaveAddress, ushort registerAddress, ushort value)
    {
        lock (_locker)
        {
            if (!_tcpClient.Connected) InitModbusMaster();

            for (var i = 0; i <= _options.RequestAttemptsCount; i++)
            {
                try
                {
                    _modbusMaster.WriteSingleRegister(slaveAddress, registerAddress, value);
                    return;
                }
                catch (Exception)
                {
                    _logger.LogWarning("Failed try of modbus request, try number:{Try}", i);
                    if (i == _options.RequestAttemptsCount)
                    {
                        _tcpClient.Close();
                        throw;
                    }

                    Thread.Sleep(_options.WaitBeforeCommand);
                }
            }
        }
    }

    public TResponse ExecuteCustomMessage<TResponse>(IModbusMessage request) where TResponse : IModbusMessage, new()
    {
        return _modbusMaster.ExecuteCustomMessage<TResponse>(request);
    }

    public bool[] ReadCoils(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
    {
        return _modbusMaster.ReadCoils(slaveAddress, startAddress, numberOfPoints);
    }

    public Task<bool[]> ReadCoilsAsync(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
    {
        return _modbusMaster.ReadCoilsAsync(slaveAddress, startAddress, numberOfPoints);
    }

    public ushort[] ReadInputRegisters(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
    {
        return _modbusMaster.ReadInputRegisters(slaveAddress, startAddress, numberOfPoints);
    }

    public Task<ushort[]> ReadInputRegistersAsync(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
    {
        return _modbusMaster.ReadInputRegistersAsync(slaveAddress, startAddress, numberOfPoints);
    }

    public bool[] ReadInputs(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
    {
        return _modbusMaster.ReadInputs(slaveAddress, startAddress, numberOfPoints);
    }

    public Task<bool[]> ReadInputsAsync(byte slaveAddress, ushort startAddress, ushort numberOfPoints)
    {
        return _modbusMaster.ReadInputsAsync(slaveAddress, startAddress, numberOfPoints);
    }

    public ushort[] ReadWriteMultipleRegisters(byte slaveAddress, ushort startReadAddress,
        ushort numberOfPointsToRead, ushort startWriteAddress, ushort[] writeData)
    {
        return _modbusMaster.ReadWriteMultipleRegisters(slaveAddress, startReadAddress, numberOfPointsToRead,
            startWriteAddress, writeData);
    }

    public Task<ushort[]> ReadWriteMultipleRegistersAsync(byte slaveAddress, ushort startReadAddress,
        ushort numberOfPointsToRead, ushort startWriteAddress, ushort[] writeData)
    {
        return _modbusMaster.ReadWriteMultipleRegistersAsync(slaveAddress, startReadAddress, numberOfPointsToRead,
            startWriteAddress, writeData);
    }

    public void WriteFileRecord(byte slaveAdress, ushort fileNumber, ushort startingAddress, byte[] data)
    {
        _modbusMaster.WriteFileRecord(slaveAdress, fileNumber, startingAddress, data);
    }

    public void WriteMultipleCoils(byte slaveAddress, ushort startAddress, bool[] data)
    {
        _modbusMaster.WriteMultipleCoils(slaveAddress, startAddress, data);
    }

    public Task WriteMultipleCoilsAsync(byte slaveAddress, ushort startAddress, bool[] data)
    {
        return _modbusMaster.WriteMultipleCoilsAsync(slaveAddress, startAddress, data);
    }

    public void WriteMultipleRegisters(byte slaveAddress, ushort startAddress, ushort[] data)
    {
        _modbusMaster.WriteMultipleRegisters(slaveAddress, startAddress, data);
    }

    public Task WriteMultipleRegistersAsync(byte slaveAddress, ushort startAddress, ushort[] data)
    {
        return _modbusMaster.WriteMultipleRegistersAsync(slaveAddress, startAddress, data);
    }

    public void WriteSingleCoil(byte slaveAddress, ushort coilAddress, bool value)
    {
        _modbusMaster.WriteSingleCoil(slaveAddress, coilAddress, value);
    }

    public Task WriteSingleCoilAsync(byte slaveAddress, ushort coilAddress, bool value)
    {
        return _modbusMaster.WriteSingleCoilAsync(slaveAddress, coilAddress, value);
    }

    public IModbusTransport Transport => _modbusMaster.Transport;

    public void Dispose()
    {
        _modbusMaster.Dispose();
    }
}