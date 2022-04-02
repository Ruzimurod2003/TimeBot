using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeBot.ViewModel
{
    public class ZiraatBankRate
    {
        public List<data1> Data { get; set; }
    }
    public class data1
    {
        public string course_Type { get; set; }
        public string currencyIdStr { get; set; }
        public int atmBuy { get; set; }
        public int atmSell { get; set; }
        public double corporateBuy { get; set; }
        public double corporateSell { get; set; }
        public int id { get; set; }
        public int currencyID { get; set; }
        public int tenantID { get; set; }
        public string name { get; set; }
        public bool active { get; set; }
        public int value { get; set; }
        public int oldValue { get; set; }
        public object difference { get; set; }
        public int orderNumber { get; set; }
        public int unit { get; set; }
        public double effectiveBuy { get; set; }
        public object effectiveSell { get; set; }
    }
}
