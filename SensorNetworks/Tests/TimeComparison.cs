using CsvHelper;
using Newtonsoft.Json;
using SensorNetworks.BruteForce;
using SensorNetworks.Data;
using SensorNetworks.Routing.BruteForceAlt;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Text;

namespace SensorNetworks.Tests
{
    public class TimeComparison
    {
        public void CompareTools()
        {
            var parameters = ReadParametersFromFile();
            var routing = new AlgorithmRouting();
            var bruteForceRouting = new BruteForceRoutingAlt();
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
                if (bruteForceResult != null)
                {
                    bruteForceSuccess.Add(bruteForceResult.Count > 1);
                }
                else bruteForceSuccess.Add(false);
                algorithmSuccess.Add(routingResult.Count > 1);
                watch.Reset();
            }
            watch.Stop();
            SaveToCSV(algorithmSuccess,bruteForceSuccess,algorithmTimeMeasures,bruteForceTimeMeasures);
        }

        private void SaveToCSV(List<bool> algorithmSuccess, List<bool> bruteForceSuccess, List<double> algorithmTimeMeasures, List<double> bruteForceTimeMeasures)
        {
            var records = new List<dynamic>();

            for(int i=0; i<algorithmSuccess.Count; i++)
            {
                dynamic record = new ExpandoObject();
                record.AlgorithmSuccess = algorithmSuccess[i];
                record.BruteSuccess = bruteForceSuccess[i];
                record.algorithmTime = algorithmTimeMeasures[i];
                record.bruteForceTime = bruteForceTimeMeasures[i];
                records.Add(record);
            }
            string docPath =
             Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            using (TextWriter writer = new StreamWriter("TimeTest.csv", false, System.Text.Encoding.UTF8))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(records);

            }
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
