using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SensorNetworks.Tests
{
    public class ComparisonData
    {
        private Random _randomGenerator;
        private string _fileName = "trafficLoadComparisonData.json";
        public ComparisonData()
        {
            _randomGenerator = new Random();
            var data = LoadAlgorithmParameters();
            WriteToFile(data);
        }
        public List<Instance> GenerateData()
        {
            var dict = new Dictionary<int, int>();
            /*dict.Add(5, 10);
            dict.Add(7, 10);
            dict.Add(9, 10);
            dict.Add(12, 10);
            dict.Add(15, 10);
            dict.Add(17, 10);
            dict.Add(20, 10);
            dict.Add(30, 10);
            dict.Add(40, 10);*/
            dict.Add(12, 90);
            var generator = new DataGenerator(dict, 50);
            return generator.GenerateDataAndReturn();
        }
        public List<AlgorithmParameters> LoadAlgorithmParameters()
        {
            var instances = GenerateData();
            return instances.Select(x => CreateParameters(x)).ToList();
        }
        private AlgorithmParameters CreateParameters(Instance instance)
        {
            var configuration = GetConfigurationFromRanges();
            var loader = new ConfigLoader();
            return loader.CreateParameters(configuration, instance);
        }

        private Configuration GetConfigurationFromRanges()
        {
            return new Configuration
            {
                E_max = _randomGenerator.Next(1000, 2000),
                E_min = _randomGenerator.Next(20, 40),
                T = _randomGenerator.Next(5, 40),
                R = _randomGenerator.Next(20, 50),
                ni = _randomGenerator.NextDouble() / 5 + 0.3,
                Beta = _randomGenerator.Next(5, 9),
                Gamma = _randomGenerator.Next(15, 20),
                Q = _randomGenerator.Next(40000, 50000)
            };
        }
        private void WriteToFile(List<AlgorithmParameters> data)
        {
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            using (StreamWriter file = File.CreateText(_fileName))
            {
                file.Write(json);
            }
        }
    }
}
