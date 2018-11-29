using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BISTickerAPI.Entities;

namespace BISTickerAPI.Helpers
{
    public class AppSettings
    {
        public virtual string[] FetchPairs { get; set; } = new string[0];
        public virtual Coin[] Coins { get; set; } = new Coin[0];
    }
}
