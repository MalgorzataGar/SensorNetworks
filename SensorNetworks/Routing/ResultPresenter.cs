using System;
using System.Collections.Generic;
using System.Text;

namespace SensorNetworks
{
    public static class ResultPresenter
    {
        //public static void PrintPath(IList<int> path)
        //{
        //    if (path == null || path.Count <= 1)
        //    {
        //        Console.WriteLine("Cannot find solution");
        //    }
        //    else
        //    {
        //        var vi = 0;
        //        var sb = new StringBuilder();
        //        while (vi != -1)
        //        {
        //            sb.Append($"{vi}->");
        //            vi = path[vi];
        //        }

        //        sb.Length = sb.Length - 2;
        //        Console.WriteLine($"{sb}\n");
        //    }
        //}

        public static void PrintPath(IList<int> path)
        {
            if (path == null || path.Count <= 1)
            {
                Console.WriteLine("Cannot find solution");
            }
            else
            {
                var sb = new StringBuilder();
                foreach(var node in path)
                {
                    sb.Append($"{node}->");
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