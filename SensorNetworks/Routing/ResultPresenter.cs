using System;
using System.Collections.Generic;
using System.Text;

namespace SensorNetworks
{
    public static class ResultPresenter
    {
        public static void PrintPath(IList<int> previous)
        {
            if (previous == null || previous.Count <= 1)
            {
                Console.WriteLine("Cannot find solution");
            }
            else
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

        public static void PrintTime(int start, int end)
        {
            //To Do 
        }
    }
}