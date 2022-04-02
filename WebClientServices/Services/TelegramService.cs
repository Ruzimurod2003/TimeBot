using DB_Services;
using HtmlToImage;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using TimeBot.ViewModel;

namespace WebClientServices.Services
{
    public interface ITelegramService
    {
        Task SendRegionalRate(List<RegionRate> updatedList);
        Task SendMarketRate(List<MarketRate> updatedList);
        Task SendCbuRate(List<CbuRate> updatedList);
        Task SendBankRate(List<BankRate> updatedList);
        Task SendBirjaRate(List<BirjaRate> updatedList);
    }
    public class TelegramService : ITelegramService
    {
        public static string bot_api = TimeBot.Model.Settings.GetSettings().Telegram.BotApi;
        private readonly ITelegramBotClient botClient = new TelegramBotClient(bot_api);
        private readonly IConvertHtml convertHtml;
        private readonly IDataBaseServices dataBaseServices;

        public TelegramService(IConvertHtml _convertHtml, IDataBaseServices _dataBaseServices)
        {
            convertHtml = _convertHtml;
            dataBaseServices = _dataBaseServices;
        }

        //dinamik id larni o'zgartiramiz
        private long channelId = TimeBot.Model.Settings.GetSettings().Telegram.Id.Channel;
        private long groupId = TimeBot.Model.Settings.GetSettings().Telegram.Id.Group;

        public async Task SendRegionalRate(List<RegionRate> updatedList)
        {
            byte[] bytes = await convertHtml.ConvertRegionRateAsync(updatedList);
            using (Stream stream = new MemoryStream(bytes))
            {
                InputOnlineFile inputOnlineFile1 = new InputOnlineFile(stream);
                await botClient.SendPhotoAsync(
                    chatId: channelId,
                    parseMode: ParseMode.Html,
                    photo: inputOnlineFile1,
                    caption: "<b>Худудларидаги доллар курси</b>" + "\n" +
                                  "🗓Cана — " + DateTime.Now.ToString("dd.MM.yyyy") + "\n" +
                                  "⏱ Янгиланган вақт - " + DataBaseServices.GetLastTimeRegion() + "\n" +
                                  "\n"
                    ); //send to channel
            }
            using (Stream stream = new MemoryStream(bytes))
            {
                InputOnlineFile inputOnlineFile = new InputOnlineFile(stream);
                await botClient.SendPhotoAsync(
                    chatId: groupId,
                    parseMode: ParseMode.Html,
                    photo: inputOnlineFile,
                    caption: "<b>Худудларидаги доллар курси</b>" + "\n" +
                                  "🗓Cана — " + DateTime.Now.ToString("dd.MM.yyyy") + "\n" +
                                  "⏱ Янгиланган вақт - " + DataBaseServices.GetLastTimeRegion() + "\n" +
                                  "\n"
                    ); //send to group
            }
            foreach (var item in dataBaseServices.GetUsersId())
            {
                using (Stream stream = new MemoryStream(bytes))
                {
                    InputOnlineFile inputOnlineFile = new InputOnlineFile(stream);
                    await botClient.SendPhotoAsync(
                        chatId: item,
                        parseMode: ParseMode.Html,
                        photo: inputOnlineFile,
                        caption: "<b>Худудларидаги доллар курси</b>" + "\n" +
                                      "🗓Cана — " + DateTime.Now.ToString("dd.MM.yyyy") + "\n" +
                                      "⏱ Янгиланган вақт - " + DataBaseServices.GetLastTimeRegion() + "\n" +
                                      "\n"
                        ); //send to user
                }
            }
        }

        public async Task SendMarketRate(List<MarketRate> updatedList)
        {
            byte[] bytes = await convertHtml.ConvertMarketRateAsync(updatedList);
            using (Stream stream = new MemoryStream(bytes))
            {
                InputOnlineFile inputOnlineFile1 = new InputOnlineFile(stream);
                await botClient.SendPhotoAsync(
                    chatId: channelId,
                    photo: inputOnlineFile1,
                    caption: "<b>Бозор курси(норасмий)</b>" + "\n" +
                                  "\n" +
                                  "🗓Cана — " + DateTime.Now.ToString("dd.MM.yyyy") + "\n" +
                                  "⏰ Янгиланган вақт — " + DataBaseServices.GetLastTimeMarket() + "\n" +
                                  "\n",
                         parseMode: ParseMode.Html
                    ); //send to channel
            }
            using (Stream stream = new MemoryStream(bytes))
            {
                InputOnlineFile inputOnlineFile = new InputOnlineFile(stream);
                await botClient.SendPhotoAsync(chatId: groupId,
                    photo: inputOnlineFile,
                    caption: "<b>Бозор курси(норасмий)</b>" + "\n" +
                                  "\n" +
                                  "🗓Cана — " + DateTime.Now.ToString("dd.MM.yyyy") + "\n" +
                                  "⏰ Янгиланган вақт — " + DataBaseServices.GetLastTimeMarket() + "\n" +
                                  "\n",
                         parseMode: ParseMode.Html
                    ); //send to group
            }
            foreach (var item in dataBaseServices.GetUsersId())
            {
                using (Stream stream = new MemoryStream(bytes))
                {
                    InputOnlineFile inputOnlineFile = new InputOnlineFile(stream);
                    await botClient.SendPhotoAsync(
                        chatId: item,
                        photo: inputOnlineFile,
                        caption: "<b>Бозор курси(норасмий)</b>" + "\n" +
                                      "\n" +
                                      "🗓Cана — " + DateTime.Now.ToString("dd.MM.yyyy") + "\n" +
                                      "⏰ Янгиланган вақт — " + DataBaseServices.GetLastTimeMarket() + "\n" +
                                      "\n",
                             parseMode: ParseMode.Html
                        ); //send to user
                }
            }
        }

        public async Task SendCbuRate(List<CbuRate> updatedList)
        {
            byte[] bytes = await convertHtml.ConvertCbuRateAsync(updatedList);
            using (Stream stream = new MemoryStream(bytes))
            {
                InputOnlineFile inputOnlineFile1 = new InputOnlineFile(stream);
                await botClient.SendPhotoAsync(
                    chatId: channelId,
                    photo: inputOnlineFile1,
                    caption: "<b>Марказий банк расмий курси" + "</b>\n" +
                                  "🗓Cана — " + DateTime.Now.ToString("dd.MM.yyyy") + "\n" +
                                  "⏰ Янгиланган вақт — " + DataBaseServices.GetLastTimeCbu() + "\n" +
                                  "\n",
                         parseMode: ParseMode.Html
                    ); //send to channel
            }
            using (Stream stream = new MemoryStream(bytes))
            {
                InputOnlineFile inputOnlineFile = new InputOnlineFile(stream);
                await botClient.SendPhotoAsync(
                    chatId: groupId,
                    photo: inputOnlineFile,
                    caption: "<b>Марказий банк расмий курси" + "</b>\n" +
                                  "🗓Cана — " + DateTime.Now.ToString("dd.MM.yyyy") + "\n" +
                                  "⏰ Янгиланган вақт — " + DataBaseServices.GetLastTimeCbu() + "\n" +
                                  "\n",
                         parseMode: ParseMode.Html
                    ); //send to group
            }
            foreach (var item in dataBaseServices.GetUsersId())
            {
                using (Stream stream = new MemoryStream(bytes))
                {
                    InputOnlineFile inputOnlineFile = new InputOnlineFile(stream);
                    await botClient.SendPhotoAsync(
                        chatId: item,
                        photo: inputOnlineFile,
                        caption: "<b>Марказий банк расмий курси" + "</b>\n" +
                                      "🗓Cана — " + DateTime.Now.ToString("dd.MM.yyyy") + "\n" +
                                      "⏰ Янгиланган вақт — " + DataBaseServices.GetLastTimeCbu() + "\n" +
                                      "\n",
                             parseMode: ParseMode.Html
                        ); //send to user
                }
            }
        }

        public async Task SendBankRate(List<BankRate> updatedList)
        {
            byte[] bytes = await convertHtml.ConvertBankRateAsync(updatedList);
            using (Stream stream = new MemoryStream(bytes))
            {
                InputOnlineFile inputOnlineFile1 = new InputOnlineFile(stream);
                await botClient.SendPhotoAsync(
                        chatId: channelId,
                        photo: inputOnlineFile1,
                        caption: "<b>Банклардаги бугунги доллар курси" + "\n" +
                                  "🗓<b>Cана — " + DateTime.Now.ToString("dd.MM.yyyy") + "</b>\n" +
                                  "⏱ <b>Янгиланган вақт - " + DataBaseServices.GetLastTimeBank() + "</b>\n" +
                                  "\n",
                         parseMode: ParseMode.Html
                    ); //send to channel
            }
            using (Stream stream = new MemoryStream(bytes))
            {
                InputOnlineFile inputOnlineFile = new InputOnlineFile(stream);
                await botClient.SendPhotoAsync(
                        chatId: groupId,
                        photo: inputOnlineFile,
                        caption: "<b>Банклардаги бугунги доллар курси" + "\n" +
                                  "🗓<b>Cана — " + DateTime.Now.ToString("dd.MM.yyyy") + "</b>\n" +
                                  "⏱ <b>Янгиланган вақт - " + DataBaseServices.GetLastTimeBank() + "</b>\n" +
                                  "\n",
                         parseMode: ParseMode.Html
                    ); //send to group
            }
            foreach (var item in dataBaseServices.GetUsersId())
            {
                using (Stream stream = new MemoryStream(bytes))
                {
                    InputOnlineFile inputOnlineFile = new InputOnlineFile(stream);
                    await botClient.SendPhotoAsync(
                            chatId: item,
                            photo: inputOnlineFile,
                            caption: "<b>Ўзбекистон банклардаги бугунги доллар курси" + "\n" +
                                      "🗓<b>Cана — " + DateTime.Now.ToString("dd.MM.yyyy") + "</b>\n" +
                                      "⏱ <b>Янгиланган вақт - " + DataBaseServices.GetLastTimeBank() + "</b>\n" +
                                      "\n",
                             parseMode: ParseMode.Html
                        ); //send to user
                }
            }
        }

        public async Task SendBirjaRate(List<BirjaRate> updatedList)
        {
            byte[] bytes = await convertHtml.ConvertBirjaRateAsync(updatedList);
            using (Stream stream = new MemoryStream(bytes))
            {
                InputOnlineFile inputOnlineFile1 = new InputOnlineFile(stream);
                await botClient.SendPhotoAsync(
                        chatId: channelId,
                        photo: inputOnlineFile1,
                        caption: "<b>Валюта  биржаси курси</b>" + "\n" +
                                  "🗓Cана — " + DateTime.Now.ToString("dd.MM.yyyy") + "\n" +
                                  "⏱ Янгиланган вақт - " + DataBaseServices.GetLastTimeBirja() + "\n" +
                                  "\n",
                         parseMode: ParseMode.Html
                    ); //send to channel
            }
            using (Stream stream = new MemoryStream(bytes))
            {
                InputOnlineFile inputOnlineFile = new InputOnlineFile(stream);
                await botClient.SendPhotoAsync(
                        chatId: groupId,
                        photo: inputOnlineFile,
                        caption: "<b>Валюта  биржаси курси</b>" + "\n" +
                                  "🗓Cана — " + DateTime.Now.ToString("dd.MM.yyyy") + "\n" +
                                  "⏱ Янгиланган вақт - " + DataBaseServices.GetLastTimeBirja() + "\n" +
                                  "\n",
                         parseMode: ParseMode.Html
                    ); //send to group
            }
            foreach (var item in dataBaseServices.GetUsersId())
            {
                using (Stream stream = new MemoryStream(bytes))
                {
                    InputOnlineFile inputOnlineFile = new InputOnlineFile(stream);
                    await botClient.SendPhotoAsync(
                            chatId: item,
                            photo: inputOnlineFile,
                            caption: "<b>Валюта  биржаси курси</b>" + "\n" +
                                      "🗓Cана — " + DateTime.Now.ToString("dd.MM.yyyy") + "\n" +
                                      "⏱ Янгиланган вақт - " + DataBaseServices.GetLastTimeBirja() + "\n" +
                                      "\n",
                             parseMode: ParseMode.Html
                        ); //send to user
                }
            }
        }
    }
}
