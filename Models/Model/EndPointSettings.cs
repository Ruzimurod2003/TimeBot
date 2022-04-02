using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeBot.Model
{
    public class EndPointSettings
    {
        public bool ProcessRegionCurrencyRate { get; set; }
        public bool ProcessMarketCurrencyRate { get; set; }
        public bool ProcessBankCurrencyRate { get; set; }
        public bool ProcessCbCurrencyRate { get; set; }
        public bool ProcessBirjaCurrencyRate { get; set; }
    }
}
