using SensorNetworks.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace SensorNetworks.Experiments
{
    public class Reliability
    {
        private List<AlgorithmParameters> _parameters;
        private IRouting _algorithm_router;
        private IRouting _brute_router;
        private List<ReliabilityResult> _results;
        public Reliability(List<AlgorithmParameters> parameters, IRouting algorithm_router, IRouting brute_router)
        {
            _parameters = parameters;
            _algorithm_router = algorithm_router;
            _brute_router = brute_router;
        }
        public void Start()
        {
            _results = new List<ReliabilityResult>();
            foreach (var param in _parameters)
            {
                var routingResult = _algorithm_router.FindPath(param);
                var bruteForceResult = _brute_router.FindPath(param);
                Console.WriteLine("Algorithm result: ");
                ResultPresenter.PrintPath(routingResult);
                Console.WriteLine("Brute Force result: ");
                ResultPresenter.PrintPath(bruteForceResult);
                var routingReliability = CalculateReliability(routingResult, param);
                var bruteForceReliability = CalculateReliability(bruteForceResult, param);
                var instanceNumber = _parameters.IndexOf(param);
                Console.WriteLine($"alg: {routingReliability}");
                Console.WriteLine($"bf:{bruteForceReliability}");
                Console.WriteLine($"in:{instanceNumber}");
                var result = new ReliabilityResult()
                {
                    AlgorithmPath = routingResult,
                    BruteForcePath = bruteForceResult,
                    InstanceSize = param.Instance.V_size,
                    InstanceNumber = instanceNumber,
                    AlgorithmResult = routingReliability,
                    BruteForceResult = bruteForceReliability,
                };
                _results.Add(result);
            }
            Common.Common.SaveObject(_results, "ReliabilityResults.json");
        }
        private double CalculateReliability(List<int> Path,AlgorithmParameters param)
        {
            if (Path == null || Path.Count <= 1)
                return 0;
            var reliability = 1.0;
            for (int i = 1; i < Path.Count; i++)
            {
                reliability *= param.p[Path[i]];
            }
            return reliability;
        }
    }
}
