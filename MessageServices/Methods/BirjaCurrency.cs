using DB_Services;
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
    public interface IBirjaCurrency
    {
        Task GetSendBirjaCurrencyAsync(ITelegramBotClient botClient, long chatId, CancellationToken cancellationToken);
    }
    public class BirjaCurrency : IBirjaCurrency
    {
        private readonly IConvertHtml convertHtml;
        public BirjaCurrency(IConvertHtml _convertHtml)
        {
            convertHtml = _convertHtml;
        }

        public async Task GetSendBirjaCurrencyAsync(ITelegramBotClient botClient, long chatId, CancellationToken cancellationToken)
        {
            try
            {
                byte[] bytes = await convertHtml.ConvertBirjaRateForBot();
                Stream stream = new MemoryStream(bytes);
                //System.IO.File.WriteAllBytes("image.jpg", bytes);
                InputOnlineFile inputOnlineFile = new InputOnlineFile(stream);
                Message message0 = await botClient.SendPhotoAsync(
                         chatId: chatId,
                         photo: inputOnlineFile,
                         caption: "<b>Валюта  биржаси курси</b>" + "\n" +
                                  "🗓Cана — " + DateTime.Now.ToString("dd.MM.yyyy") + "\n" +
                                  "⏱ Янгиланган вақт - " + DataBaseServices.GetLastTimeBirja() + "\n" ,
                         parseMode: ParseMode.Html,
                         cancellationToken: cancellationToken);
            }
            catch (Exception)
            {

            }
        }
    }
}
