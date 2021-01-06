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
            //var data = new ComparisonData();
            var comparison = new NetworkLifetimeComparison();
            comparison.CompareTools();
            var dict = new Dictionary<int, int>();
            dict.Add(20, 25);
            dict.Add(30, 25);
            dict.Add(40, 25);
            dict.Add(10, 25);
            var generator = new TestDataGenerator(dict, 50);
            generator.GenerateData();
            var loader = new ConfigLoader();
            var parameters = loader.LoadAlgorithmParameters();
            var routing = new AlgorithmRouting();
            var results = new List<Result>();
            var bruteForceRouting = new BruteForceRoutingAlt();
            bruteForceRouting.size_limit = false;
            //foreach (var param in parameters)
            //{
            //    var routingResult = routing.FindPath(param);
            //    results.Add(routing.result);
            //    bruteForceRouting.size_limit = false;
            //    var bruteForceResult = bruteForceRouting.FindPath(param);
            //    Console.WriteLine("Algorithm result: ");
            //    ResultPresenter.PrintPath(routingResult);
            //    Console.WriteLine("Brute Force result: ");
            //    ResultPresenter.PrintPath(bruteForceResult);
            //    Console.WriteLine("\n");
            //}
            //Common.Common.SaveObject(results, "Results.json");
            var reliability_exp = new Reliability(parameters, routing, bruteForceRouting);
            reliability_exp.Start();
            Console.ReadKey();
        }
    }
}
