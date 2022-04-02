using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeBot.ViewModel
{
    public class AsakaBankRate
    {
        public int count { get; set; }
        public string next { get; set; }
        public string previous { get; set; }
        public List<result> results { get; set; }
    }
    public class result
    {
        public int id { get; set; }
        public string name { get; set; }
        public string rate { get; set; }
        public string diff { get; set; }
        public string code { get; set; }
        public string date { get; set; }
        public string buy { get; set; }
        public string sell { get; set; }
        public string currency_type { get; set; }
        public string type { get; set; }
        public string modified_date { get; set; }
    }
}
