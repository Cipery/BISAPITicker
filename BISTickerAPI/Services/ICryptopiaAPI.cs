using BISTickerAPI.Model.POCO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BISTickerAPI.Services
{
    public interface ICryptopiaAPI
    {
        CryptopiaMarkets FetchMarkets(string baseCoinSymbol);
    }
}
