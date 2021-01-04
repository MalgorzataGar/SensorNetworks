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
        public ComparisonData()
        {
            _randomGenerator = new Random();
            var data = LoadAlgorithmParameters();
            WriteToFile(data);
        }
        public List<Instance> GenerateData()
        {
            var dict = new Dictionary<int, int>();
            dict.Add(10, 1);
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
                E_max = 2000,
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
            using (StreamWriter file = File.CreateText("lifetimeComparisonData.json"))
            {
                file.Write(json);
            }
        }
    }
}
