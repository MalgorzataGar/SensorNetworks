using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class ConfigLoader
    {
        private Random _randomGenerator;
        public ConfigLoader()
        {
            _randomGenerator = new Random();
        }
        public List<AlgorithmParameters> LoadAlgorithmParameters()
        {
            var configuration = ReadConfigurationFromFile();
            var instances = ReadInstancesFromFile();
            return instances.Select(x =>CreateParameters(configuration,x)).ToList();
        }
        private AlgorithmParameters CreateParameters(Configuration configuration, Instance instance)
        {
            var parameters = new AlgorithmParameters(instance, configuration);

            for (int i = 0; i < instance.V_size + 1; i++)
            {
                parameters.p[i] = (_randomGenerator.NextDouble() / 2) + 0.5;
                parameters.E_nergy[i] = configuration.E_max;
                parameters.Tao[i] = _randomGenerator.NextDouble();
                SetDisctance(ref parameters, i, instance);
                SetNeighbours(ref parameters, i);
            }
            return parameters;
        }
        private void SetNeighbours(ref AlgorithmParameters parameters, int i)
        {
            parameters.Neighbour.Add(i, new List<int>());
            var neighbours = parameters.E.Where(x => x.From == i).Select(x => x.To);
            parameters.Neighbour[i].AddRange(neighbours);
        }
        private void SetDisctance(ref AlgorithmParameters parameters, int i, Instance instance)
        {
            for (int j = 0; j < instance.V_size + 1; j++)
            {
                parameters.D[i, j] = Math.Sqrt(Math.Pow(instance.Coordinates[j].X - instance.Coordinates[i].X, 2) + Math.Pow(instance.Coordinates[j].Y - instance.Coordinates[i].Y, 2));
                if (parameters.D[i, j] < parameters.Configuration.R && i != j)
                {
                    parameters.Delta[i, j] = 1;
                    parameters.E.Add(new Node
                    {
                        From = i,
                        To = j,
                    });
                }
                else
                {
                    parameters.Delta[i, j] = 0;
                }
            }
        }
        private Configuration ReadConfigurationFromFile()
        {
            using (StreamReader r = new StreamReader("Configuration.json"))
            {
                string json = r.ReadToEnd();
                return  JsonConvert.DeserializeObject<Configuration>(json);
            }
        }
        private List<Instance> ReadInstancesFromFile()
        {
            using (StreamReader r = new StreamReader("Instances.json"))
            {
                string json = r.ReadToEnd();
                return JsonConvert.DeserializeObject<List<Instance>>(json);
            }
        }
    }
}
