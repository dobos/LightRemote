using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LightRemote
{
    [XmlType("light")]
    public class LightModel : LightRemoteObject
    {
        private string name;
        private List<LightMode> modes;
        private LightLocation location;

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
                OnNotifyPropertyChanged("Location");
            }
        }

        public LightModel()
        {
            InitializeMembers();
        }

        public LightModel(string name)
            : this()
        {
            this.name = name;
        }

        public LightModel(LightModel other)
        {
            CopyMembers(other);
        }

        private void InitializeMembers()
        {
            this.name = null;
            this.modes = new List<LightMode>();
            this.location = LightLocation.Front;
        }

        private void CopyMembers(LightModel other)
        {
            this.name = other.name;
            this.modes = other.modes;
            this.location = other.location;
        }
    }
}
