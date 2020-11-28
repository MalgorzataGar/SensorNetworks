using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace SensorNetworks
{
    public class Routing
    {
        public const int V_size = 35;
        public int vq = 35;
        public int vs = 0;
        public List<int> V = new List<int>();
        public List<Node> E = new List<Node>();
        public Dictionary<int, Coordinates> Coordinates = new Dictionary<int, Coordinates>();

        public List<int> W = new List<int>();
        public double[] L = new double[V_size+1];
        public double[] M = new double[V_size+1];                     //payment offer from destination
        public int[] Previous = new int[V_size+1];
        public List<int> Path = new List<int>();

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
        public double[,] D = new double[V_size+1, V_size+1];            //distance
        public double[,] Delta = new double[V_size+1, V_size+1];        //is in transmision range 0/1 distance <R
        public double[] p = new double[V_size + 1];
        public Dictionary<int, List<int>> Neighbour = new Dictionary<int, List<int>>();

        public void SetLambda()
        {
            for (int i = 1; i < V_size + 1; i++)
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
        private void GenerateData()
        {
            E_min = 20.0;
            E_max = 2000.0;
            R = 18.0;
            ni = 0.3;
            Beta = 5.0;
            Gamma = 20.0;
            Q = 50000.0;
            for (int i = 0; i < V_size + 1; i++)
            {
                var randomGenerator = new Random();
                p[i] = (randomGenerator.NextDouble() / 2) + 0.5;
                //handicap dla node'ów po skosie
                if ((i % 6 == i / 6) && i != 35 && i != 0)
                    p[i] = 0.2;
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
            var randomGenerator = new Random(0);
            for (int i = 0; i < V_size + 1; i++)
            {
                var coordinates = new Coordinates()
                {
                    X = randomGenerator.Next(0, 50),
                    Y = randomGenerator.Next(0, 50)
                };
                Coordinates.Add(i, coordinates);
            }
            //Coordinates[7] = new Coordinates() { X = 100, Y = 100 };
            //Coordinates[15] = new Coordinates() { X = 100, Y = 100 };
            //Coordinates[6] = new Coordinates() { X = 100, Y = 100 };
            //Coordinates.Add(vq, new Coordinates
            //{
            //    X = 50.0,
            //    Y = 50.0,
            //});

        }

        private void CalculateDistance()
        {
            for (int i = 0; i < V_size + 1; i++)
            {
                for (int j = 0; j < V_size + 1; j++)
                {
                    D[i, j] = Math.Sqrt(Math.Pow(Coordinates[j].X - Coordinates[i].X, 2) + Math.Pow(Coordinates[j].Y - Coordinates[i].Y, 2));
                    if (D[i, j] < R && i!=j)
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
            for (var vi = 0; vi < V_size; vi++)
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
        public void FindPath()
        {
            GenerateData();
            Initialize();
            do
            {
                int v_j = GetVj();
                var v_i_List = new List<int>();

                foreach (var node in E.Where(x => (x.To == v_j)).ToList())
                {
                    v_i_List.Add(node.From);
                }
                foreach (var v_i in v_i_List)
                {
                    var c = CostFunction(v_i, v_j);
                    var x = L[v_j] + c;
                    if (L[v_i] > x)
                    {
                        M[v_i] = M[v_j] * p[v_i];
                        if ((M[v_i] - c) < 0)
                        {
                            Console.WriteLine($"Utility function at node {v_i}");
                            E.Remove(E.Where(node => (node.From == v_i && node.To == v_j)).FirstOrDefault());
                            E.Remove(E.Where(node => (node.From == v_j && node.To == v_i)).FirstOrDefault());
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
            Visualize();
        }

        private bool WHasNeighbour()
        {
            if (W.Any(vi => E.Any(node => (node.To == vi) && (!W.Contains(node.From)))))
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
            SetLambda();
            return (Beta / Q) * (D[v_i, v_j] / Delta[v_i, v_j]) * (E_max / (E_nergy[v_j] * Lambda[v_j])) * (1 + (Gamma * Tao[v_j]));
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
            while(vi != -1)
            {
                Console.WriteLine($"{vi}->");
                vi = Previous[vi];
            }
        }

        private void Visualize()
        {
            var node_count = V_size + 1;
            var width = 6;
            var number_format = "D2";
            string horizontal_empty_space = "  ";
            string horizontal_empty_space_one = " ";
            string horizontal_link = "--";
            string empty_space_under_number = "  ";
            string right_slash = "/";
            string left_slash = "\\";
            string vertical_link = "| ";
            for(int i = 0; i < width; i++)
            {
                var line = "";
                var start_node = node_count - (i + 1) * width;
                for(int j = 0; j < width;j++)
                {
                    var current_node = start_node + j;
                    line = String.Concat(line, current_node.ToString(number_format));
                    if(j!=width-1)
                    {
                        if (IsNeighbourPath(current_node,current_node + 1))
                            line = String.Concat(line, horizontal_link);
                        else
                            line = String.Concat(line, horizontal_empty_space);
                    }
                }
                Console.WriteLine(line);
                if (i != width - 1)
                {
                    var line2 = "";
                    for (int j = 0; j < width; j++)
                    {
                        var current_node = start_node + j;
                        if (IsNeighbourPath(current_node,current_node-width))
                            line2 = String.Concat(line2, vertical_link);
                        else
                            line2 = String.Concat(line2, empty_space_under_number);
                        if (j != width - 1)
                        {
                            if (IsNeighbourPath(current_node, current_node - (width - 1)))
                                line2 = String.Concat(line2, left_slash);
                            else
                                line2 = String.Concat(line2, horizontal_empty_space_one);
                            if (IsNeighbourPath(current_node + 1, current_node - width))
                                line2 = String.Concat(line2, right_slash);
                            else
                                line2 = String.Concat(line2, horizontal_empty_space_one);
                        }
                    }
                    Console.WriteLine(line2);
                    var line3 = "";
                    for (int j = 0; j < width; j++)
                    {
                        var current_node = start_node + j;
                        if (IsNeighbourPath(current_node, current_node - width))
                            line3 = String.Concat(line3, vertical_link);
                        else
                            line3 = String.Concat(line3, empty_space_under_number);
                        if (j != width - 1)
                        {
                            if (IsNeighbourPath(current_node + 1, current_node - width))
                                line3 = String.Concat(line3, right_slash);
                            else
                                line3 = String.Concat(line3, horizontal_empty_space_one);
                            if (IsNeighbourPath(current_node, current_node - (width - 1)))
                                line3 = String.Concat(line3, left_slash);
                            else
                                line3 = String.Concat(line3, horizontal_empty_space_one);
                        }
                    }
                    Console.WriteLine(line3);
                }
            }
        }
        private bool IsNeighbourPath(int v_i, int v_j)
        {
            if(Path.Contains(v_i) && Path.Contains(v_j))
            {
                if (Math.Abs(Path.IndexOf(v_i) - Path.IndexOf(v_j)) == 1)
                    return true;
            }
            return false;
        }
    }
}
