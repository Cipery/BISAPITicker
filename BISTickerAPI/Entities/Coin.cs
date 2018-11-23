using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BISTickerAPI.Entities
{
    public class Coin
    {
        [Key]
        public string Symbol { get; set; }
        public string Name { get; set; }
    }
}
