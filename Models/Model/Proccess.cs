using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeBot.Model
{
    public class Proccess
    {
        public Weather Weather { get; set; }
        public Currency Currency { get; set; }
        public Prediction Prediction { get; set; }
        public Other Other { get; set; }
    }
}
