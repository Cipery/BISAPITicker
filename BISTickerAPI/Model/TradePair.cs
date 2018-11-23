using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BISTickerAPI.Model
{
    public class TradePair
    {
        public string Item1 { get; private set; }
        public string Item2 { get; private set; }

        public TradePair(string item1, string item2)
        {
            Item1 = item1;
            Item2 = item2;
        }

        public TradePair(string pair)
        {
            var split = pair.Split('/');
            Item1 = split[0];
            Item2 = split[1];
        }
    }
}
