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
using DB_Services;

namespace TimeBot.Methods
{
    public interface ICbuCurrency
    {
        Task GetSendCbuCurrencyAsync(ITelegramBotClient botClient, long chatId, CancellationToken cancellationToken);
    }
    public class CbuCurrency : ICbuCurrency
    {
        private readonly IConvertHtml convertHtml;
        public CbuCurrency(IConvertHtml _convertHtml)
        {
            convertHtml = _convertHtml;
        }

        public async Task GetSendCbuCurrencyAsync(ITelegramBotClient botClient, long chatId, CancellationToken cancellationToken)
        {
            try
            {
                byte[] bytes = await convertHtml.ConvertCbuRateForBot();
                Stream stream = new MemoryStream(bytes);
                InputOnlineFile inputOnlineFile = new InputOnlineFile(stream);
                Message message0 = await botClient.SendPhotoAsync(
                         chatId: chatId,
                         photo: inputOnlineFile,
                         caption: "<b>Марказий банк расмий курси</b>"+"\n" +
                                  "🗓Cана — " + DateTime.Now.ToString("dd.MM.yyyy") + "\n" +
                                  "⏰ Янгиланган вақт — " + DataBaseServices.GetLastTimeCbu() + "\n",
                         parseMode: ParseMode.Html,
                         cancellationToken: cancellationToken);
                  }
            catch (Exception)
            {

            }
        }
    }
}
