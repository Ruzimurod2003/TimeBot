using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeBot.ViewModel
{
    public class Weather
    {
        public int Id { get; set; }
        public string RegionName { get; set; }
        public string Temperature { get; set; }
        public string Humidity { get; set; }    //namlik
        public string Precipitation { get; set; }   //yog'ingarchilik ehtimolligi
        public string Wind { get; set; }    //shamol tezligi
        public string Weather_Name { get; set; }    //bu yerda masalan: quyoshli,yomg'irli vahokazo....
        public string Date { get; set; }
    }
}
