using Newtonsoft.Json;
using SensorNetworks.BruteForce;
using SensorNetworks.Data;
using System;
using System.Collections.Generic;
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
            var routing = new Routing();
            var bruteForceRouting = new BruteForceRouting();
            var algorithmSuccess = new List<bool>();
            var bruteForceSuccess = new List<bool>();
            do
            {
                FindPathAndUpdate(parametersRouting,algorithmSuccess,routing);
                FindPathAndUpdate(parametersBrute, bruteForceSuccess, bruteForceRouting);

            } while (algorithmSuccess.LastOrDefault() != false && bruteForceSuccess.LastOrDefault() != false);
            
           Console.ReadKey();
        }

        private void FindPathAndUpdate(AlgorithmParameters parameters, List<bool> successList, IRouting routing)
        {
            var routingResult = routing.FindPath(parameters);
            successList.Add(routingResult.Count > 1);
            UpdateEnergy(routingResult, parameters);
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
    }
}
