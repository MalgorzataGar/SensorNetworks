using System;
using System.Collections.Generic;
using System.Text;

namespace SensorNetworks
{
    class Simulation
    {
        AlgorithmParameters _parameters;
        public IRouting Router;
        Simulation(AlgorithmParameters parameters, IRouting router)
        {
            _parameters = parameters;
            Router = router;
        }
    }
}
