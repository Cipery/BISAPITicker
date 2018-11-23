using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BISTickerAPI.Model.Exceptions
{
    public class CoinNotFoundException : Exception
    {
        public string CoinSymbol { get; set; }
    }
}
