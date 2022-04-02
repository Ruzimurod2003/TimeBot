using DB_Services;
using HtmlToImage;
using MessageServices;
using Microsoft.Extensions.DependencyInjection;
using TimeBot.Methods;
using TimeBot.Services;

ServiceProvider serviceProvider= new ServiceCollection()
    .AddScoped<IRegionalCurrency, RegionalCurrency>()
    .AddScoped<IBankCurrency, BankCurrency>()
    .AddScoped<IBirjaCurrency, BirjaCurrency>()
    .AddScoped<ICbuCurrency, CbuCurrency>()
    .AddScoped<IMarketCurrency, MarketCurrency>()
    .AddScoped<IMainBotService, MainBotService>()
    .AddScoped<IMessageService, MessageService>()
    .AddScoped<IWorkUser,WorkUser>()
    .AddScoped<IDataBaseServices, DataBaseServices>()
    .AddScoped<IConvertHtml, ConvertHtml>()
    //.AddScoped<ICreateUser, CreateUser>()
    //.AddScoped<IGetUsers, GetUsers>()
    .BuildServiceProvider();

serviceProvider?.GetService<IMainBotService>()?.StartReceiving();