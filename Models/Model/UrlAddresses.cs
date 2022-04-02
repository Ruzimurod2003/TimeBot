using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeBot.Model
{
    public class UrlAddresses
    {
        public string UrlRegionCurrencyRate { get; set; }
        public string UrlCbCurrencyRate { get; set; }
        public string UrlBirjaCurrencyRate { get; set; }
        public TimeBot.ViewModel.BankNames UrlBanks { get; set; }
        public UrlWeather UrlWeather { get; set; }
    }
}
