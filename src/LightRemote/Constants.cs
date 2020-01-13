using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightRemote
{
    public static class Constants
    {
        public static readonly Guid BTLESvcGuidBattery = new Guid("0000180F-0000-1000-8000-00805F9B34FB");
        public static readonly Guid BTLEChrGuidBattery = new Guid("00002A19-0000-1000-8000-00805F9B34FB");

        public static readonly Guid BTLESvcGuidLight = new Guid("71261000-3692-AE93-E711-472BA41689C9");
        public static readonly Guid BTLEChrGuidTemperature = new Guid("71261200-3692-AE93-E711-472BA41689C9");
        public static readonly Guid BTLEChrGuidMode = new Guid("71261001-3692-AE93-E711-472BA41689C9");
    }
}
