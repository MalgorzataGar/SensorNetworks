using System;
using System.Collections.Generic;
using System.Text;

namespace SensorNetworks.Data
{
    public class ReliabilityResult
    {
        public double BruteForceResult;
        public double AlgorithmResult;
        public int InstanceNumber;
        public int InstanceSize;
        public List<int> BruteForcePath;
        public List<int> AlgorithmPath;
    }
}
