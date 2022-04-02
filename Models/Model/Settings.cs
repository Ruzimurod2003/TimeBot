using Newtonsoft.Json;

namespace TimeBot.Model
{
    public class Settings 
    {
        public ConnectionString ConnectionString { get; set; }
        public Proccess Proccess { get; set; }
        public Telegram Telegram { get; set; }
        public Path Path { get; set; }
        public UrlAddresses UrlAddresses { get; set; }
        public static Settings GetSettings()
        {
            Settings settings;
            using (StreamReader sr = new StreamReader(Configuration.ConfigurationPath))
            {
                string json = sr.ReadToEnd();
                settings = JsonConvert.DeserializeObject<Settings>(json);
            }
            return settings;
        }
    }
}
