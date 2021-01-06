using System;
using System.Collections.Generic;
using System.Text;

namespace SensorNetworks
{
    public interface IRouting
    {
        List<int> FindPath(AlgorithmParameters parameters);
        double GetUtility(List<int> path, AlgorithmParameters parameters);
    }
}
