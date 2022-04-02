using DB_Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TimeBot.Methods;
using TimeBot.Model;

namespace TimeBot.Services
{
    public interface IMessageService
    {
        Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken);
        Task StartMethod(ITelegramBotClient botClient, Update update, long chatId, CancellationToken cancellationToken);
    }
    public class MessageService : IMessageService
    {
        private readonly IWorkUser workUser;
        private readonly IRegionalCurrency regionalCurrency;
        private readonly IBankCurrency bankCurrency;
        private readonly IMarketCurrency marketCurrency;
        private readonly IBirjaCurrency birjaCurrency;
        private readonly ICbuCurrency cbuCurrency;

        public MessageService(IWorkUser _workUser, IRegionalCurrency _regionalCurrency, IBankCurrency _bankCurrency, IMarketCurrency _marketCurrency, IBirjaCurrency _birjaCurrency, ICbuCurrency _cbuCurrency)
        {
            workUser = _workUser;
            regionalCurrency = _regionalCurrency;
            bankCurrency = _bankCurrency;
            marketCurrency = _marketCurrency;
            birjaCurrency = _birjaCurrency;
            cbuCurrency = _cbuCurrency;
        }
        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            string path = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            // Only process Message updates
            if (update.Type != UpdateType.Message)
            {
                return;
            }
            // Only process text messages
            if (update.Message!.Type != MessageType.Text)
            {
                return;
            }

            long chatId = update.Message.Chat.Id;
            string messageText = update.Message.Text;

            await workUser.InsertUser(botClient, update, cancellationToken);
            Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");
            if (messageText == "/start")
            {
                #region Welcome
                await botClient.SendTextMessageAsync(chatId, "" +
                    "Assalom alaykum, " + update.Message.Chat.FirstName + " " + update.Message.Chat.LastName + "\n" +
                    "\n" +
                    "Botga hush kelibsiz\n" +
                    "", replyMarkup: new ReplyKeyboardRemove()).ConfigureAwait(false);
                #endregion

                await StartMethod(botClient, update, chatId, cancellationToken).ConfigureAwait(false);
            }
            else if (messageText == "💰💰Dollar kursi💰💰")
            {
                await SendCurrencyButtons(botClient, chatId, cancellationToken);
            }
            else if (messageText == "🔙👈Orqaga")
            {
                await SendMainButtons(botClient, chatId, cancellationToken);
            }
            else if (messageText == "/users")
            {
                await SendUsers(botClient, chatId, update, messageText, cancellationToken);
            }
            #region Currency
            else if (messageText == "Hududlar kursi")
            {
                await SendRegionalCurrency(botClient, chatId, cancellationToken);
            }
            else if (messageText == "Banklar kursi")
            {
                await SendBankCurrency(botClient, chatId, cancellationToken);
            }
            else if (messageText == "Bozor kursi (Norasmiy)")
            {
                await SendMarketCurrency(botClient, chatId, cancellationToken);
            }
            else if (messageText == "Markaziy Bank")
            {
                await SendCbuCurrency(botClient, chatId, cancellationToken);
            }
            else if (messageText == "Valyuta birjasi")
            {
                await SendBirjaCurrency(botClient, chatId, cancellationToken);
            }
            #endregion
            else if (messageText == "Biz bilan aloqa")
            {
                await SendAloqa(botClient, chatId, update, messageText, cancellationToken);
            }
            else if (messageText == "Javob yozish")
            {
                await SendJavob(botClient, chatId, update, messageText, cancellationToken);
            }
            else
            {
                await SendText(botClient, chatId, update, messageText, cancellationToken);
            }
        }

        public async Task StartMethod(ITelegramBotClient botClient, Update update, long chatId, CancellationToken cancellationToken)
        {
            long userId = update.Message.Chat.Id;
            try
            {
                ChatMember userInfo = await botClient.GetChatMemberAsync(Settings.GetSettings().Telegram.Id.Channel, userId, cancellationToken).ConfigureAwait(false);
                if (userInfo.Status == ChatMemberStatus.Left)
                {
                    await SendJoinMarkUp(botClient, chatId, update, cancellationToken);
                }
                else
                {
                    await SendMainButtons(botClient, chatId, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public async Task SendCurrencyButtons(ITelegramBotClient botClient, long chatId, CancellationToken cancellationToken)
        {
            int tekshir = Settings.GetSettings().Telegram.Id.Admin.Where(i => i == chatId).Count();
            if (tekshir != 1)
            {
                ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
                                {
                new KeyboardButton[] { "Hududlar kursi", "Bozor kursi (Norasmiy)" },
                new KeyboardButton[] { "Markaziy Bank", "Banklar kursi" },
                new KeyboardButton[] { "Biz bilan aloqa","Valyuta birjasi" },
                new KeyboardButton[] { "🔙👈Orqaga" }
                })
                {
                    ResizeKeyboard = true
                };
                Message sentMessage = await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "Valyuta kurslari 👇",
                replyMarkup: replyKeyboardMarkup,
                cancellationToken: cancellationToken);
            }
            else
            {
                ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
                {
                new KeyboardButton[] { "Hududlar kursi", "Bozor kursi (Norasmiy)" },
                new KeyboardButton[] { "Markaziy Bank", "Banklar kursi" },
                new KeyboardButton[] { "Javob yozish","Valyuta birjasi" },
                new KeyboardButton[] { "🔙👈Orqaga" }
                })
                {
                    ResizeKeyboard = true
                };
                Message sentMessage = await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "Valyuta kurslari 👇",
                replyMarkup: replyKeyboardMarkup,
                cancellationToken: cancellationToken);

            }
        }

        public async Task SendMainButtons(ITelegramBotClient botClient, long chatId, CancellationToken cancellationToken)
        {//💰💰Dollar kursi💰💰
            int tekshir = Settings.GetSettings().Telegram.Id.Admin.Where(i => i == chatId).Count();
            if (tekshir != 1)
            {
                ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
                                {
                new KeyboardButton[] { "Ob-havo", "💰💰Dollar kursi💰💰" },
                new KeyboardButton[] { "Munajjimlar bashorati" },
                })
                {
                    ResizeKeyboard = true
                };
                Message sentMessage = await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "Hamma malumotlar 👇",
                replyMarkup: replyKeyboardMarkup,
                cancellationToken: cancellationToken);
            }
            else
            {
                ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
                {
                new KeyboardButton[] { "Ob-havo", "💰💰Dollar kursi💰💰" },
                new KeyboardButton[] { "Munajjimlar bashorati" },
                })
                {
                    ResizeKeyboard = true
                };
                Message sentMessage = await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "Hush kelibsiz admin",
                replyMarkup: replyKeyboardMarkup,
                cancellationToken: cancellationToken);

            }

        }

        public async Task SendJoinMarkUp(ITelegramBotClient botClient, long chatId, Update update, CancellationToken cancellationToken)
        {
            ChatInviteLink chatInviteLink = await botClient.CreateChatInviteLinkAsync(Settings.GetSettings().Telegram.Id.Channel);
            Message message = await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "Iltimos bu botni ishlatish uchun ushbu kanalga azo bo'lib qo'ying",
                parseMode: ParseMode.MarkdownV2,
                disableNotification: true,
                replyToMessageId: update.Message?.MessageId,
                replyMarkup: new InlineKeyboardMarkup(
                InlineKeyboardButton.WithUrl(
                "Qo'shilish", chatInviteLink.InviteLink)),
                cancellationToken: cancellationToken);

            await botClient.SendTextMessageAsync(chatId, "Siz kanalga obuna bo'lganingizdan keyin yana qaytadan /start ni bossangiz botdan to'liq foydalanasiz ", replyMarkup: new ReplyKeyboardRemove()).ConfigureAwait(false);
        }

        #region SendCurrency
        public async Task SendRegionalCurrency(ITelegramBotClient botClient, long chatId, CancellationToken cancellationToken)
        {
            await regionalCurrency.GetSendRegionalCurrencyAsync(botClient, chatId, cancellationToken);
        }

        public async Task SendBankCurrency(ITelegramBotClient botClient, long chatId, CancellationToken cancellationToken)
        {
            await bankCurrency.GetSendBankCurrencyAsync(botClient, chatId, cancellationToken);
        }
        public async Task SendBirjaCurrency(ITelegramBotClient botClient, long chatId, CancellationToken cancellationToken)
        {
            await birjaCurrency.GetSendBirjaCurrencyAsync(botClient, chatId, cancellationToken);
        }

        public async Task SendMarketCurrency(ITelegramBotClient botClient, long chatId, CancellationToken cancellationToken)
        {
            await marketCurrency.GetSendMarketCurrencyAsync(botClient, chatId, cancellationToken);
        }

        public async Task SendCbuCurrency(ITelegramBotClient botClient, long chatId, CancellationToken cancellationToken)
        {
            await cbuCurrency.GetSendCbuCurrencyAsync(botClient, chatId, cancellationToken);
        }
        #endregion

        public async Task SendText(ITelegramBotClient botClient, long chatId, Update update, string messageText, CancellationToken cancellationToken)
        {
            int tekshir = Settings.GetSettings().Telegram.Id.Admin.Where(i => i == chatId).Count();
            if (tekshir == 1)
            {
                try
                {
                    string messageText_new = messageText.Replace("Javob matni", string.Empty).Replace("\n", string.Empty).Replace(" ", string.Empty);
                    var startIndex = messageText_new.IndexOf(":", 0, 10);
                    var index = messageText_new.IndexOf(":", 5);
                    long userId = Convert.ToInt64(messageText_new.Substring(startIndex + 1, (index - 3)));
                    string text = messageText.Substring(messageText.IndexOf(":", 5) + 1);
                    await botClient.SendTextMessageAsync(chatId: userId, text: text, parseMode: ParseMode.Html, cancellationToken: cancellationToken);
                    await botClient.SendTextMessageAsync(chatId: chatId, text: "<code>Muvofaqiyatli jo'natdim</code>", parseMode: ParseMode.Html, cancellationToken: cancellationToken);

                }
                catch (Exception)
                {
                    await botClient.SendTextMessageAsync(chatId: chatId, text: "<b>Jo'natishda hatolik yuz berdi.</b>", parseMode: ParseMode.Html, cancellationToken: cancellationToken);
                }
            }
            if (tekshir != 1)
            {
                string Text = update?.Message?.Text;
                string Username = update?.Message?.Chat.Username;
                string FirstName = update?.Message?.Chat.FirstName;
                ChatPhoto Photo = update?.Message?.Chat.Photo;
                string LastName = update?.Message?.Chat.LastName;
                foreach (var item in Settings.GetSettings().Telegram.Id.Admin)
                {
                    await botClient.SendTextMessageAsync(chatId: item, text: "UserId: " + chatId + "\n" + "Username: @" + Username + "\n" + "Firstname: " + FirstName + "\n" + "Lastname: " + LastName + "\n" + "Text: " + Text, cancellationToken: cancellationToken);
                }
            }
        }

        public async Task SendAloqa(ITelegramBotClient botClient, long chatId, Update update, string messageText, CancellationToken cancellationToken)
        {
            int tekshir = Settings.GetSettings().Telegram.Id.Admin.Where(i => i == chatId).Count();

            if (tekshir == 1)
            {
                string messageText1 = "O'zingizga o'zingiz jo'natmang";

                await botClient.SendTextMessageAsync(chatId: chatId, text: messageText1, cancellationToken: cancellationToken);

            }
            else
            {
                string Text = "Ассалом алайкум " + update?.Message?.Chat.FirstName + " " + update?.Message?.Chat.LastName + "\n" +
                                                "Савол ва таклифлар бўлса,ёзиб қолдиринг.Тез орада жавоб берамиз!";
                await botClient.SendTextMessageAsync(chatId: chatId, text: Text, cancellationToken: cancellationToken);
            }
        }

        public async Task SendJavob(ITelegramBotClient botClient, long chatId, Update update, string messageText, CancellationToken cancellationToken)
        {
            int tekshir = Settings.GetSettings().Telegram.Id.Admin.Where(i => i == chatId).Count();

            if (tekshir == 1)
            {
                string messageText1 = "Id: <b>820042407</b>" + "\n" +
                                      "Javob matni: <i>Bu yerga javobingizni matnini kiritasiz!!!</i>";

                await botClient.SendTextMessageAsync(chatId: chatId, text: messageText1, parseMode: ParseMode.Html, cancellationToken: cancellationToken);

            }
            else
            {
                string Text = "Kechirasiz siz admin emassiz!!!";
                await botClient.SendTextMessageAsync(chatId: chatId, text: Text, cancellationToken: cancellationToken);
            }
        }

        public async Task SendUsers(ITelegramBotClient botClient, long chatId, Update update, string messageText, CancellationToken cancellationToken)
        {
            int tekshir = Settings.GetSettings().Telegram.Id.Admin.Where(i => i == chatId).Count();
            if (tekshir == 1)
            {
                string users = "";
                users += "Jami foydalanuvchilar " + @"<code>" + workUser.GetUsers().Count() + "</code> ta" + "\n";
                foreach (var itemuser in workUser.GetUsers())
                {
                    users += "" + itemuser.Id + ") " + @"<a href = 'tg://user?id=" + itemuser.UserId + "'> " + itemuser.FullName + "</a>" + "\n";
                }
                await botClient.SendTextMessageAsync(chatId: chatId, text: users, parseMode: ParseMode.Html, cancellationToken: cancellationToken);

            }
            else
            {
                string Text = "Kechirasiz siz admin emassiz!!!";
                await botClient.SendTextMessageAsync(chatId: chatId, text: Text, cancellationToken: cancellationToken);


            }
        }
    }
}
