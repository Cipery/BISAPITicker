using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BISTickerAPI.Helpers
{
    public class AppSettings
    {
        public virtual string[] FetchPairs { get; set; } = new string[0];
    }
}
