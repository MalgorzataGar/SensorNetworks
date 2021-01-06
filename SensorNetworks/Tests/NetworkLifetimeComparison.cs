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
using System.Linq;
using System.Text;

namespace SensorNetworks.Tests
{
    public class NetworkLifetimeComparison
    {
        public void CompareTools()
        {
            var parametersRouting = ReadParametersFromFile().FirstOrDefault();
            var parametersBrute = ReadParametersFromFile().FirstOrDefault();
            var routing = new AlgorithmRouting();
            var bruteForceRouting = new BruteForceRoutingAlt();
            var algorithmSuccess = new List<bool>();
            var bruteForceSuccess = new List<bool>();
            do
            {
                FindPathAndUpdate(parametersRouting,algorithmSuccess,routing);
                FindPathAndUpdate(parametersBrute, bruteForceSuccess, bruteForceRouting);

            } while (algorithmSuccess.LastOrDefault() != false && bruteForceSuccess.LastOrDefault() != false);
            SaveToCSV(algorithmSuccess, bruteForceSuccess);
           Console.ReadKey();
        }

        private void FindPathAndUpdate(AlgorithmParameters parameters, List<bool> successList, IRouting routing)
        {
            var routingResult = routing.FindPath(parameters);
            if (routingResult != null)
            {
                successList.Add(routingResult.Count > 1);
                UpdateEnergy(routingResult, parameters);
            }
            else successList.Add(false);
        }

        private void UpdateEnergy(List<int> result, AlgorithmParameters parameters)
        {
            foreach (var res in result)
            {
                if (res != 0 && res != result.LastOrDefault())
                {
                    parameters.E_nergy[res] = parameters.E_nergy[res] - 500;
                }
            }
        }

        private List<AlgorithmParameters> ReadParametersFromFile()
        {
            using (StreamReader r = new StreamReader("lifetimeComparisonData.json"))
            {
                string json = r.ReadToEnd();
                return JsonConvert.DeserializeObject<List<AlgorithmParameters>>(json);
            }
        }
        private void SaveToCSV(List<bool> algorithmSuccess, List<bool> bruteForceSuccess)
        {
            var records = new List<dynamic>();

            for (int i = 0; i < algorithmSuccess.Count; i++)
            {
                dynamic record = new ExpandoObject();
                record.AlgorithmSuccess = algorithmSuccess[i];
                record.BruteSuccess = bruteForceSuccess[i];
                records.Add(record);
            }

            using (TextWriter writer = new StreamWriter("LifeTimeTest.csv", false, System.Text.Encoding.UTF8))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(records);

            }
        }
    }
}
