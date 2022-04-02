using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeBot.Model
{
    public class ThreadSettings
    {
        public int SleepMinutes { get; set; }
        public bool LogToFile { get; set; }
        public bool LogToConsole { get; set; }
    }
}
