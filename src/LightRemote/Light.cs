using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml.Serialization;

namespace LightRemote
{
    [XmlType("light")]
    public class Light : LightRemoteObject
    {
        private string id;
        private string name;
        private List<LightMode> modes;
        private LightLocation location;
        private bool isConnected;
        private int batteryLevel;
        private double temperature;
        private byte mode;
        private byte requestedMode;

        [XmlIgnore]
        public string ID
        {
            get { return id; }
        }

        [XmlAttribute("name")]
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnNotifyPropertyChanged($"{nameof(Name)}");
            }
        }

        [XmlArray("modes")]
        public List<LightMode> Modes
        {
            get { return modes; }
        }

        [XmlAttribute("location")]
        public LightLocation Location
        {
            get { return location; }
            set
            {
                location = value;
                OnNotifyPropertyChanged($"{nameof(Location)}");
            }
        }

        [XmlIgnore]
        public bool IsConnected
        {
            get { return isConnected; }
        }

        [XmlIgnore]
        public int BatteryLevel
        {
            get { return batteryLevel; }
        }

        [XmlIgnore]
        public bool HasBatteryLevel
        {
            get { return batteryLevel != -1; }
        }

        [XmlIgnore]
        public string BatteryIconGlyph
        {
            get
            {
                if (batteryLevel == 0)
                {
                    return "\uEBA0";    // Empty
                }
                else if (batteryLevel > 0 && batteryLevel <= 25)
                {
                    return "\uEBA2";
                }
                else if (batteryLevel > 25 && batteryLevel <= 50)
                {
                    return "\uEBA5";
                }
                else if (batteryLevel > 50 && batteryLevel <= 75)
                {
                    return "\uEBA8";
                }
                else if (batteryLevel > 75 && batteryLevel <= 100)
                {
                    return "\uEBAA";    // Full
                }
                else
                {
                    return "\uEC02";    // N/A
                }
            }
        }

        [XmlIgnore]
        public SolidColorBrush BatteryIconColor
        {
            get
            {
                if (batteryLevel == 0)
                {
                    return Application.Current.Resources["ToggleButtonBackgroundChecked"] as SolidColorBrush;
                }
                else if (batteryLevel > 0 && batteryLevel <= 25)
                {
                    return new SolidColorBrush(Windows.UI.Colors.Red);
                }
                else if (batteryLevel > 25 && batteryLevel <= 50)
                {
                    return new SolidColorBrush(Windows.UI.Colors.Orange);
                }
                else if (batteryLevel > 50 && batteryLevel <= 75)
                {
                    return new SolidColorBrush(Windows.UI.Colors.Green);
                }
                else if (batteryLevel > 75 && batteryLevel <= 100)
                {
                    return new SolidColorBrush(Windows.UI.Colors.Green);
                }
                else
                {
                    return Application.Current.Resources["SystemBaseHighColor"] as SolidColorBrush;
                }
            }
        }

        [XmlIgnore]
        public double Temperature
        {
            get { return temperature; }
        }

        [XmlIgnore]
        public bool HasTemperature
        {
            get { return !double.IsNaN(temperature); }
        }

        [XmlIgnore]
        public string TemperatureText
        {
            get
            {
                if (HasTemperature)
                {
                    return String.Format("{0:00.0}°C", temperature);
                }
                else
                {
                    return String.Empty;
                }
            }
        }

        [XmlIgnore]
        public byte Mode
        {
            get { return mode; }
        }

        [XmlIgnore]
        public byte RequestedMode
        {
            get { return requestedMode; }
            set { requestedMode = value; }
        }

        public Light()
        {
            InitializeMembers();
        }

        public Light(Light other)
        {
            CopyMembers(other);
        }

        private void InitializeMembers()
        {
            this.id = null;
            this.name = null;
            this.modes = new List<LightMode>();
            this.location = LightLocation.Front;
            this.isConnected = false;
            this.batteryLevel = -1;
            this.temperature = double.NaN;
            this.mode = 255;
            this.requestedMode = 1;
        }

        private void CopyMembers(Light other)
        {
            this.id = null;
            this.name = other.name;
            this.modes = new List<LightMode>(other.modes.Select(x => new LightMode(x, this)));
            this.location = other.location;
            this.isConnected = other.isConnected;
            this.batteryLevel = other.batteryLevel;
            this.temperature = other.temperature;
            this.mode = other.mode;
            this.requestedMode = other.requestedMode;
        }

        public static async Task<Light> FromBluetoothDevice(DeviceInformation info)
        {
            var light = new Light(LightManager.Instance.Config.Lights[info.Name])
            {
                id = info.Id
            };

            using (var device = await light.GetBluetoothDevice())
            {
                await light.ReadAllStatus(device);
            }

            return light;
        }


        private async Task<BluetoothLEDevice> GetBluetoothDevice()
        {
            return await BluetoothLEDevice.FromIdAsync(id);
        }

        private async Task ReadAllStatus(BluetoothLEDevice device)
        {
            try
            {
                await ReadBatteryLevel(device);
                await ReadTemperature(device);
                await ReadMode(device);

                //isConnected = device.ConnectionStatus == BluetoothConnectionStatus.Connected;
                isConnected = true;
            }
            catch (Exception)
            {
                isConnected = false;
            }
        }

        private async Task<byte[]> ReadBluetoothValue(BluetoothLEDevice device, Guid svcGuid, Guid chrGuid)
        {
            var svc = (await device.GetGattServicesForUuidAsync(svcGuid)).Services[0];
            var chr = (await svc.GetCharacteristicsForUuidAsync(chrGuid)).Characteristics[0];
            return (await chr.ReadValueAsync()).Value.ToArray();
        }

        private async Task WriteBluetoothValue(BluetoothLEDevice device, Guid svcGuid, Guid chrGuid, byte[] input)
        {
            var svc = (await device.GetGattServicesForUuidAsync(svcGuid)).Services[0];
            var chr = (await svc.GetCharacteristicsForUuidAsync(chrGuid)).Characteristics[0];
            await chr.WriteValueAsync(input.AsBuffer());
        }

        private async Task<byte> ReadBluetoothValueByte(BluetoothLEDevice device, Guid svcGuid, Guid chrGuid)
        {
            var input = await ReadBluetoothValue(device, svcGuid, chrGuid);
            return input[0];
        }

        private async Task<int> ReadBluetoothValueInt(BluetoothLEDevice device, Guid svcGuid, Guid chrGuid)
        {
            var buffer = await ReadBluetoothValue(device, svcGuid, chrGuid);
            int i = 0;
            int value = (buffer[i++] << 24) | (buffer[i++] << 16) | (buffer[i++] << 8) | buffer[i++];
            return value;
        }

        private async Task ReadBatteryLevel(BluetoothLEDevice device)
        {
            try
            {
                batteryLevel = await ReadBluetoothValueByte(device, Constants.BTLESvcGuidBattery, Constants.BTLEChrGuidBattery);
            }
            catch (Exception)
            {
                batteryLevel = -1;
            }
            OnNotifyPropertyChanged($"{nameof(BatteryLevel)}");
            OnNotifyPropertyChanged($"{nameof(BatteryIconGlyph)}");
            OnNotifyPropertyChanged($"{nameof(BatteryIconColor)}");
        }

        private async Task ReadTemperature(BluetoothLEDevice device)
        {
            try
            {
                var buffer = await ReadBluetoothValue(device, Constants.BTLESvcGuidLight, Constants.BTLEChrGuidTemperature);
                if (buffer[2] != 0)
                {
                    temperature = buffer[3] / 10.0;
                }
                else
                {
                    temperature = double.NaN;
                }
            }
            catch (Exception)
            {
                temperature = double.NaN;
            }
            OnNotifyPropertyChanged($"{nameof(Temperature)}");
            OnNotifyPropertyChanged($"{nameof(HasTemperature)}");
            OnNotifyPropertyChanged($"{nameof(TemperatureText)}");
        }

        private async Task ReadMode(BluetoothLEDevice device)
        {
            try
            {
                this.mode = await ReadBluetoothValueByte(device, Constants.BTLESvcGuidLight, Constants.BTLEChrGuidMode);
            }
            catch (Exception)
            {
                this.mode = 255;
            }

            foreach (var m in modes)
            {
                m.Active = m.ID == this.mode;
            }

            OnNotifyPropertyChanged($"{nameof(Mode)}");
        }

        private async Task WriteMode(BluetoothLEDevice device)
        {
            try
            {
                await WriteBluetoothValue(device, Constants.BTLESvcGuidLight, Constants.BTLEChrGuidMode, new byte[] { mode });
            }
            catch (Exception)
            {
                mode = 255;
            }

            foreach (var m in modes)
            {
                m.Active = m.ID == this.mode;
            }

            OnNotifyPropertyChanged($"{nameof(Mode)}");
        }

        public async Task TurnOn()
        {
            using (var device = await GetBluetoothDevice())
            {
                mode = requestedMode;
                await WriteMode(device);
            }
            OnNotifyPropertyChanged($"{nameof(Mode)}");
        }

        public async Task TurnOff()
        {
            using (var device = await GetBluetoothDevice())
            {
                mode = 0;
                await WriteMode(device);
            }
            OnNotifyPropertyChanged($"{nameof(Mode)}");
        }
    }
}
