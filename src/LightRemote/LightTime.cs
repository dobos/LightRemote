using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LightRemote
{
    public enum LightTime
    {
        [XmlEnum("off")]
        Off,

        [XmlEnum("day")]
        Day,

        [XmlEnum("night")]
        Night
    }
}
