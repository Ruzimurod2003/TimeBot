using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using TimeBot.Model;
using Newtonsoft.Json;
using TimeBot.Services;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;

namespace MessageServices
{
    public interface IMainBotService
    {
        void StartReceiving();
    }
    public class MainBotService : IMainBotService
    {
        private readonly IMessageService messageServices;
        public MainBotService(IMessageService _messageServices)
        {
            messageServices = _messageServices;
        }
        private readonly ITelegramBotClient botClient = new TelegramBotClient(Settings.GetSettings().Telegram.BotApi);
        public void StartReceiving()
        {
            using CancellationTokenSource cts = new CancellationTokenSource();

            // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
            ReceiverOptions receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { } // receive all update types
            };

            botClient.StartReceiving(
                messageServices.HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                cancellationToken: cts.Token);

            Console.WriteLine($"Start listening for botClient.GetMeAsync().Id: @{botClient.GetMeAsync().Id} \n" +
                $" and botClient.GetMeAsync().Result: @{botClient.GetMeAsync().Result}");
            Console.ReadLine();

            cts.Cancel();
        }
        private Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            string ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }
    }
}

