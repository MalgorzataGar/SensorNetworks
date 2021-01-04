using Newtonsoft.Json;
using SensorNetworks.BruteForce;
using SensorNetworks.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SensorNetworks.Tests
{
    public class TimeComparison
    {
        public void CompareTools()
        {
            var parameters = ReadParametersFromFile();
            var routing = new Routing();
            var bruteForceRouting = new BruteForceRouting();
            var algorithmTimeMeasures = new List<double>();
            var bruteForceTimeMeasures = new List<double>();
            var algorithmSuccess = new List<bool>();
            var bruteForceSuccess = new List<bool>();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            foreach (var param in parameters)
            {
                watch.Start();
                var routingResult = routing.FindPath(param);
                watch.Stop();
                var routingTime = watch.Elapsed.TotalMilliseconds;
                watch.Reset();
                watch.Start();
                var bruteForceResult = bruteForceRouting.FindPath(param);
                watch.Stop();
                var elapsedMS = watch.Elapsed.TotalMilliseconds;
                algorithmTimeMeasures.Add(routingTime);
                bruteForceTimeMeasures.Add(elapsedMS);
                algorithmSuccess.Add(routingResult.Count > 1);
                bruteForceSuccess.Add(bruteForceResult.Count > 1);
                watch.Reset();
            }
            watch.Stop();
            Console.ReadKey();
        }
        private List<AlgorithmParameters> ReadParametersFromFile()
        {
            using (StreamReader r = new StreamReader("timeComparisonData.json"))
            {
                string json = r.ReadToEnd();
                return JsonConvert.DeserializeObject<List<AlgorithmParameters>>(json);
            }
        }
    }
}
