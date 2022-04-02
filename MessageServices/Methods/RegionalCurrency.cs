using DB_Services;
using HtmlToImage;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;

namespace TimeBot.Methods
{
    public interface IRegionalCurrency
    {
        Task GetSendRegionalCurrencyAsync(ITelegramBotClient botClient, long chatId, CancellationToken cancellationToken);
    }
    public class RegionalCurrency : IRegionalCurrency
    {
        private readonly IConvertHtml convertHtml;
        public RegionalCurrency(IConvertHtml _convertHtml)
        {
            convertHtml = _convertHtml;
        }

        public async Task GetSendRegionalCurrencyAsync(ITelegramBotClient botClient, long chatId, CancellationToken cancellationToken)
        {
            try
            {
                byte[] bytes = await convertHtml.ConvertRegionalRateForBot();
                Stream stream = new MemoryStream(bytes);
                //System.IO.File.WriteAllBytes("image.jpg", bytes);
                InputOnlineFile inputOnlineFile = new InputOnlineFile(stream);
                Message message0 = await botClient.SendPhotoAsync(
                         chatId: chatId,
                         photo: inputOnlineFile,
                         caption: "<b>Худудларидаги доллар курси</b>" + "\n" +
                                  "🗓Cана — " + DateTime.Now.ToString("dd.MM.yyyy") + "\n" +
                                  "⏱ Янгиланган вақт - " + DataBaseServices.GetLastTimeRegion() + "\n",
                         parseMode: ParseMode.Html,
                         cancellationToken: cancellationToken);
            }
            catch (Exception)
            {

            }
        }
    }
}
