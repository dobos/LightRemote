using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LightRemote
{
    public enum LightLocation
    {
        [XmlEnum("front")]
        Front,

        [XmlEnum("back")]
        Back
    }
}
