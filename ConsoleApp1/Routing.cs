using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ConsoleApp1
{
    public class Routing
    {
        public const int V_size = 100;
        public List<int> W = new List<int>();
        public List<int> V = new List<int>();
        public List<Node> E = new List<Node>();
        public double[] L = new double[V_size+1];
        public double[] M = new double[V_size+1];                     //payment offer from destination
        public double[] p = new double[V_size+1];
        public int vs = 0;
        public int vq = V_size;
        public Dictionary<int, List<int>> Neighbour = new Dictionary<int, List<int>>();
        public int[] Prevoius = new int[V_size+1];

        public double E_min;
        public double E_max;
        public double Beta;                                         //coefficient value of transmission cost
        public double Gamma;                                        //coefficient vale of growth
        public double Q;                                            //payment for transmission
        public double ni;
        public double T;                                            //total nuber of packages
        public double R;
        public double[] E_nergy = new double[V_size+1];
        public double[] T_ = new double[V_size+1];                    //traffic load
        public double[] Tao = new double[V_size+1];                   //tao = T_i/T
        public double[] Lambda = new double[V_size+1];                //if energy left is less than e_min
        public double[,] D = new double[V_size, V_size+1];            //distance
        public double[,] Delta = new double[V_size+1, V_size+1];        //is in transmision range 0/1 distance <R

        public Dictionary<int, Coordinates> Coordinates = new Dictionary<int, Coordinates>();

        private void GenerateData()
        {
            E_min = 20;
            E_max = 200;
            R = 20;
            ni = 0.3;
            Beta = 5;
            Gamma = 3;
            Q = 4;
            for (int i = 1; i < V_size; i++)
            {
                var randomGenerator = new Random();
                p[i] = (randomGenerator.NextDouble() / 2) + 0.5;
                E_nergy[i] = E_max;
                Tao[i] = randomGenerator.NextDouble();
            }
            GenerateLocations();
            CalculateDistance();
            SetNeighbours();
        }

        private void SetNeighbours()
        {
            var randomGenerator = new Random();
            for (int i = 1; i < V_size; i++)
            {
                Neighbour.Add(i, new List<int>());
                var neighbours = E.Where(x => x.From == i).Select(x => x.To);
                Neighbour[i].AddRange(neighbours);
            }
        }

        private void GenerateLocations()
        {
            var randomGenerator = new Random();
            for (int i = 1; i < V_size; i++)
            {
                var coordinates = new Coordinates()
                {
                    X = randomGenerator.Next(0, 50),
                    Y = randomGenerator.Next(0, 50)
                };
                Coordinates.Add(i, coordinates);
            }
            Coordinates.Add(vq, new Coordinates
            {
                X = 25,
                Y = 17,
            });

        }

        private void CalculateDistance()
        {
            for (int i = 1; i < V_size; i++)
            {
                for (int j = 1; j < V_size; j++)
                {
                    D[i, j] = Math.Sqrt(Math.Pow((Coordinates[j].X - Coordinates[i].X), 2) + Math.Pow((Coordinates[j].Y - Coordinates[i].Y), 2));
                    if (D[i, j] < R && i != j)
                    {
                        Delta[i, j] = 1;
                        E.Add(new Node
                        {
                            From = i,
                            To = j,
                        });

                    }
                    else 
                    {
                        Delta[i, j] = 0;
                    }

                }
            }
        }

        private void Initialize()
        {
            W = new List<int>();
            L[vq] = 0;
            M[vq] = 1;
            for (var vi = 1; vi < V_size; vi++)
            {
                V.Add(vi);
                L[vi] = 999999999999999999;
                M[vi] = 0;
                Prevoius[vi] = -1;
            }
        }
        public void FindPath()
        {
            GenerateData();
            Initialize();
            while ((!W.Contains(vs)) && Neighbour.ContainsKey(W.LastOrDefault()) && Neighbour[W.LastOrDefault()].Any())
            {
                int v_j = GetVj();
                var v_i_List = new List<int>();

                foreach (var node in E.Where(x => (x.To == v_j)).ToList())
                {
                     v_i_List.Add(node.From);
                }
                foreach (var v_i in v_i_List)
                {
                    var x = L[v_j] + UtilityFunction(v_i, v_j);
                    if (L[v_i] > x)
                    {
                        L[v_i] = x;
                        M[v_i] = M[v_j] * p[v_i];
                        if ((M[v_i] - UtilityFunction(v_i, v_j)) < 0)
                        {
                            E.Remove(E.Where(a => a.From == v_i && a.To == v_j).FirstOrDefault());
                        }
                        else
                        {
                            Prevoius[v_i] = v_j;
                        }
                    }
                }
            }
        }

        private double UtilityFunction(int v_i, int v_j)
        {
            SetLambda();
            return (Beta / Q) * (D[v_i, v_j] / Delta[v_i, v_j]) * (E_max / (E_nergy[v_j] * Lambda[v_j])) * (1 + (Gamma * Tao[v_j]));
        }

        private void SetLambda()
        {
            for (int i = 1; i < V_size; i++)
            {
                if (E_nergy[i] >= E_min)
                {
                    Lambda[i] = 1;
                }
                else
                {
                    Lambda[i] = 0;
                }
            }
        }
        
        private int GetVj()
        {
            for (int vj = 1; vj < V_size; vj++)
            {
                bool isLarger = false;
                V.Remove(vj);
                foreach (var vk in V)
                {
                    if (L[vj] > L[vk])
                    {
                        isLarger = true;
                    }
                }
                if (!isLarger)
                {
                    W.Add(vj);
                    return vj;
                }
                else V.Add(vj);
            }
            return -1;
        }
    }
}
