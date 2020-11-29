using System;
using System.Collections.Generic;
using System.Text;

namespace SensorNetworks
{
    public class ResultPresenter
    {
        public static void PrintPath(List<int> path)
        {
            var sb = new StringBuilder();
            foreach (var node in path)
            {
                sb.Append($"{node}->");
            }

            sb.Length = sb.Length - 2;
            Console.WriteLine(sb.ToString());
        }
        
        public static void PrintPath(int[] previous)
        {
            var vi = 0;
            var sb = new StringBuilder();
            while (vi != -1)
            {
                sb.Append($"{vi}->");
                vi = previous[vi];
            }
            sb.Length = sb.Length - 2;
            Console.WriteLine($"{sb}\n");
        }
    }
}