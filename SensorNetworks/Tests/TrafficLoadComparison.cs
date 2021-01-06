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
    public class TrafficLoadComparison
    {
        public void CompareTools()
        {
            var parameters = ReadParametersFromFile();
            var routing = new AlgorithmRouting();
            var bruteForceRouting = new BruteForceRoutingAlt();
            int i = 0;
            foreach (var param in parameters)
            {
                i++;
                var parametersRouting = param;
                var parametersBrute = param;
                var algorithmSuccess = new List<bool>();
                var bruteForceSuccess = new List<bool>();
                var algorithmAliveNodes = new List<int>();
                var bruteForceAliveNodes = new List<int>();
                var algorithmUtility = new List<double>();
                var bruteForceUtility = new List<double>();
                do
                {
                    FindPathAndUpdate(parametersRouting, algorithmSuccess, routing, algorithmUtility);
                    FindPathAndUpdate(parametersBrute, bruteForceSuccess, bruteForceRouting, bruteForceUtility);

                } while (algorithmSuccess.LastOrDefault() != false && bruteForceSuccess.LastOrDefault() != false);
                SaveToCSV(algorithmSuccess, bruteForceSuccess, parametersRouting, algorithmUtility, bruteForceUtility, i);

            }
        }

        private void FindPathAndUpdate(AlgorithmParameters parameters, List<bool> successList, IRouting routing, List<double> utility)
        {
            var routingResult = routing.FindPath(parameters);
            if (routingResult != null)
            {
                successList.Add(routingResult.Count > 1);
                UpdateTrafficLoad(routingResult, parameters);
                utility.Add(routing.GetUtility(routingResult, parameters));
            }
            else
            {
                successList.Add(false);
                utility.Add(0);
            }
        }

        private void UpdateTrafficLoad(List<int> result, AlgorithmParameters parameters)
        {
            foreach (var res in result)
            {
                if (res != 0 && res != result.LastOrDefault())
                {
                    parameters.Tao[res] = parameters.Tao[res] + 1;
                }
            }
        }

        private List<AlgorithmParameters> ReadParametersFromFile()
        {
            using (StreamReader r = new StreamReader("trafficLoadComparisonData.json"))
            {
                string json = r.ReadToEnd();
                return JsonConvert.DeserializeObject<List<AlgorithmParameters>>(json);
            }
        }
        private void SaveToCSV(List<bool> algorithmSuccess, List<bool> bruteForceSuccess, AlgorithmParameters parametersRouting, List<double> algorithmUtility, List<double> bruteForceUtility, int instanceNumber)
        {
            var records = new List<dynamic>();

            dynamic record = new ExpandoObject();
            record.InstanceSize = parametersRouting.Instance.V_size;
            record.AlgorithmSuccess = GetSuccessCount(algorithmSuccess);
            record.BruteSuccess = GetSuccessCount(bruteForceSuccess);
            record.algorithmUtility = GetUtilityMean(algorithmUtility);
            record.bruteForceUtility = GetUtilityMean(bruteForceUtility);
            records.Add(record);

            if (instanceNumber == 1)
            {
                using (TextWriter writer = new StreamWriter("TrafficLoadTest.csv", false, System.Text.Encoding.UTF8))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteRecords(records);

                }
            }
            else
            {
                using (var stream = File.Open("TrafficLoadTest.csv", FileMode.Append))
                using (var writer = new StreamWriter(stream))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.Configuration.HasHeaderRecord = false;
                    csv.WriteRecords(records);
                }
            }
        }

        private dynamic GetUtilityMean(List<double> algorithmUtility)
        {
            var sum = 0.0;
            for (int i = 0; i < algorithmUtility.Count; i++)
            {
                sum += algorithmUtility[i];   
            }
            return sum / algorithmUtility.Count();
        }

        private int GetSuccessCount(List<bool> algorithmSuccess)
        {
            return algorithmSuccess.Where(x => x == true).Count();
        }
    }
}


