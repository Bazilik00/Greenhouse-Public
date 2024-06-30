using Infrastructure.Esp.Modbus;
using Microsoft.Extensions.Logging;
using Shared.Models;
using Shared.ValueModels;

namespace Infrastructure.Esp;

public class EspClient(ModbusEspOptions options, ILogger<EspClient> logger)
    : ModbusClientProxy(options.ModbusOptions, logger)
{
    private const ushort RegistersCount = 10;
    private const ushort StartAddress = 0;
    private const ushort StartOutsideAddress = 9;
    private const ushort RegistersOutsideCount = 4;

    public ModbusEspOptions Options { get; private set; } = options;

    private readonly byte _slaveId = (byte)options.ServerId;

    public async Task<EspOutsideState> GetOutsideEspState()
    {
            var stateRegisters = await ReadOutsideSensorsRegisters();

            var state = new EspOutsideState
            {
                Temperature = (double)stateRegisters[0] / 10,
                Humidity = (double)stateRegisters[1] / 10,
                Illumination = stateRegisters[2],
                HasWater = stateRegisters[3] == 1
            };

            return state;
    }

    public async Task<EspInsideState> GetEspState()
    {
        var stateRegisters = await ReadStateRegisters();

        var state = new EspInsideState
        {
            Temperature = (double)stateRegisters[0] / 10,
            Humidity = (double)stateRegisters[1] / 10,
            WindowOpen = ((stateRegisters[7] >> 5) & 1) == 1,
            SoilHumidity = stateRegisters[3],
            Illumination = stateRegisters[6],
            FanOn = ((stateRegisters[7] >> 2) & 1) == 1,
            HumidifierOn = ((stateRegisters[7] >> 3) & 1) == 1, // ((myByte >> 3) & 1) == 1
            ValveOn = ((stateRegisters[7] >> 4) & 1) == 1,
            HasPlant = stateRegisters[8] == 1,
            HasWater = stateRegisters[2] == 1,
            Rgb = new RgbState
            {
                Color = MapRgb565ToRgb888(stateRegisters[4]),
                Brightness = stateRegisters[5],
                Power =  ((stateRegisters[7] >> 0) & 1) == 1,
                Mode = (RgbMode)((stateRegisters[7] >> 1) & 1),
            }
        };

        return state;
    }

    public async Task SetRgbPower(bool value)
    {
        await SetStateRegisterBit(value, 0);
    }

    public async Task SetRgbMode(RgbMode mode)
    {
        var bitValue = mode == RgbMode.BiColor;

        await SetStateRegisterBit(bitValue, 1);
    }

    public async Task SetFanOn(bool value)
    {
        await SetStateRegisterBit(value, 2);
    }

    public async Task SetHumidifierOn(bool value)
    {
        await SetStateRegisterBit(value, 3);
    }

    public async Task SetValveOn(bool value)
    {
        await SetStateRegisterBit(value, 4);
    }

    public async Task SetWindowOn(bool value)
    {
        await SetStateRegisterBit(value, 5);
    }

    public async Task SetRgbColor(string color)
    {
        await WriteSingleRegisterAsync(_slaveId, 4, rgb888ToRgb565(color));
    }

    public async Task SetRgbBrightness(int value)
    {
        await WriteSingleRegisterAsync(_slaveId, 5, (ushort)value);
    }


    private static string MapRgb565ToRgb888(ushort rgb565)
    {
        // Извлекаем компоненты
        var r5 = (rgb565 >> 11) & 0x1F;
        var g6 = (rgb565 >> 5) & 0x3F;
        var b5 = rgb565 & 0x1F;

        // Расширяем до 8 бит
        var red = (r5 * 527 + 23) >> 6;
        var green = (g6 * 259 + 33) >> 6;
        var blue = (b5 * 527 + 23) >> 6;

        return $"#{red:X2}{green:X2}{blue:X2}";
    }

    private ushort rgb888ToRgb565(string hexString)
    {
        var r5 = Convert.ToInt16(hexString[1..3], 16);
        var g6 = Convert.ToInt16(hexString[3..5], 16);
        var b5 = Convert.ToInt16(hexString[5..7], 16);

        var r = (ushort)(r5 >> 3);
        var g = (ushort)(g6 >> 2);
        var b = (ushort)(b5 >> 3);


        // Комбинируем биты в одно значение RGB565
        return (ushort)((r << 11) | (g << 5) | b);
    }

    private async Task SetStateRegisterBit(bool value, int bitNumber)
    {
        var state = (await ReadHoldingRegistersAsync(_slaveId, 7, 1))[0];

        state = SetBit(state, (byte)bitNumber, value);

        await WriteSingleRegisterAsync(_slaveId, 7, state);
    }

    private static ushort SetBit(int value, byte bit, bool bitValue) => bitValue switch
    {
        true => (ushort)(value | (1 << bit)),
        false => (ushort)(value & ~(1 << bit)),
    };


    private async Task<ushort[]> ReadStateRegisters()
        => await ReadHoldingRegistersAsync(_slaveId, StartAddress, RegistersCount);
    
    private async Task<ushort[]> ReadOutsideSensorsRegisters()
        => await ReadHoldingRegistersAsync(_slaveId, StartOutsideAddress, RegistersOutsideCount);

    public async Task SetSyncWater(bool value)
    {
        await WriteSingleRegisterAsync(_slaveId, 2, value ? (ushort)1 : (ushort)0);
    }
    
    public async Task SetPumpTicksWater(int value)
    {
        await WriteSingleRegisterAsync(_slaveId, 14, (ushort)value);
    }
}