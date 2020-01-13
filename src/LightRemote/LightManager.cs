using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;
using System.Xml.Serialization;

namespace LightRemote
{
    public class LightManager
    {
        private static LightManager instance;

        public static LightManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new LightManager();
                }

                return instance;
            }
        }

        private Config config;

        public Config Config
        {
            get { return config; }
        }

        private Dictionary<string, Light> lights;

        public Dictionary<string, Light> Lights
        {
            get { return lights; }
        }

        private LightManager()
        {
            this.config = null;
            this.lights = new Dictionary<string, Light>();
        }

        /// <summary>
        /// Load lights configuration from xml
        /// </summary>
        public async Task LoadConfig()
        {
            var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/Config.xml"));
            using (var stream = await file.OpenReadAsync())
            {
                using (var instream = stream.AsStreamForRead())
                {
                    var x = new XmlSerializer(typeof(Config));
                    config = (Config)x.Deserialize(instream);
                }
            }
        }

        public async Task FindConnectedLights()
        {
            lights.Clear();

            var selector = BluetoothLEDevice.GetDeviceSelector();
            var devices = await DeviceInformation.FindAllAsync(selector);

            foreach (var d in devices)
            {
                if (config.Lights.ContainsKey(d.Name))
                {
                    var light = await Light.FromBluetoothDevice(d);
                    lights.Add(d.Name, light);
                }
            }
        }


    }
}
