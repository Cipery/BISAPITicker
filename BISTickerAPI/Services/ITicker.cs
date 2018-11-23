using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BISTickerAPI.Services
{
    public interface ITicker
    {
        bool UpdateTicker(string[] currencyPairs);
        string GetExchangeName();
    }
}
