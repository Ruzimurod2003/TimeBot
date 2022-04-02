using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeBot.Model
{
    public class Other
    {
        public ThreadSettings ThreadSettings { get; set; }
        public string DatabaseName { get; set; }
        public string LogFileName { get; set; }
        public EndPointSettings EndPointSettings { get; set; }
    }
}
