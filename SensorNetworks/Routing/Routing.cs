using SensorNetworks.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace SensorNetworks
{
    public class Routing : IRouting
    {
        private AlgorithmParameters _parameters;
        private CostCalculator _costCalculator = new CostCalculator();
        public List<int> W = new List<int>();
        public List<int> V = new List<int>();
        public double[] L;
        public double[] M;
        public int[] Previous;
        public double[] G;  //zysk
        public List<int> Path = new List<int>();
        private int vs = 0;
        private int vq;
        public Result result;

        public List<int> FindPath(AlgorithmParameters parameters)
        {
            ClearAll();
            _parameters = parameters;
            Initialize();
            do
            {
                int v_j = GetVj();
                var v_i_List = new List<int>();

                foreach (var node in _parameters.E.Where(x => (x.To == v_j)).ToList())
                {
                    v_i_List.Add(node.From);
                }
                foreach (var v_i in v_i_List)
                {
                    var c = _costCalculator.Calculate(v_i, v_j, _parameters);
                    var x = L[v_j] + c;
                    G[v_j] = (M[v_j] - c) * parameters.Configuration.Q;
                    if (L[v_i] > x)
                    {
                        M[v_i] = M[v_j] * _parameters.p[v_i];
                        if ((M[v_i] - c) < 0)
                        {
                            Console.WriteLine($"Utility function at node {v_i}");
                            _parameters.E.Remove(_parameters.E.Where(node => (node.From == v_i && node.To == v_j)).FirstOrDefault());
                            _parameters.E.Remove(_parameters.E.Where(node => (node.From == v_j && node.To == v_i)).FirstOrDefault());
                        }
                        else
                        {
                            L[v_i] = x;
                            Previous[v_i] = v_j;
                        }
                    }
                }
            } while (!W.Contains(vs) && WHasNeighbour());
            BuildPath();
            var presenter = new ResultPresenter(Path, Previous);
            presenter.Present();
            var welfare = _costCalculator.CalculateWelfare(vs,vq,G,Path);
            result = new Result { Path = Path, Welfare = welfare }
            return Path;
        }

        private void ClearAll()
        {
            _parameters = null;
            W = new List<int>();
            V = new List<int>();
            Path = new List<int>();
        }

        private void Initialize()
        {
            L = new double[_parameters.Instance.V_size + 1];
            M = new double[_parameters.Instance.V_size + 1];
            G = new double[_parameters.Instance.V_size + 1];
            Previous = new int[_parameters.Instance.V_size + 1];
            vq = _parameters.Instance.V_size;
            for (var vi = 0; vi < _parameters.Instance.V_size + 1; vi++)
            {
                V.Add(vi);
                L[vi] = Double.PositiveInfinity;
                M[vi] = 1.0;
                G[vi] = 0.0;
                Previous[vi] = -1;
            }
            L[vq] = 0.0;
            V.Add(vq);
        }

        private bool WHasNeighbour()
        {
            if (W.Any(vi => _parameters.E.Any(node => (node.To == vi) && (!W.Contains(node.From))) ||
                            _parameters.E.Any(node => (node.From == vi) && (!W.Contains(node.To)))))
            {
                return true;
            }
            else
            {
                Console.WriteLine($"No neighbours :(");
                return false;
            }
        }

        private int GetVj()
        {
            foreach (var vj in V)
            {
                bool isLarger = false;
                foreach (var vk in V)
                {
                    if (L[vj] > L[vk] && vj != vk)
                    {
                        isLarger = true;
                    }
                }

                if (!isLarger)
                {
                    V.Remove(vj);
                    W.Add(vj);
                    return vj;
                }
            }

            return -1;
        }
        private void BuildPath()
        {
            var vi = 0;
            while (vi != -1)
            {
                Path.Add(vi);
                vi = Previous[vi];
            }
            
        }

    }
}
