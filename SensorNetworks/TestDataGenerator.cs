using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace SensorNetworks
{
    public class TestDataGenerator
    {
        private readonly Random _randomGenerator;
        private Dictionary<int, int> _nodesPerInstance;     // key - number of nodes/ value - number with instances with this amount of nodes
        private List<Instance> _instances;
        private int _areaSize;
        private const string _fileName = "Instances.json";
        private readonly string _path = Path.Combine(Environment.CurrentDirectory, _fileName);
        public TestDataGenerator(Dictionary<int, int> nodesPerInstance, int areaSize)
        {
            _nodesPerInstance = nodesPerInstance;
            _instances = new List<Instance>();
            _areaSize = areaSize;
            _randomGenerator = new Random();
        }

        public void GenerateData()
        {
            foreach (KeyValuePair<int, int> config in _nodesPerInstance)
            {
                for (int i = 0; i < config.Value; i++)
                {
                    _instances.Add(GetInstance(config.Key));
                }
            }
            WriteToFile();
        }
        private Instance GetInstance(int size)
        {
            return new Instance
            {
                V_size = size,
                Coordinates = GenerateCoordinates(size)
            };
        }

        private Dictionary<int, Coordinates> GenerateCoordinates(int size)
        {
            var coordinates = new Dictionary<int, Coordinates>();
            for (int i = 1; i < size/2; i++)
            {
                var coordinate = new Coordinates()
                {
                    X = _randomGenerator.Next(0, _areaSize/2),
                    Y = _randomGenerator.Next(0, _areaSize/2)
                };
                coordinates.Add(i, coordinate);
            }
            for (int i = size/2; i < size ; i++)
            {
                var coordinate = new Coordinates()
                {
                    X = _randomGenerator.Next(_areaSize/2, _areaSize),
                    Y = _randomGenerator.Next(_areaSize / 2, _areaSize)
                };
                coordinates.Add(i, coordinate);
            }
            coordinates.Add(0, new Coordinates { X = 0, Y = 0 });
            coordinates.Add(size, new Coordinates { X = _areaSize, Y = _areaSize });
            return coordinates;
        }

        private void WriteToFile()
        {
            string json = JsonConvert.SerializeObject(_instances, Formatting.Indented);
            using (StreamWriter file = File.CreateText(_path))
            {
                file.Write(json);
            }
        }
    }
}
