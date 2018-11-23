using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BISTickerAPI.Entities
{
    public class Exchange
    {
        [Key]
        public string Name { get; set; }
        public string Website { get; set; }
    }
}
