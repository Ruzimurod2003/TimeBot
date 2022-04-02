﻿using DB_Services;
using HtmlToImage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;

namespace TimeBot.Methods
{
    public interface IMarketCurrency
    {
        Task GetSendMarketCurrencyAsync(ITelegramBotClient botClient, long chatId, CancellationToken cancellationToken);
    }
    public class MarketCurrency : IMarketCurrency
    {
        private readonly IConvertHtml convertHtml;
        public MarketCurrency(IConvertHtml _convertHtml)
        {
            convertHtml = _convertHtml;
        }

        public async Task GetSendMarketCurrencyAsync(ITelegramBotClient botClient, long chatId, CancellationToken cancellationToken)
        {
            try
            {
                byte[] bytes = await convertHtml.ConvertMarketRateForBot();
                Stream stream = new MemoryStream(bytes);
                InputOnlineFile inputOnlineFile = new InputOnlineFile(stream);
                Message message0 = await botClient.SendPhotoAsync(
                         chatId: chatId,
                         photo: inputOnlineFile,
                         caption: "<b>Бозор курси(норасмий)</b>" + "\n" +
                                  "\n" +
                                  "🗓Cана — " + DateTime.Now.ToString("dd.MM.yyyy") + "\n" +
                                  "⏰ Янгиланган вақт — " + DataBaseServices.GetLastTimeMarket() + "\n",
                         parseMode: ParseMode.Html,
                         cancellationToken: cancellationToken);
            }
            catch (Exception)
            {

            }
        }
    }
}
