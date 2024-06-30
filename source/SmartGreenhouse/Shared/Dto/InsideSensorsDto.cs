using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Models;
using Shared.ValueModels;

namespace Shared.Dto
{
    public class InsideSensorsDto
    {
        public required double Temperature { get; set; }
        public required double Humidity { get; set; }

        public required bool WindowOpen { get; set; }

        public required int SoilHumidity { get; set; }

        public required RgbState Rgb { get; set; }

        public required int Illumination { get; set; }

        public required bool FanOn { get; set; }

        public required bool ValveOn { get; set; }

        public required bool HumidifierOn { get; set; }

        public required bool HasPlant { get; set; }
    }
}
