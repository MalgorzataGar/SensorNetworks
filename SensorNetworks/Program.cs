using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorNetworks
{
    class Program
    {
        static void Main(string[] args)
        {
            //var routing = new Routing();
            //routing.FindPath();
            var loader = new ConfigLoader();
            var param = loader.LoadAlgorithmParameters();
        }
    }
}
