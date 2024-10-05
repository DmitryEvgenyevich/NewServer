using NewServer.Models;
using NewServer.Enums;
using NewServer.Server;
using Newtonsoft.Json.Linq;
using NewServer.Database;
using NewServer.Interfaces;

namespace NewServer.Services
{
    public class MessengerFunctionality
    {
        private static Response HandleUserNotFound()
        {
            Logger.Logger.Log("User not found.", LogLevel.ERROR);
            return new Response { errorMessage = "User not found. Login or password is wrong" };
        }

        private static Response HandleError(string message)
        {
            Logger.Logger.Log(message, LogLevel.INFO);
            return new Response { errorMessage = message };
        }

        private static Response LogAndReturnServerError(Exception ex, string customMessage = "Error from server")
        {
            Logger.Logger.Log($"{customMessage}: {ex.Message}", LogLevel.ERROR);
            return new Response { errorMessage = $"{customMessage} {ex.Message}" };
        }

        public static async Task<Response> SignIn(Request request, Echo client)
        {
            try
            {
                var deserializedUser = request.data?.ToObject<User>();
                if (deserializedUser == null || string.IsNullOrWhiteSpace(deserializedUser.email) || string.IsNullOrWhiteSpace(deserializedUser.password))
                {
                    return HandleError("Invalid JSON data or empty email or password provided.");
                }

                var result = await DatabaseSupabase.GetUserByEmailAndPassword(deserializedUser.email, deserializedUser.password);
                if (result == null)
                {
                    return HandleUserNotFound();
                }

                WebSocketServerManager.AddAuthenticatedClients(result.id, request.Socket!);
                Logger.Logger.Log("Operation successfully completed.", LogLevel.INFO);
                return new Response { data = JObject.FromObject(result) };
            }
            catch (Exception ex)
            {
                return LogAndReturnServerError(ex);
            }
        }

        public static async Task<Response> SignUp(Request request, Echo client)
        {
            try
            {
                var deserializedUser = request.data?.ToObject<User>();
                if (deserializedUser == null || string.IsNullOrEmpty(deserializedUser.username) || string.IsNullOrEmpty(deserializedUser.password))
                {
                    return HandleError("Invalid user data provided.");
                }

                if (await DatabaseSupabase.GetUserByUsername(deserializedUser.username) != null)
                {
                    return HandleError("A user with this username already exists.");
                }

                if (await DatabaseSupabase.GetUserByEmail(deserializedUser.email!) != null)
                {
                    return new Response { errorMessage = "A user with this email already exists." };
                }

                int newCode = GlobalUtilities.GlobalUtilities.CreateRandomNumber(1000000, 9999999);
                Authentication.Authentication.UpdateOrAddNewUser(deserializedUser, newCode.ToString());

                Logger.Logger.Log("User successfully signed up.", LogLevel.INFO);
                return new Response();
            }
            catch (Exception ex)
            {
                return LogAndReturnServerError(ex);
            }
        }

        public static async Task<Response> IsCodeRight(Request request, Echo client)
        {
            try
            {
                var deserializedEmailConfirmation = request.data?.ToObject<EmailConfirmation>();

                if (deserializedEmailConfirmation?.user == null || string.IsNullOrEmpty(deserializedEmailConfirmation.authenticationCode))
                {
                    return HandleError("Invalid user data or code.");
                }

                var result = await Authentication.Authentication.IsCodeRight_DeleteFromList(
                    deserializedEmailConfirmation.user,
                    deserializedEmailConfirmation.authenticationCode,
                    deserializedEmailConfirmation.insertUserToDatabase
                );

                if (!string.IsNullOrEmpty(result.errorMessage))
                {
                    Logger.Logger.Log(result.errorMessage, LogLevel.ERROR);
                    return result;
                }

                Logger.Logger.Log("Operation successfully completed.", LogLevel.INFO);
                return result;
            }
            catch (Exception ex)
            {
                return LogAndReturnServerError(ex, "Server error during code verification");
            }
        }

        public static async Task<Response> ForgotPassword(Request request, Echo client)
        {
            try
            {
                var deserializedUser = request.data?.ToObject<User>();

                if (deserializedUser == null || string.IsNullOrEmpty(deserializedUser.email))
                {
                    return HandleError("Invalid user data or code.");
                }

                var result = await DatabaseSupabase.GetUserByEmail(deserializedUser.email);

                if (result == null || result.id <= 0)
                {
                    return HandleUserNotFound();
                }

                int newCode = GlobalUtilities.GlobalUtilities.CreateRandomNumber(1000000, 9999999);
                Authentication.Authentication.UpdateOrAddNewUser(deserializedUser, newCode.ToString());

                Logger.Logger.Log("Operation successfully completed.", LogLevel.INFO);
                return new Response();
            }
            catch (Exception ex)
            {
                return LogAndReturnServerError(ex);
            }
        }

        public static async Task<Response> SetNewPassword(Request request, Echo client)
        {
            try
            {
                var deserializedUser = request.data?.ToObject<User>();

                if (deserializedUser == null || string.IsNullOrEmpty(deserializedUser.email))
                {
                    return HandleError("Invalid user data or code.");
                }

                var result = await DatabaseSupabase.SetNewPassword(deserializedUser.email, deserializedUser.password!);

                if (result == null || result.id <= 0)
                {
                    return HandleUserNotFound();
                }

                Logger.Logger.Log("Operation successfully completed.", LogLevel.INFO);
                return new Response();
            }
            catch (Exception ex)
            {
                return LogAndReturnServerError(ex);
            }
        }

        public static async Task<Response> GetMyChats(Request request, Echo? client)
        {
            try
            {
                var deserializedUser = request.data?.ToObject<User>();

                var contactsData = await DatabaseSupabase.GetChatsByUserId(deserializedUser!.id);
                if (contactsData == null)
                {
                    Logger.Logger.Log("Error from db.", LogLevel.ERROR);
                    return HandleError("Error from server");
                }

                Logger.Logger.Log("Operation successfully completed.", LogLevel.INFO);
                JArray jsonArray = JArray.Parse(contactsData!);
                return new Response { data = jsonArray };
            }
            catch (Exception ex)
            {
                return LogAndReturnServerError(ex);
            }
        }

        public static async Task<Response> GetChatInfo(Request request, Echo? client)
        {
            try
            {
                var deserializedUserChats = request.data?.ToObject<UserChats>();

                var chatInfo = await DatabaseSupabase.GetChatInfoById(deserializedUserChats!.chat_id, deserializedUserChats.user_id);
                if (chatInfo == null)
                {
                    Logger.Logger.Log("Error from db.", LogLevel.ERROR);
                    return HandleError("Error from server");
                }

                Logger.Logger.Log("Operation successfully completed.", LogLevel.INFO);
                JArray jsonArray = JArray.Parse(chatInfo!);
                return new Response { data = jsonArray[0] };
            }
            catch (Exception ex)
            {
                return LogAndReturnServerError(ex);
            }
        }

        public static async Task<Response> GetMessages(Request request, Echo? client)
        {
            try
            {
                var deserializedUserChats = request.data?.ToObject<UserChats>();

                var messages = await DatabaseSupabase.GetMessagesByUserChatId(deserializedUserChats!.id);
                if (messages == null)
                {
                    Logger.Logger.Log("Error from db.", LogLevel.ERROR);
                    return HandleError("Error from server");
                }

                Logger.Logger.Log("Operation successfully completed.", LogLevel.INFO);
                JArray jsonArray = JArray.Parse(messages);
                
                _ = ResetCountOfUnreadMessages(request, client);

                return new Response { data = jsonArray };
            }
            catch (Exception ex)
            {
                return LogAndReturnServerError(ex);
            }
        }

        public static async Task<Response> ResetCountOfUnreadMessages(Request request, Echo? client)
        {
            try
            {
                var deserializedUserChats = request.data?.ToObject<UserChats>();
                await DatabaseSupabase.setIdOfUnreadMessages(deserializedUserChats.user_id, deserializedUserChats.id);
                Logger.Logger.Log("Operation successfully completed.", LogLevel.INFO);

                return new Response { sendToClient = false };
            }
            catch (Exception ex)
            {
                Logger.Logger.Log($"Server error occurred.: {ex.Message}", LogLevel.ERROR);
                return new Response { sendToClient = false };
            }
        }

        public static async Task<Response> SendTextMessage(Request request, Echo? client)
        {
            try
            {
                var deserializedMessage = request.data?.ToObject<Message>();
                var newMessage = await DatabaseSupabase.AddNewTextMessage(deserializedMessage!);
                
                Logger.Logger.Log("Operation successfully completed.", LogLevel.INFO);

                _ = DatabaseSupabase.UpdateFirstUnreadMessage(deserializedMessage!.user_chat_id, newMessage.id);

                _ = sendMessageToUsers(deserializedMessage!.user_chat_id, newMessage.id);

                return new Response { data = new JObject
                    {
                        { "message_id", newMessage.id }
                    }
                };
            }
            catch (Exception ex)
            {
                return LogAndReturnServerError(ex);
            }
        }

        public static async Task<Response> LogOut(Request request, Echo? client)
        {
            WebSocketServerManager.RemoveClient(client!);

            return new Response { sendToClient = false };
        }

        private static async Task<Response> sendMessageToUsers(int user_chat_id, int message_id)
        {
            try
            {
                List<DataForNotificationNewMessage> userChatsList = await DatabaseSupabase.getDataForNotificationNewMessage(user_chat_id, message_id);

                foreach (var userChat in userChatsList) { 
                    WebSocketServerManager.SendNotificationToUser(
                        userChat.recipient_id,
                        new Notification 
                        { 
                            typeOfNotification = NotificationTypes.NewMessage, 
                            data = JObject.FromObject(userChat)
                    });
                }

                return new Response { sendToClient = false };
            }
            catch (Exception ex)
            {
                Logger.Logger.Log($"Server error occurred.: {ex.Message}", LogLevel.ERROR);
                return new Response { sendToClient = false };
            }
        }

        public static async Task<Response> DeleteChat(Request request, Echo? client)
        {
            try
            {
                var deserializedChat = request.data?.ToObject<Chat>();

                _ = DatabaseSupabase.DeleteChatByChatId(deserializedChat!.id);

                Logger.Logger.Log("Operation successfully completed.", LogLevel.INFO);

                return new Response { sendToClient = false };
            }
            catch (Exception ex)
            {
                Logger.Logger.Log(ex.Message, LogLevel.INFO);
                return new Response { sendToClient = false };
            }
        }

        public static async Task<Response> GetChatsByChatTitle(Request request, Echo? client)
        {
            try
            {
                var deserializedUserChatSearchCriteria = request.data?.ToObject<UserChatSearchCriteria>();

                var chats = await DatabaseSupabase.FindChatsByTitle(deserializedUserChatSearchCriteria.chat_title, deserializedUserChatSearchCriteria.user_id);

                Logger.Logger.Log("Operation successfully completed.", LogLevel.INFO);
                JArray jsonArray = JArray.Parse(chats!);
                return new Response { data = jsonArray };
            }
            catch (Exception ex)
            {
                return LogAndReturnServerError(ex);
            }
        }
    }
}
