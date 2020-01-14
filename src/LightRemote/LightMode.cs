using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace LightRemote
{
    [XmlType("mode")]
    public class LightMode : LightRemoteObject
    {
        private byte id;
        private Light light;
        private string name;
        private LightTime time;
        private int flash;
        private int brightness;
        private bool active;

        [XmlAttribute("id")]
        public byte ID
        {
            get { return id; }
            set
            {
                id = value;
                OnNotifyPropertyChanged($"{nameof(ID)}");
            }
        }

        [XmlIgnore]
        public Light Light
        {
            get { return light; }
            set { light = value; }
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

        [XmlAttribute("time")]
        public LightTime Time
        {
            get { return time; }
            set
            {
                time = value;
                OnNotifyPropertyChanged($"{nameof(Time)}");
                OnNotifyPropertyChanged($"{nameof(ModeIconGlyph)}");
            }
        }

        [XmlAttribute("flash")]
        public int Flash
        {
            get { return flash; }
            set
            {
                flash = value;
                OnNotifyPropertyChanged($"{nameof(Flash)}");
                OnNotifyPropertyChanged($"{nameof(ModeIconGlyph)}");
            }
        }

        [XmlAttribute("brightness")]
        public int Brightness
        {
            get { return brightness; }
            set
            {
                brightness = value;
                OnNotifyPropertyChanged($"{nameof(Brightness)}");
                OnNotifyPropertyChanged($"{nameof(ModeIconGlyph)}");
            }
        }

        [XmlIgnore]
        public string ModeIconGlyph
        {
            get
            {
                if (time == LightTime.Off)
                {
                    return "\uF13D";
                }
                else if (flash > 0)
                {
                    return "\uE9A9";
                }
                else
                {
                    return "\uE9A8";
                }
            }
        }

        [XmlIgnore]
        public bool Active
        {
            get { return active; }
            set
            {
                active = value;
                OnNotifyPropertyChanged($"{nameof(Active)}");
                OnNotifyPropertyChanged($"{nameof(ActiveButtonColor)}");
            }
        }

        [XmlIgnore]
        public SolidColorBrush ActiveButtonColor
        {
            get
            {
                if (active)
                {
                    return Application.Current.Resources["ToggleButtonBackgroundChecked"] as SolidColorBrush;
                }
                else
                {
                    return Application.Current.Resources["ToggleButtonBackground"] as SolidColorBrush;
                }
            }
        }

        public LightMode()
        {
            InitializeMembers();
        }

        public LightMode(LightMode other, Light light)
        {
            CopyMembers(other, light);
        }

        private void InitializeMembers()
        {
            this.id = 255;
            this.light = null;
            this.name = null;
            this.time = LightTime.Off;
            this.flash = 0;
            this.brightness = 0;
            this.active = false;
        }

        private void CopyMembers(LightMode other, Light light)
        {
            this.id = other.id;
            this.light = light;
            this.name = other.name;
            this.time = other.time;
            this.flash = other.flash;
            this.brightness = other.brightness;
            this.active = other.active;
        }
    }
}
