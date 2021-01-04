using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace SensorNetworks.BruteForce
{
    public class BruteForceRouting : IRouting
    {
        private readonly CostCalculator _calculator = new CostCalculator();
        private AlgorithmParameters _parameters;

        private int v_s;
        private int v_q;
        private List<int> _result;
        private Dictionary<int, double> _pathIndexToCost = new Dictionary<int, double>();
        private List<List<int>> _paths = new List<List<int>>();

        private int _startTime;
        private int _endTime;

        public List<int> FindPath(AlgorithmParameters parameters)
        {
            _parameters = parameters;
            if (_parameters.Instance.V_size > 20)
            {
                return _result;
            }
            Initialize();
            ChooseBestPath();
            return _result;
        }

        private void Initialize()
        {
            var oldV_q = v_q;
            v_q = _parameters.Instance.V_size;
            if (oldV_q != v_q)
            {
                _paths = new List<List<int>>();
                GenerateAllPossiblePaths();
            }
            v_s = 0;
            _result = new List<int>();
            _pathIndexToCost = new Dictionary<int, double>();
        }

        private void GenerateAllPossiblePaths()
        {
            var inBetweenNodes = GenerateInBetweenNodes(v_s, v_q);
            var combinations = inBetweenNodes.Combinations();
            foreach (var combination in combinations)
            {
                var permutations = combination.Permute();
                foreach (var permutation in permutations)
                {
                    var path = GeneratePathFromPermutation(permutation);
                    _paths.Add(path);
                }
            }
        }
        
        private List<int> GeneratePathFromPermutation(IEnumerable<int> permutation)
        {
            var path = new List<int>();
            path.Add(v_s);
            path.AddRange(permutation);
            path.Add(v_q);
            return path;
        }

        private List<int> GenerateInBetweenNodes(int source, int destination)
        {
            var result = new List<int>();
            for (int i = 1; i < destination; i++)
            {
                result.Add(i);
            }
            return result;
        }
        
        private void ChooseBestPath()
        {
            for(int i = 0; i < _paths.Count; i++)
            {
                var cost = CalculateCost(_paths[i]);
                _pathIndexToCost.Add(i, cost);
            }
            var resultIndex = _pathIndexToCost.OrderBy(x => x.Value).First();
            if (resultIndex.Value != Double.PositiveInfinity)
            {
                _result = _paths[resultIndex.Key];
            }
        }

        private double CalculateCost(List<int> path)
        {
            double totalCost = 0;
            for (int i = 0; i < path.Count - 1; i++)
            {
                totalCost += _calculator.Calculate(path[i], path[i + 1], _parameters);
            }

            return totalCost;
        }
    }
}