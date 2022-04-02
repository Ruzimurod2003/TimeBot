using Dapper;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using TimeBot.Model;
using TimeBot.ViewModel;

namespace DB_Services
{
    public interface IWorkUser
    {
        Task InsertUser(ITelegramBotClient botClient, Telegram.Bot.Types.Update update, CancellationToken cancellationToken);
        Task<User> GetUser(long userId);
        IEnumerable<User> GetUsers();
    }
    public class WorkUser : IWorkUser
    {
        private static IDbConnection _db = new SqliteConnection();
        public WorkUser()
        {
            _db = new SqliteConnection("Data Source=" + Settings.GetSettings().Path.Database+Settings.GetSettings().Proccess.Other.DatabaseName);
        }
        public Task<User> GetUser(long userId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<User> GetUsers()
        {
            var users = _db.Query<User>("select * from User");
            return users;
        }

        public async Task InsertUser(ITelegramBotClient botClient, Telegram.Bot.Types.Update update, CancellationToken cancellationToken)
        {
            SetupUser();
            try
            {
                long Id = update.Message.Chat.Id;
                string UserName = update.Message.Chat.Username;
                string FirstName = update.Message.Chat.FirstName;
                string LastName = update.Message.Chat.LastName;
                Telegram.Bot.Types.ChatMember chatMember = await botClient.GetChatMemberAsync(Settings.GetSettings().Telegram.Id.Channel, Id, cancellationToken);
                //if (chatMember.Status == ChatMemberStatus.Creator)
                //{
                string DbId = _db.QuerySingleOrDefault<string>("select Id from User where UserId=@UserId", new { UserId = Id.ToString() });
                if (DbId is null)
                {
                    string processQuery = "INSERT INTO User VALUES (((IFNULL((SELECT MAX(Id) FROM User),0) + 1)),@FullName, @UserId,@Created,@UserName,@FirstName,@LastName,@RegionName,@Language)";
                    await _db.ExecuteAsync(processQuery, new { FullName = FirstName + " " + LastName, UserId = Id, Created = DateTime.Now.ToString("dd.MM.yyyy HH:mm"), UserName = UserName, FirstName = FirstName, LastName = LastName, RegionName = RegionName.Андижон_вилояти.ToString(), Language = Language.Uzbek });

                }
                //}
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
        public void SetupUser()
        {
            var table = _db.Query<string>("SELECT name FROM sqlite_master WHERE type='table' AND name = 'User';");
            var tableName = table.FirstOrDefault();
            if (!string.IsNullOrEmpty(tableName) && tableName == "User")
                return;

            _db.Execute("Create Table User (" +
                "Id int ," +
                "FullName VARCHAR(50) ," +
                "UserId VARCHAR(50) ," +
                "Created VARCHAR(50) ," +
                "UserName VARCHAR(50) ," +
                "FirstName VARCHAR(50) ," +
                "LasName VARCHAR(50) ," +
                "RegionName VARCHAR(50) ," +
                "Language VARCHAR(50) );");
        }
    }
}
