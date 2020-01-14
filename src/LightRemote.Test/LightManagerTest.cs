using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml.Serialization;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using LightRemote;

namespace LightRemote.Test
{
    [TestClass]
    public class LightManagerTest
    {
        [TestMethod]
        public void TestSaveConfig()
        {
            var l1 = new Light()
            {
                Name = "ION PRO RT"
            };
            l1.Modes.Add(new LightMode()
            {
                Name = "Day steady",
                Time = LightTime.Day,
                Flash = 0,
                Brightness = 5
                });

            var config = new Config();
            config.Lights.Add(l1.Name, l1);

            using (var writer = new StringWriter())
            {
                var s = new XmlSerializer(typeof(Config));
                s.Serialize(writer, config);
                var xml = writer.ToString();
            }
        }

        [TestMethod]
        public async Task TestLoadConfig()
        {
            await LightManager.Instance.LoadConfig();
        }

        [TestMethod]
        public async Task TestFindConnectedLights()
        {
            await LightManager.Instance.LoadConfig();
            await LightManager.Instance.FindConnectedLights();
        }

        [TestMethod]
        public async Task TestTurnOn()
        {
            await LightManager.Instance.LoadConfig();
            await LightManager.Instance.FindConnectedLights();

            foreach (var light in LightManager.Instance.Lights.Values)
            {
                await light.TurnOn();
            }
        }

        [TestMethod]
        public async Task TestTurnOff()
        {
            await LightManager.Instance.LoadConfig();
            await LightManager.Instance.FindConnectedLights();

            foreach (var light in LightManager.Instance.Lights.Values)
            {
                await light.TurnOff();
            }
        }
    }
}
