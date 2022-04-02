using DB_Services;
using HtmlToImage;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;

namespace TimeBot.Methods
{
    public interface IBankCurrency
    {
        Task GetSendBankCurrencyAsync(ITelegramBotClient botClient, long chatId, CancellationToken cancellationToken);
    }
    public class BankCurrency : IBankCurrency
    {
        private readonly IConvertHtml convertHtml;
        public BankCurrency(IConvertHtml _convertHtml)
        {
            convertHtml = _convertHtml;
        }

        public async Task GetSendBankCurrencyAsync(ITelegramBotClient botClient, long chatId, CancellationToken cancellationToken)
        {
            try
            {
                byte[] bytes = await convertHtml.ConvertBankRateForBot();
                Stream stream = new MemoryStream(bytes);
                //System.IO.File.WriteAllBytes("image.jpg", bytes);
                InputOnlineFile inputOnlineFile = new InputOnlineFile(stream);
                Message message0 = await botClient.SendPhotoAsync(
                         chatId: chatId,
                         photo: inputOnlineFile,
                         caption: "<b>Банклардаги бугунги доллар курси" + "\n" +
                                  "🗓Cана — " + DateTime.Now.ToString("dd.MM.yyyy") + "\n" +
                                  "⏱Янгиланган вақт - " + DataBaseServices.GetLastTimeBank() + "</b>\n",
                         parseMode: ParseMode.Html,
                         cancellationToken: cancellationToken);
            }
            catch (Exception)
            {

            }
        }
    }
}
