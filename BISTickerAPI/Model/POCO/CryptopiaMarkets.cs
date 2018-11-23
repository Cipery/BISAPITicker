using BISTickerAPI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BISTickerAPI.Model.POCO
{
    public class CryptopiaMarkets
    {
        public bool Success { get; set; }
        public String Message { get; set; }
        public List<TickerEntry> Data { get; set; }
    }
}
