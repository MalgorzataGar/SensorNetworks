using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorNetworks
{
    public class AlgorithmParameters
    {
        public Instance Instance {get; set;}
        public Configuration Configuration { get; set; }
        
        public double[] E_nergy;
        public double[] T_;
        public double[] Tao;
        public double[] Lambda;
        public double[] p;
        public double[,] D;
        public double[,] Delta;
        public List<Node> E;
        public Dictionary<int, List<int>> Neighbour;
        public void SetLambda()
        {
            for (int i = 1; i < Instance.V_size + 1; i++)
            {
                if (E_nergy[i] >= Configuration.E_min)
                {
                    Lambda[i] = 1;
                }
                else
                {
                    Lambda[i] = 0;
                }
            }
        }

        public AlgorithmParameters(Instance instance, Configuration configuration)
        {
            Instance = instance;
            Configuration = configuration;
            E_nergy = new double[Instance.V_size + 1];
            T_ = new double[Instance.V_size + 1];
            Tao = new double[Instance.V_size + 1];
            Lambda = new double[Instance.V_size + 1];
            D = new double[Instance.V_size + 1, Instance.V_size + 1];
            Delta = new double[Instance.V_size + 1, Instance.V_size + 1];
            p = new double[Instance.V_size + 1];
            E = new List<Node>();
            Neighbour = new Dictionary<int, List<int>>();
        }
    }
}
