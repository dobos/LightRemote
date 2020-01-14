using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace LightRemote
{
    [XmlRoot("config")]
    public class Config
    {
        private Dictionary<string, Light> lights;

        [XmlArray("lights")]
        public Light[] Lights_forXml
        {
            get
            {
                return lights.Values.ToArray();
            }
            set
            {
                lights.Clear();
                foreach (var light in value)
                {
                    lights.Add(light.Name, light);
                }
            }
        }

        [XmlIgnore]
        public Dictionary<string, Light> Lights
        {
            get { return lights; }
        }

        public Config()
        {
            this.lights = new Dictionary<string, Light>();
        }
    }
}
