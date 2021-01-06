using CsvHelper;
using Newtonsoft.Json;
using SensorNetworks.Data;
using SensorNetworks.Routing.BruteForceAlt;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace SensorNetworks.Tests
{
    public class ReliabilityComparison
    {
        public void CompareTools()
        {
            var results = new List<ReliabilityResult>();
            var parameters = ReadParametersFromFile();
            var routing = new AlgorithmRouting();
            var bruteForceRouting = new BruteForceRoutingAlt();
            var algorithmSuccess = new List<bool>();
            var bruteForceSuccess = new List<bool>();
            var max = parameters.LastOrDefault().Instance.V_size;
            var p = new double[max + 1];
            var random = new Random();
            for (int i=0; i < max; i++)
            {
                p[i] = ((random.NextDouble() / 2) + 0.5);
            }
            
            foreach (var param in parameters)
            {
                param.p = p;
                var routingResult = routing.FindPath(param);
                var bruteForceResult = bruteForceRouting.FindPath(param);
                var routingReliability = CalculateReliability(routingResult, param);
                var bruteForceReliability = CalculateReliability(bruteForceResult, param);
                var instanceNumber = parameters.IndexOf(param);
                var result = new ReliabilityResult()
                {
                    AlgorithmPath = routingResult,
                    BruteForcePath = bruteForceResult,
                    InstanceSize = param.Instance.V_size,
                    InstanceNumber = instanceNumber,
                    AlgorithmResult = routingReliability,
                    BruteForceResult = bruteForceReliability,
                };
                results.Add(result);
                if (bruteForceResult != null)
                {
                    bruteForceSuccess.Add(bruteForceResult.Count > 1);
                }
                else bruteForceSuccess.Add(false);
                algorithmSuccess.Add(routingResult.Count > 1);
            }
            SaveToCSV(algorithmSuccess, bruteForceSuccess, results);
        }

        private void SaveToCSV(List<bool> algorithmSuccess, List<bool> bruteForceSuccess, List<ReliabilityResult> results)
        {
            var records = new List<dynamic>();

            for (int i = 0; i < algorithmSuccess.Count; i++)
            {
                dynamic record = new ExpandoObject();
                record.InstanceSize = results[i].InstanceSize;
                record.AlgorithmSuccess = algorithmSuccess[i];
                record.BruteSuccess = bruteForceSuccess[i];
                record.AlgorithmReliability = results[i].AlgorithmResult;
                record.BruteForceReliability = results[i].BruteForceResult;
                records.Add(record);
            }
            using (TextWriter writer = new StreamWriter("ReliabilityTest.csv", false, System.Text.Encoding.UTF8))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(records);

            }
        }

        private List<AlgorithmParameters> ReadParametersFromFile()
        {
            using (StreamReader r = new StreamReader("reliabilityComparisonData.json"))
            {
                string json = r.ReadToEnd();
                return JsonConvert.DeserializeObject<List<AlgorithmParameters>>(json);
            }

        }
        private double CalculateReliability(List<int> Path, AlgorithmParameters param)
        {
            if (Path == null || Path.Count <= 1)
                return 0;
            var reliability = 1.0;
            for (int i = 1; i < Path.Count; i++)
            {
                reliability *= param.p[Path[i]];
            }
            return reliability;
        }
    }
}

