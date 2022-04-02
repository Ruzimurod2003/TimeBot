using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeBot.ViewModel
{
    public class CbuRate
    {
        public int id { get; set; }
        public string Code { get; set; }
        public string Ccy { get; set; }
        public string CcyNm_RU { get; set; }
        public string CcyNm_UZ { get; set; }
        public string CcyNm_UZC { get; set; }
        public string CcyNm_EN { get; set; }
        public string Nominal { get; set; }
        public string Rate { get; set; }
        public string Diff { get; set; }
        public string Date { get; set; }
    }

}
