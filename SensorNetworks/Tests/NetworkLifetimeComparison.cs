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
            var parameters = ReadParametersFromFile();
            var routing = new AlgorithmRouting();
            var bruteForceRouting = new BruteForceRoutingAlt();
            int i = 0;
            foreach(var param in parameters)
            {
                i++;
                var parametersRouting = param;
                var parametersBrute = param;
                var algorithmSuccess = new List<bool>();
                var bruteForceSuccess = new List<bool>();
                var algorithmAliveNodes = new List<int>();
                var bruteForceAliveNodes = new List<int>();
                do
                {
                    FindPathAndUpdate(parametersRouting, algorithmSuccess, routing, algorithmAliveNodes);
                    FindPathAndUpdate(parametersBrute, bruteForceSuccess, bruteForceRouting, bruteForceAliveNodes);

                } while (algorithmSuccess.LastOrDefault() != false && bruteForceSuccess.LastOrDefault() != false);
                SaveToCSV(algorithmSuccess, bruteForceSuccess, parametersRouting, parametersBrute, algorithmAliveNodes, bruteForceAliveNodes,i);
            }
           
        }

        private void FindPathAndUpdate(AlgorithmParameters parameters, List<bool> successList, IRouting routing, List<int> aliveNodes)
        {
            var routingResult = routing.FindPath(parameters);
            if (routingResult != null)
            {
                successList.Add(routingResult.Count > 1);
                UpdateEnergy(routingResult, parameters);
                CalculateAliveNodes(aliveNodes, parameters);
            }
            else
            {
                successList.Add(false);
                aliveNodes.Add(0);
            }
            
        }

        private void CalculateAliveNodes(List<int> aliveNodes, AlgorithmParameters parameters)
        {
            int count = 0;
            foreach (var energyLevel in parameters.E_nergy)
            {
                if (energyLevel > parameters.Configuration.E_min)
                {
                    count++;
                }
            }
            aliveNodes.Add(count);
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
        private void SaveToCSV(List<bool> algorithmSuccess, List<bool> bruteForceSuccess, AlgorithmParameters parametersRouting, AlgorithmParameters parametersBrute, List<int> algorithmAliveNodes, List<int> bruteForceAliveNodes, int instanceNumber)
        {
            var records = new List<dynamic>();
            dynamic record = new ExpandoObject();
                record.InstanceNumber = instanceNumber;
                record.IterationCount = algorithmSuccess.Count();
                record.InstanceSize = parametersRouting.Instance.V_size;
                record.AlgorithmSuccess = GetSuccessCount(algorithmSuccess);
                record.BruteSuccess = GetSuccessCount(bruteForceSuccess);
                record.AlgoritmAliveNodes = GetAliveNodes(algorithmAliveNodes, algorithmSuccess);
                record.BruteAliveNodes = GetAliveNodes(bruteForceAliveNodes, bruteForceSuccess);
            records.Add(record);
            
            if (instanceNumber == 1)
            {
                using (TextWriter writer = new StreamWriter("LifeTimeTest.csv", false, System.Text.Encoding.UTF8))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteRecords(records);

                }
            }
            else
            {
                using (var stream = File.Open("LifeTimeTest.csv", FileMode.Append))
                using (var writer = new StreamWriter(stream))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.Configuration.HasHeaderRecord = false;
                    csv.WriteRecords(records);
                }
            }
        }

        private int GetAliveNodes(List<int> algorithmAliveNodes,List<bool>success )
        {
            if (success.FirstOrDefault() == false)
            {
                return 0;
            }
            else if (success.LastOrDefault() == true)
            {
                return algorithmAliveNodes.LastOrDefault();
            }
            else 
            {
                
                return algorithmAliveNodes[success.Count() - 2];
            }
                
        }

        private int GetSuccessCount(List<bool> algorithmSuccess)
        {
            return algorithmSuccess.Where(x => x == true).Count();
        }
    }
}
