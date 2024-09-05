using Supabase;
using NewServer.Enums;
using NewServer.Models;

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
            _database = new Client(Environment.GetEnvironmentVariable("SUPABASE_URL")!, Environment.GetEnvironmentVariable("SUPABASE_KEY"), options);
            await _database.InitializeAsync();
        }

        public static async Task<User?> GetUserByEmailAndPassword(string email, string password)
        {
            try
            {
                var value = await _database!.From<User>()
                    .Select(x => new object[] { x.id, x.username!, x.email!, x.description! })
                    .Where(x => x.email == email && x.password == password)
                    .Single();

                Logger.Logger.Log("Operation successfully completed.", LogLevel.INFO);

                return value;
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
                var value = await _database!.From<User>()
                    .Select(x => new object[] { x.id, x.username!, x.email! })
                    .Where(x => x.username == username)
                    .Single();

                Logger.Logger.Log("Operation successfully completed.", LogLevel.INFO);

                return value;
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
                var value = await _database!.From<User>()
                    .Select(x => new object[] { x.id, x.username!, x.email! })
                    .Where(x => x.email == email)
                    .Single();

                Logger.Logger.Log("Operation successfully completed.", LogLevel.INFO);

                return value;
            }
            catch (Exception ex)
            {
                Logger.Logger.Log(ex.Message, LogLevel.ERROR);
                return null;
            }
        }

        public static async Task<User?> InsertUserToTableUsers(User user)
        {
            try
            {
                var value = await _database!.From<User>()
                    .Select(x => new object[] { x.id, x.username!, x.email! })
                    .Insert(user!);

                Logger.Logger.Log("Operation successfully completed.", LogLevel.INFO);

                return value.Model!;
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
                var value = await _database!
                    .From<User>()
                    .Select(x => new object[] { x.id })
                    .Where(x => x.email == email)
                    .Set(x => x.password!, password)
                    .Update();

                Logger.Logger.Log("Operation successfully completed.", LogLevel.INFO);

                return value.Model;
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
                var result = (await _database!.Rpc("get_chat_data", new Dictionary<string, object> { { "id_of_user", Id } })).Content!;

                Logger.Logger.Log("Operation successfully completed.", LogLevel.INFO);

                return result;
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
                var result = (await _database!.Rpc("get_chat_info", new Dictionary<string, object> { { "param_chat_id", chat_id }, { "param_user_id", user_id } })).Content!;

                Logger.Logger.Log("Operation successfully completed.", LogLevel.INFO);

                return result;
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
                var result = (await _database!.Rpc("get_messages_by_chat_user_id", new Dictionary<string, object> { { "chat_user_id", user_chat_id } })).Content!;

                Logger.Logger.Log("Operation successfully completed.", LogLevel.INFO);

                return result;
            }
            catch (Exception ex)
            {
                Logger.Logger.Log(ex.Message, LogLevel.ERROR);
                return null;
            }
        }

        async static public Task ResetIdOfUnreadMessages(int userId, int userChatId)
        {
            try
            {
                await _database!
                .From<UserChats>()
                .Set(x => x.first_unread_message_id!, null)
                .Where(x => x.id == userChatId)
                .Where(x => x.user_id == userId)
                .Update();

                Logger.Logger.Log("Operation successfully completed.", LogLevel.INFO);
            }
            catch (Exception ex)
            {
                Logger.Logger.Log(ex.Message, LogLevel.ERROR);
                throw new Exception("ResetIdOfUnreadMessages", ex);
            }
        }
    }
}
