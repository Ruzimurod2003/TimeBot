using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeBot.ViewModel
{
    public class User
    {        
        public int Id { get; set; }
        public string FullName { get; set; }
        public string UserId { get; set; }
        public string Created { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public RegionName RegionName { get; set; }
        public Language Language { get; set; }
    }
    public enum RegionName
    {
        Тошкент_шахри,
        Тошкент_вилояти,
        Сирдарё_вилояти,
        Бухоро_вилояти,
        Хоразм_вилояти,
        Наманган_вилояти,
        Андижон_вилояти,
        Фарғона_вилояти,
        Қашқадарё_вилояти,
        Сурхондарё_вилояти,
        Жиззах_вилояти,
        Қорақалпоғистон_Республикаси,
        Навоий_вилояти,
        Самарқанд_вилояти
    }
    public enum Language
    {
        Uzbek, Russian, English
    }
}
