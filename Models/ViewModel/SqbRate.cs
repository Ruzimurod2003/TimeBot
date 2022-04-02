namespace TimeBot.ViewModel
{   
    public class Offline
    {
        public int unit { get; set; }
        public int min { get; set; }
        public string code { get; set; }
        public int prev_buy { get; set; }
        public int rate { get; set; }
        public string num_code { get; set; }
        public string max { get; set; }
        public int prev_sell { get; set; }
        public int buy { get; set; }
        public int sell { get; set; }
        public string name { get; set; }
        public string type { get; set; }
    }

    public class Online
    {
        public string code { get; set; }
        public int prev_buy { get; set; }
        public int prev_sell { get; set; }
        public int buy { get; set; }
        public int sell { get; set; }
    }

    public class Data
    {
        public List<Offline> offline { get; set; }
        public List<Online> online { get; set; }
    }

    public class SqbData
    {
        public Data data { get; set; }
        public bool success { get; set; }
    }
}
