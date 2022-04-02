using System.Text;
using CoreHtmlToImage;
using DB_Services;
using Microsoft.Data.Sqlite;
using System.Data;
using Dapper;
using Newtonsoft.Json;
//using TimeBot.Model;
using TimeBot.ViewModel;

namespace HtmlToImage
{
    public interface IConvertHtml
    {
        Task<byte[]> ConvertRegionalRateForBot();
        Task<byte[]> ConvertBankRateForBot();
        Task<byte[]> ConvertMarketRateForBot();
        Task<byte[]> ConvertCbuRateForBot();
        Task<byte[]> ConvertBirjaRateForBot();

        Task<byte[]> ConvertRegionRateAsync(List<RegionRate> updatedList);
        Task<byte[]> ConvertMarketRateAsync(List<MarketRate> updatedList);
        Task<byte[]> ConvertCbuRateAsync(List<CbuRate> updatedList);
        Task<byte[]> ConvertBankRateAsync(List<BankRate> updatedList);
        Task<byte[]> ConvertBirjaRateAsync(List<BirjaRate> updatedList);
    }

    public class ConvertHtml : IConvertHtml
    {
        private readonly IDataBaseServices dataBaseServices;
        private static IDbConnection _db = new SqliteConnection();
        public ConvertHtml(IDataBaseServices _dataBaseServices)
        {
            dataBaseServices = _dataBaseServices;
            _db = new SqliteConnection("Data Source=" + TimeBot.Model.Settings.GetSettings().Path.Database + TimeBot.Model.Settings.GetSettings().Proccess.Other.DatabaseName);
        }

        public async Task<byte[]> ConvertRegionalRateForBot()
        {
            string tableData = "";
            var NameRegions = _db.Query<string>("select RegionName from RegionRate");
            foreach (var itemName in NameRegions.Distinct())
            {
                var MaxId = _db.Query<int>("select max(Id) from RegionRate where RegionName=@RegionName", new { RegionName = itemName.ToString() });
                foreach (var itemId in MaxId)
                {
                    var regionRates = _db.Query<RegionRate>("select * from RegionRate where Id=@Id", new { Id = itemId }).ToList();

                    foreach (RegionRate r in regionRates)
                    {
                        tableData +=
                                    "<tr>" +
                                        "<td style=\"font-size:40px;float:left;\">" + GetRegionNames(r.RegionName) + "</td>" +
                                        "<td style=\"font-size:45px;\">" + r.BuyRate + "</td> " +
                                        "<td  style=\"font-size:45px;\">" + r.SelRate + "</td>" +
                                        "<td  class=\"font-weight-bold\" style=\"font-size:40px;\">" + r.UpdateTime.Substring(9) + " " + r.UpdateTime.Substring(0, 8) + "</td>" +
                                    "</tr> ";
                    }
                }
            }


            //string path = ExtensionMethods.MainExecutionDirectoryPath();
            string path = TimeBot.Model.Settings.GetSettings().Path.Html;
            string oldHtmlFilePath = path + @"regional_currency.html";

            string? oldHtmlContent = await File.ReadAllTextAsync(oldHtmlFilePath);

            string newHtml = oldHtmlContent.Replace("table_content", tableData);
            string new_html = newHtml.Replace("time_date", DateTime.Now.ToString("dd.MM.yyyy"));

            string newHtmlFilePath = path + @"regional_currency2.html";
            await File.WriteAllTextAsync(newHtmlFilePath, new_html, Encoding.UTF8);

            HtmlConverter? converter = new HtmlConverter();

            byte[] bytes = converter.FromUrl(newHtmlFilePath, 1000, ImageFormat.Jpg, 100);
            return bytes;
        }

        public async Task<byte[]> ConvertBankRateForBot()
        {
            //List<string> BankNames1 = new List<string>() { "Infin Bank","Anor Bank","Universal Bank","Xalq Banki","Trust Bank","Aloqa Bank","Hamkor Bank","SQB","Kapital Bank","IpakYuliBank","Agro Bank","Ipoteka Bank","NBU","Mikrokreditbank","Ziraat Bank","Asaka Bank","Ravnaq Bank","QQB","Savdogar Bank","Orient Finans Bank","Turon Bank","Tenge Bank","AsiaAllianceBank","Ziraat Bank","Aloqa Bank"};
            int max = 0; int min = 15000;
            string tableData = "";
            var BankNames1 = _db.Query<string>("select BankName from BankRate");
            foreach (var itemName in BankNames1.Distinct())
            {
                var maxId = _db.Query<int>("select max(Id) from BankRate where BankName=@BankName", new { BankName = itemName.ToString() });
                foreach (var itemId in maxId)
                {
                    var bankRates = _db.Query<BankRate>("select * from BankRate where Id=@Id", new { Id = itemId }).ToList();
                    foreach (var item in bankRates)
                    {
                        if (int.Parse(item.SelRate) < min)
                        {
                            min = int.Parse(item.SelRate);
                        }
                        if (int.Parse(item.BuyRate) > max)
                        {
                            max = int.Parse(item.BuyRate);
                        }
                    }
                }
            }
            foreach (var itemName in BankNames1.Distinct())
            {
                var maxId = _db.Query<int>("select max(Id) from BankRate where BankName=@BankName", new { BankName = itemName.ToString() });
                foreach (var itemId in maxId)
                {
                    var bankRates = _db.Query<BankRate>("select * from BankRate where Id=@Id", new { Id = itemId }).ToList();
                    foreach (BankRate r in bankRates)
                    {
                        tableData +=
                                      "<tr>" +
                                          "<td class=\"m-0 p-0\" style=\"font-size:35px;float:left;\">" + r.BankName + "</td>";

                        if (int.Parse(r.BuyRate) == max)
                        {
                            tableData += "<td class=\"m-0 p-0\" style=\"font-size:35px;background-color:#3dd19a;\">" + r.BuyRate + "</td> ";
                        }
                        else
                        {
                            tableData += "<td class=\"m-0 p-0\" style=\"font-size:35px;\">" + r.BuyRate + "</td> ";
                        }
                        if (int.Parse(r.SelRate) == min)
                        {
                            tableData += "<td class=\"m-0 p-0\" style=\"font-size:35px;background-color:#3dd19a;\">" + r.SelRate + "</td>";
                        }
                        else
                        {
                            tableData += "<td class=\"m-0 p-0\" style=\"font-size:35px;\">" + r.SelRate + "</td>";
                        }
                        tableData +=
                                    "<td  class=\"m-0 p-0\" style=\"font-size:35px;\" class=\"font-weight-bold\">" + r.UpdateTime.Substring(11) + " " + r.UpdateTime.Substring(0, 10) + "</td>" +
                                        "</tr> ";
                    }
                }
            }


            //string path = ExtensionMethods.MainExecutionDirectoryPath();
            string path = TimeBot.Model.Settings.GetSettings().Path.Html;

            string oldHtmlFilePath = path + @"bank_currency.html";

            string? oldHtmlContent = await File.ReadAllTextAsync(oldHtmlFilePath);

            string newHtml = oldHtmlContent.Replace("table_content", tableData);

            string new_html = newHtml.Replace("time_date", DateTime.Now.ToString("dd.MM.yyyy"));

            string newHtmlFilePath = path + @"bank_currency2.html";
            await File.WriteAllTextAsync(newHtmlFilePath, new_html, Encoding.UTF8);

            HtmlConverter? converter = new HtmlConverter();

            byte[] bytes = converter.FromUrl(newHtmlFilePath, 1000, ImageFormat.Jpg, 100);
            return bytes;
        }

        public async Task<byte[]> ConvertMarketRateForBot()
        {
            string newBuyRate = ""; string newSelRate = "";
            string oldBuyRate = ""; string oldSelRate = "";
            string tableData = "";

            var MarketNames = _db.Query<string>("select [Name] from MarketRate");
            var MarketId = _db.QuerySingle<int>("select max(Id) from MarketRate");
            var marketRatesNew = _db.Query<MarketRate>("select * from MarketRate where Id=@Id", new { Id = MarketId - 1 }).ToList();

            foreach (MarketRate r in marketRatesNew)
            {
                oldBuyRate = r.BuyRate;
                oldSelRate = r.SelRate;
            }

            foreach (var itemName in MarketNames.Distinct())
            {
                var maxId = _db.Query<int>("select max(Id) from MarketRate where [Name]=@Name", new { Name = itemName.ToString() });
                List<int> MyList = maxId.ToList<int>();
                int a = MyList[0];
                foreach (var itemId in maxId)
                {
                    var marketRatesOld = _db.Query<MarketRate>("select * from MarketRate where Id=@Id", new { Id = itemId }).ToList();

                    foreach (MarketRate r in marketRatesOld)
                    {
                        newBuyRate = r.BuyRate;
                        newSelRate = r.SelRate;
                        tableData +=
                                "<tr>" +
                                    "<td style=\"font-size:30px;width:200px;background-color:#7abaff\">" + "Валюта" + "</td>" +
                                    "<td style=\"font-size:30px;\">" + GetFlagIcons("AQSH dollari") + GetCurrencyNames("AQSH dollari") + "</td>" +
                                "</tr>";
                        tableData +=
                            "<tr>" +
                                    "<td style=\"font-size:30px;width:200px;background-color:#7abaff\">" + "Олиш" + "</td>" +
                                    "<td style=\"font-size:35px;width:250px;\">" + r.BuyRate + GetRate(Convert.ToDouble(newBuyRate) - Convert.ToDouble(oldBuyRate)) + "</td>" +
                                "</tr>";
                        tableData +=
                            "<tr>" +
                                    "<td style=\"font-size:30px;width:200px;background-color:#7abaff\">" + "Сотиш" + "</td>" +
                                    "<td style=\"font-size:35px;width:250px;\">" + r.SelRate + GetRate(Convert.ToDouble(newSelRate) - Convert.ToDouble(oldSelRate)) + "</td>" +
                                "</tr>";
                        tableData +=
                            "<tr>" +
                                    "<td style=\"font-size:30px;width:200px; background-color:#7abaff\">" + "Янгиланиш" + "</td>" +
                                    "<td class=\"font-weight-bold\" style=\"font-size:35px;\">" + r.UpdateTime.Substring(11) + " " + r.UpdateTime.Substring(0, 10) + "</td" +
                                "</tr>";
                    }
                }
            }
            //string path = ExtensionMethods.MainExecutionDirectoryPath();
            string path = TimeBot.Model.Settings.GetSettings().Path.Html;

            string oldHtmlFilePath = path + @"market_currency.html";

            string? oldHtmlContent = await File.ReadAllTextAsync(oldHtmlFilePath);

            string newHtml = oldHtmlContent.Replace("table_content", tableData);

            string new_html = newHtml.Replace("time_date", DateTime.Now.ToString("dd.MM.yyyy"));

            string newHtmlFilePath = path + @"market_currency2.html";
            await File.WriteAllTextAsync(newHtmlFilePath, new_html, Encoding.UTF8);

            HtmlConverter? converter = new HtmlConverter();

            byte[] bytes = converter.FromUrl(newHtmlFilePath, 850, ImageFormat.Jpg, 100);
            return bytes;
        }

        public async Task<byte[]> ConvertCbuRateForBot()
        {
            string tableData = "";
            List<string> CbuNames = new List<string>() { "AQSH dollari", "EVRO", "Rossiya rubli", "Xitoy yuani" };
            foreach (var itemName in CbuNames.Distinct())
            {
                var maxId = _db.Query<int>("select max(Id) from CbuRate where [CcyNm_UZ]=@CcyNm_UZ", new { CcyNm_UZ = itemName.ToString() });
                foreach (var itemId in maxId)
                {
                    var cbuRates = _db.Query<CbuRate>("select * from CbuRate where Id=@Id", new { Id = itemId });

                    foreach (CbuRate r in cbuRates)
                    {
                        if (r.Diff.Substring(0, 1) == "-")
                        {
                            tableData +=
                                        "<tr style=\"height:30px;\">" +
                                            "<th scope=\"row\" style=\"font-size:45px;float:left;\">" + GetFlagIcons(r.CcyNm_UZ) + GetCurrencyNames(r.CcyNm_UZ) + "</th>" +
                                            "<td style=\"font-size:45px;\">" + r.Rate + "</td> " +
                                            "<td style=\"font-size:45px;color:white;background-color:#dc3545;border:1px solid white;border-radius:5px;\">" + r.Diff + "</td>" +
                                            "<td style=\"font-size:40px;\">" + r.Date + "</td>" +
                                        "</tr> ";
                        }
                        else
                        {
                            tableData +=
                                        "<tr style=\"height:30px;\">" +
                                            "<th scope=\"row\" style=\"font-size:45px;float:left;\">" + GetFlagIcons(r.CcyNm_UZ) + GetCurrencyNames(r.CcyNm_UZ) + "</th>" +
                                            "<td style=\"font-size:45px;\">" + r.Rate + "</td> " +
                                            "<td style=\"font-size:45px;color:white;background-color:#5cb85c;border:1px solid white;border-radius:5px;\">" + r.Diff + "</td>" +
                                            "<td style=\"font-size:40px;\">" + r.Date + "</td>" +
                                        "</tr> ";
                        }
                    }
                }
            }


            // string path = ExtensionMethods.MainExecutionDirectoryPath();
            string path = TimeBot.Model.Settings.GetSettings().Path.Html;

            string oldHtmlFilePath = path + @"cbu_currency.html";

            string? oldHtmlContent = await File.ReadAllTextAsync(oldHtmlFilePath);

            string newHtml = oldHtmlContent.Replace("table_content", tableData);

            string new_html = newHtml.Replace("time_date", DateTime.Now.ToString("dd.MM.yyyy"));

            string newHtmlFilePath = path + @"cbu_currency2.html";
            await File.WriteAllTextAsync(newHtmlFilePath, new_html, Encoding.UTF8);

            HtmlConverter? converter = new HtmlConverter();

            byte[] bytes = converter.FromUrl(newHtmlFilePath, 1200, ImageFormat.Jpg, 100);
            return bytes;
        }

        public async Task<byte[]> ConvertBirjaRateForBot()
        {
            string Currency_names = "";
            string Currency_rates = "";
            string Currency_dates = "";
            var BirjaNames = _db.Query<string>("select [Name] from BirjaRate");
            foreach (var itemName in BirjaNames.Distinct())
            {
                var MaxId = _db.Query<int>("select max(Id) from BirjaRate where [Name]=@Name", new { Name = itemName.ToString() });
                foreach (var itemId in MaxId)
                {
                    var BirjaRates = _db.Query<BirjaRate>("select * from BirjaRate where Id=@Id", new { Id = itemId }).ToList();

                    //Nomlarini qo'shib olish
                    foreach (BirjaRate r in BirjaRates)
                    {
                        Currency_names +=
                            "<td style=\"font-size:25px;\">" + GetFlagIcons(r.Name) + GetCurrencyNames(r.Name) + " </td>";
                    }

                    //Kurslarini qo'shib olish
                    foreach (BirjaRate r in BirjaRates)
                    {
                        if (r.Diff.Substring(0, 1) == "-")
                        {
                            Currency_rates +=
                            "<td style=\"font-size:25px;\">" + r.Rate + "<svg style=\"color:red\" width=\"40px\" height=\"40px\" fill=\"currentColor\" class=\"bi bi-arrow-down\" viewBox=\"0 0 16 16\">" +
                              "<path fill-rule=\"evenodd\" d=\"M8 1a.5.5 0 0 1 .5.5v11.793l3.146-3.147a.5.5 0 0 1 .708.708l-4 4a.5.5 0 0 1-.708 0l-4-4a.5.5 0 0 1 " + ".708-.708L7.5 13.293V1.5A.5.5 0 0 1 8 1z\"/>" +
                           "</svg>" + " " + r.Diff + " </td>";
                        }
                        else
                        {
                            Currency_rates +=
                        "<td style=\"font-size:25px;\">" + r.Rate + "<svg style=\"color:green\" width=\"40px\" height=\"40px\"  fill=\"currentColor\" class=\"bi bi-arrow-up\" viewBox=\"0 0 16 16\">" +
                              "<path fill-rule=\"evenodd\" d=\"M8 15a.5.5 0 0 0 .5-.5V2.707l3.146 3.147a.5.5 0 0 0 .708-.708l-4-4a.5.5 0 0 0-.708 0l-4 4a.5.5 0 1 0 " + ".708.708L7.5 2.707V14.5a.5.5 0 0 0 .5.5z\"/>" +
                           "</svg>" + " " + r.Diff.Substring(1) + " </td>";
                        }
                    }
                    //Yangilanishlarini qo'shib olish
                    foreach (BirjaRate r in BirjaRates)
                    {
                        Currency_dates +=
                            "<td style=\"font-size:25px;\">" + r.UpdateTime.Substring(11) + " " + r.UpdateTime.Substring(0, 10) + "</td>";
                    }
                }
            }


            //string path = ExtensionMethods.MainExecutionDirectoryPath();
            string path = TimeBot.Model.Settings.GetSettings().Path.Html;
            string oldHtmlFilePath = path + @"birja_currency.html";

            string? oldHtmlContent = await File.ReadAllTextAsync(oldHtmlFilePath);

            string newHtml1 = oldHtmlContent.Replace("currency_names", Currency_names);
            string newHtml2 = newHtml1.Replace("currency_rates", Currency_rates);
            string newHtml = newHtml2.Replace("currency_dates", Currency_dates);
            string new_html = newHtml.Replace("time_date", DateTime.Now.ToString("dd.MM.yyyy"));

            string newHtmlFilePath = path + @"birja_currency2.html";
            await File.WriteAllTextAsync(newHtmlFilePath, new_html, Encoding.UTF8);

            HtmlConverter? converter = new HtmlConverter();

            byte[] bytes = converter.FromUrl(newHtmlFilePath, 850, ImageFormat.Jpg, 100);
            return bytes;
        }

        public async Task<byte[]> ConvertRegionRateAsync(List<RegionRate> updatedList)
        {
            string tableData = "";
            foreach (RegionRate r in updatedList)
            {
                tableData +=
                        "<tr>" +
                            "<td style=\"font-size:40px;float:left;\">" + GetRegionNames(r.RegionName) + "</td>" +
                            "<td style=\"font-size:45px;\">" + r.BuyRate + "</td> " +
                            "<td  style=\"font-size:45px;\">" + r.SelRate + "</td>" +
                            "<td  class=\"font-weight-bold\" style=\"font-size:40px;\">" + r.UpdateTime.Substring(9) + " " + r.UpdateTime.Substring(0, 8) + "</td>" +
                        "</tr> ";
            }

            //string path = ExtensionMethods.MainExecutionDirectoryPath();
            string path = TimeBot.Model.Settings.GetSettings().Path.Html;

            string oldHtmlFilePath = path + @"regional_currency.html";

            string? oldHtmlContent = await File.ReadAllTextAsync(oldHtmlFilePath);

            string newHtml = oldHtmlContent.Replace("table_content", tableData);
            string new_html = newHtml.Replace("time_date", DateTime.Now.ToString("dd.MM.yyyy"));

            string newHtmlFilePath = path + @"regional_currency2.html";
            await File.WriteAllTextAsync(newHtmlFilePath, new_html, Encoding.UTF8);

            HtmlConverter? converter = new HtmlConverter();

            byte[] bytes = converter.FromUrl(newHtmlFilePath, 1200, ImageFormat.Jpg, 100);

            //File.Delete(newHtmlFilePath);

            return bytes;
        }

        public async Task<byte[]> ConvertBankRateAsync(List<BankRate> updatedList)
        {
            int max = 0; int min = 15000;
            string tableData = "";
            var BankNames1 = _db.Query<string>("select BankName from BankRate");
            foreach (var itemName in BankNames1.Distinct())
            {
                var maxId = _db.Query<int>("select max(Id) from BankRate where BankName=@BankName", new { BankName = itemName.ToString() });
                foreach (var itemId in maxId)
                {
                    var bankRates = _db.Query<BankRate>("select * from BankRate where Id=@Id", new { Id = itemId }).ToList();
                    foreach (var item in bankRates)
                    {
                        if (int.Parse(item.SelRate) < min)
                        {
                            min = int.Parse(item.SelRate);
                        }
                        if (int.Parse(item.BuyRate) > max)
                        {
                            max = int.Parse(item.BuyRate);
                        }
                    }
                }
            }
            foreach (BankRate r in updatedList)
            {
                tableData +=
                              "<tr>" +
                                  "<td class=\"m-0 p-0\" style=\"font-size:35px;float:left;\">" + r.BankName + "</td>";

                if (int.Parse(r.BuyRate) == max)
                {
                    tableData += "<td class=\"m-0 p-0\" style=\"font-size:35px;background-color:#3dd19a;\">" + r.BuyRate + "</td> ";
                }
                else
                {
                    tableData += "<td class=\"m-0 p-0\" style=\"font-size:35px;\">" + r.BuyRate + "</td> ";
                }
                if (int.Parse(r.SelRate) == min)
                {
                    tableData += "<td class=\"m-0 p-0\" style=\"font-size:35px;background-color:#3dd19a;\">" + r.SelRate + "</td>";
                }
                else
                {
                    tableData += "<td class=\"m-0 p-0\" style=\"font-size:35px;\">" + r.SelRate + "</td>";
                }
                tableData +=
                            "<td  class=\"m-0 p-0\" style=\"font-size:35px;\" class=\"font-weight-bold\">" + r.UpdateTime.Substring(11) + " " + r.UpdateTime.Substring(0, 10) + "</td>" +
                                "</tr> ";
            }

            //  string path = ExtensionMethods.MainExecutionDirectoryPath();
            string path = TimeBot.Model.Settings.GetSettings().Path.Html;

            string oldHtmlFilePath = path + @"bank_currency.html";

            string? oldHtmlContent = await File.ReadAllTextAsync(oldHtmlFilePath);

            string newHtml = oldHtmlContent.Replace("table_content", tableData);
            string new_html = newHtml.Replace("time_date", DateTime.Now.ToString("dd.MM.yyyy"));

            string newHtmlFilePath = path + @"bank_currency2.html";//dizayn
            await File.WriteAllTextAsync(newHtmlFilePath, new_html, Encoding.UTF8);

            HtmlConverter? converter = new HtmlConverter();

            byte[] bytes = converter.FromUrl(newHtmlFilePath, 1200, ImageFormat.Jpg, 100);

            //File.Delete(newHtmlFilePath);

            return bytes;
        }

        public async Task<byte[]> ConvertMarketRateAsync(List<MarketRate> updatedList)
        {
            string oldBuyRate = ""; string oldSelRate = "";
            string newBuyRate = ""; string newSelRate = "";
            string tableData = "";
            var MarketNames = _db.Query<string>("select [Name] from MarketRate");
            var MarketId = _db.QuerySingle<int>("select max(Id) from MarketRate");
            var marketRatesNew = _db.Query<MarketRate>("select * from MarketRate where Id=@Id", new { Id = MarketId - 1 }).ToList();

            foreach (MarketRate r in marketRatesNew)
            {
                oldBuyRate = r.BuyRate;
                oldSelRate = r.SelRate;
            }
            foreach (MarketRate r in updatedList)
            {
                newBuyRate = r.BuyRate;
                newSelRate = r.SelRate;
                tableData +=
                        "<tr>" +
                            "<td style=\"font-size:30px;width:200px;background-color:#7abaff\">" + "Валюта" + "</td>" +
                            "<td style=\"font-size:30px;\">" + GetFlagIcons("AQSH dollari") + GetCurrencyNames("AQSH dollari") + "</td>" +
                        "</tr>";
                tableData +=
                    "<tr>" +
                            "<td style=\"font-size:30px;width:200px;background-color:#7abaff\">" + "Олиш" + "</td>" +
                            "<td style=\"font-size:35px;width:250px;\">" + r.BuyRate + GetRate(Convert.ToDouble(newBuyRate) - Convert.ToDouble(oldBuyRate)) + "</td>" +
                        "</tr>";
                tableData +=
                    "<tr>" +
                            "<td style=\"font-size:30px;width:200px;background-color:#7abaff\">" + "Сотиш" + "</td>" +
                            "<td style=\"font-size:35px;width:250px;\">" + r.SelRate + GetRate(Convert.ToDouble(newSelRate) - Convert.ToDouble(oldSelRate)) + "</td>" +
                        "</tr>";
                tableData +=
                    "<tr>" +
                            "<td style=\"font-size:30px;width:200px; background-color:#7abaff\">" + "Янгиланиш" + "</td>" +
                            "<td style=\"font-size:35px;\">" + r.UpdateTime.Substring(11) + " " + r.UpdateTime.Substring(0, 10) + "</td" +
                        "</tr>";
            }

            //string path = ExtensionMethods.MainExecutionDirectoryPath();
            string path = TimeBot.Model.Settings.GetSettings().Path.Html;

            string oldHtmlFilePath = path + @"market_currency.html";

            string? oldHtmlContent = await File.ReadAllTextAsync(oldHtmlFilePath);

            string newHtml = oldHtmlContent.Replace("table_content", tableData);
            string new_html = newHtml.Replace("time_date", DateTime.Now.ToString("dd.MM.yyyy"));

            string newHtmlFilePath = path + @"market_currency2.html";
            await File.WriteAllTextAsync(newHtmlFilePath, new_html, Encoding.UTF8);

            HtmlConverter? converter = new HtmlConverter();

            byte[] bytes = converter.FromUrl(newHtmlFilePath, 850, ImageFormat.Jpg, 100);

            //File.Delete(newHtmlFilePath);

            return bytes;
        }

        public async Task<byte[]> ConvertCbuRateAsync(List<CbuRate> updatedList)
        {
            string tableData = "";
            foreach (CbuRate r in updatedList)
            {
                if (r.Diff.Substring(0, 1) == "-")
                {
                    tableData +=
                                "<tr>" +
                                    "<th scope=\"row\" style=\"font-size:45px;float:left;\">" + GetFlagIcons(r.CcyNm_UZ) + GetCurrencyNames(r.CcyNm_UZ) + "</th>" +
                                    "<td style=\"font-size:45px;\">" + r.Rate + "</td> " +
                                    "<td style=\"font-size:45px;color:white;background-color:#dc3545;border:1px solid white;border-radius:5px;\">" + r.Diff + "</td>" +
                                    "<td style=\"font-size:40px;\">" + r.Date + "</td>" +
                                "</tr> ";
                }
                else
                {
                    tableData +=
                                "<tr>" +
                                    "<th scope=\"row\" style=\"font-size:45px;float:left;\">" + GetFlagIcons(r.CcyNm_UZ) + GetCurrencyNames(r.CcyNm_UZ) + "</th>" +
                                    "<td style=\"font-size:45px;\">" + r.Rate + "</td> " +
                                    "<td style=\"font-size:45px;color:white;background-color:#5cb85c;border:1px solid white;border-radius:5px;\">" + r.Diff + "</td>" +
                                    "<td style=\"font-size:40px;\">" + r.Date + "</td>" +
                                "</tr> ";
                }
            }
            // string path = ExtensionMethods.MainExecutionDirectoryPath();
            string path = TimeBot.Model.Settings.GetSettings().Path.Html;

            string oldHtmlFilePath = path + @"cbu_currency.html";

            string? oldHtmlContent = await File.ReadAllTextAsync(oldHtmlFilePath);

            string newHtml = oldHtmlContent.Replace("table_content", tableData);

            string new_html = newHtml.Replace("time_date", DateTime.Now.ToString("dd.MM.yyyy"));

            string newHtmlFilePath = path + @"cbu_currency2.html";
            await File.WriteAllTextAsync(newHtmlFilePath, new_html, Encoding.UTF8);

            HtmlConverter? converter = new HtmlConverter();

            byte[] bytes = converter.FromUrl(newHtmlFilePath, 1200, ImageFormat.Jpg, 100);

            //File.Delete(newHtmlFilePath);

            return bytes;
        }

        public async Task<byte[]> ConvertBirjaRateAsync(List<BirjaRate> updatedList)
        {
            string Currency_names = "";
            string Currency_rates = "";
            string Currency_dates = "";

            foreach (BirjaRate r in updatedList)
            {
                Currency_names +=
                               "<td style=\"font-size:25px;\">" + GetFlagIcons(r.Name) + GetCurrencyNames(r.Name) + " </td>";

                if (r.Diff.Substring(0, 1) == "-")
                {
                    Currency_rates +=
                    "<td style=\"font-size:25px;\">" + r.Rate + "<svg style=\"color:red\" width=\"40px\" height=\"40px\" fill=\"currentColor\" class=\"bi bi-arrow-down\" viewBox=\"0 0 16 16\">" +
                      "<path fill-rule=\"evenodd\" d=\"M8 1a.5.5 0 0 1 .5.5v11.793l3.146-3.147a.5.5 0 0 1 .708.708l-4 4a.5.5 0 0 1-.708 0l-4-4a.5.5 0 0 1 " + ".708-.708L7.5 13.293V1.5A.5.5 0 0 1 8 1z\"/>" +
                   "</svg>" + " " + r.Diff + " </td>";
                }
                else
                {
                    Currency_rates +=
                "<td style=\"font-size:25px;\">" + r.Rate + "<svg style=\"color:green\" width=\"40px\" height=\"40px\"  fill=\"currentColor\" class=\"bi bi-arrow-up\" viewBox=\"0 0 16 16\">" +
                      "<path fill-rule=\"evenodd\" d=\"M8 15a.5.5 0 0 0 .5-.5V2.707l3.146 3.147a.5.5 0 0 0 .708-.708l-4-4a.5.5 0 0 0-.708 0l-4 4a.5.5 0 1 0 " + ".708.708L7.5 2.707V14.5a.5.5 0 0 0 .5.5z\"/>" +
                   "</svg>" + " " + r.Diff.Substring(1) + " </td>";
                }
                Currency_dates +=
                           "<td style=\"font-size:25px;\">" + r.UpdateTime.Substring(11) + " " + r.UpdateTime.Substring(0, 10) + "</td>";

            }

            //  string path = ExtensionMethods.MainExecutionDirectoryPath();
            string path = TimeBot.Model.Settings.GetSettings().Path.Html;

            string oldHtmlFilePath = path + @"birja_currency.html";

            string? oldHtmlContent = await File.ReadAllTextAsync(oldHtmlFilePath);

            string newHtml1 = oldHtmlContent.Replace("currency_names", Currency_names);
            string newHtml2 = newHtml1.Replace("currency_rates", Currency_rates);
            string newHtml = newHtml2.Replace("currency_dates", Currency_dates);
            string new_html = newHtml.Replace("time_date", DateTime.Now.ToString("dd.MM.yyyy"));

            string newHtmlFilePath = path + @"birja_currency2.html";//dizayn
            await File.WriteAllTextAsync(newHtmlFilePath, new_html, Encoding.UTF8);

            HtmlConverter? converter = new HtmlConverter();

            byte[] bytes = converter.FromUrl(newHtmlFilePath, 850, ImageFormat.Jpg, 100);

            //File.Delete(newHtmlFilePath);

            return bytes;
        }

        public string GetFlagIcons(string name)
        {
            string qaytar = "";
            switch (name)
            {
                case "AQSH dollari" or "USD":
                    qaytar = "<svg style=\"width: 40px;\" viewBox=\"0 0 640 480\">" +
                                        "<g fill-rule=\"evenodd\">" +
                                          "<g stroke-width=\"1pt\">" +
                                            "<path fill=\"#bd3d44\" d=\"M0 0h972.8v39.4H0zm0 78.8h972.8v39.4H0zm0 78.7h972.8V197H0zm0 78.8h972.8v39.4H0zm0 78.8h972.8v39.4H0zm0 78.7h972.8v39.4H0zm0 78.8h972.8V512H0z\" transform=\"scale(.9375)\"/>" +
                                            "<path fill=\"#fff\" d=\"M0 39.4h972.8v39.4H0zm0 78.8h972.8v39.3H0zm0 78.7h972.8v39.4H0zm0 78.8h972.8v39.4H0zm0 78.8h972.8v39.4H0zm0 78.7h972.8v39.4H0z\" transform=\"scale(.9375)\"/>" +
                                          "</g>" +
                                          "<path fill=\"#192f5d\" d=\"M0 0h389.1v275.7H0z\" transform=\"scale(.9375)\"/>" +
                                          "<path fill=\"#fff\" d=\"M32.4 11.8 36 22.7h11.4l-9.2 6.7 3.5 11-9.3-6.8-9.2 6.7 3.5-10.9-9.3-6.7H29zm64.9 0 3.5 10.9h11.5l-9.3 6.7 3.5 11-9.2-6.8-9.3 6.7 3.5-10.9-9.2-6.7h11.4zm64.8 0 3.6 10.9H177l-9.2 6.7 3.5 11-9.3-6.8-9.2 6.7 3.5-10.9-9.3-6.7h11.5zm64.9 0 3.5 10.9H242l-9.3 6.7 3.6 11-9.3-6.8-9.3 6.7 3.6-10.9-9.3-6.7h11.4zm64.8 0 3.6 10.9h11.4l-9.2 6.7 3.5 11-9.3-6.8-9.2 6.7 3.5-10.9-9.2-6.7h11.4zm64.9 0 3.5 10.9h11.5l-9.3 6.7 3.6 11-9.3-6.8-9.3 6.7 3.6-10.9-9.3-6.7h11.5zM64.9 39.4l3.5 10.9h11.5L70.6 57 74 67.9l-9-6.7-9.3 6.7L59 57l-9-6.7h11.4zm64.8 0 3.6 10.9h11.4l-9.3 6.7 3.6 10.9-9.3-6.7-9.3 6.7L124 57l-9.3-6.7h11.5zm64.9 0 3.5 10.9h11.5l-9.3 6.7 3.5 10.9-9.2-6.7-9.3 6.7 3.5-10.9-9.2-6.7H191zm64.8 0 3.6 10.9h11.4l-9.3 6.7 3.6 10.9-9.3-6.7-9.2 6.7 3.5-10.9-9.3-6.7H256zm64.9 0 3.5 10.9h11.5L330 57l3.5 10.9-9.2-6.7-9.3 6.7 3.5-10.9-9.2-6.7h11.4zM32.4 66.9 36 78h11.4l-9.2 6.7 3.5 10.9-9.3-6.8-9.2 6.8 3.5-11-9.3-6.7H29zm64.9 0 3.5 11h11.5l-9.3 6.7 3.5 10.9-9.2-6.8-9.3 6.8 3.5-11-9.2-6.7h11.4zm64.8 0 3.6 11H177l-9.2 6.7 3.5 10.9-9.3-6.8-9.2 6.8 3.5-11-9.3-6.7h11.5zm64.9 0 3.5 11H242l-9.3 6.7 3.6 10.9-9.3-6.8-9.3 6.8 3.6-11-9.3-6.7h11.4zm64.8 0 3.6 11h11.4l-9.2 6.7 3.5 10.9-9.3-6.8-9.2 6.8 3.5-11-9.2-6.7h11.4zm64.9 0 3.5 11h11.5l-9.3 6.7 3.6 10.9-9.3-6.8-9.3 6.8 3.6-11-9.3-6.7h11.5zM64.9 94.5l3.5 10.9h11.5l-9.3 6.7 3.5 11-9.2-6.8-9.3 6.7 3.5-10.9-9.2-6.7h11.4zm64.8 0 3.6 10.9h11.4l-9.3 6.7 3.6 11-9.3-6.8-9.3 6.7 3.6-10.9-9.3-6.7h11.5zm64.9 0 3.5 10.9h11.5l-9.3 6.7 3.5 11-9.2-6.8-9.3 6.7 3.5-10.9-9.2-6.7H191zm64.8 0 3.6 10.9h11.4l-9.2 6.7 3.5 11-9.3-6.8-9.2 6.7 3.5-10.9-9.3-6.7H256zm64.9 0 3.5 10.9h11.5l-9.3 6.7 3.5 11-9.2-6.8-9.3 6.7 3.5-10.9-9.2-6.7h11.4zM32.4 122.1 36 133h11.4l-9.2 6.7 3.5 11-9.3-6.8-9.2 6.7 3.5-10.9-9.3-6.7H29zm64.9 0 3.5 10.9h11.5l-9.3 6.7 3.5 10.9-9.2-6.7-9.3 6.7 3.5-10.9-9.2-6.7h11.4zm64.8 0 3.6 10.9H177l-9.2 6.7 3.5 11-9.3-6.8-9.2 6.7 3.5-10.9-9.3-6.7h11.5zm64.9 0 3.5 10.9H242l-9.3 6.7 3.6 11-9.3-6.8-9.3 6.7 3.6-10.9-9.3-6.7h11.4zm64.8 0 3.6 10.9h11.4l-9.2 6.7 3.5 11-9.3-6.8-9.2 6.7 3.5-10.9-9.2-6.7h11.4zm64.9 0 3.5 10.9h11.5l-9.3 6.7 3.6 11-9.3-6.8-9.3 6.7 3.6-10.9-9.3-6.7h11.5zM64.9 149.7l3.5 10.9h11.5l-9.3 6.7 3.5 10.9-9.2-6.8-9.3 6.8 3.5-11-9.2-6.7h11.4zm64.8 0 3.6 10.9h11.4l-9.3 6.7 3.6 10.9-9.3-6.8-9.3 6.8 3.6-11-9.3-6.7h11.5zm64.9 0 3.5 10.9h11.5l-9.3 6.7 3.5 10.9-9.2-6.8-9.3 6.8 3.5-11-9.2-6.7H191zm64.8 0 3.6 10.9h11.4l-9.2 6.7 3.5 10.9-9.3-6.8-9.2 6.8 3.5-11-9.3-6.7H256zm64.9 0 3.5 10.9h11.5l-9.3 6.7 3.5 10.9-9.2-6.8-9.3 6.8 3.5-11-9.2-6.7h11.4zM32.4 177.2l3.6 11h11.4l-9.2 6.7 3.5 10.8-9.3-6.7-9.2 6.7 3.5-10.9-9.3-6.7H29zm64.9 0 3.5 11h11.5l-9.3 6.7 3.6 10.8-9.3-6.7-9.3 6.7 3.6-10.9-9.3-6.7h11.4zm64.8 0 3.6 11H177l-9.2 6.7 3.5 10.8-9.3-6.7-9.2 6.7 3.5-10.9-9.3-6.7h11.5zm64.9 0 3.5 11H242l-9.3 6.7 3.6 10.8-9.3-6.7-9.3 6.7 3.6-10.9-9.3-6.7h11.4zm64.8 0 3.6 11h11.4l-9.2 6.7 3.5 10.8-9.3-6.7-9.2 6.7 3.5-10.9-9.2-6.7h11.4zm64.9 0 3.5 11h11.5l-9.3 6.7 3.6 10.8-9.3-6.7-9.3 6.7 3.6-10.9-9.3-6.7h11.5zM64.9 204.8l3.5 10.9h11.5l-9.3 6.7 3.5 11-9.2-6.8-9.3 6.7 3.5-10.9-9.2-6.7h11.4zm64.8 0 3.6 10.9h11.4l-9.3 6.7 3.6 11-9.3-6.8-9.3 6.7 3.6-10.9-9.3-6.7h11.5zm64.9 0 3.5 10.9h11.5l-9.3 6.7 3.5 11-9.2-6.8-9.3 6.7 3.5-10.9-9.2-6.7H191zm64.8 0 3.6 10.9h11.4l-9.2 6.7 3.5 11-9.3-6.8-9.2 6.7 3.5-10.9-9.3-6.7H256zm64.9 0 3.5 10.9h11.5l-9.3 6.7 3.5 11-9.2-6.8-9.3 6.7 3.5-10.9-9.2-6.7h11.4zM32.4 232.4l3.6 10.9h11.4l-9.2 6.7 3.5 10.9-9.3-6.7-9.2 6.7 3.5-11-9.3-6.7H29zm64.9 0 3.5 10.9h11.5L103 250l3.6 10.9-9.3-6.7-9.3 6.7 3.6-11-9.3-6.7h11.4zm64.8 0 3.6 10.9H177l-9 6.7 3.5 10.9-9.3-6.7-9.2 6.7 3.5-11-9.3-6.7h11.5zm64.9 0 3.5 10.9H242l-9.3 6.7 3.6 10.9-9.3-6.7-9.3 6.7 3.6-11-9.3-6.7h11.4zm64.8 0 3.6 10.9h11.4l-9.2 6.7 3.5 10.9-9.3-6.7-9.2 6.7 3.5-11-9.2-6.7h11.4zm64.9 0 3.5 10.9h11.5l-9.3 6.7 3.6 10.9-9.3-6.7-9.3 6.7 3.6-11-9.3-6.7h11.5z\" transform=\"scale(.9375)\"/>" +
                                        "</g>" +
                                      "</svg> &nbsp";
                    break;
                case "EVRO" or "EUR":
                    qaytar =
                        "<svg style=\"width: 50px;\" viewBox=\"0 0 640 480\">" +
                                        "<defs>" +
                                          "<g id=\"d\">" +
                                            "<g id=\"b\">" +
                                              "<path id=\"a\" d=\"m0-1-.3 1 .5.1z\"/>" +
                                              "<use xlink:href=\"#a\" transform=\"scale(-1 1)\"/>" +
                                                "</g>" +
                                                "<g id=\"c\">" +
                                                "<use xlink:href=\"#b\" transform=\"rotate(72)\"/>" +
                                                "<use xlink:href=\"#b\" transform=\"rotate(144)\"/>" +
                                                "</g>" +
                                                "<use xlink:href=\"#c\" transform=\"scale(-1 1)\"/>" +
                                                "</g>" +
                                                "</defs>" +
                                                "<path fill=\"#039\" d=\"M0 0h640v480H0z\"/>" +
                                                "<g fill=\"#fc0\" transform=\"translate(320 242.3) scale(23.7037)\">" +
                                                "<use xlink:href=\"#d\" width=\"100%\" height=\"100%\" y=\"-6\"/>" +
                                                "<use xlink:href=\"#d\" width=\"100%\" height=\"100%\" y=\"6\"/>" +
                                                "<g id=\"e\">" +
                                                "<use xlink:href=\"#d\" width=\"100%\" height=\"100%\" x=\"-6\"/>" +
                                                "<use xlink:href=\"#d\" width=\"100%\" height=\"100%\" transform=\"rotate(-144 -2.3 -2.1)\"/>" +
                                                "<use xlink:href=\"#d\" width=\"100%\" height=\"100%\" transform=\"rotate(144 -2.1 -2.3)\"/>" +
                                                "<use xlink:href=\"#d\" width=\"100%\" height=\"100%\" transform=\"rotate(72 -4.7 -2)\"/>" +
                                                "<use xlink:href=\"#d\" width=\"100%\" height=\"100%\" transform=\"rotate(72 -5 .5)\"/>" +
                                                "</g>" +
                                                "<use xlink:href=\"#e\" width=\"100%\" height=\"100%\" transform=\"scale(-1 1)\"/>" +
                                                "</g>" +
                                "</svg> &nbsp";
                    break;
                case "Rossiya rubli":
                    qaytar =
                        "<svg style=\"width: 50px;\" viewBox=\"0 0 640 480\">" +
                            "<g fill-rule=\"evenodd\" stroke-width=\"1pt\">" +
                                "<path fill=\"#fff\" d=\"M0 0h640v480H0z\"/>" +
                                "<path fill=\"#0039a6\" d=\"M0 160h640v320H0z\"/>" +
                                 "<path fill=\"#d52b1e\" d=\"M0 320h640v160H0z\"/>" +
                            "</g>" +
                        "</svg> &nbsp";
                    break;
                case "Xitoy yuani":
                    qaytar =
                        "<svg style=\"width: 50px;\" viewBox=\"0 0 640 480\">" +
                            "<defs>" +
                              "<path id=\"a\" fill=\"#ffde00\" d=\"M-.6.8 0-1 .6.8-1-.3h2z\"/>" +
                            "</defs>" +
                            "<path fill=\"#de2910\" d=\"M0 0h640v480H0z\"/>" +
                            "<use xlink:href=\"#a\" width=\"30\" height=\"20\" transform=\"matrix(71.9991 0 0 72 120 120)\"/>" +
                            "<use xlink:href=\"#a\" width=\"30\" height=\"20\" transform=\"matrix(-12.33562 -20.5871 20.58684 -12.33577 240.3 48)\"/>" +
                            "<use xlink:href=\"#a\" width=\"30\" height=\"20\" transform=\"matrix(-3.38573 -23.75998 23.75968 -3.38578 288 95.8)\"/>" +
                            "<use xlink:href=\"#a\" width=\"30\" height=\"20\" transform=\"matrix(6.5991 -23.0749 23.0746 6.59919 288 168)\"/>" +
                            "<use xlink:href=\"#a\" width=\"30\" height=\"20\" transform=\"matrix(14.9991 -18.73557 18.73533 14.99929 240 216)\"/>" +
                        "</svg> &nbsp";
                    break;
            }

            return qaytar;
        }

        public string GetBankIcons(string name)
        {
            string qaytar = "";
            switch (name)
            {
                case "Infin Bank":
                    #region Infin Bank
                    qaytar =
                        "<svg  id=\"BankIcon\" viewBox=\"0 0 153 26\" fill=\"none\">" +
                                "<path fill-rule=\"evenodd\" clip-rule=\"evenodd\" d=\"M2.59935 6.00806H5.33432C5.37953 6.25767 5.44734 6.4846 5.56036 6.71152C5.60556" +
                                 "6.7796 5.62817 6.84767 5.67337 6.91575C6.46448 8.27729 8.36314 8.88998 10.4426 8.59498C10.8495 8.52691 11.2789 8.45883 11.6632 " +
                                 "8.32268C12.0474 8.20921 12.4317 8.05037 12.8159 7.86883L12.8838 7.84614C13.1098 7.73267 13.3132 7.64191 13.494 7.52844C14.2399 " +
                                 "7.09729 14.8502 6.57537 15.3249 6.00806H17.2913C18.7153 6.00806 19.8907 7.18806 19.8907 8.61768V23.4131C19.8907 24.8427 " +
                                 "18.7153 26 17.2913 26H2.59935C1.17536 26 0 24.82 0 23.4131V8.59498C0 7.16537 1.17536 6.00806 2.59935 6.00806ZM7.59463 " +
                                 "19.8277L7.54943 10.2061C7.54943 10.0246 7.68504 9.88845 7.84327 9.88845H11.9796C12.1378 9.88845 12.2735 10.0246 12.2735" +
                                  "10.2061L12.3187 19.8277H14.7146C14.8728 19.8277 15.0084 20.0319 15.0084 20.2588V21.3254C15.0084 21.575 14.8728 21.7565" +
                                   "14.7146 21.7565H5.1761C5.01788 21.7565 4.88226 21.5523 4.88226 21.3254V20.2588C4.88226 20.0092 5.01788 19.8277 5.1761 " +
                                   "19.8277H7.59463Z\" fill=\"#234A98\"/>" +
                                "<path fill-rule=\"evenodd\" clip-rule=\"evenodd\" d=\"M8.45347 0.629951C10.6686 -0.436588 13.1549 -0.118896 13.9912 " +
                                "1.35611C14.8501 2.80841 13.7426 4.85073 11.5275 5.91726C9.31239 6.9838 6.84865 6.66611 5.98974 5.19111C5.15342 3.7388 " +
                                "6.23837 1.69649 8.45347 0.629951ZM15.144 3.87495C15.0084 5.05496 13.9912 6.30303 12.4316 7.07457C11.1206 7.70996 9.71924 " +
                                "7.84611 8.61169 7.55111C8.92814 7.55111 9.24458 7.52842 9.56102 7.48304C9.96788 7.43765 10.3747 7.34688 10.7816 7.21073C11.1658 " +
                                "7.09727 11.5501 6.93842 11.9343 6.75688L12.0022 6.73419C12.2056 6.64342 12.409 6.52996 12.6124 6.4165C13.7878 5.71303 14.6467 " +
                                "4.80534 15.144 3.87495ZM8.76992 1.03841C10.4877 0.221489 12.4542 0.471105 13.1097 1.60572C13.2453 1.83264 13.3131 2.10495 13.3357" +
                                 "2.37726C13.3131 2.30918 13.2679 2.24111 13.2227 2.17303C12.5446 1.03841 10.6008 0.766105 8.88293 1.60572C7.50414 2.2638 6.69043 " +
                                 "3.42111 6.73564 4.41957C6.28358 3.28495 7.14249 1.83264 8.76992 1.03841Z\" fill=\"#9B3589\"/>" +
                                "<path fill-rule=\"evenodd\" clip-rule=\"evenodd\" d=\"M24.6823 25.2285C25.4734 25.2058 26.0159 25.1604 26.2645 25.0696C26.7844 " +
                                "24.8881 27.146 24.4569 27.3269 23.8216L31.0564 10.3423C31.2824 9.57077 31.2146 9.07153 30.8756 8.82192C30.6721 8.66307 30.1297 " +
                                "8.59499 29.2933 8.5723L29.4968 7.82346H36.7071L36.5037 8.5723C35.6448 8.66307 35.1023 8.75384 34.8989 8.82192C34.4016 9.02615" +
                                 "34.0626 9.4573 33.8818 10.1154L30.1749 23.4812C29.9714 24.2527 30.0166 24.7519 30.3783 24.9789C30.6043 25.1377 31.1242 25.2058" +
                                  "31.9605 25.2285L31.7571 25.9773H24.4789L24.6823 25.2285ZM32.9098 25.3192C33.5427 25.3192 33.9948 25.2058 34.3338 25.0016C34.7181" +
                                   "24.7519 34.9893 24.3435 35.1701 23.7308L37.3174 16.0154C37.4756 15.4708 37.3852 15.1077 37.0914 14.9262C36.888 14.79 36.4811 14.6992" +
                                    "35.8934 14.6992L36.0969 13.9958C37.8373 13.9504 39.3065 13.8823 40.4819 13.7462L39.962 15.6069C40.8435 14.8808 41.4764 14.4042 41.9058 " +
                                    "14.1773C42.5839 13.8142 43.2846 13.61 44.0305 13.61C46.3813 13.61 47.2402 14.7446 46.6299 17.0139L44.7538 23.7762C44.5956 24.3662 " +
                                    "44.5956 24.7746 44.799 24.9789C45.0025 25.1831 45.3867 25.2739 45.997 25.2512L45.7936 25.9773H39.8264L40.0298 25.2512C40.6401 " +
                                    "25.2285 41.0921 25.1604 41.386 25.0016C41.7702 24.7973 42.0641 24.4116 42.1997 23.8442L43.9853 " +
                                    "17.445C44.2114 16.6054 44.2792 15.9927 44.1436 15.6069C43.9853 15.1077 43.5107 14.8581 42.697 14.8581C41.9511 14.8581 41.2278 " +
                                    "15.2665 40.4819 16.0835C39.849 16.7869 39.3969 17.5585 39.1935 18.3527C39.0353 18.92 38.7866 19.805 38.4702 20.985C38.1537 22.165 " +
                                    "37.9051 23.0273 37.7469 23.6173C37.5661 24.2754 37.5661 24.7292 37.7469 24.9562C37.9277 25.1831 38.3572 25.2739 39.0579 25.2512L38.8544 " +
                                    "25.9773H32.729L32.9098 25.3192ZM50.1786 13.8823H51.9416C51.9868 13.7462 52.032 13.5646 52.0998 13.315C52.1676 13.0654 52.2128 12.8838 " +
                                    "52.2581 12.7704C52.6649 11.2046 53.1622 10.0473 53.7499 9.25307C54.5862 8.11846 55.6937 7.55115 57.0725 7.55115C57.9314 7.55115 58.5643 " +
                                    "7.80076 58.9712 8.32269C59.4006 8.84461 59.491 9.50269 59.265 10.3423C59.1746 10.6827 58.9486 10.9777 58.5869 11.25C58.2253 11.5223 57.8862" +
                                     "11.6585 57.5472 11.6585C57.1855 11.6585 56.9143 11.5223 56.7109 11.2273C56.53 10.9323 56.4622 10.6146 56.5752 10.2515C56.6204 10.0473 " +
                                     "56.7787 9.775 56.9821 9.4573C57.1855 9.13961 " +
                                    "57.3211 8.91269 57.3663 8.75384C57.4568 8.45884 57.2533 8.29999 56.7561 8.29999C56.4848 8.29999 56.191 8.45884 55.8745 " +
                                    "8.75384C55.5581 9.04884 55.3773 9.34384 55.2869 9.66153C55.1512 10.1608 54.993 10.8642 54.8574 11.7492C54.6766 " +
                                    "12.7477 54.541 13.4285 54.4506 13.8142H57.1403L56.8917 14.6992H54.2019L51.7382 23.5946C51.5574 24.23 51.58 24.6612 " +
                                    "51.806 24.9335C51.9868 25.1831 52.4163 25.2966 53.0718 25.2512L52.8909 25.9092H46.8559L47.0367 25.2512C47.6922 " +
                                    "25.2058 48.1443 25.1377 48.3703 25.0242C48.732 24.8427 49.0032 24.4569 49.1614 23.8669L51.7156 14.6765H49.9525L" +
                                    "50.1786 13.8823ZM55.0382 25.3419C55.7163 25.2966 56.1684 25.2058 56.3944 25.0923C56.8239 24.9108 57.0951 24.525 " +
                                    "57.2533 23.9577L59.4458 16.0608C59.6041 15.4708 59.5588 15.0623 59.265 14.8581C59.0842 14.7219 58.6547 14.6312 58." +
                                    "0218 14.6085L58.2027 13.9958L62.5199 13.7915L59.7171 23.8669C59.5362 24.4796 59.5815 24.8654 59.8075 25.0469C59.9205 " +
                                    "25.115 60.3047 25.2285 61.0054 25.3419L60.8246 25.9773H54.8574L55.0382 25.3419ZM62.7459 7.80076H62.7233C63.1075 7.8007" +
                                    "6 63.424 7.95961 63.6274 8.2773C63.8308 8.59499 63.876 8.93538 63.7856 9.34384C63.6726 9.775 63.424 " +
                                    "10.1154 63.0397 10.365C62.6555 10.6373 62.2486 10.7508 61.8192 10.7508C61.4349 10.7508 61.1411 10.5919 60.9376 10.2" +
                                    "742C60.7342 9.95653 60.6664 9.61615 60.7794 9.23038C60.8924 8.84461 61.1411 8.50423 61.5479 8.20923C61.9322 7.93692 6" +
                                    "2.339 7.80076 62.7459 7.80076ZM61.8418 25.3192C62.4746 25.3192 62.9267 25.2058 63.2658 25.0016C63.65 24.7519 63.9212 24.3" +
                                    "435 64.1021 23.7308L66.2494 16.0154C66.4076 15.4708 66.3172 15.1077 66.0233 14.9262C65.8199 14.79 65.413 " +
                                    "14.6992 64.8254 14.6992L65.0288 13.9958C66.7692 13.9504 68.2384 13.8823 69.4138 13.7462L68.8939 15.6069C69.7754 14.8808 " +
                                    "70.4083 14.4042 70.8378 14.1773C71.5159 13.8142 72.2166 13.61 72.9625 13.61C75.3132 13.61 76.1721 " +
                                    "14.7446 75.5618 17.0139L73.6858 23.7762C73.5275 24.3662 73.5275 24.7746 73.731 24.9789C73.9344 25.1831 74.3187 25.2739" +
                                     "74.9289 25.2512L74.7255 25.9773H68.7583L68.9617 25.2512C69.572 25.2285 70.0241 25.1604 70.3179 25.0016C70.7022 24.79" +
                                     "73 70.996 24.4116 71.1316 23.8442L72.9173 17.445C73.1433 16.6054 73.2111 15.9927 73.0755 15.6069C72.9173 15.1077 72." +
                                     "4426 14.8581 71.6289 14.8581C70.883 14.8581 70.1597 15.2665 69.4138 16.0835C68.7809 16.7869 68.3288 17.5585 68.1254 " +
                                     "18.3527C67.9672 18.92 67.7186 19.805 67.4021 20.985C67.0857 22.165 66.837 23.0273 66.6788 23.6173C66.498 24.2754 66." +
                                     "498 24.7292 66.6788 24.9562C66.8596 25.1831 67.2891 25.2739 67.9898 25.2512L67.7864 25.9773H61.6609L61.8418 25.3192Z" +
                                     "fill=\"#9B3589\"/>" +
                                "<path d=\"M80.4895 7.46033H87.8581C89.3273 7.46033 90.4123 7.5284 91.1356 7.64187C91.8589 7.75533 92.4918 8.02763 93.0568" +
                                 "8.4134C93.6219 8.79917 94.0966 9.3211 94.4808 9.95648C94.8651 10.5919 95.0459 11.318 95.0459 12.1123C95.0459 12.9746 94" +
                                 ".8199 13.7688 94.3452 14.4949C93.8705 15.2211 93.2377 15.7657 92.4465 16.1288C93.5767 16.4465 94.4356 17.0138 95.0459 1" +
                                 "7.808C95.6562 18.6023 95.95 19.5326 95.95 20.5992C95.95 21.4388 95.7466 22.2557 95.3623 23.05C94.9781 23.8442 94.4356 24." +
                                 "4796 93.7575 24.9561C93.0794 25.4327 92.2431 25.7277 91.2486 25.8411C90.6383 25.9092 89.1239 25.9546 86.7506 25.9773H80.4669V7.460" +
                                 "33H80.4895ZM84.1964 10.5465V14.8353H86.6376C88.0842 14.8353 88.9883 14.8126 89.3499 14.7673C89.9828 14.6992 90.4801 14.4723 90" +
                                 ".8643 14.1092C91.226 13.7461 91.4068 13.2696 91.4068 12.6569C91.4068 12.0896 91.2486 11.613 90.9321 11.2499C90.6157 10.8869 " +
                                 "90.141 10.6826 89.5308 10.6146C89.1691 10.5692 88.1068 10.5465 86.3437 10.5465H84.1964ZM84.1964 17.9215V22.8684H87.6321C88.9" +
                                 "657 22.8684 89.8246 22.823 90.1862 22.755C90.7513 22.6642 91.1808 22.4146 91.5424 22.0288C91.881" +
                                 "5 21.643 92.0623 21.1211 92.0623 20.463C92.0623 19.8957 91.9267 19.4419 91.6554 19.0561C91.3842 18.6703 90.9999 18.3753 90.5" +
                                 "027 18.2165C90.0054 18.035 88.8979 17.9442 87.2026" +
                                 "17.9442L84.1964 17.9215ZM115.705 25.9773H111.659L110.054 21.7792H102.686L101.171 25.9773H97.1932L104.381 7.46033H108.314L115" +
                                 ".705 25.9773ZM108.856 18.6703L106.325 11.7946L103.838 18.6703H108.856ZM117.717 25.9773V7.46033H121.333L128.883 " +
                                "19.8276V7.46033H132.341V25.9773H128.611L121.175 13.9049V25.9773H117.717ZM136.342 25.9773V7.46033H140.071V15.6749L147.598 7.4" +
                                "6033H152.616L145.677 14.6765L153 25.9773H148.186L143.123 17.2861L140.094 20.3723V25.9773H136.342Z\" " +
                                "fill=\"#234A98\">" +
                        "</svg>";
                    break;
                #endregion
                case "Anor Bank":
                    #region Anor Bank
                    qaytar = "<svg  id=\"BankIcon\" viewBox=\"0 0 194 28\">" +
                                  "<g transform=\"translate(-20.621 -20.621)\">" +
                                    "<g id=\"Сгруппировать_2889\" data-name=\"Сгруппировать 2889\" transform=\"translate(54.648 25.428)\">" +
                                     "<path id=\"Контур_25069\" data-name=\"Контур 25069\"" +
                                       "d=\"M146.33,38.166l7.316,17.727a14.777,14.777,0,0,1-2.951.335,10.6,10.6,0,0,1-1.691-.14l-1.414-3.545h-7.316l-1.414,3.545a17.1,17.1,0,0" +
                                       ",1-2.09.112,7.886,7.886,0,0,1-2.428-.307l7.378-17.727A13.677,13.677,0,0,1,144.086,38,12.812,12.812,0,0,1,146.33,38.166Zm-4.642,10.916" +
                                       "h4.488l-1.66-4.132L143.9,43.22l-.553,1.731Z\" transform=\"translate(-134.341 -37.58)\" fill=\"#a30041\"/>" +
                                      "<path id=\"Контур_25070\" data-name=\"Контур 25070\" d=\"M220.6,38a17.356,17.356,0,0,1,2.336.167V55.976a19.085,19.085,0,0,1-4.4.223l-" +
                                      "7.624-10.19-.646-1.061.123,1.284V56.06a16.691,16.691,0,0,1-2.305.167,17.768,17.768,0,0,1-2.244-.167V38.3a9.207,9.207,0,0,1,2.121-.321," +
                                      "13.909,13.909,0,0,1,2.275.042l7.593,10.413.676,1.088-.123-1.423V38.165A17.074,17.074,0,0,1,220.6,38Z\" transform=\"translate(-184.45 " +
                                      "-37.551)\" fill=\"#a30041\"/>" +
                                      "<path id=\"Контур_25071\" data-name=\"Контур 25071\" d=\"M283.291,55.618a12.11,12.11,0,0,1-4.119-.684,9.868,9.868,0,0,1-3.3-1.94,8.62" +
                                      "4,8.624,0,0,1-2.167-3.029,9.9,9.9,0,0,1,0-7.649,8.767,8.767,0,0,1,2.167-3.043,9.939,9.939,0,0,1,3.3-1.968,11.891,11.891,0,0,1,4.119-." +
                                      "7,12.244,12.244,0,0,1,4.135.684,9.683,9.683,0,0,1,3.3,1.94,8.73,8.73,0,0,1,2.152,3.029,9.623,9.623,0,0,1,.768,3.867,9.409,9.409,0,0,1-" +
                                      ".768,3.782,8.771,8.771,0,0,1-2.167,3.043,9.9,9.9,0,0,1-3.32,1.968A11.938,11.938,0,0,1,283.291,55.618Zm0-3.936a5.19,5.19,0,0,0,3.9-1.52" +
                                      "1,5.547,5.547,0,0,0,1.476-4.034,5.575,5.575,0,0,0-1.476-4.076,5.809,5.809,0,0,0-7.808,0,5.576,5.576,0,0,0-1.475,4.076,5.548,5.548,0,0" +
                                      ",0,1.475,4.034A5.19,5.19,0,0,0,283.291,51.682Z\" transform=\"translate(-231.462 -36.607)\" fill=\"#a30041\"/>" +
                                      "<path id=\"Контур_25072\" data-name=\"Контур 25072\" d=\"M363.7,48.674l4.611,6.9a17.084,17.084,0,0,1-3.873.335l-1.168-.028-4.057-6.28" +
                                      "1h-.215l-2.244-.028v6.2a12.7,12.7,0,0,1-2.275.167,14.138,14.138,0,0,1-2.337-.167V38.121q1.353-.251,3.228-.419t3.412-.167a18.681,18.68" +
                                      "1,0,0,1,3.151.265,8.2,8.2,0,0,1,2.72.935,4.909,4.909,0,0,1,2.551,4.55,6.928,6.928,0,0,1-.308,1.94,4.989,4.989,0,0,1-1.153,1.968A6.41" +
                                      "4,6.414,0,0,1,363.7,48.674Zm-6.947-7.537v5l2.428.028a3.7,3.7,0,0,0,2.521-.74,2.411,2.411,0,0,0,.83-1.884,2.179,2.179,0,0,0-.861-1.81" +
                                      "5,4.047,4.047,0,0,0-2.521-.67Q357.949,41.053,356.75,41.136Z\" transform=\"translate(-286.969 -37.256)\" fill=\"#a30041\"/>" +
                                      "<path id=\"Контур_25073\" data-name=\"Контур 25073\" d=\"M426.077,46.738a6.031,6.031,0,0,1,1.983,1.075,3.641,3.641,0,0,1,1.122,1.52" +
                                      "1,4.744,4.744,0,0,1-1.8,5.625q-2.106,1.438-6.471,1.438a38.611,38.611,0,0,1-6.517-.5V38.279a34.761,34.761,0,0,1,6.394-.558q7.839,0," +
                                      "7.839,5.137A3.837,3.837,0,0,1,426.077,46.738Zm-4.949-5.667q-.985,0-2.152.084V45.2h2.337a3.427,3.427,0,0,0,2.152-.545,1.939,1.939" +
                                      ",0,0,0,.676-1.605,1.672,1.672,0,0,0-.768-1.451A3.945,3.945,0,0,0,421.128,41.071Zm.246,11.976q3.474,0,3.474-2.345A1.89,1.89" +
                                      ",0,0,0,424,49.055a4.272,4.272,0,0,0-2.444-.586h-2.582v4.495Q420.144,53.047,421.374,53.047Z\" transform=\"translate(-3" +
                                      "30.598 -37.386)\" fill=\"#a30041\"/>" +
                                      "<path id=\"Контур_25074\" data-name=\"Контур 25074\" d=\"M478.78,38.166,486.1,55.894a14.781,14.781,0,0,1-2.951.335," +
                                      "10.6,10.6,0,0,1-1.691-.14l-1.414-3.545h-7.317l-1.414,3.545a17.1,17.1,0,0,1-2.09.112,7.887,7.887,0,0,1-2.429-.307l7" +
                                      ".378-17.727A13.676,13.676,0,0,1,476.536,38,12.816,12.816,0,0,1,478.78,38.166Zm-4.642,10.916h4.488l-1.66-4.132-.615-1" +
                                      ".731L475.8,44.95Z\" transform=\"translate(-367.315 -37.58)\" fill=\"#a30041\"/>" +
                                      "<path id=\"Контур_25075\" data-name=\"Контур 25075\" d=\"M553.05,38a17.354,17.354,0,0,1,2.336.167V55.976a19.084,19.084" +
                                      ",0,0,1-4.4.223l-7.624-10.19-.645-1.061.123,1.284V56.06a16.685,16.685,0,0,1-2.306.167,17.766,17.766,0,0,1-2.244-.167V38.3a" +
                                      "9.206,9.206,0,0,1,2.121-.321,13.909,13.909,0,0,1,2.275.042l7.593,10.413.676,1.088-.123-1.423V38.165A17.076,17.076,0,0,1,553" +
                                      ".05,38Z\" transform=\"translate(-417.424 -37.551)\" fill=\"#a30041\"/>" +
                                      "<path id=\"Контур_25076\" data-name=\"Контур 25076\" d=\"M618.609,46.607,627,55.987a23.5,23.5,0,0,1-3.474.223l-2.429-" +
                                      ".056-6.056-6.868-1.383.921v5.946a15.2,15.2,0,0,1-2.275.167,16.685,16.685,0,0,1-2.305-.167V38.259a14.383,14.383,0,0,1,2.213-" +
                                      ".167,17.57,17.57,0,0,1,2.367.167v7.258a27.838,27.838,0,0,0,3.75-3.294,34.556,34.556,0,0,0,3.2-4.048q1.075-.083,2.305-.084a" +
                                      "26.248,26.248,0,0,1,3.074.167A39.283,39.283,0,0,1,618.609,46.607Z\" transform=\"translate(-467.028 -37.645)\" fill=\"#a30041\"/>" +
                                    "</g>" +
                                    "<g id=\"Сгруппировать_2894\" data-name=\"Сгруппировать 2894\" transform=\"translate(20.621 20.621)\">" +
                                      "<g id=\"Сгруппировать_2891\" data-name=\"Сгруппировать 2891\" transform=\"translate(10.847 10.899)\">" +
                                        "<g id=\"Сгруппировать_2890\" data-name=\"Сгруппировать 2890\">" +
                                          "<path id=\"Контур_25077\" data-name=\"Контур 25077\" d=\"M63.043,59.969v3.1H59.957a3.093,3.093,0,0,1-3.084-3.1,3.1,3.1,0,0,1,." +
                                          "9-2.193,3.082,3.082,0,0,1,5.267,2.193Z\" transform=\"translate(-56.873 -56.868)\" fill=\"#a30041\"/>" +
                                        "</g>" +
                                      "</g>" +
                                      "<g id=\"Сгруппировать_2893\" data-name=\"Сгруппировать 2893\">" +
                                        "<g id=\"Сгруппировать_2892\" data-name=\"Сгруппировать 2892\">" +
                                          "<path id=\"Контур_25078\" data-name=\"Контур 25078\" d=\"M39.863,20.621H29.243a8.644,8.644,0,0,0-8.622,8.664V39.957a8.645,8.645,0," +
                                          "0,0,8.622,8.664H48.484V29.285A8.643,8.643,0,0,0,39.863,20.621Zm2.5,21.845H34.553a7.826,7.826,0,0,1-7.805-7.845,7.838,7.838,0,0,1,5." +
                                          "176-7.389V24.886a.182.182,0,0,1,.312-.128l1.253,1.279a.182.182,0,0,0,.276-.019l.643-.863a.182.182,0,0,1,.292,0l.644.863a.181.18" +
                                          "1,0,0,0,.276.019l1.254-1.279a.182.182,0,0,1,.312.128v2.346a7.842,7.842,0,0,1,5.175,7.389Z\" transform=\"translate(-20.621 -20.621)\" fill=\"#a30041\"/>" +
                                        "</g>" +
                                      "</g>" +
                                    "</g>" +
                                  "</g>" +
                                "</svg>";
                    break;
                #endregion
                case "Trust Bank":
                    #region Trust Bank
                    qaytar = "<svg  id=\"BankIcon\"  viewBox=\"0 0 146 64\"><g cl" +
                                "ass=\"logo__one\">    <path d=\"M88.9084 56.4961C89.036 55.3023 89." +
                                "1201 54.3636 89.1635 53.6807C89.2226 52.7478 " +
                                "89.2819 51.4678 89.3409 49.8436C89.3972 48.216" +
                                "5 89.428 47.0699 89.428 46.4006C89.428 46.162 " +
                                "89.4251 45.826 89.4188 45.3929C90.2279 45.409" +
                                "5 90.8689 45.4179 91.3419 45.4179C92.0048 45." +
                                "4179 92.7766 45.404 93.6574 45.3818C94.5377 45" +
                                ".3568 95.2943 45.343 95.9258 45.343C96.6884 45" +
                                ".343 97.2267 45.3627 97.5501 45.4013C97.9765 " +
                                "45.4485 98.3469 45.5375 98.6674 45.6624C98.98" +
                                "47 45.7873 99.2526 45.9401 99.4641 46.126C99." +
                                "7005 46.3426 99.8873 46.6009 100.021 46.9061" +
                                "C100.174 47.2533 100.248 47.6227 100.248 48." +
                                "0196C100.248 48.3277 100.205 48.6248 100.118" +
                                 "48.9053C100.027 49.1887 99.9031 49.4386 99." +
                                 "7411 49.6579C99.5824 49.8772 99.4019 50.06" +
                                 "05 99.1999 50.2077C99.0005 50.3548 98.8045" +
                                  "50.4687 98.6117 50.549C98.4217 50.6296 98" +
                                  ".1635 50.7102 97.843 50.7937C98.1636 50.8" +
                                  "407 98.4218 50.8964 98.6178 50.9602C98.81" +
                                  "06 51.0213 99.0194 51.1211 99.2404 51.259" +
                                  "9C99.4642 51.3988 99.6605 51.5764 99.8378" +
                                   "51.793C100.015 52.0097 100.146 52.2539 1" +
                                   "00.236 52.5206C100.323 52.7899 100.367 53" +
                                   ".0731 100.367 53.373C100.367 53.784 100.2" +
                                   "92 54.1727 100.14 54.5363C99.9869 54.9001 99.7754 55.2111 99.5014 55.4721C99.27" +
                                   "13 55.6941 98.9694 55.8829 98.5991 56.0412C98.2287 56.1994 97.8491 56.3076 97.4" +
                                   "602 56.3661C97.1426 56.4104 96.5669 56.4409 95.733 56.4632C94.8958 56.4855 94." +
                                   "0089 56.4965 93.0753 56.4965L90.676 56.4882L88.9084 56.4961ZM15.1455 52.7827C2" +
                                   "4.0254 39.1465 44.8161 29.6339 58.0547 33.3894C59.5198 33.9587 60.5218 32.1576" +
                                    "59.5167 31.267C48.307 21.3475 20.8769 35.8005 15.1455 52.7827ZM56.4089 35.09" +
                                    "73C55.5932 34.8226 55.4444 35.4835 56.0893 35.8176C62.9093 40.0436 70.0271 33.0" +
                                    "927 76.1943 33.7007C79.1216 33.9893 83.0495 34.7514 85.7191 35.1823C86.3786 35.2684 86.6893 34" +
                                    ".6545 85.6143 34.3166C81.778 33.1667 77.6891 29.9165 73.2999 29.9128C68.1457 29.9083 " +
                                    "62.1206 37.5813 56.4089 35.0973ZM92.9728 49.7186H94.2674C94.6938 49.7186 94.9958 49." +
                                    "7102 95.1731 49.6908C95.3535 49.6714 95.5279 49.6381 95.6989 49.588C95.87 49.5409 96.01" +
                                    "65 49.4578 96.1409 49.3437C96.2654 49.2299 96.3525 49.1022 96.4054 48.9633C96.458" +
                                    "2 48.8217 96.4833 48.68 96.4833 48.5384C96.4833 48.3386 96.4302 48.1609 96.3244" +
                                     "48.0027C96.2188 47.8444 96.0445 47.7279 95.8079 47.6528C95.5715 47.5777 95.2104 47.5" +
                                     "391 94.7251 47.5391H93.0849L92.9728 49.7186ZM92.7643 54.2665H94.5131C94.9768 " +
                                     "54.2665 95.3161 54.2415 95.5339 54.1944C95.8014 54.1333 96.0068 54.0555 96.1468 5" +
                                     "3.9584C96.2837 53.8583 96.3958 53.728 96.4767 53.5586C96.5608 53.3893 96.6013 53.2031 96.6" +
                                     "013 52.9976C96.6013 52.8173 96.5669 52.6616 96.5015 52.5256C96.4329 52.3897 96" +
                                     ".3335 52.2757 96.1998 52.1842C96.0659 52.0926 95.9041 52.0259 95.708 51.9842C95.515" +
                                     "2 51.9427 95.21 51.9232 94.7963 51.9232H92.8761L92.7643 54.2665ZM100.488 5" +
                                     "6.4961L101.244 55.3243L105.959 47.4029L106.46 46.5144C106.606 46.2563 106.808 45.88" +
                                     "12 107.07 45.3927C108.14 45.4037 108.921 45.4093 109.413 45.4093C110.272 45.4093 11" +
                                     "1.062 45.4037 111.794 45.3927L113.126 48.7855L115.833 55.3076L116.362 56.4958C115" +
                                     ".31 56.4848 114.672 56.4792 114.448 56.4792C113.938 56.4792 113.188 56.4848 112.195 " +
                                     "56.4958C111.928 55.6017 111.542 54.5495 111.038 53.3417H106.441C105.912 54.2969 105.3" +
                                     "64 55.3464 104.798 56.4958C103.668 56.4848 102.978 56.4792 102.722 56.4792C102.258 56" +
                                     ".4792 101.515 56.4851 100.488 56.4961ZM107.546 50.9736H110.381C109.675 49.0605 109.28" +
                                     "9 48.0053 109.223 47.8111C108.822 48.608 108.48 49.2606 108.203 49.7712L107.546 50.9" +
                                     "736ZM117.327 56.4961L117.51 54.7886L117.865 48.9967L117.965 46.6616V45.3927C118" +
                                     ".908 45.4037 119.739 45.4093 120.46 45.4093C120.977 45.4093 121.702 45.4037 122" +
                                     ".639 45.3927L126.545 51.7063L127.083 52.6336C127.148 51.7231 127.217 50.4458 127.285 " +
                                     "48.802C127.353 47.1582 127.388 46.0227 127.388 45.3925C128.138 45.4035 128.735 4" +
                                     "5.4091 129.187 45.4091C129.392 45.4091 129.986 45.4035 130.973 45.3925L130.755 " +
                                     "47.8164L130.347 54.5523L130.291 56.4958C129.501 56.4848 128.763 56.4792 128." +
                                     "075 56.4792C127.506 56.4792 126.715 56.4848 125.707 56.4958C125.39 55.9655 125." +
                                     "156 55.5795 125.004 55.3406L123.47 53.0223C122.966 52.2616 122.502 51.5369 122." +
                                     "076 50.8428C121.795 50.4013 121.506 49.9209 121.21 49.3933L121.021 53.25L120.92" +
                                     "7 56.4957C120.174 56.4847 119.601 56.4791 119.206 56.4791C118.87 56.4791 118.2" +
                                     "45 56.4851 117.327 56.4961ZM132.635 56.4961C132.75 55.4827 132.834 54.6413 132" +
                                     ".88 53.9749C132.974 52.792 133.07 51.1735 133.173 49.1188C133.248 47.6862 133." +
                                     "282 46.6782 133.282 46.0924V45.3928C134.296 45.4038 134.997 45.4094 135.379 45" +
                                     ".4094C135.635 45.4094 136.291 45.4038 137.349 45.3928L136.92 51.4678L136.702 5" +
                                     "6.4961L134.723 56.4795C134.443 56.4795 133.746 56.4851 132.635 56.4961ZM137.2" +
                                     "03 50.6377L139.244 48.1581L140.894 46.0924L141.407 45.3928C142.537 45.4038 14" +
                                     "3.302 45.4094 143.704 45.4094C143.813 45.4094 144.538 45.4038 145.882 45.3928" +
                                     "L142.036 49.7268L141.314 50.5959L141.989 51.5566L145.802 56.4961L143.558 56.4" +
                                     "795C143.072 56.4795 142.248 56.4851 141.087 56.4961C140.303 55.1774 139.008 " +
                                    "53.2255 137.203 50.6377Z\"></path></g>" +
                            "<g class=\"logo__two\">    <path d=\"M94.4322 32.172C9" +
                                "4.0446 31.7959 94.2233 31.5491 94.1472 31.0177C91" +
                                ".8067 14.6961 64.8101 10.3279 52.0743 10.4387C48." +
                                "7364 10.3578 47.2174 8.85457 44.267 6.68471C41.31" +
                                "63 4.51498 36.3696 1.82431 34.5048 1.29207C30.801" +
                                "7 0.234414 33.7599 5.11449 34.5215 6.04403C34.305" +
                                "4 2.22984 43.3894 9.66299 44.5503 10.5077C47.1474" +
                                 "12.3972 48.969 13.2695 51.9062 13.3421C62.6684 13" +
                                 ".2563 73.0275 15.8985 79.5799 19.5444C86.1324 23." +
                                 "1897 88.2244 25.8037 88.897 30.4253C88.965 30.887" +
                                 "6 88.8081 31.102 89." +
                                "1494 31.4302C89.6995 31.9306 94.4314 36.3038 91.78" +
                                "78 36.7599C95.4253 38.2226 97.8445 38.4105 98.3025 " +
                                "37.5997C98.8212 36.6809 96.9801 34.5168 94.4322 32" +
                                ".172ZM36.1132 7.58815C37.1619 9.66536 37.3917 12.1128 34.5377 14.106C21.5943 22.474 9." +
                                "30173 33.6603 7.52441 49.8214C7.44225 50.6211 7.40872 51.1497 6.74074 51.6666C3.40726" +
                                 "54.2468 1.2942 57.7886 0.0985137 61.7776C-0.358916 63.424 0.900622 62.2408 1.33919 61" +
                                 ".7815C3.68711 59.2315 6.42722 61.5016 8.4394 58.444C9.81155 59.4674 12.0522 57.7431 1" +
                                 "3.047 59.5935C13.2934 60.051 13.6192 60.1154 13.9713 59.7279C15.491 58.0562 14.7669 5" +
                                 "6.2754 14.2133 54.5209C13.9589 54.4832 13.7179 54.4006 13.5014 54.2413C11.7336 56.90" +
                                 "1 9.32059 54.9235 7.25615 57.1415C6.87026 57.541 5.76343 58.5701 6.16497 57.1371C7.214" +
                                 "8 53.667 9.60435 50.7509 12.5363 48.5075C13.1237 48.0584 13.1527 47.5983 13.2247 46.90" +
                                 "23C14.7794 32.8421 26.1052 23.6674 37.4883 16.3934C41.2775 13.7779 37.8345 9.54046 36." +
                                 "1132 7.58815ZM29.3356 56.4961C29.6406 53.8612 29.8209 50.9597 29.8834 47.7916L25.9996" +
                                  "47.8804L26.0432 46.9779C26.0679 46.4949 26.0868 45.9673 26.0993 45.3926C27.8262 45.4" +
                                  "036 29.8367 45.4092 32.1332 45.4092L34.3863 45.4008H36.5738L37.7596 45.3926L37.7034 " +
                                  "46.1896C37.6784 46.6172 37.6629 46.9697 37.6537 47.2474C37.6443 47.5277 37.6413 47.7" +
                                  "387 37.6413 47.8802C36.3404 47.8219 35.2669 47.7915 34.4236 47.7915H33.7857C33.6951 " +
                                  "48.7493 33.6237 49.6352 33.5771 50.4457C33.5085 51.609 33.4557 52.7391 33.4214 53.835" +
                                  "9C33.3841 54.9327 33.3655 55.8183 33.3655 56.4958C32.3695 56.4848 31.7254 56.4792 31." +
                                  "4328 56.4792C31.1714 56.4792 30.4715 56.485 29.3356 56.4961ZM38.3978 56.4961C38.5129 " +
                                  "55.5408 38.6063 54.683 38.6718 53.9166C38.7401 53.1533 38.7991 52.2674 38.8459 51.256" +
                                  "8C38.8959 50.2488 38.9331 49.1854 38.9581 48.0636C38.9797 46.9448 38.9923 46.0534 38.9" +
                                  "923 45.3927C39.7517 45.4093 40.3677 45.4177 40.8345 45.4177C41.3201 45.4177 41.9145 45" +
                                  ".4038 42.6114 45.3816C43.3116 45.3566 43.9901 45.3428 44.653 45.3428C45.4308 45.3428 46.12" +
                                  "51 45.3706 46.7319 45.4262C47.1893 45.4622 47.603 45.5428 47.9764 45.6622C48.3499 45.7817" +
                                   "48.6641 45.926 48.9192 46.0954C49.1745 46.2676 49.3892 46.4589 49.5572 46.67C49.7286 " +
                                   "46.881 49.8591 47.1283 49.9558 47.4144C50.049 47.6974 50.0957 48.0029 50.0957 48.3276" +
                                   "C50.0957 48.6914 50.0428 49.0329 49.9403 49.3494C49.8375 49.6659 49.6758 49.9658 49." +
                                   "4516 50.2435C49.2304 50.5239 48.9504 50.7656 48.6144 50.9682C48.2782 51.1708 47.8053" +
                                    "51.3792 47.1954 51.59L47.6716 52.623C47.8053 52.9117 47.9484 53.2034 48.1009 53.503" +
                                    "1L49.3209 55.8855C49.3771 55.9882 49.4735 56.1936 49.6135 56.4963C48.6113 56.4853 " +
                                    "47.9359 56.4797 47.5905 56.4797C47.1518 56.4797 46.4361 56.4853 45.4402 56.4963C45" +
                                    ".3407 56.213 45.1665 55.691 44.9081 54.9276C44.8053 54.6056 44.7184 54.3612 44.6437 5" +
                                    "4.1864C44.4975 53.8002 44.2828 53.2617 43.9964 52.5676L43.0566 50.2658C43.3865 50.3186" +
                                     "43.707 50.3464 44.0244 50.3464C44.4665 50.3464 44.871 50.2879 45.232 50.1684C45.5929" +
                                      "50.049 45.8667 49.8631 46.0534 49.6133C46.2373 49.3634 46.3306 49.1025 46.3306 48.8" +
                                      "248C46.3306 48.5915 46.262 48.386 46.122 48.2111C45.982 48.0335 45.7611 47.903 45.4" +
                                      "59 47.8169C45.1603 47.7308 44.6968 47.6864 44.0711 47.6864C43.8564 47.6864 43.6385 " +
                                      "47.689 43.4082 47.6974C43.1811 47.7058 42.9166 47.7142 42.6115 47.7197C42.5337 48.83" +
                                      "03 42.4623 50.1853 42.403 51.7873C42.3409 53.3867 42.3002 54.9556 42.2816 56.4963C41" +
                                      ".2672 56.4853 40.6076 56.4797 40.3055 56.4797C39.9508 56.4797 39.3159 56.485 38.397" +
                                      "8 56.4961ZM49.8685 56.4961L50.6246 55.3243L55.3395 47.4028L55.8405 46.5143C55.9866 " +
                                      "46.2563 56.1889 45.8812 56.4503 45.3926C57.5208 45.4036 58.302 45.4092 58.7935 45.4" +
                                      "092C59.6524 45.4092 60.4429 45.4036 61.1741 45.3926L62.5062 48.7855L65.2136 55.3075L" +
                                      "65.7427 56.4958C64.6908 56.4848 64.0528 56.4792 63.8289 56.4792C63.3185 56.4792 62.5" +
                                      "685 56.4848 61.5758 56.4958C61.3083 55.6017 60.9224 54.5495 60.4183 53.3417H55.8218C" +
                                      "55.2929 54.2969 54.7453 55.3463 54.1787 56.4958C53.049 56.4848 52.3582 56.4792 52.10" +
                                      "29 56.4792C51.6394 56.4792 50.8955 56.485 49.8685 56.4961ZM56.9264 50.9735H59.7615C5" +
                                      "9.0549 49.0604 58.669 48.0053 58.6036 47.811C58.2024 48.608 57.8601 49.2605 57.583 4" +
                                      "9.7711L56.9264 50.9735ZM66.5891 56.3238C66.5082 55.1689 66.4179 54.2444 66.3154 53.5" +
                                      "501C66.9969 53.8223 67.6036 54.0111 68.1359 54.1166C68.665 54.2221 69.1318 54.2748 6" +
                                      "9.5331 54.2748C70.0621 54.2748 70.4855 54.2053 70.7965 54.0665C71.111 53.9306 71.266" +
                                      "5 53.7251 71.2665 53.453C71.2665 53.3615 71.251 53.2724 71.2198 53.1893C71.1918 53.105" +
                                      "8 71.1328 53.0116 71.0484 52.9033C70.9616 52.7976 70.8402 52.684 70.6844 52.5589L68.9" +
                                      "699 51.2262C68.5686 50.9207 68.2885 50.7012 68.1299 50.5653C67.8092 50.2905 67.5572 5" +
                                      "0.0322 67.3797 49.7933C67.1992 49.5547 67.0686 49.3131 66.9877 49.066C66.9036 48.8187" +
                                       "66.8632 48.558 66.8632 48.2803C66.8632 47.9499 66.9285 47.6248 67.0591 47.3083C67.18" +
                                       "99 46.9918 67.3859 46.6949 67.6473 46.4172C67.909 46.1424 68.2544 45.9035 68.6868 45." +
                                       "7009C69.1163 45.501 69.5798 45.3648 70.0716 45.2927C70.5632 45.2176 71.0706 45.1816" +
                                        "71.5934 45.1816C72.6204 45.1816 73.7376 45.2871 74.948 45.498C74.9669 45.8729 74.98" +
                                        "85 46.2146 75.0102 46.5283C75.0352 46.839 75.0943 47.3864 75.1845 48.1664C74.6244 4" +
                                        "7.9665 74.1171 47.8276 73.6534 47.7471C73.1929 47.6696 72.7946 47.6306 72.4615 47.63" +
                                        "06C71.9886 47.6306 71.5934 47.7084 71.2822 47.8665C70.9679 48.0221 70.8123 48.2193 7" +
                                        "0.8123 48.4524C70.8123 48.5608 70.837 48.6663 70.8837 48.7689C70.9337 48.8717 71.023" +
                                        "7 48.9912 71.1577 49.1298C71.2915 49.2686 71.4688 49.4214 71.6868 49.5851C71.9046 4" +
                                        "9.7517 72.25 49.9961 72.7168 50.3208L73.3733 50.7845C73.5661 50.926 73.7748 51.0983" +
                                         "74.002 51.2982C74.226 51.4981 74.419 51.6951 74.5839 51.8923C74.7489 52.0865 74.873" +
                                         "4 52.2671 74.9635 52.4283C75.0507 52.5919 75.1161 52.756 75.1597 52.9197C75.2001 5" +
                                         ".0863 75.2219 53.2641 75.2219 53.4527C75.2219 53.9413 75.0912 54.405 74.8298 54.84" +
                                         "92C74.5684 55.2908 74.198 55.6546 73.7189 55.9377C73.2364 56.2238 72.7105 56.421 72" +
                                         ".1348 56.5346C71.5621 56.6484 70.9552 56.7068 70.3174 56.7068C69.3341 56.7068 68.089" +
                                         "1 56.5792 66.5891 56.3238ZM79.79 56.4961C80.095 53.8612 80.2754 50.9597 80.33" +
                                         "77 47.7916L76.454 47.8804L76.4976 46.9779C76.5226 46.4949 76.5412 45.9673 76.5538 45" +
                                         ".3926C78.2809 45.4036 80.291 45.4092 82.5877 45.4092L84.8407 45.4008H87.0285L88.2142" +
                                          "45.3926L88.158 46.1896C88.133 46.6172 88.1175 46.9697 88.1083 47.2474C88.0989 47.52" +
                                          "77 88.0958 47.7387 88.0958 47.8802C86.7952 47.8219 85.7214 47.7915 84.878 47.7915H" +
                                          "84.2401C84.15 48.7493 84.0782 49.6352 84.0315 50.4457C83.9633 51.609 83.9105 52.73" +
                                          "91 83.8761 53.8359C83.8387 54.9327 83.82 55.8183 83.82 56.4958C82.8241 56.4848 82" +
                                          ".18 56.4792 81.8874 56.4792C81.6261 56.4792 80.9262 56.485 79.79 56.4961Z\"></path></g>" +
                                      "</svg>";
                    break;
                #endregion
                case "AsiaAllianceBank":
                    qaytar = "<img   id=\"BankIcon\" src=\"https://aab.uz/bitrix/templates/main/images/logo.png\">";
                    break;
                case "Xalq Banki":
                    qaytar = "<img  id=\"BankIcon\" src=\"https://www.xb.uz/static/media/logo.8da5216f.png\"";
                    break;
                case "Universal Bank":
                    qaytar = "<img  id=\"BankIcon\" src=\"https://universalbank.uz/storage/settings/June2020/UVS1FjXAGJMaGwrHmWZ7.png\">";
                    break;
                case "Aloqa Bank":
                    #region Aloqa Bank
                    qaytar = "<svg  id=\"BankIcon\" viewBox=\"0 0 800 127.3\"" +
                                "xml:space=\"preserve\">" +
                                "<g fill-rule=\"evenodd\" clip-rule=\"evenodd\">" +
                                    "<path d=\"M0 57v56.9l113.1.1c1-4 .6-50.3.2-57.1-7.2-3.2-54.1-28.1-56.4-28.2C55.1 28.6 5.9 54.3 0 57z\"" +
                                        "fill=\"#0077b9\" />" +
                                    "<path" +
                                        "d=\"M381.6 32.2c0-4.7-.8-13.1 4.5-13.2 5.7-.2 4.7 8.1 4.7 12.7v49.1c0 5 1.2 15.8-4.2 16.2-4 .3-4.4-2.2-4.7-5.7-.9-9.5-.3-47.4-.3-59.1zm11.5 95.1h26.7l.2-13.5c-9.5-1.2-18.8 2.6-17.7-5.8 6.1-2.7 10.1-3.1 13.6-7.9 6.3-8.6 4-47.1 4-58.5 0-14.1-.7-25.8-8.8-32.8-15.3-13.3-48.5-12.7-56.4 10.3-3.1 9.2-2.2 28.9-2.2 39.8 0 22.4-2.9 39.6 12.6 50.2 5 3.4 9.2 3.6 16.1 4.7.1 6.3.5 10.8 6.5 12.4l5.4 1.1zm138.7-63.8c9.6.4 9.5 2.1 9.5 11.6v7.8c0 9.1-.2 11.1-9.5 11.6v-31zm0-42c9.5.3 9.6 2.9 9.5 12.3-.1 10 .6 12.1-9.5 12.3V21.5zm-29.2 92.1c12.1 0 24.1.1 36.1 0 28-.3 33.6-3.4 31.5-40-.7-12.1-5.5-17.3-16.1-19.7 4.5-3.9 13.7-.3 14.1-19.1.5-25.7-9.7-31.7-32.6-32.3-10.9-.3-22-.1-32.9-.1l-.1 111.2zm150-.1l24.3.1.1-48.9 15 48.7c3.2.9 20.8.3 25.4.2l.2-111.1h-24.3l-.1 49.4c-.5-2.3-16-48.4-16.6-49.3l-23.9-.1-.1 111zM304.3 32.2c0-4.7-.8-13.1 4.5-13.2 5.7-.2 4.7 8.1 4.7 12.7v49.1c0 5 1.2 15.8-4.2 16.2-4 .3-4.4-2.2-4.7-5.7-.9-9-.3-47.8-.3-59.1zM302.9.5c-13.2 2.1-22.6 7.8-25.9 19.7-2.7 9.8-1.8 32.9-1.8 44.4 0 13.7-1.1 30.7 6 39.6 14 17.7 52.6 16.1 59.7-8.6 2.6-9.3 1.7-32.8 1.7-44.1 0-14.2 1.2-29.9-5.8-39.5-6-8.1-19.1-13.8-33.9-11.5zm426.4 113.1h29.1l.1-45c1.7 1.7 5.6 20.2 6.1 22.5 1.9 7.7 3.5 15.1 5.6 22.6h29.7c-.3-2.9-6.7-23.1-8.1-27.9-2.8-9.5-5.5-18.6-8.4-28.2-2.1-7-.8-7.2 1.5-13.8 1.5-4.6 3-8.9 4.6-13.7 1.6-4.7 3-9.1 4.6-13.7 1.3-4 3.8-10.1 4.4-13.9h-27.1c-1.5 5.5-11.2 39.2-12.7 41.3l-.2-41.1-29-.3-.2 111.2zM605.9 73.9c-.3-6.7 1.5-16.6 2.2-23.4.8-7 2.5-15.5 3-22.3l4.4 45.7h-9.6zm-31.7 39.6l30.1.1 1.7-19.7 10.5-.3 1.4 19.8 29.6.2L631 2.5l-42-.1-14.8 111.1zM177.5 73.9c-.2-8.6 3.9-34.1 5.1-44.7.7 1.7 4.6 40.7 4.5 44.7h-9.6zm-31.6 39.6l30.1.1 1.8-19.9h10.5l1.4 19.9h29.7L202.7 2.4h-42l-14.8 111.1zm309.8-39.6c0-6 4-41.3 5.5-45l4.3 45h-9.8zm-31.6 39.7h30.2L456 94c2.3-.6 8.2-.6 10.5 0l1.4 19.5 29.6.2L481 2.4h-42l-14.9 111.2zm-199.7-.2l46.7.2.1-22.2-17.6-.1-.1-88.8h-29.1v110.9z\"" +
                                        "fill=\"#0093dc\" />" +
                                    "<path d=\"M0 42.9c3.5-.5 53.8-27.6 56.5-27.9 3.1-.3 48.4 25 56.7 27.9l.1-41.5L0 1.6v41.3z\" fill=\"#30b1ea\" />" +
                                "</g>" +
                            "</svg>";
                    break;
                #endregion
                case "Hamkor Bank":
                    #region Hamkor Bank
                    qaytar = "<svg  id=\"BankIcon\" viewBox=\"0 0 1117 154\" >" +
                                "<g id=\"Page-1\" stroke=\"none\" stroke-width=\"1\" fill=\"none\" fill-rule=\"evenodd\">" +
                                    "<g id=\"dw\" transform=\"translate(-20.000000, 0.000000)\">" +
                                        "<g id=\"English\" transform=\"translate(20.000000, 0.000000)\">" +
                                            "<path" +
                                                "d=\"M665.871,118.578 L665.871,54.941 L704.461,54.656 C709.824,54.617 714.617,54.836 718.828,55.133 C721.992,55.359 724.883,55.801 727.504,56.488 C730.129,57.172 732.355,58 734.184,58.969 C736.012,59.957 737.57,61.055 738.816,62.262 C740.082,63.473 741.082,64.887 741.863,66.527 C742.625,68.152 743.07,69.902 743.203,71.762 C743.352,73.848 743.125,75.805 742.547,77.621 C741.965,79.434 740.973,81.152 739.539,82.742 C738.133,84.352 736.297,85.734 734.062,86.895 C731.828,88.059 728.652,89.25 724.531,90.461 L743.625,117.988 L717.395,117.988 L695.457,82.871 C697.75,83.172 700.527,83.234 702.715,83.234 C705.762,83.234 709.137,83.215 711.578,82.527 C714.02,81.844 714.668,80.562 715.852,79.129 C717.016,77.699 717.555,76.203 717.441,74.613 C717.348,73.273 716.793,72.098 715.754,71.094 C714.715,70.078 713.141,69.328 711.023,68.836 C708.93,68.344 705.715,68.086 701.402,68.086 C699.922,68.086 698.418,68.109 696.836,68.152 L692.004,68.277 L692.648,118.578 L665.871,118.578 Z\"" +
                                                "id=\"Fill-27\" fill=\"#006430\"></path>" +
                                            "<polygon id=\"Fill-28\" fill=\"#006430\"" +
                                                "points=\"366.961 55.684 401.09 55.684 414.266 94.152 427.379 55.684 461.391 55.684 461.391 118.914 440.203 118.914 440.203 70.699 423.766 118.914 404.59 118.914 388.211 70.699 388.211 118.914 366.961 118.914\">" +
                                            "</polygon>" +
                                            "<path" +
                                                "d=\"M322.242,94.801 L312.957,72.07 L303.73,94.801 L322.242,94.801 Z M327.676,108.477 L298.121,108.477 L294.055,118.914 L267.551,118.914 L299.098,55.684 L327.383,55.684 L358.949,118.914 L331.797,118.914 L327.676,108.477 Z\"" +
                                                "id=\"Fill-29\" fill=\"#006430\"></path>" +
                                            "<polygon id=\"Fill-30\" fill=\"#006430\"" +
                                                "points=\"178.441 55.684 204.379 55.684 204.379 77.812 232.738 77.812 232.738 55.684 258.793 55.684 258.793 118.914 232.738 118.914 232.738 93.344 204.379 93.344 204.379 118.914 178.441 118.914\">" +
                                            "</polygon>" +
                                            "<path" +
                                                "d=\"M505.199,85.336 C513.738,75.555 522.52,65.844 530.34,55.695 C539.977,55.777 549.586,55.766 559.223,55.695 L531.719,85.102 L562.93,118.445 C552.781,118.379 542.648,118.359 532.5,118.445 C526.938,110.992 517.836,99.961 505.199,85.336 Z M477.895,118.445 L504.199,118.445 L504.199,55.695 L477.895,55.695 L477.895,118.445 Z\"" +
                                                "id=\"Fill-31\" fill=\"#006430\"></path>" +
                                            "<path" +
                                                "d=\"M590.07,54.504 L628.957,54.504 C642.453,54.504 653.488,64.277 653.488,76.223 L653.488,96.848 C653.488,108.793 642.453,118.566 628.957,118.566 L590.07,118.566 C576.574,118.566 565.539,108.793 565.539,96.848 L565.539,76.223 C565.539,64.277 576.574,54.504 590.07,54.504 Z M599.547,70.117 L619.48,70.117 C626.395,70.117 632.051,75.129 632.051,81.25 L632.051,91.82 C632.051,97.941 626.395,102.949 619.48,102.949 L599.547,102.949 C592.633,102.949 586.977,97.941 586.977,91.82 L586.977,81.25 C586.977,75.129 592.633,70.117 599.547,70.117 Z\"" +
                                                "id=\"Fill-32\" fill=\"#006430\"></path>" +
                                            "<polygon id=\"Fill-33\" fill=\"#55AE24\"" +
                                                "points=\"937.262 55.684 961.5 55.684 993.109 90.664 993.109 55.684 1017.559 55.684 1017.559 118.914 993.109 118.914 961.656 84.199 961.656 118.914 937.262 118.914\">" +
                                            "</polygon>" +
                                            "<path" +
                                                "d=\"M892.094,94.801 L882.809,72.07 L873.582,94.801 L892.094,94.801 Z M897.527,108.477 L867.973,108.477 L863.906,118.914 L837.402,118.914 L868.949,55.684 L897.234,55.684 L928.801,118.914 L901.648,118.914 L897.527,108.477 Z\"" +
                                                "id=\"Fill-34\" fill=\"#55AE24\"></path>" +
                                            "<path" +
                                                "d=\"M782.891,105.328 L796.121,105.328 C800.598,105.328 803.742,104.734 805.562,103.547 C807.395,102.348 808.316,100.746 808.316,98.73 C808.316,96.863 807.418,95.359 805.602,94.227 C803.781,93.094 800.617,92.52 796.062,92.52 L782.891,92.52 L782.891,105.328 Z M782.891,80.492 L794.168,80.492 C798.215,80.492 801.035,79.969 802.629,78.914 C804.219,77.863 804.996,76.34 804.996,74.367 C804.996,72.527 804.215,71.082 802.629,70.051 C801.027,69.008 798.27,68.492 794.344,68.492 L782.891,68.492 L782.891,80.492 Z M756.715,55.684 L805.289,55.684 C813.383,55.684 819.594,57.207 823.914,60.219 C828.25,63.238 830.426,66.977 830.426,71.422 C830.426,75.16 828.887,78.375 825.793,81.051 C823.727,82.836 820.711,84.246 816.742,85.273 C822.762,86.363 827.207,88.242 830.055,90.898 C832.898,93.555 834.316,96.906 834.316,100.926 C834.316,104.207 833.305,107.156 831.285,109.773 C829.258,112.398 826.496,114.453 822.977,115.984 C820.809,116.93 817.523,117.617 813.125,118.047 C807.281,118.617 803.41,118.914 801.496,118.914 L756.715,118.914 L756.715,55.684 Z\"" +
                                                "id=\"Fill-35\" fill=\"#55AE24\"></path>" +
                                            "<path" +
                                                "d=\"M1059.309,85.336 C1067.848,75.555 1076.625,65.844 1084.445,55.695 C1094.082,55.777 1103.691,55.766 1113.328,55.695 L1085.824,85.102 L1117.035,118.445 C1106.887,118.379 1096.754,118.359 1086.605,118.445 C1081.043,110.992 1071.945,99.961 1059.309,85.336 Z M1032.004,118.445 L1058.305,118.445 L1058.305,55.695 L1032.004,55.695 L1032.004,118.445 Z\"" +
                                                "id=\"Fill-36\" fill=\"#55AE24\"></path>" +
                                            "<polygon id=\"Fill-37\" fill=\"#55AE24\" points=\"51.078 31.301 95.301 31.301 44.602 153.855 0.379 153.855\">" +
                                            "</polygon>" +
                                            "<polygon id=\"Fill-38\" fill=\"#55AE24\" points=\"114.035 0 158.258 0 107.559 122.555 63.336 122.555\">" +
                                            "</polygon>" +
                                        "</g>" +
                                    "</g>" +
                                "</g>" +
                            "</svg>";
                    #endregion
                    break;
                case "SQB":
                    #region SQB
                    qaytar = "<svg id=\"BankIcon\" viewBox=\"0 0 101 36\" fill=\"none\">" +
                                "<g clip-path=\"url(#clip0)\">" +
                                    "<path fill-rule=\"evenodd\" clip-rule=\"evenodd\"" +
                                        "d=\"M33.2849 8.59693C31.7449 6.08006 29.6032 3.97214 27.061 2.47366C24.3939 0.901925 21.2821 0 17.9566 0C14.6312 0 11.5192 0.901725 8.85229 2.47366C6.30988 3.97204 4.16843 6.08006 2.62823 8.59683C5.42552 9.56461 7.6456 11.7509 8.66917 14.508C9.17103 13.1564 9.95899 11.9442 10.9599 10.9452C12.7524 9.15596 15.2259 8.04956 17.9566 8.04956C20.6881 8.04956 23.1625 9.15666 24.9549 10.9468C25.9547 11.9455 26.7423 13.1568 27.2438 14.5072C28.2696 11.7415 30.492 9.56521 33.2849 8.59693Z\"" +
                                        "fill=\"#B1B3B3\" />" +
                                    "<path fill-rule=\"evenodd\" clip-rule=\"evenodd\"" +
                                        "d=\"M26.6252 22.7547C26.1746 23.5679 25.613 24.3106 24.9612 24.9627C23.1684 26.755 20.6917 27.8636 17.9566 27.8636C15.2209 27.8636 12.7441 26.7547 10.9514 24.9619C9.15856 23.1691 8.04966 20.6922 8.04966 17.9567C8.04966 14.174 5.60824 10.8456 2.00982 9.69092C1.3937 10.8768 0.906216 12.1384 0.565824 13.4577C0.196491 14.889 0 16.3966 0 17.9568C0 22.9153 2.00992 27.4046 5.25937 30.654C8.50881 33.9034 12.998 35.9133 17.9566 35.9133C20.3077 35.9133 22.5369 34.9751 24.1785 33.2907C25.7008 31.7288 26.6368 29.5966 26.6276 27.247L26.6251 22.7548L26.6252 22.7547Z\"" +
                                        "fill=\"#003D64\" />" +
                                    "<path fill-rule=\"evenodd\" clip-rule=\"evenodd\"" +
                                        "d=\"M35.9131 17.9567C35.9131 16.3965 35.7165 14.8889 35.3472 13.4576C35.0072 12.1385 34.5197 10.8767 33.9032 9.69092C30.3197 10.8298 27.8699 14.1714 27.8612 17.9341V27.2444C27.8716 30.0963 26.647 32.8029 24.5038 34.6831C26.8659 33.7577 28.9847 32.3464 30.7366 30.572C33.9373 27.3299 35.913 22.8748 35.913 17.9567H35.9131Z\"" +
                                        "fill=\"#D52023\" />" +
                                    "<path fill-rule=\"evenodd\" clip-rule=\"evenodd\"" +
                                        "d=\"M51.2578 27.9347C53.5405 27.9347 55.4155 27.3914 56.8558 26.2771C58.2961 25.163 59.0297 23.6955 59.0297 21.8205C59.0297 20.1901 58.5133 18.9128 57.4807 18.0161C56.448 17.1194 55.0079 16.3585 53.1871 15.7607C52.3992 15.5161 51.149 15.19 50.5786 14.9998L49.709 14.7009C49.1383 14.4564 48.9208 14.1846 48.9208 13.8312C48.9208 12.989 49.7632 12.5813 51.421 12.5813C52.5079 12.5813 53.622 12.7715 54.7906 13.1248C55.9591 13.4781 56.9644 13.9129 57.8068 14.4292V9.72798C55.8229 8.66818 53.6762 8.15186 51.3664 8.15186C46.8011 8.15186 43.8933 10.1898 43.8933 13.8586C43.8933 14.9727 44.2194 15.9782 44.5728 16.6576C44.9804 17.3369 45.8772 17.989 46.4207 18.3967C46.7197 18.5869 47.1272 18.7772 47.6707 18.9945L48.9206 19.4836L50.3066 19.8912L52.0186 20.4076C52.3446 20.5162 53.024 20.7066 53.2414 20.8968C53.6761 21.1956 53.9751 21.4401 53.9751 22.0108C53.9751 23.0162 52.9969 23.5054 51.0673 23.5054C49.8445 23.5054 48.5403 23.2608 47.1543 22.7446C45.7685 22.2282 44.6814 21.6304 43.8932 20.8968V25.8969C45.8771 27.2556 48.3228 27.9348 51.2576 27.9348L51.2578 27.9347Z\"" +
                                        "fill=\"#003D64\" />" +
                                    "<path fill-rule=\"evenodd\" clip-rule=\"evenodd\"" +
                                        "d=\"M94.2065 27.5598C96.2445 27.5598 97.9023 27.0707 99.1251 26.0652C100.375 25.0597 101 23.7283 101 22.0978C101 20.712 100.62 19.5977 99.886 18.701C99.1524 17.8043 98.3099 17.2607 97.386 17.0434C98.5545 16.5 99.6142 15.1955 99.6142 13.3476C99.6142 10.2226 97.1686 8.53784 93.6904 8.53784H84.4242V27.5598H94.2067H94.2065ZM89.3151 19.4348H93.2553C94.6956 19.4348 95.6467 20.1957 95.6467 21.4727C95.6467 22.75 94.7499 23.4837 93.2553 23.4837H89.3151V19.4348ZM89.3151 12.5054H92.9566C94.1522 12.5054 94.9131 13.1304 94.9131 14.1902C94.9131 15.1956 94.1522 15.8206 92.9566 15.8206H89.3151V12.5054Z\"" +
                                        "fill=\"#003D64\" />" +
                                    "<path fill-rule=\"evenodd\" clip-rule=\"evenodd\"" +
                                        "d=\"M77.4204 32.4726C78.5415 32.4726 79.5534 32.2538 80.4832 31.8437V27.7687C79.7997 28.1516 79.0338 28.343 78.2135 28.343C77.1469 28.343 76.2718 27.9602 75.6155 27.1672C77.3929 26.374 78.7879 25.1435 79.827 23.5298C80.8661 21.9164 81.3859 20.1114 81.3859 18.115C81.3859 15.3529 80.4285 13.001 78.5143 11.0593C76.6001 9.11751 74.1934 8.1604 71.2945 8.1604C68.3957 8.1604 65.9891 9.11761 64.0748 11.0593C62.1605 13.001 61.2031 15.3528 61.2031 18.115C61.2031 20.713 62.051 22.9829 63.7739 24.8699C65.4969 26.7568 67.6846 27.7962 70.3374 28.0149C72.9902 31.2967 75.1232 32.4726 77.4204 32.4726ZM67.6845 21.9984C66.7274 20.9865 66.2623 19.674 66.2623 18.1151C66.2623 16.5562 66.7274 15.2436 67.6845 14.2318C68.6417 13.2199 69.8451 12.7005 71.2944 12.7005C72.7439 12.7005 73.9197 13.22 74.8769 14.2318C75.8342 15.2437 76.3264 16.5562 76.3264 18.1151C76.3264 19.674 75.8342 20.9866 74.8769 21.9984C73.9196 23.0103 72.7438 23.5298 71.2944 23.5298C69.845 23.5298 68.6416 23.0102 67.6845 21.9984Z\"" +
                                        "fill=\"#003D64\" />" +
                                "</g>" +
                                "<defs>" +
                                    "<clipPath id=\"clip0\">" +
                                        "<rect width=\"101\" height=\"35.9131\" fill=\"white\" />" +
                                    "</clipPath>" +
                                "</defs>" +
                            "</svg>";
                    break;
                #endregion
                case "Kapital Bank":
                    qaytar = "<img  id=\"BankIcon\" src=\"https://kapitalbank.uz/bitrix/templates/main/images/Kapitalbank-Logo_Wh.png\">";
                    break;
                case "IpakYuliBank":
                    qaytar = "<img id=\"BankIcon\" src=\"https://ipakyulibank.uz/logo_uz.png\">";
                    break;
                case "Agrobank":
                    #region Agrobank
                    qaytar = "<svg id=\"BankIcon\" viewBox=\"0 0 697 136\" fill=\"none\">" +
    "<g clip-path=\"url(#clip0_204_2)\">" +
        "<g style=\"mix-blend-mode:darken\">" +
            "<g style=\"mix-blend-mode:darken\">" +
                "<g style=\"mix-blend-mode:darken\">" +
                    "<path d=\"M0 135.01H67.5L0 67.5V135.01Z\" fill=\"#00CD69\" />" +
                "</g>" +
                "<g style=\"mix-blend-mode:darken\">" +
                    "<path d=\"M67.5 135.01H135.01V67.5L67.5 135.01Z\" fill=\"#00CD69\" />" +
                "</g>" +
                "<g style=\"mix-blend-mode:darken\">" +
                    "<path d=\"M67.5 0L135.01 67.5V0H67.5Z\" fill=\"#00CD69\" />" +
                "</g>" +
                "<g style=\"mix-blend-mode:darken\">" +
                    "<path d=\"M0 0V67.5L67.5 0H0Z\" fill=\"#00CD69\" />" +
                "</g>" +
                "<g style=\"mix-blend-mode:darken\">" +
                    "<path" +
                        "d=\"M196.22 32.0498V43.2498H210.22L186.43 101.25H203.83L209.14 86.9998H238.83L244.23 101.25H262.23L233.82 32.0498H196.22ZM213.92 74.4198L223.84 47.8698L233.97 74.4198H213.92Z\"" +
                        "fill=\"black\" />" +
                "</g>" +
                "<g style=\"mix-blend-mode:darken\">" +
                    "<g style=\"mix-blend-mode:darken\">" +
                        "<path" +
                            "d=\"M305.52 57.53C303.15 53.62 297.82 49.25 289.3 49.25C276.13 49.25 265.81 58.69 265.81 74.91C265.81 90.91 276.13 100.37 289.3 100.37C297.18 100.37 302.3 96.63 304.93 92.97V97.62C304.93 106.96 297.46 110.21 289.79 110.21C282.917 110.261 276.195 108.201 270.53 104.31V116.79C275.83 119.94 282.81 121.9 292.35 121.9C308.35 121.9 320.66 113.75 320.66 97.9V50.43H305.52V57.53ZM293.52 88.47C286.34 88.47 281.62 83.06 281.62 74.91C281.53 66.55 286.34 61.34 293.52 61.34C297.753 61.2672 301.846 62.8577 304.92 65.77V83.95C301.861 86.896 297.766 88.5197 293.52 88.47\"" +
                            "fill=\"black\" />" +
                   " </g>" +
                    "<g style=\"mix-blend-mode:darken\">" +
                        "<path" +
                            "d=\"M355.459 61.8302L352.999 50.4302H329.409V61.0502H339.929V101.25H355.749V69.9902C358.009 66.3502 362.439 63.5002 370.599 63.5002C372.687 63.513 374.765 63.7783 376.789 64.2902V50.1402C375.386 49.5758 373.881 49.3069 372.369 49.3502C362.239 49.3502 356.739 55.3502 355.459 61.8302\"" +
                            "fill=\"black\" />" +
                    "</g>" +
                    "<g style=\"mix-blend-mode:darken\">" +
                        "<path" +
                            "d=\"M410.22 49.25C393.81 49.25 381.72 59.87 381.72 75.89C381.72 91.91 393.81 102.43 410.22 102.43C426.44 102.43 438.63 91.91 438.63 75.89C438.63 59.87 426.44 49.25 410.22 49.25ZM410.22 90.05C402.65 90.05 397.54 84.15 397.54 75.89C397.54 67.63 402.65 61.62 410.22 61.62C417.79 61.62 422.81 67.51 422.81 75.87C422.81 84.23 417.69 90.03 410.22 90.03\"" +
                            "fill=\"black\" />" +
                    "</g>" +
                    "<g style=\"mix-blend-mode:darken\">" +
                        "<path" +
                            "d=\"M477.579 49.25C474.674 49.162 471.79 49.7599 469.159 50.9952C466.529 52.2305 464.226 54.0685 462.439 56.36V29.79H446.619V101.25H461.759V94.37C464.249 98.29 469.469 102.43 477.579 102.43C490.749 102.43 501.079 92.6 501.079 75.89C501.079 59.18 490.749 49.25 477.579 49.25ZM473.349 90.05C469.304 90.0924 465.398 88.5779 462.439 85.8201V65.92C465.374 63.116 469.29 61.5725 473.349 61.62C480.629 61.62 485.249 67.22 485.249 75.87C485.249 84.52 480.629 90.03 473.349 90.03\"" +
                            "fill=\"black\" />" +
                    "</g>" +
                    "<g style=\"mix-blend-mode:darken\">" +
                        "<path" +
                            "d=\"M558.68 88.47V69.11C558.68 56.82 549.24 49.25 534.01 49.25C524.57 49.25 517.79 51.31 512.77 54.36V66.75C518.233 62.979 524.712 60.9563 531.35 60.95C538.35 60.95 543.35 63.6 543.35 70.48V71.8C539.695 70.8399 535.929 70.3658 532.15 70.39C519.76 70.39 508.46 75.3 508.46 86.6C508.46 95.85 516.71 102.43 527.73 102.43C536.4 102.43 541.05 99.43 543.91 95.69C545.53 100.23 550.53 102.43 555.54 102.43C558.804 102.562 562.05 101.875 564.98 100.43V91.32C564.151 91.5652 563.294 91.6997 562.43 91.72C560.36 91.72 558.69 90.83 558.69 88.47H558.68ZM532.64 91.72C527.53 91.72 524.28 89.36 524.28 85.82C524.28 81.2 529.59 79.62 534.28 79.62C537.325 79.6198 540.354 80.0473 543.28 80.89V86.89C541.976 88.4503 540.334 89.6941 538.478 90.5273C536.623 91.3605 534.603 91.7615 532.57 91.7\"" +
                            "fill=\"black\" />" +
                    "</g>" +
                    "<g style=\"mix-blend-mode:darken\">" +
                        "<path" +
                            "d=\"M608.719 49.2503C605.31 49.1915 601.935 49.9454 598.874 51.4495C595.814 52.9537 593.156 55.1649 591.119 57.9003L589.549 50.4303H565.949V61.0503H576.469V101.25H592.299V65.8602C594.559 63.4102 598.199 62.1303 602.129 62.1303C608.519 62.1303 612.129 65.2703 612.129 71.9603V101.25H627.949V67.4403C627.949 55.5403 619.699 49.2503 608.689 49.2503\"" +
                            "fill=\"black\" />" +
                    "</g>" +
                    "<g style=\"mix-blend-mode:darken\">" +
                        "<path" +
                            "d=\"M695.91 101.1V90.64H688.62L676.44 73.14L692.45 50.43H676.66L663.79 68.91H656.78V29.79H640.96V101.25H656.78V80.22H663.9L677.85 101.25H678.31H695.91H696.01L695.91 101.1Z\"" +
                            "fill=\"black\" />" +
                    "</g>" +
                "</g>" +
            "</g>" +
        "</g>" +
    "</g>" +
    "<defs>" +
        "<clipPath id=\"clip0_204_2\">" +
            "<rect width=\"696.01\" height=\"135.01\" fill=\"white\" />" +
        "</clipPath>" +
    "</defs>" +
"</svg>";
                    break;
                #endregion
                case "Ipoteka Bank":
                    #region Ipoteka Bank
                    qaytar = "<svg  id=\"BankIcon\" viewBox=\"0 0 2048.000000 897.000000\">" +
                                "<g transform=\"translate(0.000000,897.000000) scale(0.100000,-0.100000)\" fill=\"#0c54a0\" stroke=\"none\">" +
                                "<path d=\"M6669 7392 c-2011 -867 -3651 -1578 -3645 -1580 7 -1 1767 520 3912 1158 l3901 1161 2376 -1072 c1308 -590 2375 -1074 2373 -1076 -3 -3 -11970 -235 -12601 -244 l-110 -2 80 -8 c83 -9 14680 -263 14750 -257 36 3 -348 186 -3645 1747 -2027 960 -3696 1746 -3710 1748 -16 2 -1330 -560 -3681 -1575z\"/>" +
                                "<path d=\"M2790 4566 l0 -45 58 -6 c31 -4 68 -14 81 -22 l24 -16 -7 -461 c-7 -468 -13 -557 -35 -579 -6 -6 -40 -14 -74 -17 l-62 -5 -3 -42 -3 -43 184 0 c101 0 239 -3 306 -7 l122 -6 -3 44 -3 44 -64 7 c-35 4 -69 12 -75 18 -27 27 -15 996 12 1047 9 17 25 22 82 28 l70 7 0 42 0 41 -283 2 c-155 1 -292 5 -304 8 -21 5 -23 2 -23 -39z\"/>" +
                                "<path d=\"M3530 4549 c0 -42 0 -42 84 -53 21 -3 41 -8 44 -10 3 -3 9 -27 13 -53 20 -122 0 -942 -24 -992 -10 -22 -22 -27 -75 -34 l-62 -9 0 -39 0 -39 138 0 c75 0 215 -3 310 -7 l172 -6 0 40 0 41 -52 7 c-96 11 -109 16 -119 42 -18 49 -5 1011 15 1030 3 3 33 7 68 10 153 12 249 -62 264 -204 18 -166 -90 -290 -233 -268 -64 9 -78 -6 -53 -55 19 -37 22 -39 87 -46 252 -27 461 131 480 362 15 168 -71 277 -237 303 -36 5 -235 12 -442 14 l-378 5 0 -39z\"/>" +
                                "<path d=\"M5250 4569 c-165 -19 -288 -77 -395 -184 -125 -125 -176 -260 -177 -470 0 -143 17 -225 68 -327 71 -142 206 -245 384 -295 94 -26 406 -26 500 0 173 48 328 168 403 313 100 193 104 491 10 667 -38 70 -145 175 -218 214 -148 78 -355 107 -575 82z m242 -110 c125 -27 207 -93 263 -212 85 -178 84 -483 -1 -662 -52 -111 -171 -196 -292 -210 -72 -8 -182 9 -247 39 -108 50 -201 193 -231 356 -30 156 -7 385 48 495 83 162 259 237 460 194z\"/>" +
                                "<path d=\"M6227 4543 c-4 -3 -7 -75 -7 -160 l0 -153 45 0 c49 0 42 -13 62 120 l8 55 110 8 c61 5 139 5 174 2 l63 -7 -7 -316 c-7 -364 -21 -681 -31 -705 -7 -20 -63 -37 -121 -37 l-43 0 0 -42 0 -43 273 -2 c149 -1 291 -5 315 -8 l42 -6 0 45 0 44 -65 7 c-99 11 -99 12 -103 162 -5 175 16 885 27 896 11 11 311 -9 323 -21 6 -6 12 -46 15 -91 l6 -81 42 0 42 0 8 93 c4 50 11 116 15 145 5 29 7 58 4 65 -3 9 -64 11 -252 9 -232 -4 -888 13 -923 24 -8 2 -18 1 -22 -3z\"/>" +
                                "<path d=\"M7520 4479 c0 -41 2 -43 82 -54 25 -3 52 -13 59 -21 7 -8 14 -56 17 -112 4 -101 -14 -771 -25 -879 -6 -61 -6 -61 -54 -88 -38 -20 -49 -32 -49 -51 l0 -24 113 0 c61 0 261 -5 442 -11 182 -7 346 -9 365 -6 l36 7 12 133 c7 73 15 143 19 155 5 20 2 22 -39 22 l-45 0 -24 -92 -24 -93 -111 -8 c-62 -5 -165 -5 -230 0 l-119 8 -3 200 c-2 110 0 219 3 243 l6 42 92 -1 c121 0 224 -12 239 -27 7 -7 14 -41 16 -77 l4 -65 44 0 44 0 0 138 c0 75 3 167 7 205 l6 67 -46 0 -46 0 -6 -56 c-11 -86 -13 -87 -196 -83 -86 2 -160 6 -163 9 -6 6 14 423 21 430 3 3 73 4 156 4 151 -2 262 -15 281 -34 6 -6 12 -39 14 -75 l4 -65 39 0 c35 0 38 2 43 33 5 37 26 192 29 222 2 18 -6 21 -88 26 -49 4 -278 9 -507 11 l-418 5 0 -38z\"/>" +
                                "<path d=\"M8670 4463 c0 -45 12 -53 81 -53 19 0 44 -6 55 -14 18 -13 19 -30 22 -258 4 -261 -12 -724 -26 -772 -10 -37 -23 -43 -94 -51 l-58 -6 0 -40 0 -39 143 0 c78 0 213 -3 300 -7 l157 -6 0 41 0 41 -47 6 c-27 3 -59 9 -73 13 l-25 7 -3 248 c-2 210 0 247 12 247 17 0 181 -185 358 -403 68 -83 134 -163 148 -178 l25 -28 198 -3 197 -4 0 43 c0 38 -2 43 -24 43 -48 0 -84 23 -147 93 -149 164 -457 532 -482 574 -10 16 327 316 440 390 42 29 65 36 118 40 l65 6 0 43 0 44 -165 2 c-91 0 -171 1 -177 2 -8 1 -14 -15 -16 -43 -3 -42 -10 -50 -165 -205 -90 -89 -212 -204 -272 -256 l-110 -94 -3 44 c-1 25 1 134 6 244 9 222 11 226 81 226 69 0 81 8 81 51 l0 39 -163 0 c-90 0 -225 3 -300 7 l-137 6 0 -40z\"/>" +
                                "<path d=\"M10576 4068 c-208 -480 -337 -752 -358 -761 -9 -3 -36 -9 -62 -12 l-46 -7 0 -44 0 -44 198 0 c108 0 207 -3 220 -6 20 -6 22 -3 22 39 l0 45 -52 7 c-90 11 -91 12 -86 59 3 22 23 85 44 139 l38 97 186 0 c102 0 208 -3 237 -6 l52 -7 46 -125 c25 -68 45 -132 43 -141 -2 -12 -21 -18 -70 -25 l-66 -8 -5 -37 c-2 -20 -4 -37 -3 -37 1 -1 135 -4 299 -7 l297 -7 0 39 0 40 -56 11 -56 12 -38 81 c-21 45 -131 309 -245 587 -113 278 -210 509 -214 513 -4 4 -39 10 -77 13 l-69 6 -179 -414z m243 -128 c38 -96 77 -192 86 -212 l15 -38 -172 0 c-95 0 -179 4 -186 8 -10 7 9 59 80 220 51 116 96 208 99 204 4 -4 39 -86 78 -182z\"/>" +
                                "<path d=\"M12080 4401 c0 -38 1 -39 43 -46 90 -13 98 -17 109 -43 12 -33 4 -971 -9 -1017 -6 -18 -24 -40 -46 -54 -24 -16 -37 -32 -37 -46 l0 -23 293 -7 c201 -4 314 -3 362 5 148 23 276 114 338 239 29 59 32 74 31 151 -1 100 -16 141 -75 205 -45 48 -113 79 -198 91 -61 8 -73 19 -28 28 17 3 56 17 86 31 124 58 182 139 184 253 1 142 -74 223 -228 251 -33 5 -232 12 -442 15 l-383 5 0 -38z m676 -100 c62 -35 79 -71 79 -166 -1 -154 -58 -208 -237 -222 l-78 -6 0 150 c0 83 3 178 7 213 l6 62 91 -4 c71 -3 100 -9 132 -27z m-33 -513 c95 -32 147 -116 147 -239 0 -191 -87 -289 -257 -289 -39 0 -78 3 -87 6 -14 5 -16 36 -16 238 0 127 3 248 7 270 l6 39 84 -7 c45 -4 98 -12 116 -18z\"/>" +
                                "<path d=\"M13946 4388 c-90 -212 -392 -893 -441 -995 -36 -76 -71 -136 -83 -142 -11 -6 -39 -11 -61 -11 l-41 0 0 -47 0 -48 220 -2 220 -2 0 39 c0 43 1 43 -93 55 -27 4 -50 12 -53 20 -7 16 17 96 57 196 l30 77 232 -5 c128 -3 235 -8 238 -12 7 -7 67 -169 85 -228 12 -39 11 -44 -6 -53 -10 -5 -44 -10 -74 -10 l-56 0 0 -45 0 -45 223 0 c123 0 258 -3 300 -6 l77 -7 0 45 0 45 -51 6 c-28 4 -57 11 -64 17 -16 13 -81 165 -321 753 l-179 437 -72 0 c-73 0 -73 0 -87 -32z m93 -542 c45 -109 81 -203 81 -207 0 -10 -199 -10 -302 0 l-67 6 97 218 c68 156 98 213 103 200 4 -10 43 -108 88 -217z\"/>" +
                                "<path d=\"M14810 4357 l0 -46 58 -7 c90 -11 100 -17 107 -68 9 -59 -13 -919 -24 -969 -5 -21 -12 -42 -16 -45 -4 -4 -37 -9 -73 -12 l-67 -5 -3 -42 -3 -43 169 0 c93 0 197 -3 231 -6 l61 -7 0 45 0 45 -62 7 c-35 3 -69 13 -78 21 -13 13 -15 59 -18 278 -1 144 1 331 5 416 l8 153 65 -83 c133 -173 686 -825 737 -871 19 -18 158 -52 178 -44 8 3 14 137 18 466 8 537 15 694 32 717 9 12 32 18 79 21 l66 4 0 44 0 44 -177 0 c-98 0 -202 3 -230 6 l-53 7 0 -45 0 -45 69 -7 c39 -4 74 -13 81 -21 16 -20 22 -259 13 -549 l-8 -249 -141 164 c-152 176 -410 491 -539 657 l-80 102 -180 0 c-99 0 -190 3 -202 6 -21 6 -23 3 -23 -39z\"/>" +
                                "<path d=\"M16390 4324 l0 -41 67 -6 c37 -4 71 -11 77 -17 8 -8 11 -157 10 -473 -1 -450 -7 -564 -34 -591 -6 -6 -39 -12 -75 -14 l-65 -4 0 -44 0 -44 239 0 c131 0 266 -3 300 -6 l61 -7 0 45 0 45 -62 7 c-42 4 -67 12 -75 23 -17 22 -19 478 -2 488 16 10 195 -191 443 -498 l87 -108 199 -4 200 -5 0 40 c0 35 -3 40 -27 45 -16 3 -39 9 -53 13 -14 4 -87 79 -174 177 -162 185 -411 484 -405 489 2 1 67 58 144 127 138 122 248 214 313 258 19 14 60 27 101 33 l69 10 4 39 4 39 -102 0 c-55 0 -135 3 -177 6 l-77 7 0 -38 c0 -34 -8 -47 -77 -119 -162 -167 -470 -453 -479 -444 -1 2 1 110 5 241 6 194 11 241 23 253 9 9 41 18 76 21 l60 6 6 40 c4 22 5 41 4 42 -2 2 -139 5 -306 7 l-302 3 0 -41z\"/>" +
                                "<path d=\"M2736 2378 c-3 -29 -6 -162 -6 -295 l0 -242 143 5 c78 3 3391 72 7362 154 6680 138 7631 160 7535 167 -30 3 -14876 263 -14986 263 l-41 0 -7 -52z\"/>" +
                                "<path d=\"M1515 1226 c-4 -174 -5 -319 -2 -322 4 -4 16931 306 17387 318 l95 3 -75 7 c-41 4 -3884 73 -8540 153 -4656 80 -8553 148 -8661 151 l-196 6 -8 -316z\"/>" +
                                "<path d=\"M10087 485 c-5543 -67 -10080 -125 -10083 -127 -6 -6 -715 6 10508 -189 5471 -96 9950 -170 9955 -166 4 5 9 143 11 308 l3 299 -158 -1 c-87 -1 -4693 -57 -10236 -124z\"/>" +
                                "</g>" +
                            "</svg>";
                    break;
                #endregion
                case "NBU":
                    #region NBU
                    qaytar = "<svg  id=\"BankIcon\" viewBox=\"0 0 64.02 94.8\"><defs><style>.cls-1{fill:#bf945d;}.cls-2{fill:#fff;}</style></defs><g id=\"Layer_2\" data-name=\"Layer 2\"><g id=\"Слой_1\" data-name=\"Слой 1\"><path class=\"cls-1\" d=\"M7.31,17.6a11.63,11.63,0,0,1,.4-1.8A11.63,11.63,0,0,0,7.31,17.6Z\"/><path class=\"cls-1\" d=\"M7.71,15.8a9.51,9.51,0,0,0-.4,1.8C2,19.4-.19,23.1,0,29.4V94.8H19V5.9C13.61,8.6,9,11.8,7.71,15.8Z\"/><path class=\"cls-1\" d=\"M56.31,15.8a9.51,9.51,0,0,1,.4,1.8c5.3,1.8,7.5,5.5,7.3,11.8V94.8H45V5.9C50.51,8.6,55,11.8,56.31,15.8Z\"/><path class=\"cls-1\" d=\"M32.21,0a84.47,84.47,0,0,0-9.5,4.1V94.8h18.6V4A91.81,91.81,0,0,0,32.21,0Z\"/><polygon class=\"cls-2\" points=\"13.11 83.3 7.31 67.3 5.81 67.3 4.61 67.3 4.21 67.3 4.21 86.1 5.81 86.1 5.81 70.3 11.51 86.1 13.11 86.1 13.11 86.1 14.71 86.1 14.71 67.3 13.11 67.3 13.11 83.3\"/><path class=\"cls-2\" d=\"M60.31,67.3h-1.6v4.2h0v8.7c0,1.6-.4,4.5-3,4.5a2.73,2.73,0,0,1-2.4-1,6.76,6.76,0,0,1-.8-3.6V78.9h0V67.3h-2.8V80.8a9.78,9.78,0,0,0,.2,2.6,4.35,4.35,0,0,0,1,1.6,7,7,0,0,0,1.8,1.2,8.51,8.51,0,0,0,2.2.4c1.8,0,3.2-.4,3.9-1.4A6.45,6.45,0,0,0,60,81.1V78.3h0v-11Z\"/><path class=\"cls-2\" d=\"M35,76.4c1.2-.6,2-2,2-4.3,0-4.5-3.9-4.7-5.9-4.7h-4.4V86.2h4.6c2.2,0,6.3,0,6.3-5.3C37.61,78.2,36.41,77,35,76.4Zm-5.3-7.7h1.6c1.4,0,3.2,0,3.2,3.4,0,3.8-1.8,3.6-3.2,3.6h-1.6Zm1.5,16.2h-1.6V77.2h1.6c1.6,0,3.4,0,3.4,3.8S32.81,84.9,31.21,84.9Z\"/></g></g></svg>";
                    break;
                #endregion
                case "Mikrokreditbank":
                    qaytar = "<img  id=\"BankIcon\" src=\"https://mikrokreditbank.uz/www/images/logo.jpg\">";
                    break;
                case "Ziraat Bank":
                    qaytar = "<img id=\"BankIcon\" src=\"https://www.ziraatbank.uz/lib/ziraat-tmp/assets/images/logo-russia.png\">";
                    break;
                case "Asaka Bank":
                    qaytar = "";
                    break;
                case "Ravnaq Bank":
                    qaytar = "";
                    break;
                case "QQB":
                    qaytar = "<img id=\"BankIcon\" src=\"https://qishloqqurilishbank.uz/_nuxt/img/logo.08ca9dd.png\">";
                    break;
                case "Savdogar Bank":
                    #region Savdogar Bank
                    qaytar = "<svg  id=\"BankIcon\">" +
    "<defs>" +
        "<linearGradient id=\"a\" x1=\"0%\" x2=\"0%\" y1=\"100%\" y2=\"0%\">" +
            "<stop offset=\"0%\" stop-color=\"#070808\" stop-opacity=\".14\"/>" +
            "<stop offset=\"100%\" stop-color=\"#FFF\" stop-opacity=\".14\"/>" +
        "</linearGradient>" +
    "</defs>" +
    "<path fill-rule=\"evenodd\" fill=\"#26995f\" d=\"M237.795 37.544l12.011 11.28-2.026 1.779-11.97-11.242-.231.211-1.047-1.411-.708-.665.461-.404 34.018-31.135 1.62 2.183-32.128 29.404zm-7.443 10.773l-.071-.063h-.011v-.01L211 31.269v16.99h-1.885V29.608l-.003-.002v-3.198l21.158 18.875V26.707h1.884v21.547h-1.73l-.072.063zm-25.026-.067h-1.886v-8.709h-8.738l-.9.858c.821 1.608 1.299 3.848.153 5.503-.924 1.335-3.11 2.106-3.821 2.329v.013h-.043l-.204.062-.022-.062h-4.294l-20.594 19.63-1.672-1.819L182.2 48.244h-4.44v-.061h-.008V28.231h-.035V26.32h10.136v-.035l.136.035h.234v.069c.785.245 2.639.967 3.637 2.563 1.161 1.858 1.127 4.846-.291 6.141s-2.177 1.519-1.931 1.581c.225.056 1.59.724 2.818 1.903l.817-.77v-.115h.122l11.876-11.194.055-.066v.014l.015-.014.002 2.966-.017.016V48.25zm-15.128-9.036c-1.912-1.537-4.529-2.56-4.529-2.56s5.019-1.505 5.019-4.724c0-3.029-2.827-3.672-2.827-3.672v-.027h-8.207v18.164h4.508l6.83-6.438a8.06 8.06 0 0 0-.794-.743zm2.121 2.599l-4.808 4.582h1.964c.83-.213 3.347-1.062 3.202-3.5-.015-.264-.142-.647-.358-1.082zm11.121-10.601l-6.798 6.48h6.798v-6.48zm-33.403 17.553l-8.877-10.047-.166-.333-.048-.034.025-.012-.031-.062.208-.028c.906-.473 4.554-2.547 4.554-5.301 0-3.436-2.2-4.419-3.331-4.699h-8.422v19.995H152.1V26.608h.061v-.269h9.315l-.006-.049s.241.003.631.049h.725v.115c1.946.384 5.301 1.674 5.301 6.007 0 3.375-2.313 5.42-3.89 6.426l7.458 8.456-1.658 1.422zm-23.691-18.831l-7.595 7.358h5.212v2.302h-.175c-1.163 5.012-6.322 8.794-12.524 8.794-1.224 0-2.407-.15-3.527-.426l-21.019 20.363-1.703-1.984 20.03-19.344a12.8 12.8 0 0 1-3.309-2.272l1.526-1.526c.927.932 2.086 1.694 3.4 2.237l6.049-5.842h-4.117v-2.302h6.501l11.251-10.866v-.067h1.973v21.865h-1.973v-18.29zm-16.795 16.271c.559.077 1.128.131 1.713.131 4.946 0 9.077-2.883 10.131-6.742h-5.02l-6.824 6.611zm1.713-17.216c-2.974 0-5.65 1.047-7.545 2.717l-1.694-1.789c2.327-2.086 5.602-3.392 9.239-3.392 3.228 0 6.168 1.032 8.418 2.72l-1.759 1.758c-1.804-1.256-4.124-2.014-6.659-2.014zm-20.63 19.399c-3.391 0-6.489-.991-8.88-2.624l1.411-1.683c1.976 1.397 4.591 2.255 7.469 2.255 6.15 0 11.135-3.883 11.135-8.674 0-4.79-4.985-8.673-11.135-8.673-2.548 0-4.89.674-6.767 1.797l-1.508-2.021c2.3-1.399 5.161-2.24 8.275-2.24 7.558 0 13.685 4.895 13.685 10.932 0 6.037-6.127 10.931-13.685 10.931zm-47.366-2.301h-.005V26.336h1.89v16.976l38.849-36.613 1.978 1.874-42.726 39.812.014-2.298zm-4.556-6.537H49.58l.915-1.08-1.157 1.131c.021-.252.047-.499.059-.762.044-1.022-.181-1.937-.557-2.77l9.864-9.724.007 4.083-7.39 7.232h7.391V26.335h1.905v21.912h-1.905V39.55zM36.637 52.032L20.793 67.54l-2.014-1.84 13.727-13.531h.914s1.33.094 3.217-.137zm0 0l12.701-12.431c-.79 9.553-8.28 11.892-12.701 12.431zm-29.683.137v-5.835h22.27v-.083c1.409-.128 3.854-.421 5.096-.986 1.808-.822 4.192-3.205 4.274-5.014.082-1.808-1.233-5.589-4.767-8.136-3.534-2.549-8.466-5.59-8.877-6.658-.411-1.068-.763-1.131.634-1.542 1.397-.411 3.312-.184 3.312-.184l2.917-.039c1.93 0 3.493-1.607 3.493-3.536s-1.563-3.624-3.493-3.624h-9.616c-1.705.131-3.86 1.062-3.914 1.093-1.726.986-4.017 3.476-3.935 6.517.082 3.041 3.616 5.342 7.068 8.137 3.452 2.794 5.961 4.685 6.126 6.329.164 1.643-1.77 1.677-1.77 1.677H6.954V9.923H33.42s8.645-.004 11.113 7.994c1.635 5.299-3.88 12.06-3.88 12.06s6.273 1.86 8.187 6.092l-16.334 16.1H6.954zm72.242-16.52h1.809v10.602h8.383v.066c.15.007.299.019.452.019 5.743 0 10.399-3.883 10.399-8.674 0-4.79-4.656-8.673-10.399-8.673-.291 0-.578.017-.863.036v-2.462c.287-.017.572-.038.863-.038 7.059 0 12.781 4.895 12.781 10.932 0 6.037-5.722 10.931-12.781 10.931-.152 0-.302-.005-.452-.013v.013H79.114v-2.137h.082V35.649z\"/>" +
    "<path fill=\"url(#a)\" d=\"M237.795 37.544l12.011 11.28-2.026 1.779-11.97-11.242-.231.211-1.047-1.411-.708-.665.461-.404 34.018-31.135 1.62 2.183-32.128 29.404zm-7.443 10.773l-.071-.063h-.011v-.01L211 31.269v16.99h-1.885V29.608l-.003-.002v-3.198l21.158 18.875V26.707h1.884v21.547h-1.73l-.072.063zm-25.026-.067h-1.886v-8.709h-8.738l-.9.858c.821 1.608 1.299 3.848.153 5.503-.924 1.335-3.11 2.106-3.821 2.329v.013h-.043l-.204.062-.022-.062h-4.294l-20.594 19.63-1.672-1.819L182.2 48.244h-4.44v-.061h-.008V28.231h-.035V26.32h10.136v-.035l.136.035h.234v.069c.785.245 2.639.967 3.637 2.563 1.161 1.858 1.127 4.846-.291 6.141s-2.177 1.519-1.931 1.581c.225.056 1.59.724 2.818 1.903l.817-.77v-.115h.122l11.876-11.194.055-.066v.014l.015-.014.002 2.966-.017.016V48.25zm-15.128-9.036c-1.912-1.537-4.529-2.56-4.529-2.56s5.019-1.505 5.019-4.724c0-3.029-2.827-3.672-2.827-3.672v-.027h-8.207v18.164h4.508l6.83-6.438a8.06 8.06 0 0 0-.794-.743zm2.121 2.599l-4.808 4.582h1.964c.83-.213 3.347-1.062 3.202-3.5-.015-.264-.142-.647-.358-1.082zm11.121-10.601l-6.798 6.48h6.798v-6.48zm-33.403 17.553l-8.877-10.047-.166-.333-.048-.034.025-.012-.031-.062.208-.028c.906-.473 4.554-2.547 4.554-5.301 0-3.436-2.2-4.419-3.331-4.699h-8.422v19.995H152.1V26.608h.061v-.269h9.315l-.006-.049s.241.003.631.049h.725v.115c1.946.384 5.301 1.674 5.301 6.007 0 3.375-2.313 5.42-3.89 6.426l7.458 8.456-1.658 1.422zm-23.691-18.831l-7.595 7.358h5.212v2.302h-.175c-1.163 5.012-6.322 8.794-12.524 8.794-1.224 0-2.407-.15-3.527-.426l-21.019 20.363-1.703-1.984 20.03-19.344a12.8 12.8 0 0 1-3.309-2.272l1.526-1.526c.927.932 2.086 1.694 3.4 2.237l6.049-5.842h-4.117v-2.302h6.501l11.251-10.866v-.067h1.973v21.865h-1.973v-18.29zm-16.795 16.271c.559.077 1.128.131 1.713.131 4.946 0 9.077-2.883 10.131-6.742h-5.02l-6.824 6.611zm1.713-17.216c-2.974 0-5.65 1.047-7.545 2.717l-1.694-1.789c2.327-2.086 5.602-3.392 9.239-3.392 3.228 0 6.168 1.032 8.418 2.72l-1.759 1.758c-1.804-1.256-4.124-2.014-6.659-2.014zm-20.63 19.399c-3.391 0-6.489-.991-8.88-2.624l1.411-1.683c1.976 1.397 4.591 2.255 7.469 2.255 6.15 0 11.135-3.883 11.135-8.674 0-4.79-4.985-8.673-11.135-8.673-2.548 0-4.89.674-6.767 1.797l-1.508-2.021c2.3-1.399 5.161-2.24 8.275-2.24 7.558 0 13.685 4.895 13.685 10.932 0 6.037-6.127 10.931-13.685 10.931zm-47.366-2.301h-.005V26.336h1.89v16.976l38.849-36.613 1.978 1.874-42.726 39.812.014-2.298zm-4.556-6.537H49.58l.915-1.08-1.157 1.131c.021-.252.047-.499.059-.762.044-1.022-.181-1.937-.557-2.77l9.864-9.724.007 4.083-7.39 7.232h7.391V26.335h1.905v21.912h-1.905V39.55zM36.637 52.032L20.793 67.54l-2.014-1.84 13.727-13.531h.914s1.33.094 3.217-.137zm0 0l12.701-12.431c-.79 9.553-8.28 11.892-12.701 12.431zm-29.683.137v-5.835h22.27v-.083c1.409-.128 3.854-.421 5.096-.986 1.808-.822 4.192-3.205 4.274-5.014.082-1.808-1.233-5.589-4.767-8.136-3.534-2.549-8.466-5.59-8.877-6.658-.411-1.068-.763-1.131.634-1.542 1.397-.411 3.312-.184 3.312-.184l2.917-.039c1.93 0 3.493-1.607 3.493-3.536s-1.563-3.624-3.493-3.624h-9.616c-1.705.131-3.86 1.062-3.914 1.093-1.726.986-4.017 3.476-3.935 6.517.082 3.041 3.616 5.342 7.068 8.137 3.452 2.794 5.961 4.685 6.126 6.329.164 1.643-1.77 1.677-1.77 1.677H6.954V9.923H33.42s8.645-.004 11.113 7.994c1.635 5.299-3.88 12.06-3.88 12.06s6.273 1.86 8.187 6.092l-16.334 16.1H6.954zm72.242-16.52h1.809v10.602h8.383v.066c.15.007.299.019.452.019 5.743 0 10.399-3.883 10.399-8.674 0-4.79-4.656-8.673-10.399-8.673-.291 0-.578.017-.863.036v-2.462c.287-.017.572-.038.863-.038 7.059 0 12.781 4.895 12.781 10.932 0 6.037-5.722 10.931-12.781 10.931-.152 0-.302-.005-.452-.013v.013H79.114v-2.137h.082V35.649z\"/>" +
"</svg>"; break;
                #endregion
                case "Orient Finans Bank":
                    #region Orient Finans Bank
                    qaytar = "<svg id=\"BankIcon\" viewBox=\"0 0 247 34\" >" +
    "<path fill-rule=\"evenodd\" clip-rule=\"evenodd\" d=\"M14.5476 3.10642C13.7794 1.97914 12.7666 1.14918 11.537 0.631135C10.392 0.147751 9.22235 0 8.03739 0C6.85184 0 5.68216 0.147751 4.53658 0.631135C3.30753 1.14918 2.29479 1.97914 1.52657 3.10642C0.392155 4.76999 -0.00753388 6.98444 0.000107236 8.62186C0.00716057 10.2599 0.157044 11.1531 0.621389 12.3667C1.22327 13.9379 2.20016 15.1965 3.60495 16.0848C4.94626 16.9324 6.47625 17.2352 8.03739 17.2352C9.59853 17.2352 11.1285 16.9324 12.4692 16.0848C13.874 15.1965 14.8509 13.9379 15.4528 12.3667C15.9171 11.1531 16.067 10.2599 16.0747 8.62186C16.0817 6.98444 15.682 4.76999 14.5476 3.10642ZM11.7427 12.6696C10.9633 14.0468 9.54444 14.7059 8.03738 14.7059C6.52973 14.7059 5.11083 14.0468 4.33144 12.6696C3.73954 11.6232 3.45094 9.90188 3.47152 8.8056C3.49209 7.70811 3.50678 6.84714 3.77363 5.86639C4.1551 4.46306 4.92215 3.35705 6.28874 2.8001C6.84478 2.5733 7.44255 2.52466 8.03738 2.52466C8.63162 2.52466 9.22939 2.5733 9.78543 2.8001C11.1526 3.35705 11.9191 4.46306 12.3005 5.86639C12.5674 6.84714 12.5821 7.70811 12.6026 8.8056C12.6232 9.90188 12.334 11.6232 11.7427 12.6696Z\" transform=\"translate(40.8353 7.57007)\" fill=\"#404A71\"></path>" +
    "<path fill-rule=\"evenodd\" clip-rule=\"evenodd\" d=\"M11.8719 12.9589C11.2324 12.9589 10.6247 12.9608 10.0175 12.9559C9.97518 12.9553 9.90935 12.917 9.89466 12.8793C9.30512 11.3775 8.3729 10.1085 7.38249 8.8742C6.18637 7.38392 5.00493 5.88087 3.81703 4.38329C3.79528 4.35593 3.77177 4.32918 3.70947 4.25561V4.51463C3.70888 6.60199 3.70065 8.68997 3.71652 10.7779C3.71946 11.1805 3.78705 11.5897 3.8811 11.9812C3.98807 12.4263 4.26198 12.7601 4.73396 12.9462H0.790562C1.51235 12.5151 1.59817 11.7782 1.60992 11.0455C1.63637 9.39893 1.6205 7.75178 1.6205 6.10463C1.62109 4.97004 1.58818 3.83364 1.62991 2.70088C1.6734 1.49819 1.12031 0.703499 0.162815 0.134991C0.116968 0.0948614 0.0658312 0.0614197 0 0.00183283H0.159876C1.35071 0.00183283 2.54155 0.00304889 3.73239 8.73802e-06C3.84877 -0.000599292 3.92224 0.0304102 3.99865 0.128911C5.89012 2.58657 7.7863 5.04057 9.68188 7.49519L9.7965 7.63078L9.79826 7.46236C9.79591 5.58902 9.79415 3.71507 9.78651 1.84112C9.78474 1.44955 9.70774 1.07014 9.54552 0.713228C9.40563 0.405565 9.19344 0.169041 8.84724 0.0127774H12.7348C12.0924 0.371515 11.9683 0.995353 11.8955 1.63986C11.8696 1.87092 11.8731 2.10622 11.8725 2.33971C11.8719 5.78906 11.8719 9.23841 11.8719 12.6884V12.9589Z\" transform=\"translate(89.2531 11.0857)\" fill=\"#404A71\"></path>" +
    "<path fill-rule=\"evenodd\" clip-rule=\"evenodd\" d=\"M0 0H4.46418C3.7224 0.435349 3.60484 1.1662 3.59309 1.91165C3.56781 3.51259 3.5831 5.11475 3.5831 6.7163C3.58251 8.09713 3.57898 9.47857 3.58603 10.8594C3.5878 11.1494 3.61366 11.4419 3.65833 11.7283C3.73709 12.2378 3.93929 12.6762 4.44948 12.9407H0.0382056C0.628923 12.658 0.795264 12.1253 0.871087 11.5471C0.905766 11.285 0.910468 11.0175 0.911056 10.7524C0.913407 7.89283 0.914583 5.03327 0.909293 2.17431C0.908705 1.8776 0.881667 1.57905 0.836996 1.28598C0.754119 0.7497 0.540756 0.289422 0 0Z\" transform=\"translate(71.791 11.0857)\" fill=\"#404A71\"></path>" +
    "<path fill-rule=\"evenodd\" clip-rule=\"evenodd\" d=\"M0 0H8.43227V3.3472C8.32235 3.11919 8.23183 2.89848 8.11369 2.69418C7.76925 2.09709 7.22027 1.82105 6.58076 1.79126C5.67323 1.74991 4.76335 1.7718 3.85524 1.76633C3.8376 1.76633 3.81997 1.77666 3.79117 1.78578V5.49051C3.80351 5.4978 3.81409 5.51057 3.82467 5.51057C4.66402 5.50024 5.50395 5.50571 6.34213 5.46983C6.72771 5.45281 7.08684 5.30506 7.33665 4.9147V7.84723C7.04687 7.42708 6.6319 7.31703 6.19342 7.30791C5.39874 7.29088 4.60407 7.30304 3.79352 7.30304V11.1336C3.8041 11.1385 3.82056 11.1537 3.83643 11.1537C4.8139 11.1494 5.79197 11.1537 6.76885 11.1342C7.84566 11.1142 8.61036 10.5633 9.11703 9.59836C9.14877 9.53816 9.18697 9.48101 9.25163 9.43358C9.05884 10.5949 8.86546 11.7563 8.66973 12.9328H0.191028C0.829943 12.5741 0.977475 11.9381 1.04683 11.2723C1.08504 10.9062 1.08563 10.5347 1.08621 10.1656C1.08915 7.5268 1.09033 4.88795 1.08386 2.24849C1.08328 1.92685 1.05741 1.59912 0.993345 1.28537C0.876965 0.712003 0.604824 0.24078 0 0Z\" transform=\"translate(78.9346 11.0857)\" fill=\"#404A71\"></path>" +
    "<path fill-rule=\"evenodd\" clip-rule=\"evenodd\" d=\"M5.15833 6.09713C4.64638 6.12084 4.13266 6.1026 3.6066 6.1026V1.771C4.35131 1.80626 5.09779 1.68526 5.82487 1.93152C6.62249 2.20148 7.16795 3.02719 7.1415 3.91612C7.10447 5.16988 6.36622 6.04119 5.15833 6.09713ZM9.57667 11.7938C8.96185 10.7425 8.37172 9.67602 7.77454 8.6144C7.5218 8.16385 7.25553 7.72668 6.83468 7.41901C6.82469 7.41172 6.82822 7.38618 6.82469 7.36794C8.54923 7.09858 9.64485 5.80834 9.82177 4.19646C9.97166 2.82778 9.25986 1.35574 8.03493 0.654076C7.21321 0.183461 6.32802 0.00470013 5.40227 0.00226801C3.65304 -0.00198819 1.9044 0.00105195 0.155761 0.00105195H0C0.528413 0.260681 0.805844 0.707582 0.918109 1.26271C0.988643 1.61476 1.02979 1.98019 1.03096 2.33893C1.04037 5.06229 1.03684 7.78626 1.0339 10.5096C1.03331 10.8203 1.03155 11.1335 0.993933 11.4405C0.919873 12.0498 0.769402 12.625 0.146357 12.9484H4.49709C4.1021 12.7685 3.90461 12.4626 3.79881 12.1051C3.71358 11.8169 3.64422 11.5117 3.63776 11.2131C3.61366 10.1369 3.61895 9.06069 3.61425 7.98387C3.61425 7.94739 3.62248 7.91091 3.62659 7.87625C4.10504 7.80086 4.47593 7.95773 4.74631 8.3566C4.84329 8.49948 4.93969 8.64359 5.02021 8.7962C5.7232 10.1302 6.425 11.4648 7.11976 12.8031C7.18382 12.9253 7.25318 12.9618 7.38132 12.9612C8.49046 12.9563 9.60018 12.9582 10.7099 12.9582H10.8704C10.2949 12.7338 9.88173 12.3155 9.57667 11.7938Z\" transform=\"translate(59.0912 11.0857)\" fill=\"#404A71\"></path>" +
    "<path fill-rule=\"evenodd\" clip-rule=\"evenodd\" d=\"M0 3.45057C0.10051 2.27707 0.198081 1.1437 0.29624 0H10.9962C11.0873 1.13702 11.1784 2.2716 11.273 3.44631C11.1437 3.19155 11.0396 2.96779 10.9192 2.75316C10.5765 2.14452 10.041 1.82713 9.37976 1.79126C8.58568 1.74869 7.8004 1.78213 7.00808 1.7645V2.00954C7.00808 4.88917 7.0069 7.76879 7.00925 10.6478C7.00984 11.1993 7.03394 11.7483 7.25259 12.2664C7.38602 12.5832 7.54472 12.7607 7.86388 12.9334H3.43145C4.07742 12.5765 4.16558 11.9271 4.23788 11.2747C4.26374 11.0436 4.25786 10.8089 4.25786 10.5755C4.25904 7.72806 4.25845 4.88187 4.25845 2.03447V1.76572L1.81682 1.78031C1.20318 1.79004 0.742952 2.0898 0.42849 2.62608C0.28037 2.87841 0.156937 3.14655 0 3.45057Z\" transform=\"translate(103.74 11.0857)\" fill=\"#404A71\"></path>" +
    "<path fill-rule=\"evenodd\" clip-rule=\"evenodd\" d=\"M4.41156 7.77673C4.98023 6.26629 5.54192 4.77163 6.11757 3.24072C6.70543 4.77105 7.27934 6.26629 7.85964 7.77673H4.41156ZM8.55507 12.3081H13.2585C13.0521 12.1175 12.7794 11.9374 12.605 11.6877C12.3043 11.2585 12.0148 10.8076 11.8113 10.3264C11.1623 8.79606 10.5547 7.2476 9.93197 5.70558C9.18596 3.8589 8.43819 2.01339 7.69508 0.164953C7.64624 0.0427375 7.58809 -0.00170454 7.45378 4.97499e-05C6.84673 0.00765167 6.2391 0.00648214 5.63147 0.00121928C5.50471 4.97499e-05 5.43203 0.0263641 5.39016 0.160275C5.31864 0.384824 5.22154 0.600602 5.13606 0.820472C3.97895 3.80569 2.82882 6.79324 1.66124 9.77437C1.2862 10.7293 0.91406 11.6988 0 12.3087H3.85975C3.35562 12.0403 3.13118 11.6702 3.25212 11.1731C3.39865 10.5726 3.60449 9.98722 3.78474 9.39661C5.34074 9.39661 6.86592 9.39544 8.39051 9.40129C8.43645 9.40129 8.50622 9.46444 8.52425 9.51298C8.69578 9.96617 8.86731 10.4188 9.01849 10.8784C9.08594 11.0825 9.13944 11.3018 9.13944 11.514C9.13944 11.883 8.95046 12.1544 8.55507 12.3081Z\" transform=\"translate(149.677 11.0857)\" fill=\"#404A71\"></path>" +
    "<path fill-rule=\"evenodd\" clip-rule=\"evenodd\" d=\"M3.66089 4.06294V4.25124C3.66089 6.2406 3.65159 8.22938 3.66845 10.2187C3.67194 10.6117 3.74113 11.0111 3.83649 11.3935C3.93941 11.8075 4.20165 12.1227 4.65868 12.2999H0.791953C1.33039 12.0379 1.48796 11.552 1.56472 11.0316C1.60949 10.731 1.61763 10.4234 1.61763 10.1187C1.6217 7.55573 1.60309 4.99272 1.62926 2.43028C1.63856 1.48004 1.21991 0.818092 0.480289 0.306424C0.33783 0.208184 0.173857 0.110528 0 0.00234686L0.187813 0.00293163C1.35365 0.00234686 2.51948 0.00410115 3.68531 7.81137e-06C3.80102 -0.000576952 3.87545 0.031585 3.9493 0.125147C5.35353 1.89873 6.76242 3.6694 8.16956 5.44064C8.62601 6.01488 9.08129 6.5897 9.536 7.1657L9.61508 7.27505V7.08091C9.61333 5.31024 9.6238 3.53899 9.59938 1.76833C9.59414 1.41104 9.48425 1.04615 9.37144 0.701139C9.26969 0.388291 9.03419 0.160233 8.67601 0.0111183H12.4933C11.963 0.284203 11.788 0.758446 11.706 1.27479C11.6601 1.56893 11.6502 1.87067 11.6502 2.16889C11.6461 5.4617 11.6479 8.75391 11.6479 12.0461V12.314H11.3281C10.8443 12.314 10.3599 12.318 9.87673 12.3099C9.81394 12.3087 9.71625 12.2625 9.69648 12.2122C9.26387 11.1345 8.64868 10.1667 7.92651 9.26909C6.53565 7.53994 5.12676 5.826 3.72427 4.10563C3.71729 4.09686 3.7045 4.09218 3.66089 4.06294Z\" transform=\"translate(136.183 11.0857)\" fill=\"#404A71\"></path>" +
    "<path fill-rule=\"evenodd\" clip-rule=\"evenodd\" d=\"M0 0H4.41854C3.68299 0.418106 3.5667 1.12158 3.55507 1.8385C3.53006 3.34718 3.54576 4.85587 3.54518 6.36515C3.5446 7.72472 3.54111 9.0843 3.54809 10.4433C3.54983 10.7287 3.57484 11.0164 3.62193 11.2976C3.70276 11.7801 3.9022 12.1958 4.40284 12.4461H0.0407024C0.629143 12.1701 0.793116 11.652 0.862891 11.0877C0.897198 10.8111 0.901849 10.5287 0.901849 10.2486C0.904757 7.53 0.90592 4.81143 0.900105 2.09228C0.899524 1.8075 0.872776 1.5198 0.828585 1.23794C0.74718 0.722183 0.536109 0.279517 0 0Z\" transform=\"translate(129.834 11.0857)\" fill=\"#404A71\"></path>" +
    "<path fill-rule=\"evenodd\" clip-rule=\"evenodd\" d=\"M9.22142 9.7556C8.88592 9.20593 8.38586 9.01822 7.84626 8.99658C6.84847 8.95682 5.84836 8.97436 4.8494 8.9691H4.76044V9.22464C4.76044 10.5725 4.74299 11.921 4.76916 13.2677C4.77905 13.7811 4.85289 14.2998 4.96221 14.8004C5.06629 15.2758 5.32795 15.6763 5.84894 15.9126H0.184324C0.948948 15.5687 1.16874 14.9056 1.2798 14.1904C1.33678 13.8226 1.35074 13.4449 1.3519 13.0706C1.35655 9.64099 1.35597 6.21135 1.3519 2.78172C1.3519 2.46361 1.33504 2.14316 1.28969 1.82914C1.17456 1.02684 0.84661 0.359045 0 0H10.671V4.12726C10.5972 3.96762 10.5472 3.85944 10.4983 3.75184C10.0506 2.77529 9.26562 2.25602 8.2248 2.20339C7.10141 2.14608 5.97337 2.17883 4.84708 2.17356L4.77207 2.17415V6.78325L4.96046 6.78384C5.76172 6.78442 6.56297 6.79261 7.36423 6.78033C7.64682 6.77565 7.93232 6.73706 8.21142 6.6856C8.62891 6.60899 8.97779 6.40784 9.22142 5.99382V9.7556Z\" transform=\"translate(117.134 7.96802)\" fill=\"#404A71\"></path>" +
    "<path fill-rule=\"evenodd\" clip-rule=\"evenodd\" d=\"M4.73195 12.4537H0.806489C1.39842 12.1444 1.53913 11.607 1.60949 11.0409C1.64496 10.7515 1.64845 10.4567 1.64903 10.1644C1.65136 7.5949 1.63973 5.02604 1.65601 2.45659C1.6624 1.51044 1.24317 0.830364 0.498314 0.312848C0.352367 0.211684 0.182579 0.114614 0 0.00233905L0.177928 0.00292382C1.36818 0.00233905 2.55902 0.00350858 3.74986 0C3.85743 0 3.92604 0.0286534 3.99698 0.118122C5.86871 2.48115 7.74451 4.84125 9.61915 7.20194L9.7366 7.34463L9.74009 7.20487C9.73718 5.39035 9.74416 3.57583 9.72032 1.76131C9.71567 1.42156 9.61449 1.07538 9.50983 0.747327C9.40342 0.418106 9.16909 0.173675 8.80277 0.0169581H12.6445C12.1398 0.269576 11.9537 0.721598 11.8677 1.21865C11.8124 1.54202 11.795 1.87534 11.7944 2.20456C11.7897 5.54063 11.7915 8.87671 11.7915 12.2128V12.4578H9.91453C9.57205 11.8303 9.28422 11.1924 8.89813 10.6216C8.35853 9.82519 7.7637 9.06617 7.16188 8.31475C6.05478 6.93178 4.92557 5.56636 3.80509 4.19392C3.79056 4.17579 3.77311 4.15884 3.73067 4.11439C3.72543 4.20094 3.71729 4.25766 3.71729 4.31497C3.71613 6.33474 3.71264 8.35568 3.71962 10.376C3.72078 10.6608 3.75044 10.9491 3.80219 11.2286C3.90278 11.7713 4.147 12.2221 4.73195 12.4537Z\" transform=\"translate(163.564 11.0857)\" fill=\"#404A71\"></path>" +
    "<path fill-rule=\"evenodd\" clip-rule=\"evenodd\" d=\"M7.5519 3.73821C7.29489 3.44348 7.05998 3.12947 6.78088 2.86398C6.21046 2.32249 5.53655 1.96403 4.75099 1.85878C4.28757 1.79738 3.8253 1.80849 3.37874 1.98216C2.38618 2.36986 2.17046 3.49611 2.98393 4.19315C3.31362 4.47618 3.70087 4.70131 4.08754 4.90598C4.73704 5.2504 5.41386 5.54395 6.06626 5.88429C6.60121 6.16322 7.09255 6.51408 7.50189 6.96493C8.26245 7.80231 8.49503 8.79992 8.31478 9.89576C8.07987 11.3284 7.17337 12.1834 5.86449 12.6529C4.70506 13.0687 3.51306 13.0856 2.30943 12.8868C1.53608 12.7588 0.794717 12.533 0.113242 12.1337C0.0614921 12.1032 0.00334582 12.0278 0.00334582 11.9728C-0.00363174 10.8565 -0.00188735 9.74022 0.0300931 8.60811C0.0696326 8.68472 0.108009 8.76132 0.14813 8.83676C0.591205 9.68232 1.1686 10.4028 2.05766 10.8033C2.95427 11.2068 3.88171 11.2857 4.82426 10.9665C5.51329 10.7326 5.86798 10.2162 5.8017 9.54256C5.75576 9.07768 5.49643 8.71805 5.13069 8.46719C4.66784 8.15024 4.17418 7.87599 3.68401 7.60174C3.04149 7.24328 2.37048 6.93277 1.74599 6.54682C1.01451 6.09422 0.413859 5.496 0.184181 4.62646C-0.246102 2.99439 0.400485 1.22314 2.19488 0.457684C3.44735 -0.0767897 4.74053 -0.119477 6.05114 0.20682C6.53899 0.328451 7.00998 0.498617 7.43328 0.776965C7.4891 0.813805 7.54609 0.896841 7.54667 0.959411C7.55423 1.88801 7.5519 2.8172 7.5519 3.73821Z\" transform=\"translate(179.239 11.0857)\" fill=\"#404A71\"></path>" +
    "<path fill-rule=\"evenodd\" clip-rule=\"evenodd\" d=\"M3.61101 6.61601L3.61218 6.77507C3.61803 8.11593 3.61101 9.45679 3.63908 10.7971C3.64609 11.1187 3.74958 11.4438 3.84606 11.7561C3.9396 12.0602 4.14424 12.2941 4.49038 12.4449H0.0304036C0.702791 12.0993 0.820897 11.4725 0.894567 10.8269C0.916201 10.6357 0.913277 10.4409 0.913277 10.2474C0.914447 7.56625 0.919124 4.88453 0.908015 2.2028C0.906261 1.80166 0.859486 1.39349 0.775876 1.00111C0.685835 0.577161 0.463655 0.215778 0 0.00467811H4.45588C3.82267 0.354366 3.6648 0.964859 3.64317 1.59056C3.5999 2.86125 3.6192 4.13428 3.61335 5.40614V5.52484C3.88055 5.23422 4.1536 4.93774 4.40267 4.66349C5.26684 3.71383 6.14036 2.77236 6.98698 1.80692C7.23723 1.52097 7.44011 1.18181 7.61084 0.83972C7.81431 0.429801 7.73655 0.243846 7.34773 0H12.0375C11.9223 0.0473658 11.8445 0.0771887 11.7679 0.111105C11.1347 0.392376 10.5693 0.785337 10.0858 1.27128C9.22457 2.13672 8.39256 3.03258 7.55529 3.92201C7.05597 4.45297 6.56952 4.9968 6.06318 5.54882C6.36078 5.92716 6.65897 6.30609 6.95775 6.68501C7.99205 7.99488 9.00941 9.31937 10.0706 10.6082C10.7278 11.4058 11.3359 11.9309 12.2035 12.4525H12.0603C10.8196 12.4525 9.5783 12.4514 8.33819 12.4555C8.22183 12.4555 8.14992 12.4245 8.07508 12.3286C6.64377 10.4901 5.20779 8.65566 3.77297 6.82126L3.61101 6.61601Z\" transform=\"translate(234.105 11.0857)\" fill=\"#404A71\"></path>" +
    "<path fill-rule=\"evenodd\" clip-rule=\"evenodd\" d=\"M3.73263 4.09919V4.3711C3.73263 6.36632 3.72912 8.36211 3.73613 10.3573C3.7373 10.648 3.76654 10.9427 3.81916 11.2286C3.91973 11.7719 4.16529 12.2221 4.75349 12.4537H0.806865C1.40207 12.1444 1.54298 11.607 1.61373 11.0415C1.64998 10.7515 1.65349 10.4567 1.65349 10.1644C1.65641 7.59549 1.64413 5.02604 1.66109 2.45659C1.66752 1.51044 1.24596 0.830364 0.496982 0.312848C0.349641 0.211684 0.183591 0.115783 0 0.00292382H0.174236C1.37167 0.00233905 2.5691 0.00409334 3.76595 0C3.8747 0 3.9437 0.0292382 4.01444 0.118122C5.89713 2.48115 7.78274 4.84184 9.66834 7.20194L9.78996 7.33761L9.78937 7.20545C9.78645 5.39035 9.79405 3.57583 9.76949 1.76131C9.7654 1.42156 9.66367 1.07538 9.55784 0.747912C9.45143 0.418106 9.21521 0.173675 8.84745 0.0169581H12.7105C12.2024 0.269576 12.0153 0.721598 11.9293 1.21865C11.8732 1.54202 11.8562 1.87534 11.8562 2.20456C11.851 5.54063 11.8527 8.87671 11.8527 12.2128V12.4578H9.96536C9.62098 11.8303 9.33098 11.1924 8.94275 10.6216C8.40075 9.82519 7.80203 9.06617 7.19747 8.31475C6.08364 6.93178 4.94877 5.56636 3.82208 4.19392C3.80747 4.17579 3.78993 4.15942 3.73263 4.09919Z\" transform=\"translate(219.025 11.0857)\" fill=\"#404A71\"></path>" +
    "<path fill-rule=\"evenodd\" clip-rule=\"evenodd\" d=\"M4.48456 7.86671C5.05989 6.34399 5.63054 4.83296 6.21756 3.27808C6.8157 4.82828 7.39863 6.33814 7.98916 7.86671H4.48456ZM8.75097 12.4484H13.5571C13.0993 12.1876 12.866 11.9602 12.5579 11.4988C12.1497 10.8889 11.9007 10.2041 11.627 9.53165C10.3513 6.39615 9.07723 3.26065 7.80554 0.123977C7.77104 0.0403556 7.73187 -0.000577808 7.63189 6.95476e-06C6.97704 0.00526982 6.32278 0.00526982 5.66735 6.95476e-06C5.56854 -0.000577808 5.52527 0.0356775 5.4937 0.122222C5.35863 0.476589 5.21948 0.828616 5.08149 1.18123C3.95656 4.0682 2.83747 6.9581 1.70085 9.84099C1.3132 10.8251 0.937834 11.828 0 12.452H3.93668C3.40345 12.2572 3.19062 11.7964 3.31575 11.2772C3.45899 10.6819 3.66246 10.1012 3.84079 9.50767H4.91194H8.46682C8.55218 9.50767 8.63228 9.4913 8.67204 9.61117C8.8568 10.1579 9.06904 10.6959 9.23568 11.2485C9.40991 11.8263 9.26257 12.1502 8.75097 12.4484Z\" transform=\"translate(204.835 11.0857)\" fill=\"#404A71\"></path>" +
    "<path fill-rule=\"evenodd\" clip-rule=\"evenodd\" d=\"M9.18482 12.8841C8.80653 13.3777 8.27447 13.6279 7.68569 13.6548C6.68763 13.7004 5.68548 13.6671 4.65936 13.6671V8.7937H4.79501C5.66151 8.80715 6.53094 8.78025 7.39335 8.84808C8.60774 8.94398 9.4947 9.68488 9.63327 10.9679C9.70811 11.6625 9.62333 12.3128 9.18482 12.8841ZM4.66052 2.17827C4.84996 2.17827 5.03004 2.17652 5.20896 2.17827C5.77376 2.18412 6.33857 2.17885 6.9022 2.19991C7.92423 2.23733 8.67029 2.81742 8.8609 3.81327C9.02929 4.69217 8.8761 5.52428 8.16863 6.16519C7.8108 6.48973 7.36878 6.61955 6.90103 6.63066C6.16258 6.64761 5.42354 6.63475 4.66052 6.63475V2.17827ZM12.1644 8.82752C11.4967 8.1182 10.6746 7.71004 9.73032 7.49953C9.82679 7.46561 9.92385 7.43345 10.0192 7.39778C10.1226 7.35918 10.2296 7.32527 10.3279 7.27498C11.575 6.63758 12.2977 5.62828 12.4491 4.23304C12.6005 2.83487 12.0889 1.70511 10.9716 0.868895C10.0911 0.210452 9.06027 0.00286099 7.98153 0.00169146C5.3867 -0.00123235 2.79187 0.000521937 0.197039 0.000521937H0C0.546095 0.258987 0.88112 0.682356 1.02963 1.2332C1.14189 1.65131 1.23485 2.08696 1.24596 2.51734C1.2752 3.71903 1.25824 4.9213 1.25824 6.12358C1.25766 8.55151 1.27754 10.98 1.24362 13.408C1.23018 14.3518 1.12318 15.3049 0.174821 15.9119H0.402263C3.06024 15.9119 5.71938 15.9149 8.37794 15.9108C9.53913 15.909 10.6255 15.6494 11.5855 14.9553C13.5354 13.5448 13.8155 10.5836 12.1644 8.82752Z\" transform=\"translate(190.156 7.96802)\" fill=\"#404A71\"></path>" +
    "<path fill-rule=\"evenodd\" clip-rule=\"evenodd\" d=\"M21.7305 12.2895C22.7665 12.9575 23.6721 14.2012 24.0806 15.2929C24.2197 15.6638 24.3312 16.0124 24.4181 16.3981C24.5572 17.0179 24.5889 17.6071 24.5771 18.2404C24.5619 19.0739 24.3975 19.8081 24.2256 20.6146C24.0589 21.3981 23.8758 22.221 23.9544 23.0263L24.0536 24.054L23.0265 23.9542C22.2212 23.8756 21.3983 24.0587 20.6148 24.2254C19.8077 24.3974 19.0741 24.5617 18.2406 24.577C17.6073 24.5881 17.018 24.557 16.3976 24.4179C16.012 24.3311 15.664 24.2195 15.293 24.081C14.2013 23.6719 12.9576 22.7663 12.2897 21.7303C11.6218 22.7663 10.3775 23.6719 9.28577 24.081C8.916 24.2195 8.56677 24.3311 8.18174 24.4179C7.56136 24.557 6.97207 24.5881 6.33877 24.577C5.50533 24.5617 4.77166 24.3974 3.96404 24.2254C3.18107 24.0587 2.35819 23.8756 1.55292 23.9542L0.525199 24.054L0.624978 23.0263C0.70304 22.221 0.520503 21.3981 0.353814 20.6146C0.181843 19.8081 0.0175016 19.0739 0.00224133 18.2404C-0.00949734 17.6071 0.0221971 17.0179 0.1613 16.3975C0.248166 16.0124 0.359097 15.6638 0.4982 15.2929C0.907293 14.2012 1.81293 12.9575 2.84887 12.2895C1.81293 11.6216 0.907293 10.3773 0.4982 9.2856C0.359097 8.91525 0.248166 8.56661 0.1613 8.18158C0.0221971 7.56119 -0.00949734 6.97191 0.00224133 6.33861C0.0175016 5.50516 0.181843 4.7715 0.353814 3.96446C0.520503 3.18091 0.70304 2.35803 0.624978 1.55275L0.525199 0.525033L1.55292 0.624812C2.35819 0.703461 3.18107 0.520338 3.96404 0.353649C4.77166 0.181677 5.50533 0.0173358 6.33877 0.00207549C6.97207 -0.00907624 7.56136 0.0220312 8.18174 0.161134C8.56677 0.248001 8.916 0.359518 9.28577 0.498621C10.3775 0.907127 11.6218 1.81276 12.2897 2.8487C12.9576 1.81276 14.2013 0.907127 15.293 0.498621C15.664 0.359518 16.0126 0.247414 16.3976 0.161134C17.018 0.0220312 17.6073 -0.00907624 18.2406 0.00207549C19.0741 0.0173358 19.8077 0.181677 20.6148 0.353649C21.3983 0.520338 22.2212 0.703461 23.0265 0.624812L24.0536 0.525033L23.9544 1.55275C23.8758 2.35803 24.0589 3.18091 24.2256 3.96446C24.3975 4.7715 24.5619 5.50516 24.5771 6.33861C24.5889 6.97191 24.5572 7.56119 24.4181 8.18099C24.3312 8.56661 24.2197 8.91525 24.0806 9.2856C23.6721 10.3773 22.7665 11.6216 21.7305 12.2895ZM22.2441 22.244C22.2752 21.5784 22.4008 20.9275 22.5293 20.273C22.6849 19.4824 22.9009 18.835 22.9044 18.0122C22.9067 17.5655 22.8527 17.0789 22.7395 16.6464C22.2922 14.9419 20.6001 12.9305 18.8299 12.5443L17.6613 12.2896L18.8299 12.0348C20.6001 11.6492 22.2922 9.63722 22.7395 7.93276C22.8527 7.50019 22.9067 7.01362 22.9044 6.56697C22.9009 5.74409 22.6849 5.0967 22.5293 4.3061C22.4008 3.65167 22.2752 3.00076 22.2441 2.33518C21.5779 2.30407 20.9276 2.17847 20.2732 2.04993C19.4826 1.89439 18.8346 1.6784 18.0123 1.67488C17.5656 1.67253 17.0791 1.72653 16.6465 1.83981C14.942 2.28705 12.93 3.97918 12.5444 5.74878L12.2897 6.91737L12.035 5.74937C11.6494 3.97918 9.63734 2.28705 7.93289 1.83981C7.50032 1.72653 7.01375 1.67253 6.5671 1.67488C5.74422 1.6784 5.09683 1.89439 4.30623 2.04993C3.6518 2.17847 3.00089 2.30407 2.33531 2.33518C2.3042 3.00076 2.17859 3.65167 2.05006 4.3061C1.89393 5.0967 1.67853 5.74409 1.67442 6.56697C1.67266 7.01362 1.72666 7.50019 1.83993 7.93276C2.28718 9.63722 3.97931 11.6492 5.74891 12.0348L6.91749 12.2896L5.74891 12.5443C3.97931 12.9305 2.28718 14.9419 1.83993 16.6464C1.72666 17.0789 1.67266 17.5655 1.67442 18.0122C1.67853 18.835 1.89393 19.4824 2.05006 20.273C2.17859 20.9275 2.3042 21.5784 2.33531 22.244C3.00089 22.2751 3.6518 22.4007 4.30623 22.5292C5.09683 22.6847 5.74422 22.9007 6.5671 22.9048C7.01375 22.9066 7.50032 22.8526 7.93289 22.7393C9.63734 22.2921 11.6494 20.6 12.035 18.8298L12.2897 17.6618L12.5444 18.8304C12.93 20.6 14.942 22.2921 16.6465 22.7393C17.0791 22.8526 17.5656 22.9066 18.0123 22.9048C18.8346 22.9007 19.4826 22.6847 20.2732 22.5292C20.9276 22.4007 21.5779 22.2751 22.2441 22.244Z\" transform=\"translate(4.34775 4.34814)\" fill=\"#D1AA3B\"></path>" +
    "<path fill-rule=\"evenodd\" clip-rule=\"evenodd\" d=\"M23.313 9.96202C24.518 9.70201 26.0381 9.94148 27.0987 10.4239C27.4591 10.5877 27.7848 10.7556 28.1182 10.9663C28.6553 11.3067 29.0943 11.7011 29.5339 12.1572C30.1126 12.757 30.5153 13.3926 30.9643 14.0846C31.4003 14.7561 31.8529 15.4675 32.4774 15.9816L33.275 16.6384L32.4774 17.2934C31.8523 17.8076 31.4003 18.5189 30.9643 19.191C30.5153 19.883 30.1126 20.518 29.5339 21.1185C29.0943 21.5745 28.6553 21.9689 28.1182 22.3093C27.7848 22.52 27.4591 22.6879 27.0987 22.8517C26.0381 23.3341 24.518 23.5736 23.313 23.3136C23.573 24.5186 23.3335 26.0381 22.8511 27.0993C22.6873 27.4597 22.5195 27.7848 22.3088 28.1188C21.9683 28.6558 21.5739 29.0943 21.1179 29.5345C20.518 30.1132 19.883 30.5153 19.191 30.9648C18.5189 31.4009 17.8076 31.8529 17.2934 32.478L16.6372 33.275L15.981 32.478C15.4675 31.8535 14.7561 31.4009 14.0841 30.9648C13.3921 30.5153 12.757 30.1132 12.1566 29.5345C11.7005 29.0943 11.3061 28.6558 10.9663 28.1188C10.755 27.7848 10.5871 27.4597 10.4239 27.0993C9.94089 26.0381 9.70201 24.5186 9.96143 23.3136C8.75705 23.5736 7.23689 23.3341 6.17571 22.8517C5.81534 22.6879 5.49017 22.52 5.1568 22.3088C4.61917 21.9689 4.18073 21.5745 3.74111 21.1185C3.1624 20.518 2.75976 19.883 2.31017 19.191C1.87467 18.5189 1.42214 17.8076 0.797055 17.294L0 16.6378L0.797055 15.9816C1.42214 15.468 1.87467 14.7561 2.31017 14.0846C2.75976 13.3926 3.1624 12.757 3.74111 12.1572C4.18073 11.7011 4.61917 11.3067 5.15621 10.9663C5.49017 10.7556 5.81592 10.5877 6.17571 10.4239C7.23689 9.94148 8.75705 9.70201 9.96143 9.96202C9.70201 8.75705 9.94089 7.23747 10.4239 6.1763C10.5871 5.81651 10.755 5.49017 10.9663 5.1568C11.3067 4.61975 11.7011 4.18073 12.1566 3.74111C12.757 3.1624 13.3921 2.76035 14.0841 2.31076C14.7561 1.87467 15.4675 1.42273 15.9816 0.797642L16.6372 0L17.2934 0.797642C17.8076 1.42214 18.5189 1.87467 19.191 2.31076C19.883 2.75976 20.518 3.16298 21.1179 3.74111C21.5739 4.18073 21.9677 4.61975 22.3088 5.1568C22.5195 5.49017 22.6873 5.81651 22.8511 6.1763C23.3335 7.23747 23.573 8.75705 23.313 9.96202ZM30.7154 16.6378C30.2664 16.1453 29.8949 15.5965 29.5233 15.0419C29.0743 14.3734 28.7686 13.763 28.1898 13.1784C27.8752 12.8608 27.4931 12.5545 27.1075 12.3297C25.5862 11.4405 22.9667 11.2145 21.4419 12.1929L20.4359 12.8391L21.0821 11.8331C22.0611 10.3083 21.8345 7.68878 20.9459 6.16804C20.7205 5.78183 20.4141 5.39974 20.0966 5.08514C19.512 4.50643 18.9016 4.20064 18.2331 3.75222C17.6784 3.3801 17.1303 3.00857 16.6372 2.55957C16.1448 3.00916 15.596 3.38069 15.0419 3.75222C14.3728 4.20064 13.7624 4.50643 13.1778 5.08514C12.8603 5.39974 12.5545 5.78183 12.3291 6.16804C11.4399 7.68878 11.214 10.3083 12.193 11.8331L12.8386 12.8391L11.8326 12.1929C10.3083 11.2139 7.68884 11.4405 6.1681 12.3297C5.7819 12.555 5.39921 12.8608 5.08462 13.1784C4.50649 13.763 4.2007 14.3734 3.75169 15.0419C3.38016 15.5965 3.00864 16.1447 2.55963 16.6378C3.00864 17.1302 3.38016 17.679 3.75169 18.233C4.2007 18.9021 4.5059 19.5126 5.08462 20.0971C5.39921 20.4147 5.7819 20.7205 6.1681 20.9458C7.68884 21.835 10.3083 22.061 11.8326 21.0826L12.8386 20.4364L12.193 21.4424C11.214 22.9672 11.4399 25.5861 12.3291 27.1075C12.5545 27.4937 12.8603 27.8758 13.1778 28.1904C13.7624 28.7691 14.3728 29.0749 15.0419 29.5233C15.596 29.8954 16.1448 30.2669 16.6372 30.7154C17.1303 30.2663 17.6784 29.8954 18.2331 29.5233C18.9022 29.0749 19.5126 28.7691 20.0966 28.1904C20.4141 27.8758 20.7205 27.4937 20.9459 27.1075C21.8345 25.5861 22.0611 22.9672 21.0821 21.4424L20.4364 20.4364L21.4425 21.0826C22.9667 22.061 25.5862 21.835 27.1075 20.9458C27.4931 20.7205 27.8752 20.4147 28.1904 20.0971C28.7686 19.5126 29.0743 18.9021 29.5233 18.2336C29.8949 17.679 30.2664 17.1302 30.7154 16.6378Z\" transform=\"translate(0 -0.000244141)\" fill=\"#D1AA3B\"></path>" +
"</svg>"; break;
                #endregion
                case "Turon Bank":
                    #region Turon Bank
                    qaytar = "<svg  id=\"BankIcon\" viewBox=\"0 0 191 62\">" +
                                "<g clip-path=\"url(#clip0)\">" +
                                "<path fill-rule=\"evenodd\" clip-rule=\"evenodd\" d=\"M30.846 0.391173L51.9394 21.2473L51.9228 21.2413C44.531 20.7344 39.6222 13.1239 22.9756 20.7344C43.7591 19.4564 42.5943 29.7923 55.533 24.8501L55.5811 24.8486L60.3184 29.5328C44.7267 33.686 42.4197 20.7869 22.8613 29.8403C46.1427 28.5009 41.6568 41.2755 60.8992 31.5787L53.0199 39.4517C42.3701 39.3152 42.7719 31.4212 23.087 38.6642C23.9222 39.1682 31.1048 38.7137 35.3259 39.6691C39.9834 40.9036 44.4137 43.8854 48.0283 44.3023L48.1276 44.3413L30.8445 61.6112L-0.000366211 30.8902L30.8445 0.394173L30.846 0.391173ZM68.1857 26.887H190.593V28.3494H68.1857V26.887ZM77.9115 47.7476V56.6135L77.8905 57.6365C77.8905 58.3669 78.0726 58.9234 78.4488 59.2999C78.8265 59.6689 79.3773 59.8563 80.1176 59.8563C81.2071 59.8563 82.2124 59.6194 83.1334 59.1319C84.0558 58.6519 84.7601 57.9485 85.2492 57.0305C85.7443 56.1186 85.9895 55.0956 85.9895 53.9692C85.9895 52.6748 85.6886 51.5124 85.0896 50.4894C84.4892 49.4665 83.6646 48.7496 82.6112 48.3386C81.5638 47.9276 79.9927 47.7326 77.9115 47.7476V47.7476ZM77.9115 46.1532C79.8453 46.1532 81.2703 45.9372 82.1913 45.5053C83.1123 45.0673 83.8181 44.4553 84.3131 43.6679C84.8022 42.8819 85.046 41.8725 85.046 40.6471C85.046 39.4292 84.8022 38.4272 84.3192 37.6398C83.8376 36.8598 83.1469 36.2689 82.2455 35.8579C81.3441 35.4469 79.8994 35.2519 77.91 35.2669V46.1502L77.9115 46.1532ZM85.3545 46.7097C87.9865 47.3156 89.8374 48.0596 90.8984 48.9295C92.4273 50.1685 93.1887 51.7689 93.1887 53.7382C93.1887 55.8186 92.3505 57.5375 90.6756 58.8949C88.6155 60.5448 85.6209 61.3652 81.6962 61.3652H67.5988V60.6138C68.884 60.6138 69.7492 60.4953 70.2097 60.2523C70.6642 60.0153 70.9847 59.7093 71.1668 59.3269C71.3549 58.9444 71.4467 58.0115 71.4467 56.5146V38.5742C71.4467 37.0773 71.3564 36.1384 71.1668 35.7484C70.9847 35.3584 70.6567 35.0524 70.1962 34.823C69.7357 34.586 68.8704 34.475 67.5988 34.475V33.716H80.8926C84.0694 33.716 86.3251 34.001 87.6449 34.565C88.9706 35.1289 90.012 35.9704 90.7735 37.0908C91.5409 38.2112 91.9186 39.4082 91.9186 40.6681C91.9186 42.0045 91.4371 43.1879 90.4665 44.2303C89.5034 45.2668 87.7984 46.0947 85.356 46.7082L85.3545 46.7097ZM110.488 51.9848L106.362 42.465L102.103 51.9848H110.488V51.9848ZM111.187 53.4952H101.39L100.224 56.1891C99.8356 57.0845 99.6505 57.824 99.6505 58.4089C99.6505 59.1889 99.9681 59.7528 100.593 60.1203C100.96 60.3363 101.864 60.5028 103.309 60.6138V61.3652H94.0856V60.6138C95.0773 60.4608 95.8975 60.0468 96.5355 59.3749C97.1751 58.7029 97.9636 57.311 98.9087 55.2066L108.83 33.1461H109.221L119.22 55.8201C120.169 57.9725 120.958 59.3209 121.573 59.8773C122.041 60.3018 122.697 60.5448 123.543 60.6153V61.3667H110.123V60.6153H110.675C111.751 60.6153 112.514 60.4653 112.944 60.1623C113.247 59.9493 113.399 59.6329 113.399 59.2234C113.399 58.9804 113.349 58.7314 113.272 58.4719C113.236 58.3489 113.042 57.839 112.658 56.9405L111.185 53.4952H111.187ZM135.649 33.7175L148.517 50.8644V38.9777C148.517 37.3218 148.295 36.2014 147.848 35.6164C147.231 34.8305 146.2 34.448 144.749 34.475V33.716H153.363V34.475C152.26 34.622 151.526 34.8155 151.145 35.0524C150.764 35.2894 150.468 35.6779 150.259 36.2149C150.049 36.7503 149.944 37.6758 149.944 38.9777V61.9982H149.288L131.654 38.9777V56.5566C131.654 58.1435 131.996 59.2159 132.685 59.7723C133.368 60.3363 134.155 60.6138 135.035 60.6138H135.652V61.3652H126.402V60.6138C127.839 60.6003 128.839 60.2868 129.403 59.6749C129.967 59.0629 130.25 58.025 130.25 56.5566V37.0428L129.692 36.3049C129.141 35.5744 128.655 35.0944 128.228 34.8575C127.808 34.628 127.197 34.502 126.402 34.475V33.716H135.652L135.649 33.7175ZM172.854 44.3023L182.196 56.4741C183.52 58.1854 184.656 59.3479 185.606 59.9598C186.301 60.3918 187.031 60.6138 187.798 60.6138V61.3652H173.97V60.6138C174.846 60.5298 175.408 60.3843 175.662 60.1818C175.922 59.9793 176.05 59.7288 176.05 59.4304C176.05 58.8604 175.448 57.7805 174.244 56.2086L168.153 48.2336L167.17 49.09V56.6345C167.17 58.0745 167.25 58.9729 167.411 59.3284C167.564 59.6839 167.865 59.9823 168.306 60.2328C168.755 60.4908 169.463 60.6153 170.446 60.6153V61.3667H157.045V60.6153H157.927C158.696 60.6153 159.318 60.4698 159.786 60.1833C160.127 59.9958 160.388 59.6689 160.568 59.2024C160.716 58.8829 160.782 58.0265 160.782 56.6345V38.4497C160.782 37.0368 160.716 36.1534 160.568 35.7979C160.428 35.4439 160.133 35.1364 159.698 34.8725C159.257 34.6085 158.669 34.4765 157.927 34.4765H157.045V33.7175H170.265V34.4765C169.37 34.4765 168.681 34.6085 168.213 34.8799C167.873 35.0749 167.605 35.3944 167.411 35.8399C167.25 36.1804 167.17 37.0503 167.17 38.4497V47.0517L177.027 38.7557C178.398 37.6008 179.085 36.6753 179.085 35.9854C179.085 35.4634 178.804 35.0599 178.243 34.76C177.955 34.607 177.239 34.517 176.103 34.475V33.716H186.467V34.475C185.537 34.544 184.814 34.712 184.286 34.9894C183.75 35.2684 182.534 36.2149 180.628 37.8153L172.851 44.3008L172.854 44.3023ZM85.8842 0.337177V6.70727H85.2808C84.9181 5.23286 84.5223 4.17993 84.0844 3.53947C83.648 2.89451 83.0446 2.38604 82.2786 2.00657C81.8527 1.79358 81.1063 1.69309 80.0424 1.69309H78.3404V19.8299C78.3404 21.0313 78.4036 21.7828 78.53 22.0858C78.6625 22.3873 78.9153 22.6482 79.2885 22.8732C79.6677 23.1042 80.1793 23.2167 80.8294 23.2167H81.5879V23.8557H69.6304V23.2167H70.3888C71.0494 23.2167 71.5852 23.0922 71.987 22.8492C72.2804 22.6902 72.5106 22.4113 72.6762 22.0138C72.8026 21.7408 72.8658 21.0133 72.8658 19.8299V1.69309H71.2105C69.6755 1.69309 68.5544 2.02457 67.8592 2.69952C66.8825 3.63396 66.267 4.97338 66.0082 6.70727H65.3701V0.337177H85.8827H85.8842ZM87.9428 0.337177H99.7288V0.982135H99.1419C98.2571 0.982135 97.6476 1.07663 97.3135 1.26562C96.9794 1.4546 96.7447 1.72159 96.6062 2.05307C96.4678 2.39054 96.3925 3.23049 96.3925 4.5684V16.0817C96.3925 18.189 96.5475 19.5809 96.8575 20.2664C97.1675 20.9473 97.6747 21.5098 98.3805 21.9658C99.0877 22.4158 99.9786 22.6407 101.059 22.6407C102.295 22.6407 103.348 22.3573 104.215 21.7828C105.089 21.2083 105.738 20.4209 106.17 19.4144C106.601 18.408 106.82 16.6561 106.82 14.1573V4.5669C106.82 3.51397 106.71 2.76102 106.498 2.31105C106.285 1.86108 106.015 1.5476 105.693 1.37661C105.187 1.10963 104.474 0.980635 103.548 0.980635V0.335677H111.447V0.980635H110.976C110.338 0.980635 109.804 1.11113 109.378 1.37661C108.952 1.64359 108.642 2.03957 108.452 2.57203C108.308 2.94551 108.234 3.60847 108.234 4.5669V13.5003C108.234 16.2647 108.057 18.2595 107.706 19.4849C107.349 20.7104 106.487 21.8353 105.119 22.8657C103.751 23.8962 101.876 24.4106 99.5076 24.4106C97.5362 24.4106 96.0058 24.1376 94.9253 23.5932C93.4596 22.8537 92.4183 21.9058 91.8088 20.7524C91.2054 19.5914 90.9014 18.042 90.9014 16.0817V4.5684C90.9014 3.21849 90.8321 2.37855 90.6832 2.04707C90.5387 1.71559 90.2859 1.4546 89.9247 1.25962C89.5681 1.05863 88.9074 0.970136 87.9413 0.982135V0.337177H87.9428ZM121.206 13.1359V19.7309C121.206 21.0028 121.281 21.8023 121.43 22.1278C121.585 22.4593 121.85 22.7262 122.229 22.9212C122.614 23.1162 123.328 23.2182 124.374 23.2182V23.8572H112.681V23.2182C113.739 23.2182 114.452 23.1177 114.831 22.9107C115.205 22.7097 115.469 22.4488 115.624 22.1233C115.773 21.7978 115.849 21.0043 115.849 19.7324V4.47241C115.849 3.19899 115.773 2.40104 115.624 2.06957C115.469 1.73809 115.205 1.4771 114.819 1.28212C114.44 1.08113 113.727 0.986635 112.681 0.986635V0.341676H123.293C126.059 0.341676 128.076 0.542663 129.358 0.934138C130.634 1.32561 131.681 2.05307 132.491 3.112C133.296 4.16493 133.704 5.40835 133.704 6.82876C133.704 8.56265 133.094 9.99655 131.881 11.132C131.105 11.8489 130.024 12.3874 128.632 12.7414L134.122 20.7029C134.841 21.7333 135.353 22.3723 135.651 22.6272C136.117 22.9887 136.646 23.1837 137.255 23.2197V23.8587H130.063L122.705 13.1374H121.204L121.206 13.1359ZM121.206 1.60459V11.9044H122.166C123.724 11.9044 124.891 11.7619 125.668 11.4665C126.444 11.171 127.054 10.637 127.496 9.86756C127.939 9.09811 128.163 8.09768 128.163 6.86026C128.163 5.06637 127.755 3.74646 126.938 2.88851C126.127 2.03057 124.816 1.60459 123.006 1.60459H121.206V1.60459ZM150.272 0.0116977C153.911 -0.130793 156.866 0.964136 159.143 3.28599C161.414 5.61234 162.552 8.51915 162.552 12.0109C162.552 14.9942 161.706 17.6161 160.01 19.8839C157.762 22.8912 154.589 24.3941 150.49 24.3941C146.385 24.3941 143.207 22.9617 140.959 20.0909C139.187 17.823 138.296 15.1367 138.296 12.0289C138.296 8.53565 139.452 5.62434 141.758 3.29799C144.069 0.965636 146.903 -0.129293 150.272 0.0131976V0.0116977ZM150.48 1.11863C148.386 1.11863 146.8 2.21956 145.708 4.41541C144.823 6.2153 144.379 8.80713 144.379 12.1819C144.379 16.1957 145.064 19.161 146.432 21.0793C147.386 22.4233 148.725 23.0922 150.445 23.0922C151.601 23.0922 152.567 22.8027 153.343 22.2283C154.326 21.4888 155.097 20.3039 155.649 18.6765C156.201 17.0551 156.477 14.9357 156.477 12.3244C156.477 9.21061 156.195 6.88425 155.631 5.34535C155.068 3.80045 154.355 2.71152 153.48 2.07256C152.612 1.43961 151.611 1.12013 150.48 1.12013V1.11863ZM172.372 0.337177L183.641 14.9237V4.81289C183.641 3.40448 183.445 2.45054 183.054 1.95407C182.513 1.28512 181.61 0.959637 180.34 0.983635V0.338677H187.883V0.983635C186.917 1.10813 186.272 1.27312 185.94 1.4756C185.606 1.67659 185.349 2.00807 185.163 2.46404C184.98 2.92001 184.888 3.70746 184.888 4.81439V24.3971H184.313L168.872 4.81439V19.7669C168.872 21.1168 169.171 22.0288 169.775 22.5012C170.372 22.9812 171.063 23.2182 171.833 23.2182H172.374V23.8572H164.273V23.2182C165.533 23.2062 166.406 22.9392 166.901 22.4188C167.396 21.8983 167.642 21.0163 167.642 19.7669V3.16899L167.153 2.54203C166.67 1.92107 166.246 1.5116 165.871 1.31061C165.504 1.11563 164.968 1.00913 164.273 0.985135V0.340177H172.374L172.372 0.337177Z\" fill=\"#103689\"/>" +
                                "</g><defs><clipPath id=\"clip0\">" +
                            "<rect width=\"190.593\" height=\"62\" fill=\"white\"/></clipPath></defs></svg>";
                    break;
                #endregion
                case "Tenge Bank":
                    qaytar = "<img id=\"BankIcon\" src=\"https://tengebank.uz/storage/app/uploads/public/61e/7a5/856/61e7a5856c640571064545.png\">";
                    break;
            }
            return qaytar;
        }

        public string GetCurrencyNames(string name)
        {
            string suz = "";
            switch (name)
            {
                case "AQSH dollari" or "USD":
                    suz = "Доллар";
                    break;
                case "EVRO" or "EUR":
                    suz = "Евро";
                    break;
                case "Rossiya rubli":
                    suz = "Рубль";
                    break;
                case "Xitoy yuani":
                    suz = "Юань";
                    break;
            }
            return suz;
        }

        public string GetRegionNames(string name)
        {
            string qaytar = "";
            switch (name)
            {
                case "Ташкент":
                    qaytar = "Тошкент";
                    break;
                case "Джизак":
                    qaytar = "Жиззах";
                    break;
                case "Самарканд":
                    qaytar = "Самарқанд";
                    break;
                case "Бухара":
                    qaytar = "Бухоро";
                    break;
                case "Коканд":
                    qaytar = "Қўқон";
                    break;
                case "Наманган":
                    qaytar = "Наманган";
                    break;
                case "Фергана":
                    qaytar = "Фарғона";
                    break;
                case "Маргилан":
                    qaytar = "Марғилон";
                    break;
                case "Андижан":
                    qaytar = "Андижон";
                    break;
            }
            return qaytar;
        }

        public string GetRate(double currency)
        {
            string qaytar = "";
            if (currency >= 0)
            {
                qaytar += "<svg style=\"color:green\" width=\"50px\" height=\"50px\"  fill=\"currentColor\" class=\"bi bi-arrow-up\" viewBox=\"0 0 16 16\">" +
                              "<path fill-rule=\"evenodd\" d=\"M8 15a.5.5 0 0 0 .5-.5V2.707l3.146 3.147a.5.5 0 0 0 .708-.708l-4-4a.5.5 0 0 0-.708 0l-4 4a.5.5 0 1 0 " + ".708.708L7.5 2.707V14.5a.5.5 0 0 0 .5.5z\"/>" +
                           "</svg>"
                + currency.ToString();
            }
            if (currency < 0)
            {
                qaytar += "<svg style=\"color:red\" width=\"50px\" height=\"50px\" fill=\"currentColor\" class=\"bi bi-arrow-down\" viewBox=\"0 0 16 16\">" +
                              "<path fill-rule=\"evenodd\" d=\"M8 1a.5.5 0 0 1 .5.5v11.793l3.146-3.147a.5.5 0 0 1 .708.708l-4 4a.5.5 0 0 1-.708 0l-4-4a.5.5 0 0 1 " + ".708-.708L7.5 13.293V1.5A.5.5 0 0 1 8 1z\"/>" +
                           "</svg>"
                + currency.ToString();
            }
            return qaytar;
        }
    }
}
