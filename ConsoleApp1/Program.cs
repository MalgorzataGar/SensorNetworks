﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            //var routing = new Routing();
            //routing.FindPath();
            var loader = new ConfigLoader();
            var param = loader.LoadAlgorithmParameters();
        }
    }
}
