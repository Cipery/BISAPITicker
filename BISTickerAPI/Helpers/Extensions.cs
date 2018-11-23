using BISTickerAPI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BISTickerAPI.Helpers
{
    public static class Extensions
    {
        /*public static bool HasValue(this Tuple<string, string> tuple)
        {
            return !string.IsNullOrEmpty(tuple?.Item1) && !string.IsNullOrEmpty(tuple?.Item2);
        }*/

        public static bool HasValue<T>(this Tuple<TickerEntry, TickerEntry> tuple)
        {
            return tuple != null && tuple.Item1 != null && tuple.Item2 != null;
        }
    }
}
