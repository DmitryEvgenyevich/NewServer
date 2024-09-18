using Supabase;
using NewServer.Enums;
using NewServer.Models;
using Newtonsoft.Json;
using System.Reflection.Metadata.Ecma335;

namespace NewServer.Database
{
    public class DatabaseSupabase
    {
        private static Client? _database;
        private static SupabaseOptions options = new SupabaseOptions
        {
            AutoConnectRealtime = true
        };

        public static async Task DatabaseInit()
        {
            try 
            {
                var url = Environment.GetEnvironmentVariable("SUPABASE_URL");
                var key = Environment.GetEnvironmentVariable("SUPABASE_KEY");

                if (string.IsNullOrEmpty(url))
                {
                    Logger.Logger.Log("Could not find environment variable SUPABASE_URL.", LogLevel.ERROR);
                    throw new Exception("Could not find environment variable SUPABASE_URL.");
                }
                else if (string.IsNullOrEmpty(key))
                {
                    Logger.Logger.Log("Could not find environment variable SUPABASE_KEY.", LogLevel.ERROR);
                    throw new Exception("Could not find environment variable SUPABASE_KEY.");
                }

                _database = new Client(url!, key, options);
                await _database.InitializeAsync();
            }
            catch (Exception ex)
            {
                Logger.Logger.Log(ex.Message, LogLevel.ERROR);
            }
        }

        public static async Task<User?> GetUserByEmailAndPassword(string email, string password)
        {
            try
            {
                var result = await _database!.From<User>()
                    .Select(x => new object[] { x.id, x.username!, x.email!, x.description! })
                    .Where(x => x.email == email && x.password == password)
                    .Single();

                Logger.Logger.Log("Operation successfully completed.", LogLevel.INFO);

                return result;
            }
            catch (Exception ex)
            {
                Logger.Logger.Log(ex.Message, LogLevel.ERROR);
                return null;
            }
        }

        public static async Task<User?> GetUserByUsername(string username)
        {
            try
            {
                var result = await _database!.From<User>()
                    .Select(x => new object[] { x.id, x.username!, x.email! })
                    .Where(x => x.username == username)
                    .Single();

                Logger.Logger.Log("Operation successfully completed.", LogLevel.INFO);

                return result;
            }
            catch (Exception ex)
            {
                Logger.Logger.Log(ex.Message, LogLevel.ERROR);
                return null;
            }
        }

        public static async Task<User?> GetUserByEmail(string email)
        {
            try
            {
                var result = await _database!.From<User>()
                    .Select(x => new object[] { x.id, x.username!, x.email! })
                    .Where(x => x.email == email)
                    .Single();

                Logger.Logger.Log("Operation successfully completed.", LogLevel.INFO);

                return result;
            }
            catch (Exception ex)
            {
                Logger.Logger.Log(ex.Message, LogLevel.ERROR);
                return null;
            }
        }

        public static async Task<Message?> AddNewTextMessage(Message newMessage)
        {
            try
            {
                var result = await _database!.From<Message>()
                    .Select(x => new object[] { x.id })
                    .Insert(newMessage!);

                Logger.Logger.Log("Operation successfully completed.", LogLevel.INFO);

                return result.Model!;
            }
            catch (Exception ex)
            {
                Logger.Logger.Log(ex.Message, LogLevel.ERROR);
                return null;
            }
        }

        public static async Task<User?> InsertNewUser(User user)
        {
            try
            {
                var result = await _database!.From<User>()
                    .Select(x => new object[] { x.id, x.username!, x.email! })
                    .Insert(user!);

                Logger.Logger.Log("Operation successfully completed.", LogLevel.INFO);

                return result.Model!;
            }
            catch (Exception ex)
            {
                Logger.Logger.Log(ex.Message, LogLevel.ERROR);
                return null;
            }
        }

        public static async Task<User?> SetNewPassword(string email, string password)
        {
            try
            {
                var result = await _database!.From<User>()
                    .Select(x => new object[] { x.id })
                    .Where(x => x.email == email)
                    .Set(x => x.password!, password)
                    .Update();

                Logger.Logger.Log("Operation successfully completed.", LogLevel.INFO);

                return result.Model;
            }
            catch (Exception ex)
            {
                Logger.Logger.Log(ex.Message, LogLevel.ERROR);
                return null;
            }
        }

        public static async Task<string?> GetChatsByUserId(int Id)
        {
            try
            {
                var result = await _database!.Rpc("get_chat_data", new Dictionary<string, object> { { "id_of_user", Id } });

                Logger.Logger.Log("Operation successfully completed.", LogLevel.INFO);

                return result.Content!;
            }
            catch (Exception ex)
            {
                Logger.Logger.Log(ex.Message, LogLevel.ERROR);
                return null;
            }
        }

        public static async Task<string?> GetChatInfoById(int chat_id, int user_id)
        {
            try
            {
                var result = await _database!.Rpc("get_chat_info", new Dictionary<string, object> { { "param_chat_id", chat_id }, { "param_user_id", user_id } });

                Logger.Logger.Log("Operation successfully completed.", LogLevel.INFO);

                return result.Content!;
            }
            catch (Exception ex)
            {
                Logger.Logger.Log(ex.Message, LogLevel.ERROR);
                return null;
            }
        }

        public static async Task<string?> GetMessagesByUserChatId(int user_chat_id)
        {
            try
            {
                var result = await _database!.Rpc("get_messages_by_chat_user_id", new Dictionary<string, object> { { "chat_user_id", user_chat_id } });

                Logger.Logger.Log("Operation successfully completed.", LogLevel.INFO);

                return result.Content!;
            }
            catch (Exception ex)
            {
                Logger.Logger.Log(ex.Message, LogLevel.ERROR);
                return null;
            }
        }

        public static async Task setIdOfUnreadMessages(int userId, int userChatId, int? message_id = null)
        {
            try
            {
                await _database!.From<UserChats>()
                    .Set(x => x.first_unread_message_id!, message_id)
                    .Where(x => x.id == userChatId)
                    .Where(x => x.user_id == userId)
                    .Update();

                Logger.Logger.Log("Operation successfully completed.", LogLevel.INFO);
            }
            catch (Exception ex)
            {
                Logger.Logger.Log(ex.Message, LogLevel.ERROR);
            }
        }

        public static async Task<UserChats?> GetUserChatByUserChatId(int id)
        {
            try
            {
                var result = await _database!.From<UserChats>()
                    .Select(x => new object[] { x.chat_id, x.user_id })
                    .Where(x => x.id == id)
                    .Single();

                Logger.Logger.Log("Operation successfully completed.", LogLevel.INFO);

                return result;
            }
            catch (Exception ex)
            {
                Logger.Logger.Log(ex.Message, LogLevel.ERROR);
                return null;
            }
        }

        public static async Task<List<UserChats>?> GetUserChatsByChatIdExceptUserChatId(int id, int chat_id)
        {
            try
            {
                var result = await _database!.From<UserChats>()
                    .Select(x => new object[] { x.id, x.first_unread_message_id, x.user_id, x.chat_id })
                    .Where(x => x.id != id && x.chat_id == chat_id)
                    .Get();

                Logger.Logger.Log("Operation successfully completed.", LogLevel.INFO);

                return JsonConvert.DeserializeObject<List<UserChats>>(result.Content!)!;

            }
            catch (Exception ex)
            {
                Logger.Logger.Log(ex.Message, LogLevel.ERROR);
                return null;
            }
        }

        public static async Task UpdateFirstUnreadMessage(int id, int message_id)
        {
            try
            {
                var user_chat = await GetUserChatByUserChatId(id);

                var user_chats = await GetUserChatsByChatIdExceptUserChatId(id, user_chat.chat_id);

                foreach (var uc in user_chats)
                {
                    if (uc.first_unread_message_id == null)
                    {
                        await setIdOfUnreadMessages(uc.user_id, uc.id, message_id);
                    }
                }

                Logger.Logger.Log("Operation successfully completed.", LogLevel.INFO);
            }
            catch (Exception ex)
            {
                Logger.Logger.Log(ex.Message, LogLevel.ERROR);
            }
        }

        public async static Task<List<DataForNotificationNewMessage>?> getDataForNotificationNewMessage(int user_chat_id, int message_id)
        {
            try
            {
                var result = await _database!.Rpc("get_data_for_notification_new_messages", new Dictionary<string, object> { { "param_user_chat_id", user_chat_id }, { "param_message_id", message_id } });

                Logger.Logger.Log("Operation successfully completed.", LogLevel.INFO);


                return JsonConvert.DeserializeObject<List<DataForNotificationNewMessage>>(result.Content!)!;
            }
            catch (Exception ex)
            {
                Logger.Logger.Log(ex.Message, LogLevel.ERROR);
                return null;
            }
        }

        public static async Task<string> getChatTitle(TypesOfChat type, int chat_id, int user_chat_id)
        {
            switch (type)
            {
                case TypesOfChat.CHANNEL:
                    return "";

                case TypesOfChat.PRIVATE:
                    var chat = await GetUserChatByUserChatId(user_chat_id);
                    var result = await _database.From<User>()
                        .Select(x => new object[] { x.username })
                        .Where(x => x.id == chat.user_id)
                        .Single();

                    return result.username;

                case TypesOfChat.GROUP:
                    return "";
            }

            return "";
        }
    }
}
