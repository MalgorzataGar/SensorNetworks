using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SensorNetworks.Common
{
    class Common
    {
        public static void SaveObject(object obj, string filename)
        {
            string json = JsonConvert.SerializeObject(obj, Formatting.Indented);
            using (StreamWriter file = File.CreateText(filename))
            {
                file.Write(json);
            }
        }
    }
}
