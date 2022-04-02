using DB_Services;
using Newtonsoft.Json;
using TelegramBotService.Services;
//using TimeBot.Model;
using TimeBot.ViewModel;

namespace WebClientServices.Services
{
    public interface IMainMethods
    {
        Task ProcessRegionCurrencyRate(TimeBot.Model.Settings settings);
        Task ProcessMarketCurrencyRate(TimeBot.Model.Settings settings);
        Task ProcessCbuCurrencyRate(TimeBot.Model.Settings settings);
        Task ProcessBankCurrencyRate(TimeBot.Model.Settings settings);
        Task ProcessBirjaCurrencyRate(TimeBot.Model.Settings settings);
        Task ProccessWeatherDaily(TimeBot.Model.Settings settings);
    }

    public class MainMethods : IMainMethods
    {
        private readonly IDataBaseServices databaseService;
        private readonly IWebClientService webClientService;
        private readonly IHelperService helperService;
        private readonly ITelegramService telegramService;

        public MainMethods(IDataBaseServices _dataBaseServices, IWebClientService _webClientService, IHelperService _helperService, ITelegramService _telegramService)
        {
            databaseService = _dataBaseServices;
            webClientService = _webClientService;
            helperService = _helperService;
            telegramService = _telegramService;
        }

        public string FileLocation = string.Empty;
        public bool LogToConsole = false;
        public bool LogToFile = false;

        public async Task ProcessRegionCurrencyRate(TimeBot.Model.Settings settings)
        {
            try
            {
                //int oldId = databaseService.GetMaxId("RegionRate");
                //avval bazani to'ldirib oladi
                if (TimeBot.Model.Settings.GetSettings().Proccess.Currency.RegionRate.Insert)
                {
                    await databaseService.InsertRegionRate(settings);
                }
                //int newId = databaseService.GetMaxId("RegionRate");
                // if ((oldId != newId))
                // {
                TimeBot.ViewModel.StateViewModel<List<RegionRate>> postResult = webClientService.GetRegionRate(settings);
                if (postResult.Code == 200 && postResult.Response.Any())
                {
                    if (TimeBot.Model.Settings.GetSettings().Proccess.Currency.RegionRate.Get)
                    {
                        await telegramService.SendRegionalRate(postResult.Response);
                    }
                }

                // }

            }
            catch (Exception e)
            {
                await helperService.Consoler(LogToConsole, LogToFile, $"Error posting ProcessRegionCurrencyRate: {e.Message}", FileLocation);
            }
        }

        public async Task ProcessMarketCurrencyRate(TimeBot.Model.Settings settings)
        {
            try
            {
                // int oldId = databaseService.GetMaxId("MarketRate");
                //avval bazani to'ldirib oladi
                if (TimeBot.Model.Settings.GetSettings().Proccess.Currency.MarketRate.Insert)
                {
                    await databaseService.InsertMarketRate(settings);
                }
                // int newId = databaseService.GetMaxId("MarketRate");
                // if ((oldId != newId))
                // {
                TimeBot.ViewModel.StateViewModel<List<MarketRate>> postResult = webClientService.GetMarketRate(settings);
                if (postResult.Code == 200 && postResult.Response.Any())
                {
                    if (TimeBot.Model.Settings.GetSettings().Proccess.Currency.MarketRate.Get)
                    {
                        await telegramService.SendMarketRate(postResult.Response);
                    }
                }

                // }

            }
            catch (Exception e)
            {
                await helperService.Consoler(LogToConsole, LogToFile, $"Error posting ProcessMarketCurrencyRate: {e.Message}", FileLocation);
            }
        }

        public async Task ProcessCbuCurrencyRate(TimeBot.Model.Settings settings)
        {
            try
            {
                //int oldId = databaseService.GetMaxId("CbuRate");
                //avval bazani tto'ldirib olamiz
                if (TimeBot.Model.Settings.GetSettings().Proccess.Currency.CbuRate.Insert)
                {
                    await databaseService.InsertCbuRate(settings);
                }
                //int newId = databaseService.GetMaxId("CbuRate");
                //  if ((oldId != newId))
                //  {
                TimeBot.ViewModel.StateViewModel<List<CbuRate>> postResult = webClientService.GetCbuRate(settings);
                if (postResult.Code == 200 && postResult.Response.Any())
                {
                    if (TimeBot.Model.Settings.GetSettings().Proccess.Currency.CbuRate.Get)
                    {
                        await telegramService.SendCbuRate(postResult.Response);
                    }
                }
                //  }

            }
            catch (Exception e)
            {
                await helperService.Consoler(LogToConsole, LogToFile, $"Error posting ProcessMarketCurrencyRate: {e.Message}", FileLocation);
            }
        }

        public async Task ProcessBankCurrencyRate(TimeBot.Model.Settings settings)
        {
            try
            {
                //int oldId = databaseService.GetMaxId("BankRate");
                //avval bazani to'ldirib oladi
                if (TimeBot.Model.Settings.GetSettings().Proccess.Currency.BankRate.Insert)
                {
                    await databaseService.InsertBankRate(settings);
                }
                //int newId = databaseService.GetMaxId("BankRate");
                //  if (oldId != newId)
                //  {
                TimeBot.ViewModel.StateViewModel<List<BankRate>> postResult = webClientService.GetBankRate(settings);
                if (postResult.Code == 200 && postResult.Response.Any())
                {
                    if (TimeBot.Model.Settings.GetSettings().Proccess.Currency.BankRate.Get)
                    {
                        await telegramService.SendBankRate(postResult.Response);
                    }
                }

                // }

            }
            catch (Exception e)
            {
                await helperService.Consoler(LogToConsole, LogToFile, $"Error posting ProcessMarketCurrencyRate: {e.Message}", FileLocation);
            }
        }

        public async Task ProcessBirjaCurrencyRate(TimeBot.Model.Settings settings)
        {
            try
            {
                //int oldId = databaseService.GetMaxId("BirjaRate");
                //avval bazani to'ldirib oladi
                if (TimeBot.Model.Settings.GetSettings().Proccess.Currency.BirjaRate.Insert)
                {
                    await databaseService.InsertBirjaRate(settings);
                }
                //  int newId = databaseService.GetMaxId("BirjaRate");
                //  if ((oldId != newId))
                //  {
                TimeBot.ViewModel.StateViewModel<List<BirjaRate>> postResult = webClientService.GetBirjaRate(settings);
                if (postResult.Code == 200 && postResult.Response.Any())
                {
                    if (TimeBot.Model.Settings.GetSettings().Proccess.Currency.BirjaRate.Get)
                    {
                        await telegramService.SendBirjaRate(postResult.Response);
                    }
                }

                // }

            }
            catch (Exception e)
            {
                await helperService.Consoler(LogToConsole, LogToFile, $"Error posting ProcessRegionCurrencyRate: {e.Message}", FileLocation);
            }
        }

        public async Task ProccessWeatherDaily(TimeBot.Model.Settings settings)
        {
            //int oldId = databaseService.GetMaxId("Weather");
            //avval bazani to'ldirib oladi
            //if (TimeBot.Model.Settings.GetSettings().Proccess.Currency.BirjaRate.Insert)
            //{
                await databaseService.InsertWeatherDaily(settings);
            //}
            //  int newId = databaseService.GetMaxId("Weather");
            //  if ((oldId != newId))
            //  {
            //TimeBot.ViewModel.StateViewModel<List<BirjaRate>> postResult = webClientService.GetBirjaRate(settings);
            //if (postResult.Code == 200 && postResult.Response.Any())
            //{
            //    if (TimeBot.Model.Settings.GetSettings().Proccess.Currency.BirjaRate.Get)
            //    {
                    //await telegramService.SendBirjaRate(postResult.Response);
            //    }
            //}

            // }
        }
    }
}
