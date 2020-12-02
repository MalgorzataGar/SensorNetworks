using System;
using System.Collections.Generic;
using System.Text;

namespace SensorNetworks
{
    public class ResultPresenter
    {
        public List<int> _path;
        private int[] _previous; 

        public ResultPresenter(List<int> path, int[] previous)
        {
            _path = path;
            _previous = previous;
        }
        
        public void Present()
        {
            BuildPath();
            PrintPath();
        }
        
        private void BuildPath()
        {
            var vi = 0;
            while (vi != -1)
            {
                _path.Add(vi);
                vi = _previous[vi];
            }
        }
        private void PrintPath()
        {
            var vi = 0;
            var sb = new StringBuilder();
            while (vi != -1)
            {
                sb.Append($"{vi}->");
                vi = _previous[vi];
            }
            sb.Length = sb.Length - 2;
            Console.WriteLine($"{sb}\n");
        }
    }
}