using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace SensorNetworks
{
    public class Routing : IRouting
    {
        private AlgorithmParameters _parameters;
        public List<int> W = new List<int>();
        public List<int> V = new List<int>();
        public List<int> Path = new List<int>();
        public double[] L;
        public double[] M;
        public int[] Previous;

        private int vs = 0;
        private int vq;

        private void Initialize()
        {
            L = new double[_parameters.Instance.V_size + 1];
            M = new double[_parameters.Instance.V_size + 1];
            Previous = new int[_parameters.Instance.V_size + 1];
            vq = _parameters.Instance.V_size;
            for (var vi = 0; vi < _parameters.Instance.V_size; vi++)
            {
                V.Add(vi);
                L[vi] = Double.PositiveInfinity;
                M[vi] = 1.0;
                Previous[vi] = -1;
            }
            L[vq] = 0.0;
            M[vq] = 1.0;
            Previous[vq] = -1;
            V.Add(vq);
        }
        public void FindPath(AlgorithmParameters parameters)
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
                    var c = CostFunction(v_i, v_j);
                    var x = L[v_j] + c;
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
            PrintPath();
        }

        private void ClearAll()
        {
            _parameters = null;
            W = new List<int>();
            V = new List<int>();
            Path = new List<int>();
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

        private double CostFunction(int v_i, int v_j)
        {
            _parameters.SetLambda();
            return (_parameters.Configuration.Beta / _parameters.Configuration.Q) *
                (_parameters.D[v_i, v_j] / _parameters.Delta[v_i, v_j]) *
                (_parameters.Configuration.E_max / (_parameters.E_nergy[v_j] * _parameters.Lambda[v_j])) *
                (1 + (_parameters.Configuration.Gamma * _parameters.Tao[v_j]));
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
        private void PrintPath()
        {
            var vi = 0;
            while (vi != -1)
            {
                Console.WriteLine($"{vi}->");
                vi = Previous[vi];
            }
            Console.WriteLine("\n");
        }

    }
}
