using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculateBaseRiskPremium
{
    class Utils
    {
        private static Random _global = new Random();
        [ThreadStatic]
        private static Random _local;

        public static double NextDouble()
        {
            Random inst = _local;
            if (inst == null)
            {
                int seed;
                lock (_global) seed = _global.Next();
                _local = inst = new Random(seed);
            }
            return inst.NextDouble();
        }
    }
}
