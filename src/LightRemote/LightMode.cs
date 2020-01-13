using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LightRemote
{
    [XmlType("mode")]
    public class LightMode : LightRemoteObject
    {
        private Light light;
        private byte id;
        private string name;
        private LightTime time;
        private int flash;
        private int brightness;

        [XmlAttribute("id")]
        public byte ID
        {
            get { return id; }
            set
            {
                id = value;
                OnNotifyPropertyChanged("ID");
            }
        }

        [XmlAttribute("name")]
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnNotifyPropertyChanged("Name");
            }
        }

        [XmlAttribute("time")]
        public LightTime Time
        {
            get { return time; }
            set
            {
                time = value;
                OnNotifyPropertyChanged("Time");
            }
        }

        [XmlAttribute("flash")]
        public int Flash
        {
            get { return flash; }
            set
            {
                flash = value;
                OnNotifyPropertyChanged("Flash");
            }
        }

        [XmlAttribute("brightness")]
        public int Brightness
        {
            get { return brightness; }
            set
            {
                brightness = value;
                OnNotifyPropertyChanged("Brightness");
            }
        }

        public LightMode()
        {

        }

    }
}
