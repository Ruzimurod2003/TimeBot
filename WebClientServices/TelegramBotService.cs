using DB_Services;
using HtmlToImage;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using TelegramBotService.Services;
using TimeBot.Model;
using TimeBot.ViewModel;
using WebClientServices.Services;

namespace FundEstimateWebClient
{
    internal class TelegramBotService
    {
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly CancellationToken _cancellationToken;
        private readonly Task _task;

        public void Start()
        {
            _task.Start();
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel();
            _task.Wait();
        }

        public string FileLocation = string.Empty;
        public bool LogToConsole = false;
        public bool LogToFile = false;

        private Settings settings = new Settings();
        private readonly ServiceProvider serviceProvider = new ServiceCollection()
            .AddScoped<IWebClientService, WebClientService>()
            .AddSingleton<IHelperService, HelperService>()
            .AddScoped<IDataBaseServices, DataBaseServices>()
            .AddScoped<IMainMethods, MainMethods>()
            .AddScoped<IConvertHtml, ConvertHtml>()
            .AddScoped<ITelegramService, TelegramService>()
            .BuildServiceProvider();


        public TelegramBotService()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;

            _task = new Task(DoWork, _cancellationToken);
        }

        private void DoWork()
        {
            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                settings = Settings.GetSettings();
                FileLocation = settings.Path.Log;
                LogToFile = settings.Proccess.Other.ThreadSettings.LogToFile;
                LogToConsole = settings.Proccess.Other.ThreadSettings.LogToConsole;
                string logText = "";
                IHelperService helperService = serviceProvider.GetService<IHelperService>();
                IMainMethods mainMethods = serviceProvider.GetService<IMainMethods>();
                try
                {
                    logText = $"{DateTime.Now:u}\t\t:STARTING";

                    helperService?.Consoler(settings.Proccess.Other.ThreadSettings.LogToConsole, settings.Proccess.Other.ThreadSettings.LogToFile, logText, FileLocation).Wait(_cancellationToken);

                    if (true)
                    {
                        if (settings.Proccess.Other.EndPointSettings.ProcessRegionCurrencyRate)
                        {
                            mainMethods.ProcessRegionCurrencyRate(settings).Wait(_cancellationToken);
                            helperService.Consoler(settings.Proccess.Other.ThreadSettings.LogToConsole, settings.Proccess.Other.ThreadSettings.LogToFile, $"{DateTime.Now:u}\t\t:ProcessRegionCurrencyRate", FileLocation).Wait(_cancellationToken);
                        }
                        if (settings.Proccess.Other.EndPointSettings.ProcessMarketCurrencyRate)
                        {
                            mainMethods.ProcessMarketCurrencyRate(settings).Wait(_cancellationToken);
                            helperService.Consoler(settings.Proccess.Other.ThreadSettings.LogToConsole, settings.Proccess.Other.ThreadSettings.LogToFile, $"{DateTime.Now:u}\t\t:ProcessMarketCurrencyRate", FileLocation).Wait(_cancellationToken);
                        }
                        if (settings.Proccess.Other.EndPointSettings.ProcessBankCurrencyRate)
                        {
                            mainMethods.ProcessBankCurrencyRate(settings).Wait(_cancellationToken);
                            helperService.Consoler(settings.Proccess.Other.ThreadSettings.LogToConsole, settings.Proccess.Other.ThreadSettings.LogToFile, $"{DateTime.Now:u}\t\t:ProcessBankCurrencyRate", FileLocation).Wait(_cancellationToken);
                        }

                        if (settings.Proccess.Other.EndPointSettings.ProcessCbCurrencyRate)
                        {
                            mainMethods.ProcessCbuCurrencyRate(settings).Wait(_cancellationToken);
                            helperService.Consoler(settings.Proccess.Other.ThreadSettings.LogToConsole, settings.Proccess.Other.ThreadSettings.LogToFile, $"{DateTime.Now:u}\t\t:ProcessCbCurrencyRate", FileLocation).Wait(_cancellationToken);
                        }

                        if (settings.Proccess.Other.EndPointSettings.ProcessBirjaCurrencyRate)
                        {
                            mainMethods.ProcessBirjaCurrencyRate(settings).Wait(_cancellationToken);
                            helperService.Consoler(settings.Proccess.Other.ThreadSettings.LogToConsole, settings.Proccess.Other.ThreadSettings.LogToFile, $"{DateTime.Now:u}\t\t:ProcessBirjaCurrencyRate", FileLocation).Wait(_cancellationToken);
                        }
                        mainMethods.ProccessWeatherDaily(settings).Wait(_cancellationToken);
                    }

                    int sleep = settings.Proccess.Other.ThreadSettings.SleepMinutes * 30000/*60000 milliseconds*/;
                    Thread.Sleep(sleep);
                    logText = $"{DateTime.Now:u}\t\t:SLEPT FOR {sleep}";
                    helperService.Consoler(true, settings.Proccess.Other.ThreadSettings.LogToFile, logText, FileLocation).Wait(_cancellationToken);
                }
                catch (Exception e)
                {
                    helperService.Consoler(true, settings.Proccess.Other.ThreadSettings.LogToFile, e.ToString(), FileLocation).Wait(_cancellationToken);
                }

                if (_cancellationToken.IsCancellationRequested)
                {
                    helperService.Consoler(true, settings.Proccess.Other.ThreadSettings.LogToFile, _cancellationToken.ToString(), FileLocation).Wait(_cancellationToken);
                }
            }
        }
    }
}