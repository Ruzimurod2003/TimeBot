using Dapper;
using HtmlAgilityPack;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json;
using System.Data;
using System.Net.Http.Json;
using TimeBot.ViewModel;
using WebClientServices.Services;

namespace DB_Services
{
    public interface IDataBaseServices
    {
        List<long> GetUsersId();

        Task InsertWeatherDaily(TimeBot.Model.Settings settings);

        int GetMaxId(string name);

        Task InsertRegionRate(TimeBot.Model.Settings settings);
        Task InsertBankRate(TimeBot.Model.Settings settings);
        Task InsertMarketRate(TimeBot.Model.Settings settings);
        Task InsertCbuRate(TimeBot.Model.Settings settings);
        Task InsertBirjaRate(TimeBot.Model.Settings settings);
    }

    public class DataBaseServices : IDataBaseServices
    {
        private static IDbConnection _db = new SqliteConnection();

        public DataBaseServices()
        {
            _db = new SqliteConnection("Data Source=" + TimeBot.Model.Settings.GetSettings().Path.Database + TimeBot.Model.Settings.GetSettings().Proccess.Other.DatabaseName);
        }

        public static string GetLastTimeCbu()
        {
            int id = _db.QuerySingleOrDefault<int>("select max(Id) from CbuRate");
            string value = (_db.QuerySingleOrDefault<string>(@"select [Date] from CbuRate where Id=@Id", new { Id = id },
                                 commandType: CommandType.Text, commandTimeout: 500));
            return value;
        }

        public static string GetLastTimeMarket()
        {
            int id = _db.QuerySingleOrDefault<int>("select max(Id) from MarketRate");
            string value = (_db.QuerySingleOrDefault<string>(@"select UpdateTime from MarketRate where Id=@Id", new { Id = id },
                                 commandType: CommandType.Text, commandTimeout: 500));
            return value;
        }

        public static string GetLastTimeBank()
        {
            int id = _db.QuerySingleOrDefault<int>("select max(Id) from BankRate");
            string value = (_db.QuerySingleOrDefault<string>(@"select UpdateTime from BankRate where Id=@Id", new { Id = id },
                                 commandType: CommandType.Text, commandTimeout: 500));
            return value;
        }

        public static string GetLastTimeBirja()
        {
            int id = _db.QuerySingleOrDefault<int>("select max(Id) from BirjaRate");
            string value = (_db.QuerySingleOrDefault<string>(@"select UpdateTime from BirjaRate where Id=@Id", new { Id = id },
                                 commandType: CommandType.Text, commandTimeout: 500));
            return value;
        }

        public static string GetLastTimeRegion()
        {
            int id = _db.QuerySingleOrDefault<int>("select max(Id) from RegionRate");
            string value = (_db.QuerySingleOrDefault<string>(@"select UpdateTime from RegionRate where Id=@Id", new { Id = id },
                                 commandType: CommandType.Text, commandTimeout: 500));
            return value;
        }

        public async Task InsertBankRate(TimeBot.Model.Settings settings)
        {
            SetupBankRate();
            List<BankRate> bankRates = new List<BankRate>();
            BankServices bankServices = new BankServices();

            foreach (var prop in settings.UrlAddresses.UrlBanks.GetType().GetProperties())
            {
                string url = prop.GetValue(settings.UrlAddresses.UrlBanks).ToString();
                BankRate bankRate = new BankRate();
                bankRate = await bankServices.GetBankRates(url);

                bool tekshir = false;
                var maxId = _db.Query<int>("select ifnull(max(Id),0) from BankRate where BankName=@BankName", new { BankName = bankRate.BankName });
                foreach (var itemId in maxId)
                {
                    var bank = _db.Query<BankRate>("select * from BankRate where Id=@Id", new { Id = itemId });

                    foreach (var item in bank)
                    {
                        if (item.BuyRate == bankRate.BuyRate && item.SelRate == bankRate.SelRate)
                        {
                            tekshir = true;
                        }
                    }
                }

                if (!string.IsNullOrEmpty(bankRate.SelRate) && !string.IsNullOrEmpty(bankRate.BuyRate))
                {
                    if (!tekshir)
                    {
                        string processQuery = "INSERT INTO BankRate VALUES ((IFNULL((SELECT MAX(Id) FROM BankRate),0) + 1),@BankName, @BuyRate,@SelRate,@UpdateTime)";
                        await _db.ExecuteAsync(processQuery, new { BankName = bankRate.BankName, BuyRate = bankRate.BuyRate, SelRate = bankRate.SelRate, UpdateTime = bankRate.UpdateTime });
                    }
                }
            }
        }
        public void SetupBankRate()
        {

            var table = _db.Query<string>("SELECT name FROM sqlite_master WHERE type='table' AND name = 'BankRate';");
            var tableName = table.FirstOrDefault();
            if (!string.IsNullOrEmpty(tableName) && tableName == "BankRate")
                return;

            _db.Execute("Create Table BankRate (" +
                "Id int ," +
                "BankName VARCHAR(50) ," +
                "BuyRate VARCHAR(50) ," +
                "SelRate VARCHAR(50) ," +
                "UpdateTime VARCHAR(50) );");
        }

        public async Task InsertRegionRate(TimeBot.Model.Settings settings)
        {
            var today = DateTime.Today;
            var yesterday = today.AddDays(-1);


            var url = settings.UrlAddresses.UrlRegionCurrencyRate;
            SetupRegionRate();
            HtmlWeb web = new HtmlWeb();
            HtmlDocument document = await web.LoadFromWebAsync(url);
            HtmlNode myTable = document.DocumentNode.Descendants("table")
                .Where(table => table.Attributes.Contains("id"))
                .SingleOrDefault(table => table.Attributes["id"].Value == "at_121");

            IEnumerable<string[]> tableHudud = document.DocumentNode.SelectNodes("//table[@id='at_121']")
            .Descendants("tr").Select(n => n.Elements("td").Select(e => e.InnerText).ToArray());
            RegionRate regionRate;
            bool tekshir = false;
            if (tableHudud.Any())
            {
                foreach (string[] item in tableHudud)
                {
                    var maxId = _db.Query<int>("select ifnull(max(Id),0) from RegionRate where RegionName=@RegionName", new { RegionName = item[0] });
                    foreach (var itemId in maxId)
                    {
                        var RegionRates = _db.Query<RegionRate>("select * from RegionRate where Id=@Id", new { Id = itemId });
                        foreach (var itemRegion in RegionRates)
                        {
                            if (itemRegion.BuyRate == item[1] && itemRegion.SelRate == item[2])
                            {
                                tekshir = true;
                            }

                        }
                    }
                    if (!string.IsNullOrEmpty(item[1]) && !string.IsNullOrEmpty(item[2]))
                    {
                        if (!tekshir)
                        {
                            regionRate = new RegionRate { RegionName = item[0], BuyRate = item[1], SelRate = item[2], UpdateTime = item[3] };
                            string processQuery = "INSERT INTO RegionRate VALUES (((IFNULL((SELECT MAX(Id) FROM RegionRate),0) + 1)),@RegionName, @BuyRate,@SelRate,@UpdateTime)";
                            await _db.ExecuteAsync(processQuery, new { RegionName = item[0], BuyRate = item[1], SelRate = item[2], UpdateTime = item[3] });
                        }
                    }

                }
            }
        }
        public void SetupRegionRate()
        {

            var table = _db.Query<string>("SELECT name FROM sqlite_master WHERE type='table' AND name = 'RegionRate';");
            var tableName = table.FirstOrDefault();
            if (!string.IsNullOrEmpty(tableName) && tableName == "RegionRate")
                return;

            _db.Execute("Create Table RegionRate (" +
                "Id int ," +
                "RegionName VARCHAR(50) ," +
                "BuyRate VARCHAR(50) ," +
                "SelRate VARCHAR(50) ," +
                "UpdateTime VARCHAR(50) );");
        }

        public async Task InsertMarketRate(TimeBot.Model.Settings settings)
        {
            SetupMarketRate();
            try
            {
                var url = settings.UrlAddresses.UrlRegionCurrencyRate;
                HtmlWeb web = new HtmlWeb();
                HtmlDocument document = await web.LoadFromWebAsync(url);
                bool tekshir = false;
                string nameBm = document.DocumentNode.SelectSingleNode("//div[@id='grid']/div[1]/h1[1]").InnerText.ToString().Replace("\r", "").Replace("\t", "").Replace("\n", "").Trim();
                string buyBm = document.DocumentNode.SelectSingleNode("//div[@id='grid']/div[1]/table[1]/tr[2]/td[1]").InnerText.ToString().Replace("\r", "").Replace("\t", "").Replace("\n", "").Trim();
                string selBm = document.DocumentNode.SelectSingleNode("//div[@id='grid']/div[1]/table[1]/tr[2]/td[2]").InnerText.ToString().Replace("\r", "").Replace("\t", "").Replace("\n", "").Trim();

                var NameMarket = _db.Query<string>("select ifnull([Name],0) from MarketRate");
                foreach (var itemName in NameMarket.Distinct())
                {
                    var maxId = _db.Query<int>("select max(Id) from MarketRate where [Name]=@Name", new { Name = itemName });
                    foreach (var itemId in maxId)
                    {
                        var marketRateTest = _db.Query<MarketRate>("select * from MarketRate where Id=@Id", new { Id = itemId });
                        foreach (var item1 in marketRateTest)
                        {
                            if (item1.BuyRate == buyBm && item1.SelRate == selBm)
                            {
                                tekshir = true;
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(buyBm) && !string.IsNullOrEmpty(buyBm))
                    {
                        if (!tekshir)
                        {
                            string processQuery = "INSERT INTO MarketRate VALUES (((IFNULL((SELECT MAX(Id) FROM MarketRate),0) + 1)),@Name, @BuyRate,@SelRate,@UpdateTime)";
                            await _db.ExecuteAsync(processQuery, new { Name = nameBm, BuyRate = buyBm, SelRate = selBm, UpdateTime = DateTime.Now.ToString("dd.MM.yyyy HH:mm") });
                        }
                    }

                }
                if (NameMarket.Count() == 0)
                {
                    string processQuery = "INSERT INTO MarketRate VALUES (((IFNULL((SELECT MAX(Id) FROM MarketRate),0) + 1)),@Name, @BuyRate,@SelRate,@UpdateTime)";
                    await _db.ExecuteAsync(processQuery, new { Name = nameBm, BuyRate = buyBm, SelRate = selBm, UpdateTime = DateTime.Now.ToString("dd.MM.yyyy HH:mm") });




                }
            }
            catch (Exception)
            {
            }
        }
        public void SetupMarketRate()
        {

            var table = _db.Query<string>("SELECT name FROM sqlite_master WHERE type='table' AND name = 'MarketRate';");
            var tableName = table.FirstOrDefault();
            if (!string.IsNullOrEmpty(tableName) && tableName == "MarketRate")
                return;

            _db.Execute("Create Table MarketRate (" +
                "Id int ," +
                "Name VARCHAR(50) ," +
                "BuyRate VARCHAR(50) ," +
                "SelRate VARCHAR(50) ," +
                "UpdateTime VARCHAR(50) );");
        }

        public async Task InsertCbuRate(TimeBot.Model.Settings settings)
        {
            SetupCbuRate();
            StateViewModel<List<CbuRate>> stateViewModel = new StateViewModel<List<CbuRate>>();
            bool tekshir = false;
            var today = DateTime.Today;
            var yesterday = today.AddDays(-1);
            try
            {
                var url = settings.UrlAddresses.UrlCbCurrencyRate;
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();

                    HttpResponseMessage response = await client.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        List<CbuRate> rates = await response.Content.ReadFromJsonAsync<List<CbuRate>>();
                        var KursNames = _db.Query<string>("select ifnull(CcyNm_UZ,'') from CbuRate");
                        if (KursNames.Count() == 0)
                        {
                            foreach (var item in rates)
                            {
                                string processQuery = "INSERT INTO CbuRate VALUES (((IFNULL((SELECT MAX(Id) FROM CbuRate),0) + 1)),@CcyNm_UZ, @Rate,@Diff,@Date)";
                                await _db.ExecuteAsync(processQuery, new { CcyNm_UZ = item.CcyNm_UZ, Rate = item.Rate, Diff = item.Diff, Date = item.Date });

                            }

                        }
                        foreach (var itemName in KursNames.Distinct())
                        {
                            var maxId = _db.Query<int>("select max(Id) from CbuRate where CcyNm_UZ=@CcyNm_UZ", new { CcyNm_UZ = itemName });
                            foreach (var itemId in maxId)
                            {
                                var marketRates = _db.Query<CbuRate>("select * from CbuRate where Id=@Id", new { Id = itemId });
                                foreach (var item in marketRates)
                                {
                                    var CbuRates = rates.FirstOrDefault(i => i.CcyNm_UZ == item.CcyNm_UZ);

                                    if (item.Rate == CbuRates.Rate && item.Diff == CbuRates.Diff)
                                    {
                                        tekshir = true;
                                    }
                                    try
                                    {
                                        if (!tekshir)
                                        {
                                            string processQuery = "INSERT INTO CbuRate VALUES (((IFNULL((SELECT MAX(Id) FROM CbuRate),0) + 1)),@CcyNm_UZ, @Rate,@Diff,@Date)";
                                            await _db.ExecuteAsync(processQuery, new { CcyNm_UZ = CbuRates.CcyNm_UZ, Rate = CbuRates.Rate, Diff = CbuRates.Diff, Date = CbuRates.Date });
                                        }

                                    }
                                    catch (Exception)
                                    {

                                    }
                                }

                            }

                        }


                    }
                }
            }
            catch (Exception)
            {

            }
        }
        public void SetupCbuRate()
        {
            var table = _db.Query<string>("SELECT name FROM sqlite_master WHERE type='table' AND name = 'CbuRate';");
            var tableName = table.FirstOrDefault();
            if (!string.IsNullOrEmpty(tableName) && tableName == "CbuRate")
                return;

            _db.Execute("Create Table CbuRate (" +
                "Id int ," +
                "CcyNm_UZ VARCHAR(50) ," +
                "Rate VARCHAR(50) ," +
                "Diff VARCHAR(50) ," +
                "Date VARCHAR(50) );");
        }


        public async Task InsertBirjaRate(TimeBot.Model.Settings settings)
        {
            SetupBirjaRate();
            try
            {
                var url = settings.UrlAddresses.UrlBirjaCurrencyRate;
                HtmlWeb web = new HtmlWeb();
                HtmlDocument document = await web.LoadFromWebAsync(url);

                bool tekshir = false;

                HtmlNode nameUsd1 = document.DocumentNode.SelectSingleNode(@"//p[contains(text(), 'USD')]");

                HtmlNode nameEur1 = document.DocumentNode.SelectSingleNode(@"//p[contains(text(), 'EUR')]");

                HtmlNode RateUsd1 = document.DocumentNode.SelectSingleNode(@"//p[contains(text(), '11150.12')]");

                HtmlNode difUsd1 = document.DocumentNode.SelectSingleNode(@"//p[contains(text(), '+117.09')]");

                HtmlNode RateEur1 = document.DocumentNode.SelectSingleNode(@"//p[contains(text(), '12034')]");

                HtmlNode diffEur1 = document.DocumentNode.SelectSingleNode(@"//p[contains(text(), '-166.00')]");

                string NameUSD = document.DocumentNode.SelectSingleNode("/html[1]/body[1]/div[1]/main[1]/div[1]/div[1]/aside[2]/div[1]/div[2]/div[1]/table[1]/tbody[1]/tr[1]/td[1]/p[1]").InnerText.ToString();
                string NameEUR = document.DocumentNode.SelectSingleNode("/html[1]/body[1]/div[1]/main[1]/div[1]/div[1]/aside[2]/div[1]/div[2]/div[1]/table[1]/tbody[1]/tr[2]/td[1]/p[1]").InnerText.ToString();
                string RateUSD = document.DocumentNode.SelectSingleNode("/html[1]/body[1]/div[1]/main[1]/div[1]/div[1]/aside[2]/div[1]/div[2]/div[1]/table[1]/tbody[1]/tr[1]/td[2]/div[1]/div[1]/div[2]/span[1]/p[1]").InnerText.ToString();
                string DiffUSD = document.DocumentNode.SelectSingleNode("/html[1]/body[1]/div[1]/main[1]/div[1]/div[1]/aside[2]/div[1]/div[2]/div[1]/table[1]/tbody[1]/tr[1]/td[2]/div[1]/div[1]/div[2]/span[1]/p[2]").InnerText.ToString().Replace("\r", "").Replace("\t", "").Replace("\n", "").Trim();
                string RateEUR = document.DocumentNode.SelectSingleNode("/html[1]/body[1]/div[1]/main[1]/div[1]/div[1]/aside[2]/div[1]/div[2]/div[1]/table[1]/tbody[1]/tr[2]/td[2]/div[1]/div[1]/div[2]/span[1]/p[1]").InnerText.ToString();
                string DiffEUR = document.DocumentNode.SelectSingleNode("/html[1]/body[1]/div[1]/main[1]/div[1]/div[1]/aside[2]/div[1]/div[2]/div[1]/table[1]/tbody[1]/tr[2]/td[2]/div[1]/div[1]/div[2]/span[1]/p[2]").InnerText.ToString().Replace("\r", "").Replace("\t", "").Replace("\n", "").Trim();

                var BirjaNames = _db.Query<string>("select ifnull([Name],'') from BirjaRate");
                foreach (var itemName in BirjaNames.Distinct())
                {
                    if (itemName == "USD")
                    {
                        var maxId = _db.Query<int>("select max(Id) from BirjaRate where [Name]=@Name", new { Name = NameUSD });
                        foreach (var itemId in maxId)
                        {
                            var BirjaRateTest = _db.Query<BirjaRate>("select * from BirjaRate where Id=@Id", new { Id = itemId });
                            foreach (var item1 in BirjaRateTest)
                            {
                                if (item1.Rate == RateUSD && item1.Diff == DiffUSD)
                                {
                                    tekshir = true;
                                }
                            }
                        }
                        if (!string.IsNullOrEmpty(RateUSD) && !string.IsNullOrEmpty(DiffUSD))
                        {
                            if (!tekshir)
                            {
                                string processQuery = "INSERT INTO BirjaRate VALUES (((IFNULL((SELECT MAX(Id) FROM BirjaRate),0) + 1)),@Name, @Rate,@Diff,@UpdateTime)";
                                await _db.ExecuteAsync(processQuery, new { Name = NameUSD, Rate = RateUSD, Diff = DiffUSD, UpdateTime = DateTime.Now.ToString("dd.MM.yyyy HH:mm") });
                            }
                        }
                        tekshir = false;
                    }

                    if (itemName == "EUR")
                    {
                        var maxId = _db.Query<int>("select max(Id) from BirjaRate where [Name]=@Name", new { Name = NameEUR });
                        foreach (var itemId in maxId)
                        {
                            var BirjaRateTest = _db.Query<BirjaRate>("select * from BirjaRate where Id=@Id", new { Id = itemId });
                            foreach (var item1 in BirjaRateTest)
                            {
                                if (item1.Rate == RateEUR && item1.Diff == DiffEUR)
                                {
                                    tekshir = true;
                                }
                            }
                        }
                        if (!string.IsNullOrEmpty(RateEUR) && !string.IsNullOrEmpty(DiffEUR))
                        {
                            if (!tekshir)
                            {
                                string processQuery = "INSERT INTO BirjaRate VALUES (((IFNULL((SELECT MAX(Id) FROM BirjaRate),0) + 1)),@Name, @Rate,@Diff,@UpdateTime)";
                                await _db.ExecuteAsync(processQuery, new { Name = NameEUR, Rate = RateEUR, Diff = DiffEUR, UpdateTime = DateTime.Now.ToString("dd.MM.yyyy HH:mm") });
                            }
                        }
                        tekshir = false;
                    }
                }

                if (BirjaNames.Count() == 0)
                {
                    string processQuery1 = "INSERT INTO BirjaRate VALUES (((IFNULL((SELECT MAX(Id) FROM BirjaRate),0) + 1)),@Name, @Rate,@Diff,@UpdateTime)";
                    await _db.ExecuteAsync(processQuery1, new { Name = NameUSD, Rate = RateUSD, Diff = DiffUSD, UpdateTime = DateTime.Now.ToString("dd.MM.yyyy HH:mm") });
                    string processQuery2 = "INSERT INTO BirjaRate VALUES (((IFNULL((SELECT MAX(Id) FROM BirjaRate),0) + 1)),@Name, @Rate,@Diff,@UpdateTime)";
                    await _db.ExecuteAsync(processQuery2, new { Name = NameEUR, Rate = RateEUR, Diff = DiffEUR, UpdateTime = DateTime.Now.ToString("dd.MM.yyyy HH:mm") });

                }
            }
            catch (Exception)
            {
            }
        }
        public void SetupBirjaRate()
        {
            var table = _db.Query<string>("SELECT name FROM sqlite_master WHERE type='table' AND name = 'BirjaRate';");
            var tableName = table.FirstOrDefault();
            if (!string.IsNullOrEmpty(tableName) && tableName == "BirjaRate")
                return;

            _db.Execute("Create Table BirjaRate (" +
                "Id int ," +
                "Name VARCHAR(50) ," +
                "Rate VARCHAR(50) ," +
                "Diff VARCHAR(50) ," +
                "UpdateTime VARCHAR(50) );");
        }

        public List<long> GetUsersId()
        {
            List<long> TelegramId = new List<long>();
            var users = _db.Query<string>("select UserId from User");
            foreach (var item in users)
            {
                TelegramId.Add(long.Parse(item));
            }
            return TelegramId;
        }

        public async Task InsertWeatherDaily(TimeBot.Model.Settings settings)
        {
            SetupWeatherDaily();
            try
            {
                string url = settings.UrlAddresses.UrlWeather.Daily.Toshkent;
                HtmlWeb web = new HtmlWeb();
                HtmlDocument document = await web.LoadFromWebAsync(url);
                //<p class="days-temp">11 °c</p>//*[@id="aspnetForm"]/section[4]/div/div/div[1]/div[1]/div[1]/div/div[2]/table/tbody/tr[3]/td[1]/p[2]
                HtmlNode temperature1 = document.DocumentNode.SelectSingleNode(@"//p[contains(text(), '11 °c')]");
                string nameBm = document.DocumentNode.SelectSingleNode("//form[@id='aspnetForm']/section[4]/div[1]/div[1]/div[1]/div[1]/div[1]/div[1]/div[2]/table[1]/tbody[1]/tr[3]/td[1]/p[2]").InnerText;
                HtmlNode wind1 = document.DocumentNode.SelectSingleNode(@"//p[contains(text(), 'Weather today in Tashkent, Uzbekistan is turning out to be')]");
                HtmlNode Precipitation1 = document.DocumentNode.SelectSingleNode(@"//span[contains(text(), 2%')]");
                HtmlNode weathername1 = document.DocumentNode.SelectSingleNode(@"//div[contains(text(), 'Clear')]");
                HtmlNode humdinity1 = document.DocumentNode.SelectSingleNode(@"//div[contains(text(), '50%')]");
                HtmlNode temperature2 = document.DocumentNode.SelectSingleNode(@"//div[contains(text(), '4°')]");
                //string nameBm = document.DocumentNode.SelectSingleNode("//div[@id='grid']/div[1]/h1[1]").InnerText.ToString().Replace("\r", "").Replace("\t", "").Replace("\n", "").Trim();
                string buyBm = document.DocumentNode.SelectSingleNode("//div[@id='grid']/div[1]/table[1]/tr[2]/td[1]").InnerText.ToString().Replace("\r", "").Replace("\t", "").Replace("\n", "").Trim();
                string selBm = document.DocumentNode.SelectSingleNode("//div[@id='grid']/div[1]/table[1]/tr[2]/td[2]").InnerText.ToString().Replace("\r", "").Replace("\t", "").Replace("\n", "").Trim();

                //var NameMarket = _db.Query<string>("select ifnull([Name],0) from MarketRate");
                //foreach (var itemName in NameMarket.Distinct())
                //{
                //    var maxId = _db.Query<int>("select max(Id) from MarketRate where [Name]=@Name", new { Name = itemName });
                //    foreach (var itemId in maxId)
                //    {
                //        var marketRateTest = _db.Query<MarketRate>("select * from MarketRate where Id=@Id", new { Id = itemId });
                //        foreach (var item1 in marketRateTest)
                //        {
                //            if (item1.BuyRate == buyBm && item1.SelRate == selBm)
                //            {
                //                tekshir = true;
                //            }
                //        }
                //    }
                //    if (!string.IsNullOrEmpty(buyBm) && !string.IsNullOrEmpty(buyBm))
                //    {
                //        if (!tekshir)
                //        {
                //            string processQuery = "INSERT INTO MarketRate VALUES (((IFNULL((SELECT MAX(Id) FROM MarketRate),0) + 1)),@Name, @BuyRate,@SelRate,@UpdateTime)";
                //            await _db.ExecuteAsync(processQuery, new { Name = nameBm, BuyRate = buyBm, SelRate = selBm, UpdateTime = DateTime.Now.ToString("dd.MM.yyyy HH:mm") });
                //        }
                //    }

                //}
                //if (NameMarket.Count() == 0)
                //{
                //    string processQuery = "INSERT INTO MarketRate VALUES (((IFNULL((SELECT MAX(Id) FROM MarketRate),0) + 1)),@Name, @BuyRate,@SelRate,@UpdateTime)";
                //    await _db.ExecuteAsync(processQuery, new { Name = nameBm, BuyRate = buyBm, SelRate = selBm, UpdateTime = DateTime.Now.ToString("dd.MM.yyyy HH:mm") });

                //}

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
        public void SetupWeatherDaily()
        {
            var table = _db.Query<string>("SELECT name FROM sqlite_master WHERE type='table' AND name = 'WeatherDaily';");
            var tableName = table.FirstOrDefault();
            if (!string.IsNullOrEmpty(tableName) && tableName == "WeatherDaily")
                return;

            _db.Execute("Create Table WeatherDaily (" +
                "Id int ," +
                "Time varchar(50)," +
                "RegionName VARCHAR(50) ," +
                "Tempureture VARCHAR(50) ," +
                "Humidity VARCHAR(50) ," +
                "Precipitation VARCHAR(50) ," +
                "Wind VARCHAR(50) ," +
                "Weather_Name VARCHAR(50) ," +
                "Date VARCHAR(50) );");
        }

        public int GetMaxId(string name)
        {
            string query = "select ifnull(max(Id),0) from " + name + "";
            var id = _db.QuerySingleOrDefault<int>(query);
            return id;
        }


    }
}