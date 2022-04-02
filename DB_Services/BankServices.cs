using HtmlAgilityPack;
using RestSharp;
using System.Net.Http.Json;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using JsonReader = JsonFx.Json.JsonReader;
using JsonFx.Serialization;
using JsonFx.Serialization.Resolvers;
using System.Linq;
using System.Net.Http.Headers;
using TimeBot.ViewModel;
using ViewModel;

namespace WebClientServices.Services
{
    public class BankServices
    {
        public async Task<BankRate> GetBankRates(string url)
        {
            BankRate bankRate = new BankRate();

            if (url.Contains(BanksEnum.InfinBank.ToString().ToLower()))
                bankRate = await GetInfinBankRate(url);
            else if (url.Contains(BanksEnum.AnorBank.ToString().ToLower()))
                bankRate = await GetAnorBankRate(url);
            else if (url.Contains(BanksEnum.Trustbank.ToString().ToLower()))
                bankRate = await GetTrustBankRate(url);
            else if (url.Contains(BanksEnum.AAB.ToString().ToLower()))
                bankRate = await GetAsiaAllianceBankRate(url);
            else if (url.Contains(BanksEnum.XB.ToString().ToLower()))
                bankRate = await GetXalqBankRate(url);
            else if (url.Contains(BanksEnum.UniversalBank.ToString().ToLower()))
                bankRate = await GetUniversalBankRate(url);
            else if (url.Contains(BanksEnum.AloqaBank.ToString().ToLower()))
                bankRate = await GetAloqaBankRate(url);
            else if (url.Contains(BanksEnum.HamkorBank.ToString().ToLower()))
                bankRate = await GetHamkorBankRate(url);
            else if (url.Contains(BanksEnum.SQB.ToString().ToLower()))
                bankRate = await GetSQBBankRate(url);
            else if (url.Contains(BanksEnum.KapitalBank.ToString().ToLower()))
                bankRate = await GetKapitalBankRate(url);
            else if (url.Contains(BanksEnum.IpakYuliBank.ToString().ToLower()))
                bankRate = await GetIpakYuliBankRate(url);
            else if (url.Contains(BanksEnum.AgroBank.ToString().ToLower()))
                bankRate = await GetAgroBankRate(url);
            else if (url.Contains(BanksEnum.IpotekaBank.ToString().ToLower()))
                bankRate = await GetIpotekaBankRate(url);
            //if (url.Contains(BanksEnum.DavrBank.ToString().ToLower()))   //to'g'irla
            //    bankRate = await GetDavrBankRate(url);
            else if (url.Contains(BanksEnum.NBU.ToString().ToLower()))
                bankRate = await GetNBURate(url);
            else if (url.Contains(BanksEnum.Mikrokreditbank.ToString().ToLower()))
                bankRate = await GetMikrokreditbankRate(url);
            else if (url.Contains(BanksEnum.ZiraatBank.ToString().ToLower()))
                bankRate = await GetZiraatBankRate(url);
            else if (url.Contains(BanksEnum.AsakaBank.ToString().ToLower()))
                bankRate = await GetAsakaBankRate(url);
            else if (url.Contains(BanksEnum.RavnaqBank.ToString().ToLower()))
                bankRate = await GetRavnaqBankRate(url);
            else if (url.Contains("qishloqqurilishbank"))
                bankRate = await GetQQBRate(url);
            else if (url.Contains(BanksEnum.Savdogarbank.ToString().ToLower()))
                bankRate = await GetSavdogarbankRate(url);
            else if (url.Contains("ofb"))
                bankRate = await GetOrientFinansBankRate(url);
            else if (url.Contains(BanksEnum.TuronBank.ToString().ToLower()))
                bankRate = await GetTuronBankRate(url);
            else if (url.Contains(BanksEnum.TengeBank.ToString().ToLower()))
                bankRate = await GetTengeBankRate(url);


            return bankRate;
        }

        public async Task<BankRate> GetTengeBankRate(string url)
        {
            BankRate bankRate = new BankRate();
            try
            {
                HtmlWeb web = new HtmlWeb();
                HtmlDocument document = await web.LoadFromWebAsync(url);

                string nameBm = "Tenge Bank";

                HtmlNode sell1 = document.DocumentNode.SelectSingleNode(@"//div[contains(text(), '10890')]");
                HtmlNode buy1 = document.DocumentNode.SelectSingleNode(@"//div[contains(text(), '10820')]");
                string buyBm = document.DocumentNode.SelectSingleNode("/html[1]/body[1]/div[1]/div[1]/main[1]/section[2]/div[1]/div[2]/div[1]/div[1]/div[1]/div[2]/div[1]").InnerText.Trim().Remove(0, 12).Substring(0, 5);
                string sellBm = document.DocumentNode.SelectSingleNode("/html[1]/body[1]/div[1]/div[1]/main[1]/section[2]/div[1]/div[2]/div[1]/div[1]/div[1]/div[2]/div[2]").InnerText.Trim().Remove(0, 7).Substring(0, 5);

                bankRate = new BankRate { BankName = nameBm, BuyRate = buyBm, SelRate = sellBm, UpdateTime = DateTime.Now.ToString("dd.MM.yyyy HH:mm") };

            }
            catch (Exception)
            {
                return bankRate;
            }
            return bankRate;
        }

        public async Task<BankRate> GetTuronBankRate(string url)
        {
            BankRate bankRate = new BankRate();
            try
            {
                HtmlWeb web = new HtmlWeb();
                HtmlDocument document = await web.LoadFromWebAsync(url);

                string nameBm = "Turon Bank";

                HtmlNode sell1 = document.DocumentNode.SelectSingleNode(@"//td[contains(text(), '10890')]");
                HtmlNode buy1 = document.DocumentNode.SelectSingleNode(@"//td[contains(text(), '10820')]");
                string buyBm = document.DocumentNode.SelectSingleNode("/html[1]/body[1]/div[1]/div[4]/section[2]/div[2]/div[1]/div[2]/div[1]/div[1]/div[2]/div[1]/table[1]/tr[2]/td[1]").InnerText.Trim().Substring(0, 5);
                string sellBm = document.DocumentNode.SelectSingleNode("/html[1]/body[1]/div[1]/div[4]/section[2]/div[2]/div[1]/div[2]/div[1]/div[1]/div[2]/div[1]/table[1]/tr[3]/td[1]").InnerText.Trim().Substring(0, 5);

                bankRate = new BankRate { BankName = nameBm, BuyRate = buyBm, SelRate = sellBm, UpdateTime = DateTime.Now.ToString("dd.MM.yyyy HH:mm") };

            }
            catch (Exception)
            {
                return bankRate;
            }
            return bankRate;
        }

        public async Task<BankRate> GetOrientFinansBankRate(string url)
        {
            BankRate bankRate = new BankRate();
            try
            {
                HtmlWeb web = new HtmlWeb();
                HtmlDocument document = await web.LoadFromWebAsync(url);

                string nameBm = "Orient Finans Bank";

                HtmlNode sell1 = document.DocumentNode.SelectSingleNode(@"//td[contains(text(), '10890.00')]");
                HtmlNode buy1 = document.DocumentNode.SelectSingleNode(@"//td[contains(text(), '10780.00')]");
                string buyBm = document.DocumentNode.SelectSingleNode("/html[1]/body[1]/div[3]/main[1]/div[1]/div[1]/div[1]/div[2]/div[2]/div[1]/table[1]/tbody[1]/tr[1]/td[3]").InnerText.Trim().Substring(0, 5);
                string sellBm = document.DocumentNode.SelectSingleNode("/html[1]/body[1]/div[3]/main[1]/div[1]/div[1]/div[1]/div[2]/div[2]/div[1]/table[1]/tbody[1]/tr[1]/td[4]").InnerText.Trim().Substring(0, 5);

                bankRate = new BankRate { BankName = nameBm, BuyRate = buyBm, SelRate = sellBm, UpdateTime = DateTime.Now.ToString("dd.MM.yyyy HH:mm") };

            }
            catch (Exception)
            {
                return bankRate;
            }
            return bankRate;
        }

        public async Task<BankRate> GetSavdogarbankRate(string url)
        {
            BankRate bankRate = new BankRate();
            try
            {
                HtmlWeb web = new HtmlWeb();
                HtmlDocument document = await web.LoadFromWebAsync(url);

                string nameBm = "Savdogar Bank";

                HtmlNode sell1 = document.DocumentNode.SelectSingleNode(@"//td[contains(text(), '11570,00')]");
                HtmlNode buy1 = document.DocumentNode.SelectSingleNode(@"//td[contains(text(), '11630,00')]");
                string buyBm = document.DocumentNode.SelectSingleNode("/html[1]/body[1]/main[1]/div[1]/div[2]/div[1]/div[3]/fieldset[1]/div[1]/table[1]/tr[3]/td[2]").InnerText.Trim().Substring(0, 5);
                string sellBm = document.DocumentNode.SelectSingleNode("/html[1]/body[1]/main[1]/div[1]/div[2]/div[1]/div[3]/fieldset[1]/div[1]/table[1]/tr[3]/td[3]").InnerText.Trim().Substring(0, 5);

                bankRate = new BankRate { BankName = nameBm, BuyRate = buyBm, SelRate = sellBm, UpdateTime = DateTime.Now.ToString("dd.MM.yyyy HH:mm") };

            }
            catch (Exception)
            {
                return bankRate;
            }
            return bankRate;
        }

        public async Task<BankRate> GetQQBRate(string url)
        {
            BankRate bankRate = new BankRate();
            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Accept
                  .Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var response = client.GetAsync(string.Format(url)).Result;

                if (response.IsSuccessStatusCode)
                {

                    var result1 = await response.Content.ReadAsStringAsync();
                    QQBRate rates = JsonConvert.DeserializeObject<QQBRate>(result1);

                    bankRate = rates.data.currency_rate.currencies.Where(x => x.name == "USD")
                             .Select(x => new BankRate
                             {
                                 BankName = "QQB",
                                 BuyRate = x.buy_rate.ToString().Trim().Substring(0, 5),
                                 SelRate = x.sell_rate.ToString().Trim().Substring(0, 5),

                                 UpdateTime = DateTime.Now.ToString("dd.MM.yyyy HH:mm")
                             }).FirstOrDefault();
                }
            }
            catch (Exception)
            {
                return bankRate;
            }
            return bankRate;
        }

        public async Task<BankRate> GetRavnaqBankRate(string url)
        {
            BankRate bankRate = new BankRate();
            try
            {
                HtmlWeb web = new HtmlWeb();
                HtmlDocument document = await web.LoadFromWebAsync(url);

                string nameBm = "Ravnaq Bank";

                HtmlNode sell1 = document.DocumentNode.SelectSingleNode(@"//span[contains(text(), '10880')]");
                HtmlNode buy1 = document.DocumentNode.SelectSingleNode(@"//span[contains(text(), '10830')]");
                string buyBm = document.DocumentNode.SelectSingleNode("/html[1]/body[1]/div[1]/header[1]/div[1]/div[1]/div[1]/div[1]/div[1]/div[1]/div[1]/span[1]").InnerText.Trim();
                string sellBm = document.DocumentNode.SelectSingleNode("/html[1]/body[1]/div[1]/header[1]/div[1]/div[1]/div[1]/div[1]/div[1]/div[1]/div[1]/span[2]").InnerText.Trim();

                bankRate = new BankRate { BankName = nameBm, BuyRate = buyBm, SelRate = sellBm, UpdateTime = DateTime.Now.ToString("dd.MM.yyyy HH:mm") };

            }
            catch (Exception)
            {
                return bankRate;
            }
            return bankRate;
        }

        public async Task<BankRate> GetAsakaBankRate(string url)
        {
            BankRate bankRate = new BankRate();
            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Accept
                  .Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var response = client.GetAsync(string.Format("https://back.asakabank.uz/1/currency/type=asaka&currency_type=individual&page_size=6")).Result;

                if (response.IsSuccessStatusCode)
                {

                    var result1 = await response.Content.ReadAsStringAsync();
                    AsakaBankRate rates = JsonConvert.DeserializeObject<AsakaBankRate>(result1);

                    bankRate = rates.results.Where(x => x.name == "USD")
                             .Select(x => new BankRate
                             {
                                 BankName = "Asaka Bank",
                                 BuyRate = x.buy.ToString().Trim().Substring(0, 5),
                                 SelRate = x.sell.ToString().Trim().Substring(0, 5),

                                 UpdateTime = DateTime.Now.ToString("dd.MM.yyyy HH:mm")
                             }).FirstOrDefault();
                }
            }
            catch (Exception)
            {
                return bankRate;
            }
            return bankRate;
        }

        public async Task<BankRate> GetZiraatBankRate(string url)
        {
            BankRate bankRate = new BankRate();
            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Accept
                  .Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var response = client.GetAsync(string.Format("https://ziraatbank.uz/tr/GetCurrency")).Result;

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    IEnumerable<data1> rates = JsonConvert.DeserializeObject<IEnumerable<data1>>(result);

                    bankRate = rates.Where(x => x.name == "USD")
                             .Select(x => new BankRate
                             {
                                 BankName = "Ziraat Bank",
                                 BuyRate = x.value.ToString().Trim(),
                                 SelRate = x.oldValue.ToString().Trim(),

                                 UpdateTime = DateTime.Now.ToString("dd.MM.yyyy HH:mm")
                             }).FirstOrDefault();
                }
            }
            catch (Exception)
            {
                return bankRate;
            }
            return bankRate;
        }

        public async Task<BankRate> GetMikrokreditbankRate(string url)
        {
            BankRate bankRate = new BankRate();
            try
            {
                HtmlWeb web = new HtmlWeb();
                HtmlDocument document = await web.LoadFromWebAsync(url);

                string nameBm = "Mikrokreditbank";

                HtmlNode sell1 = document.DocumentNode.SelectSingleNode(@"//td[contains(text(), '10900.00')]");
                HtmlNode buy1 = document.DocumentNode.SelectSingleNode(@"//td[contains(text(), '10830.00')]");
                string buyBm = document.DocumentNode.SelectSingleNode("/html[1]/body[1]/div[2]/div[2]/div[3]/div[2]/div[1]/div[1]/div[3]/div[1]/div[1]/div[2]/table[1]/tr[2]/td[2]").InnerText.Trim().Remove(4, 3);
                string sellBm = document.DocumentNode.SelectSingleNode("/html[1]/body[1]/div[2]/div[2]/div[3]/div[2]/div[1]/div[1]/div[3]/div[1]/div[1]/div[2]/table[1]/tr[2]/td[3]").InnerText.Trim().Remove(4, 3);

                bankRate = new BankRate { BankName = nameBm, BuyRate = buyBm, SelRate = sellBm, UpdateTime = DateTime.Now.ToString("dd.MM.yyyy HH:mm") };

            }
            catch (Exception)
            {
                return bankRate;
            }
            return bankRate;
        }

        public async Task<BankRate> GetNBURate(string url)
        {
            BankRate bankRate = new BankRate();
            try
            {
                HtmlWeb web = new HtmlWeb();
                HtmlDocument document = await web.LoadFromWebAsync(url);

                string nameBm = "NBU";

                HtmlNode sell1 = document.DocumentNode.SelectSingleNode(@"//td[contains(text(), '10900.00')]");
                HtmlNode buy1 = document.DocumentNode.SelectSingleNode(@"//td[contains(text(), '10830.00')]");
                string buyBm = document.DocumentNode.SelectSingleNode("/html[1]/body[1]/div[4]/div[1]/div[1]/div[1]/div[3]/div[1]/div[1]/div[1]/div[1]/div[1]/table[1]/tr[2]/td[2]").InnerText.Trim().Remove(4, 3);
                string sellBm = document.DocumentNode.SelectSingleNode("/html[1]/body[1]/div[4]/div[1]/div[1]/div[1]/div[4]/div[1]/div[1]/div[1]/div[1]/div[1]/table[1]/tr[2]/td[3]").InnerText.Trim().Remove(4, 3);

                bankRate = new BankRate { BankName = nameBm, BuyRate = buyBm, SelRate = sellBm, UpdateTime = DateTime.Now.ToString("dd.MM.yyyy HH:mm") };

            }
            catch (Exception)
            {
                return bankRate;
            }
            return bankRate;
        }

        public async Task<BankRate> GetDavrBankRate(string url)
        {
            BankRate bankRate = new BankRate();
            try
            {
                HtmlWeb web = new HtmlWeb();
                HtmlDocument document = await web.LoadFromWebAsync(url);
                //HtmlNode mainNode = document.DocumentNode.SelectSingleNode("//table[@class='rate__currency_table']/tr[2]");

                string nameBm = "Davr Bank";

                HtmlNode sell1 = document.DocumentNode.SelectSingleNode(@"//div[contains(text(), '10 890')]");
                HtmlNode buy1 = document.DocumentNode.SelectSingleNode(@"//div[contains(text(), '10 800')]");
                string buyBm = document.DocumentNode.SelectSingleNode("/html[1]/body[1]/div[4]/div[1]/div[1]/div[1]/div[3]/div[1]/div[1]/div[1]/div[1]/div[1]/table[1]/tr[2]/td[2]").InnerText.Trim().Remove(4, 3);
                string sellBm = document.DocumentNode.SelectSingleNode("/html[1]/body[1]/div[4]/div[1]/div[1]/div[1]/div[4]/div[1]/div[1]/div[1]/div[1]/div[1]/table[1]/tr[2]/td[3]").InnerText.Trim().Remove(4, 3);

                bankRate = new BankRate { BankName = nameBm, BuyRate = buyBm, SelRate = sellBm, UpdateTime = DateTime.Now.Date.ToString("dd.MM.yy HH:mm") };

            }
            catch (Exception)
            {
                return bankRate;
            }
            return bankRate;
        }

        public async Task<BankRate> GetIpotekaBankRate(string url)
        {
            BankRate bankRate = new BankRate();

            try
            {
                HtmlWeb web = new HtmlWeb();
                HtmlDocument document = await web.LoadFromWebAsync(url);
                //find tag by value //a[contains(text(), 'wake')]
                HtmlNode sell1 = document.DocumentNode.SelectSingleNode(@"//span[contains(text(), '10 910')]");
                HtmlNode buy1 = document.DocumentNode.SelectSingleNode(@"//span[contains(text(), '10 830')]");
                string buy = document.DocumentNode.SelectSingleNode("/html[1]/body[1]/div[3]/section[2]/div[2]/div[1]/div[1]/div[1]/div[1]/div[1]/div[2]/div[2]/div[2]/div[1]/span[1]").InnerText.Trim().Replace(" ", String.Empty).Substring(0, 5);
                string sell = document.DocumentNode.SelectSingleNode("/html[1]/body[1]/div[3]/section[2]/div[2]/div[1]/div[1]/div[1]/div[1]/div[1]/div[2]/div[2]/div[3]/div[1]/span[1]").InnerText.Trim().Replace(" ", String.Empty).Substring(0, 5);
                string nameBm = "Ipoteka Bank";
                bankRate = new BankRate { BankName = nameBm, BuyRate = buy, SelRate = sell, UpdateTime = DateTime.Now.ToString("dd.MM.yyyy HH:mm") };
            }
            catch (Exception)
            {
                return bankRate;
            }
            return bankRate;
        }

        public async Task<BankRate> GetAgroBankRate(string url)
        {
            BankRate bankRate = new BankRate();

            try
            {
                HtmlWeb web = new HtmlWeb();
                HtmlDocument document = await web.LoadFromWebAsync(url);
                //find tag by value
                HtmlNode sell1 = document.DocumentNode.SelectSingleNode(@"//td[.='10 915.00']");
                HtmlNode buy1 = document.DocumentNode.SelectSingleNode(@"//td[.='10 830.00']");
                string buy = document.DocumentNode.SelectSingleNode("/html[1]/body[1]/div[1]/main[1]/section[3]/div[1]/div[1]/div[1]/div[1]/div[2]/div[1]/table[1]/tr[2]/td[4]").InnerText.Trim().Replace(" ", String.Empty).Substring(0, 5);
                string sell = document.DocumentNode.SelectSingleNode("/html[1]/body[1]/div[1]/main[1]/section[3]/div[1]/div[1]/div[1]/div[1]/div[2]/div[1]/table[1]/tr[2]/td[3]").InnerText.Trim().Replace(" ", String.Empty).Substring(0, 5);
                string nameBm = "Agrobank";
                bankRate = new BankRate { BankName = nameBm, BuyRate = buy, SelRate = sell, UpdateTime = DateTime.Now.ToString("dd.MM.yyyy HH:mm") };
            }
            catch (Exception)
            {
                return bankRate;
            }
            return bankRate;
        }

        public async Task<BankRate> GetIpakYuliBankRate(string url)
        {
            BankRate bankRate = new BankRate();
            try
            {
                var client = new RestClient("https://ipakyulibank.uz:8888/webapi/physical/exchange-rates");
                //client.Timeout = -1;
                var request = new RestRequest("https://ipakyulibank.uz:8888/webapi/physical/exchange-rates", Method.Post);
                // client.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:96.0) Gecko/20100101 Firefox/96.0";
                request.AddHeader("Accept", "application/json, text/plain, */*");
                request.AddHeader("Accept-Language", "ru-RU,ru;q=0.8,en-US;q=0.5,en;q=0.3");
                request.AddHeader("Accept-Encoding", "gzip, deflate, br");
                request.AddHeader("X-AppRef", "/physical/valyuta-ayirboshlash/kurslar");
                request.AddHeader("X-AppKey", "blablakey");
                request.AddHeader("X-AppLang", "uz");
                request.AddHeader("Origin", "https://ipakyulibank.uz");
                request.AddHeader("Connection", "keep-alive");
                request.AddHeader("Referer", "https://ipakyulibank.uz/");
                request.AddHeader("Sec-Fetch-Dest", "empty");
                request.AddHeader("Sec-Fetch-Mode", "cors");
                request.AddHeader("Sec-Fetch-Site", "same-site");
                request.AddHeader("Content-Length", "0");
                request.AddHeader("Cookie", "idev-frontend=mbuacpmdg01qu3o3vf2mnseivr");
                RestResponse response = await client.ExecuteAsync(request);

                if (response.IsSuccessful)
                {
                    IYB rates = JsonConvert.DeserializeObject<IYB>(response.Content);

                    bankRate = new BankRate
                    {
                        BankName = "IpakYuliBank",
                        BuyRate = rates.data.USD.rates._5.Course.ToString().Trim().Substring(0, 5),
                        SelRate = rates.data.USD.rates._4.Course.ToString().Trim().Substring(0, 5),

                        UpdateTime = DateTime.Now.ToString("dd.MM.yyyy HH:mm")
                    };
                }
            }
            catch (Exception)
            {
                return bankRate;
            }
            return bankRate;
        }

        public async Task<BankRate> GetKapitalBankRate(string url)
        {
            BankRate bankRate = new BankRate();

            try
            {
                HtmlWeb web = new HtmlWeb();
                HtmlDocument document = await web.LoadFromWebAsync(url);
                //find tag by value
                //HtmlNode by = document.DocumentNode.SelectSingleNode(@"//span[.='10850']");
                //HtmlNode sl = document.DocumentNode.SelectSingleNode(@"//span[.='10880']");
                string buy = document.DocumentNode.SelectSingleNode("/html[1]/body[1]/div[4]/div[5]/div[1]/div[1]/div[1]/div[1]/div[1]/div[1]/div[2]/div[1]/div[1]/div[1]/div[1]/div[1]/div[2]/div[1]/span[1]").InnerText.Trim();
                string sell = document.DocumentNode.SelectSingleNode("/html[1]/body[1]/div[4]/div[5]/div[1]/div[1]/div[1]/div[1]/div[1]/div[1]/div[2]/div[1]/div[1]/div[1]/div[1]/div[1]/div[2]/div[2]/span[1]").InnerText.Trim();
                string nameBm = "Kapital Bank";
                bankRate = new BankRate { BankName = nameBm, BuyRate = buy, SelRate = sell, UpdateTime = DateTime.Now.ToString("dd.MM.yyyy HH:mm") };
            }
            catch (Exception)
            {
                return bankRate;
            }
            return bankRate;
        }

        public async Task<BankRate> GetSQBBankRate(string url)
        {
            BankRate bankRate = new BankRate();
            try
            {
                using (var client = new HttpClient())
                {


                    HttpResponseMessage response = await client.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        var test = response.Content;
                        SqbData rates = await response.Content.ReadFromJsonAsync<SqbData>();

                        bankRate = rates.data.online.Where(x => x.code == "USD")
                            .Select(x => new BankRate
                            {
                                BankName = "SQB",
                                BuyRate = x.buy.ToString().Trim(),
                                SelRate = x.sell.ToString().Trim(),
                                UpdateTime = DateTime.Now.ToString("dd.MM.yyyy HH:mm")
                            }).FirstOrDefault();

                    }
                }
            }
            catch (Exception)
            {
                return bankRate;
            }
            return bankRate;
        }

        public async Task<BankRate> GetHamkorBankRate(string url)
        {
            BankRate bankRate = new BankRate();

            try
            {
                HtmlWeb web = new HtmlWeb();
                HtmlDocument document = await web.LoadFromWebAsync(url);
                //find tag by value
                //HtmlNode sell1 = document.DocumentNode.SelectSingleNode(@"//span[.='10900']");
                string buy = document.DocumentNode.SelectSingleNode("/html[1]/body[1]/header[1]/div[1]/div[2]/div[1]/div[2]/div[1]/div[1]/div[1]/ul[1]/li[2]/a[1]/span[2]").InnerText.Trim();
                string sell = document.DocumentNode.SelectSingleNode("/html[1]/body[1]/header[1]/div[1]/div[2]/div[1]/div[2]/div[1]/div[1]/div[1]/ul[1]/li[3]/a[1]/span[2]").InnerText.Trim();
                string nameBm = "Hamkor Bank";
                bankRate = new BankRate { BankName = nameBm, BuyRate = buy, SelRate = sell, UpdateTime = DateTime.Now.ToString("dd.MM.yyyy HH:mm") };
            }
            catch (Exception)
            {
                return bankRate;
            }
            return bankRate;
        }

        public async Task<BankRate> GetAloqaBankRate(string url)
        {
            BankRate bankRate = new BankRate();

            try
            {
                HtmlWeb web = new HtmlWeb();
                HtmlDocument document = await web.LoadFromWebAsync(url);
                //find tag by value
                HtmlNode sell1 = document.DocumentNode.SelectSingleNode(@"//small[.='ATM'][1]");
                string buy = document.DocumentNode.SelectSingleNode("/html[1]/body[1]/div[2]/main[1]/div[1]/div[5]/section[1]/div[1]/div[2]/div[1]/table[1]/tr[2]/td[2]").InnerText.Trim().Replace(" ", String.Empty).Replace(".", String.Empty).Replace(",", String.Empty).Substring(0, 5);
                string sell = document.DocumentNode.SelectSingleNode("/html[1]/body[1]/div[2]/main[1]/div[1]/div[5]/section[1]/div[1]/div[2]/div[1]/table[1]/tr[2]/td[3]").InnerText.Trim().Replace(" ", String.Empty).Replace(".", String.Empty).Replace(",", String.Empty).Substring(0, 5);
                string nameBm = "Aloqa Bank";
                bankRate = new BankRate { BankName = nameBm, BuyRate = buy, SelRate = sell, UpdateTime = DateTime.Now.ToString("dd.MM.yyyy HH:mm") };
            }
            catch (Exception)
            {
                return bankRate;
            }
            return bankRate;
        }

        public async Task<BankRate> GetUniversalBankRate(string url)
        {
            BankRate bankRate = new BankRate();

            try
            {
                HtmlWeb web = new HtmlWeb();
                HtmlDocument document = await web.LoadFromWebAsync(url);
                //find tag by value
                //HtmlNode sell = document.DocumentNode.SelectSingleNode("//td[.='10900']");
                string buy = document.DocumentNode.SelectSingleNode("html[1]/body[1]/section[5]/div[1]/div[2]/div[1]/table[1]/tbody[1]/tr[1]/td[3]").InnerText.Trim();
                string sell = document.DocumentNode.SelectSingleNode("html[1]/body[1]/section[5]/div[1]/div[2]/div[1]/table[1]/tbody[1]/tr[1]/td[4]").InnerText.Trim();

                string nameBm = "Universal Bank";
                bankRate = new BankRate { BankName = nameBm, BuyRate = buy, SelRate = sell, UpdateTime = DateTime.Now.ToString("dd.MM.yyyy HH:mm") };
            }
            catch (Exception)
            {
                return bankRate;
            }
            return bankRate;
        }

        public async Task<BankRate> GetXalqBankRate(string url)
        {
            BankRate bankRate = new BankRate();
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();

                    HttpResponseMessage response = await client.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        List<XalqBankiRate> rates = await response.Content.ReadFromJsonAsync<List<XalqBankiRate>>();

                        bankRate = rates.Where(x => x.CODE == 840)
                            .Select(x => new BankRate
                            {
                                BankName = "Xalq Banki",
                                BuyRate = x.BUYING_RATE.Remove(x.BUYING_RATE.IndexOf('.')).Trim().Replace(" ", String.Empty),
                                SelRate = x.SELLING_RATE.Remove(x.SELLING_RATE.IndexOf('.')).Trim().Replace(" ", String.Empty),

                                UpdateTime = DateTime.Now.ToString("dd.MM.yyyy HH:mm")
                            }).FirstOrDefault();

                    }
                }
            }
            catch (Exception)
            {
                return bankRate;
            }
            return bankRate;
        }

        public async Task<BankRate> GetAsiaAllianceBankRate(string url)
        {
            BankRate bankRate = new BankRate();

            try
            {
                HtmlWeb web = new HtmlWeb();
                HtmlDocument document = await web.LoadFromWebAsync(url);
                HtmlNode mainNode = document.DocumentNode.SelectSingleNode("//div[@class='rates-list']/div[1]/div[1]");

                string nameBm = "AsiaAllianceBank";
                string buyBm = mainNode.SelectSingleNode("div[2]/div[2]/span[1]").InnerText.ToString().Trim();
                //string buyRate = buyBm.Remove(buyBm.IndexOf(','));
                string selBm = mainNode.SelectSingleNode("div[3]/div[2]/span[1]").InnerText.ToString().Trim();
                //string selRate = selBm.Remove(selBm.IndexOf(','));

                bankRate = new BankRate { BankName = nameBm, BuyRate = buyBm, SelRate = selBm, UpdateTime = DateTime.Now.ToString("dd.MM.yyyy HH:mm") };
            }
            catch (Exception)
            {
                return bankRate;
            }
            return bankRate;
        }

        public async Task<BankRate> GetTrustBankRate(string url)
        {
            BankRate bankRate = new BankRate();
            try
            {
                HtmlWeb web = new HtmlWeb();
                HtmlDocument document = await web.LoadFromWebAsync(url);
                HtmlNode mainNode = document.DocumentNode.SelectSingleNode("//table[@class='rate__currency_table']/tr[2]");

                string nameBm = "Trust Bank";
                string buyBm = mainNode.SelectSingleNode("td[2]/div[1]/span[1]").InnerText.ToString().Trim();
                string selBm = mainNode.SelectSingleNode("td[3]/div[1]/span[1]").InnerText.ToString().Trim();

                bankRate = new BankRate { BankName = nameBm, BuyRate = buyBm, SelRate = selBm, UpdateTime = DateTime.Now.ToString("dd.MM.yyyy HH:mm") };
            }
            catch (Exception)
            {
                return bankRate;
            }
            return bankRate;
        }

        public async Task<BankRate> GetAnorBankRate(string url)
        {
            BankRate bankRate = new BankRate();

            HtmlWeb web = new HtmlWeb();
            HtmlDocument document = await web.LoadFromWebAsync(url);
            HtmlNode mainNode = document.DocumentNode.SelectSingleNode("//div[@class='mini-exchange__list']/div[@class='mini-exchange__item']/ul[1]");

            string nameBm = "Anor Bank";
            string buyBm = mainNode.SelectSingleNode("li[1]/strong[1]").InnerText.ToString().Trim();
            string buyRate = buyBm.Remove(buyBm.IndexOf(',')).Replace(" ", String.Empty);
            string selBm = mainNode.SelectSingleNode("li[2]/strong[1]").InnerText.ToString().Trim();
            string selRate = selBm.Remove(selBm.IndexOf(',')).Replace(" ", String.Empty);

            bankRate = new BankRate { BankName = nameBm, BuyRate = buyRate, SelRate = selRate, UpdateTime = DateTime.Now.ToString("dd.MM.yyyy HH:mm") };
            return await Task.FromResult(bankRate);
        }

        public async Task<BankRate> GetInfinBankRate(string url)
        {
            BankRate bankRate = new BankRate();
            try
            {

                HtmlWeb web = new HtmlWeb();
                HtmlDocument document = await web.LoadFromWebAsync(url);
                HtmlNode ratesTable = document.DocumentNode.SelectSingleNode("//div[@class='rates-table']");
                IEnumerable<string[]> htmlNode1 = ratesTable.SelectSingleNode("//table").Descendants("tr").Select(n => n.Elements("td").Select(e => e.InnerText).ToArray());
                {
                    string nameBm = "Infin Bank";
                    string buyBm = htmlNode1.ElementAt(2).ElementAt(2);
                    string selBm = htmlNode1.ElementAt(3).ElementAt(1);

                    string buyRate = buyBm.Remove(buyBm.IndexOf('.')).Trim().Replace(" ", String.Empty);
                    string selRate = selBm.Remove(selBm.IndexOf('.')).Trim().Replace(" ", String.Empty);

                    bankRate = new BankRate { BankName = nameBm, BuyRate = buyRate, SelRate = selRate, UpdateTime = DateTime.Now.ToString("dd.MM.yyyy HH:mm") };
                }
            }
            catch (Exception)
            {
                return bankRate;
            }
            return bankRate;
        }
    }
}
