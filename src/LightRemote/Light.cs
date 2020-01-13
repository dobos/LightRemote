using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Storage.Streams;
using System.Runtime.InteropServices.WindowsRuntime;

namespace LightRemote
{
    public class Light : LightRemoteObject
    {
        private string id;
        private LightModel model;
        private string name;
        private bool isConnected;
        private int batteryLevel;
        private double temperature;
        private byte mode;
        private byte requestedMode;

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnNotifyPropertyChanged("Name");
            }
        }

        public LightModel Model
        {
            get { return model; }
            set { model = value; }
        }

        public bool IsConnected
        {
            get { return isConnected; }
        }

        public int BatteryLevel
        {
            get { return batteryLevel; }
        }

        public bool HasBatteryLevel
        {
            get { return batteryLevel != -1; }
        }

        public double Temperature
        {
            get { return temperature; }
        }

        public bool HasTemperature
        {
            get { return !double.IsNaN(temperature); }
        }

        public byte Mode
        {
            get { return mode; }
        }

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
            this.model = null;
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
            this.model = other.model;
            this.isConnected = other.isConnected;
            this.batteryLevel = other.batteryLevel;
            this.temperature = other.temperature;
            this.mode = other.mode;
            this.requestedMode = other.requestedMode;
        }

        public static async Task<Light> FromBluetoothDevice(DeviceInformation info)
        {
            var light = new Light()
            {
                id = info.Id,
                name = info.Name,
                model = LightManager.Instance.Config.Lights[info.Name],
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
            OnNotifyPropertyChanged("BatteryLevel");
        }

        private async Task ReadTemperature(BluetoothLEDevice device)
        {
            try
            {
                var buffer = await ReadBluetoothValue(device, Constants.BTLESvcGuidLight, Constants.BTLEChrGuidTemperature);
                temperature = buffer[3] / 10.0;
            }
            catch (Exception)
            {
                temperature = double.NaN;
            }
            OnNotifyPropertyChanged("Temperature");
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
            OnNotifyPropertyChanged("Mode");
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
            OnNotifyPropertyChanged("Mode");
        }

        public async Task TurnOn()
        {
            using (var device = await GetBluetoothDevice())
            {
                mode = requestedMode;
                await WriteMode(device);
            }
            OnNotifyPropertyChanged("Mode");
        }

        public async Task TurnOff()
        {
            using (var device = await GetBluetoothDevice())
            {
                mode = 0;
                await WriteMode(device);
            }
            OnNotifyPropertyChanged("Mode");
        }
    }
}
