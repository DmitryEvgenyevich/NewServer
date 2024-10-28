using Supabase;
using NewServer.Enums;
using NewServer.Models;
using Newtonsoft.Json;

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

        public static async Task<string> getAvatarUrl(string avatar)
        {
            try
            {
                var storage = _database.Storage;
                var bucket = storage.From("avatars");

                return await bucket.CreateSignedUrl(avatar, 3600);
            }
            catch (Exception ex)
            {
                Logger.Logger.Log(ex.Message, LogLevel.ERROR);
                return null;
            }
        }

        public static async Task<User?> GetUserByEmailAndPassword(string email, string password)
        {
            try
            {
                var result = await _database!.From<User>()
                    .Select(x => new object[] { x.id, x.username!, x.email!, x.description!, x.avatar_id! })
                    .Where(x => x.email == email && x.password == password)
                    .Single();

                var avatar = await _database!.From<Files>()
                    .Select(x => new object[] { x.path })
                    .Where(x => x.id == result.avatar_id)
                    .Single();

                var publicUrl = await getAvatarUrl(avatar.path);

                Logger.Logger.Log("Operation successfully completed.", LogLevel.INFO);

                result.avatar_url = publicUrl;

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

        public static async Task<string?> getChatTitle(TypesOfChat type, int chat_id, int user_chat_id)
        {
            try
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

                        Logger.Logger.Log("Operation successfully completed.", LogLevel.INFO);

                        return result.username;

                    case TypesOfChat.GROUP:
                        return "";

                    default:
                        Logger.Logger.Log("Can not find this TypesOfChat", LogLevel.ERROR);
                        return null;

                }
            }
            catch (Exception ex)
            {
                Logger.Logger.Log(ex.Message, LogLevel.ERROR);
                return null;
            }
        }

        public static async Task DeleteChatByChatId(int chat_id)
        {
            try
            {
                await _database!.Rpc("delete_chat_by_id", new Dictionary<string, object> { { "param_chat_id", chat_id } });

                Logger.Logger.Log("Operation successfully completed.", LogLevel.INFO);

                return;
            }
            catch (Exception ex)
            {
                Logger.Logger.Log(ex.Message, LogLevel.ERROR);
                return;
            }
        }

        public static async Task<string?> FindChatsByTitle(string chat_title, int user_id)
        {
            try
            {
                var result = await _database!.Rpc("search_chat", new Dictionary<string, object> { { "param_search_term", chat_title}, { "param_user_id", user_id } });

                Logger.Logger.Log("Operation successfully completed.", LogLevel.INFO);

                return result.Content!;
            }
            catch (Exception ex)
            {
                Logger.Logger.Log(ex.Message, LogLevel.ERROR);
                return null;
            }
        }

        public static async Task<string?> UploadBase64AvatarToSupabaseStorage(string base64String, int user_id)
        {
            try
            {
                var base64Data = base64String.Contains(",") ? base64String.Split(',')[1] : base64String;
                byte[] imageBytes = Convert.FromBase64String(base64Data);
                await _database.Storage.From("avatars").Remove(new List<string> { $"{user_id}/1.png" });

                await _database.Storage.From("avatars").Upload(imageBytes, $"{user_id}/1.png");
                return await getAvatarUrl($"{user_id}/1.png");
            }
            catch (Exception ex)
            {
                Logger.Logger.Log(ex.Message, LogLevel.ERROR);
                return null;
            }
        }
    }
}
