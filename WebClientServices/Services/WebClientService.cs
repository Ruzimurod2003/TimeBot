using Dapper;
using HtmlAgilityPack;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json;
using System.Data;
using System.Net.Http.Json;
using TimeBot.ViewModel;
using WebClientServices.Services;

namespace TelegramBotService.Services
{
    public interface IWebClientService
    {
        StateViewModel<List<BankRate>> GetBankRate(TimeBot.Model.Settings settings);
        StateViewModel<List<RegionRate>> GetRegionRate(TimeBot.Model.Settings settings);
        StateViewModel<List<MarketRate>> GetMarketRate(TimeBot.Model.Settings settings);
        StateViewModel<List<CbuRate>> GetCbuRate(TimeBot.Model.Settings settings);
        StateViewModel<List<BirjaRate>> GetBirjaRate(TimeBot.Model.Settings settings);
    }
    public class WebClientService : IWebClientService
    {
        private static IDbConnection _db = new SqliteConnection();

        public WebClientService()
        {
            _db = new SqliteConnection("Data Source=" + TimeBot.Model.Settings.GetSettings().Path.Database + TimeBot.Model.Settings.GetSettings().Proccess.Other.DatabaseName);
        }

        public StateViewModel<List<BankRate>> GetBankRate(TimeBot.Model.Settings settings)
        {
            StateViewModel<List<BankRate>> stateView = new StateViewModel<List<BankRate>>
            {
                Response = new List<BankRate>()
            };
            try
            {
                var BankNames = _db.Query<string>("select BankName from BankRate", new { UpdateTime = DateTime.Now.ToString("dd.MM.yyyy") });
                foreach (var itemName in BankNames.Distinct())
                {
                    var maxId = _db.Query<int>("select max(Id) from BankRate where BankName=@BankName ", new { BankName = itemName.ToString(), UpdateTime = DateTime.Now.ToString("dd.MM.yyyy") });
                    foreach (var itemId in maxId)
                    {
                        var bankRates = _db.Query<BankRate>("select * from BankRate where Id=@Id", new { Id = itemId }).ToList();
                        foreach (var item in bankRates)
                        {
                            stateView.Response.Add(item);
                        }

                    }
                }

                stateView.Code = 200;
                stateView.Msg = "Success";
                stateView.Count = stateView.Response.Count();
            }
            catch (Exception)
            {
                stateView.Code = 400;
                stateView.Msg = "Error";
            }
            return stateView;
        }

        public StateViewModel<List<RegionRate>> GetRegionRate(TimeBot.Model.Settings settings)
        {
            StateViewModel<List<RegionRate>> stateView = new StateViewModel<List<RegionRate>>
            {
                Response = new List<RegionRate>()
            };
            try
            {
                var NameRegions = _db.Query<string>("select RegionName from RegionRate", new { UpdateTime = DateTime.Now.ToString("dd.MM.yyyy") });
                foreach (var itemName in NameRegions.Distinct())
                {
                    var MaxId = _db.Query<int>("select max(Id) from RegionRate where RegionName=@RegionName ", new { RegionName = itemName.ToString() });
                    foreach (var itemId in MaxId)
                    {
                        var regionRates = _db.Query<RegionRate>("select * from RegionRate where Id=@Id", new { Id = itemId }).ToList();

                        foreach (RegionRate r in regionRates)
                        {
                            stateView.Response.Add(r);
                        }
                    }
                }

                stateView.Code = 200;
                stateView.Msg = "Success";
                stateView.Count = stateView.Response.Count();
            }
            catch (Exception)
            {
                stateView.Code = 400;
                stateView.Msg = "Error";
            }
            return stateView;
        }

        public StateViewModel<List<MarketRate>> GetMarketRate(TimeBot.Model.Settings settings)
        {
            StateViewModel<List<MarketRate>> stateView = new StateViewModel<List<MarketRate>>
            {
                Response = new List<MarketRate>()
            };
            try
            {
                var MarketNames = _db.Query<string>("select [Name] from MarketRate");
                foreach (var itemName in MarketNames.Distinct())
                {
                    var maxId = _db.Query<int>("select max(Id) from MarketRate where [Name]=@Name", new { Name = itemName.ToString() });
                    foreach (var itemId in maxId)
                    {
                        var marketRates = _db.Query<MarketRate>("select * from MarketRate where Id=@Id", new { Id = itemId }).ToList();

                        foreach (MarketRate r in marketRates)
                        {
                            stateView.Response.Add(r);
                        }
                    }
                }
                stateView.Code = 200;
                stateView.Msg = "Success";
                stateView.Count = stateView.Response.Count();
            }
            catch (Exception)
            {
                stateView.Code = 400;
                stateView.Msg = "Error";
            }
            return stateView;
        }

        public StateViewModel<List<CbuRate>> GetCbuRate(TimeBot.Model.Settings settings)
        {

            List<string> CbuNames = new List<string>() { "AQSH dollari", "EVRO", "Rossiya rubli", "Xitoy yuani" };

            StateViewModel<List<CbuRate>> stateView = new StateViewModel<List<CbuRate>>
            {
                Response = new List<CbuRate>()
            };
            try
            {
                foreach (var itemName in CbuNames.Distinct())
                {
                    var maxId = _db.Query<int>("select max(Id) from CbuRate where [CcyNm_UZ]=@CcyNm_UZ", new { CcyNm_UZ = itemName.ToString() });
                    foreach (var itemId in maxId)
                    {
                        var cbuRates = _db.Query<CbuRate>("select * from CbuRate where Id=@Id", new { Id = itemId });

                        foreach (CbuRate r in cbuRates)
                        {
                            stateView.Response.Add(r);
                        }
                    }
                }

                stateView.Code = 200;
                stateView.Msg = "Success";
                stateView.Count = stateView.Response.Count();
            }
            catch (Exception)
            {
                stateView.Code = 400;
                stateView.Msg = "Error";
            }
            return stateView;
        }

        public StateViewModel<List<BirjaRate>> GetBirjaRate(TimeBot.Model.Settings settings)
        {
            StateViewModel<List<BirjaRate>> stateView = new StateViewModel<List<BirjaRate>>
            {
                Response = new List<BirjaRate>()
            };
            try
            {
                var BirjaNames = _db.Query<string>("select [Name] from BirjaRate");
                foreach (var itemName in BirjaNames.Distinct())
                {
                    var maxId = _db.Query<int>("select max(Id) from BirjaRate where [Name]=@Name", new { Name = itemName.ToString() });
                    foreach (var itemId in maxId)
                    {
                        var BirjaRates = _db.Query<BirjaRate>("select * from BirjaRate where Id=@Id", new { Id = itemId });

                        foreach (BirjaRate r in BirjaRates)
                        {
                            stateView.Response.Add(r);
                        }
                    }
                }

                stateView.Code = 200;
                stateView.Msg = "Success";
                stateView.Count = stateView.Response.Count();
            }
            catch (Exception)
            {
                stateView.Code = 400;
                stateView.Msg = "Error";
            }
            return stateView;
        }
    }
}
