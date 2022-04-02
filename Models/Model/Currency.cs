using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeBot.Model
{
    public class Currency
    {
        public RegionRate RegionRate { get; set; }
        public MarketRate MarketRate { get; set; }
        public BankRate BankRate { get; set; }
        public CbuRate CbuRate { get; set; }
        public BirjaRate BirjaRate { get; set; }
    }
}
