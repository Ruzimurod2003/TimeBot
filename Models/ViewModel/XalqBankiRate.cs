using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeBot.ViewModel
{
    public class XalqBankiRate
    {
        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
        public string title { get; set; }
        public int CODE { get; set; }
        public string EQUIVAL { get; set; }
        public string DIFF_RATE { get; set; }
        public string BUYING_RATE { get; set; }
        public string DIFF_BUYING_RATE { get; set; }
        public string SELLING_RATE { get; set; }
        public string DIFF_SELLING_RATE { get; set; }
        public int day { get; set; }
        public int month { get; set; }
        public int created { get; set; }
        public int status { get; set; }

    }
}
