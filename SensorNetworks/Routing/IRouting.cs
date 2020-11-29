using System;
using System.Collections.Generic;
using System.Text;

namespace SensorNetworks
{
    public interface IRouting
    {
        void FindPath(AlgorithmParameters parameters);
    }
}
