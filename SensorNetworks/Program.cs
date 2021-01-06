using Microsoft.Extensions.DependencyInjection;
using SensorNetworks.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SensorNetworks.BruteForce;
using SensorNetworks.Tests;
using SensorNetworks.Routing.BruteForceAlt;
using SensorNetworks.Experiments;

namespace SensorNetworks
{
    class Program
    {
        static void Main(string[] args)
        {
            var trafficLoad = new TrafficLoadComparison();
            var network = new NetworkLifetimeComparison();
             trafficLoad.CompareTools();
            //network.CompareTools();
            Console.ReadKey();
        }
    }
}
