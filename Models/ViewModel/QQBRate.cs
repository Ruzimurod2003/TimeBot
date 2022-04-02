using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeBot.ViewModel
{
    public class Currency
    {
        public int id { get; set; }
        public string name { get; set; }
        public string slug { get; set; }
        public string symbol { get; set; }
        public string logo { get; set; }
        public string sell_rate { get; set; }
        public string buy_rate { get; set; }
        public string cb_rate { get; set; }
    }

    public class CurrencyRate
    {
        public int id { get; set; }
        public string date { get; set; }
        public object avtive { get; set; }
        public IEnumerable<Currency> currencies { get; set; }
    }

    public class PrevCurrencyRate
    {
        public int id { get; set; }
        public string date { get; set; }
        public object avtive { get; set; }
        public IEnumerable<Currency> currencies { get; set; }
    }

    public class QQBRateData
    {
        public CurrencyRate currency_rate { get; set; }
        public PrevCurrencyRate prev_currency_rate { get; set; }
    }

    public class QQBRate
    {
        public bool success { get; set; }
        public QQBRateData data { get; set; }
    }
}
