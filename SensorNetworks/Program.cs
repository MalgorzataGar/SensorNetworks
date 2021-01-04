using Microsoft.Extensions.DependencyInjection;
using SensorNetworks.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SensorNetworks.BruteForce;
using SensorNetworks.Tests;

namespace SensorNetworks
{
    class Program
    {
        static void Main(string[] args)
        {
            var data = new ComparisonData();
            var comparison = new NetworkLifetimeComparison();
            comparison.CompareTools();
            Console.ReadKey();
        }
    }
}
