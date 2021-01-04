using SensorNetworks.Data;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace SensorNetworks.Routing.BruteForceAlt
{

    class BruteForceRoutingAlt : IRouting
    {
        private AlgorithmParameters _parameters;
        private CostCalculator _costCalculator = new CostCalculator();
        public List<int> W = new List<int>();
        public List<int> V = new List<int>();
        public List<int> Path = new List<int>();
        private int vs = 0;
        private int vq;
        public Result result;
        public bool size_limit = true;
        private int max_strikes = 10;
        private int max_paths = 10000000;
        private int current_strike = 0;
        public List<int> FindPath(AlgorithmParameters parameters)
        {
            if (parameters.Instance.V_size >= 30 && size_limit)
            {
                Console.WriteLine("Instance is too big to brute force");
                return null;
            }
            _parameters = parameters;
            Initialize();
            var paths = GenerateFeasiblePaths();
            if (current_strike >= max_strikes)
            {
                Console.WriteLine("Brute Force Timeout");
                return null;
            }
            var costs = PricePaths(paths);
            var min_index = FindMaxIndex(costs);
            if (min_index != -1)
                return paths[min_index];
            else
                return null;
        }
        private int FindMaxIndex(List<double> list)
        {
            var max_index = -1;
            var max_val = Double.MinValue;
            for(int i = 0; i < list.Count; i++)
            {
                if (max_val < list[i])
                {
                    max_val = list[i];
                    max_index = i;
                }
            }
            return max_index;
        }
        private void Initialize()
        {
            vq = _parameters.Instance.V_size;
            current_strike = 0;
        }
        private void ClearPaths(List<List<int>> paths)
        {
            var costs = PricePaths(paths);
            var min_index = FindMaxIndex(costs);
            var best_path = paths[min_index];
            paths.Clear();
            paths.Add(best_path);
            current_strike += 1;
        }
        private void FindNextNeighbour(List<List<int>> paths, List<int> currentPath)
        {
            if (current_strike >= max_strikes)
                return;
            if (paths.Count > max_paths)
                ClearPaths(paths);
            var last_node = currentPath[currentPath.Count - 1];
            var neighbours = from border in _parameters.E.FindAll(item => item.From == last_node) select border.To;
            foreach (var neighbour in neighbours)
            {
                if (!currentPath.Contains(neighbour) && _parameters.D[neighbour,vq] < _parameters.D[last_node,vq])
                {
                    var new_path = currentPath.ToList();
                    new_path.Add(neighbour);
                    if (neighbour == vq)
                        paths.Add(new_path);
                    else
                        FindNextNeighbour(paths, new_path);
                }
            }
        }
        private List<double> PricePaths(List<List<int>> paths)
        {
            var utilities = new List<double>();
            foreach(var path in paths)
            {
                var utility = 0.0;
                var probability = 1.0;
                for(int i=1; i < path.Count; i++)
                {
                    probability *= _parameters.p[path[i]];
                }
                for(int i=0; i<path.Count-1;i++)
                {
                    var node = path[i];
                    var next_node = path[i + 1];
                    utility += probability - _costCalculator.Calculate(node, next_node, _parameters) / _parameters.p[next_node];
                }
                utilities.Add(utility/path.Count);
            }
            return utilities;
        }
        private List<List<int>> GenerateFeasiblePaths()
        {
            var paths = new List<List<int>>();
            var origin_path = new List<int>();
            origin_path.Add(vs);
            FindNextNeighbour(paths, origin_path);
            return paths;
        }
        
    }
}
