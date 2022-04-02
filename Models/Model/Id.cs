using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeBot.Model
{
    public class Id
    {
        public long Owner { get; set; }
        public List<long> Admin { get; set; }
        public long Channel { get; set; }
        public long Group { get; set; }
    }
}
