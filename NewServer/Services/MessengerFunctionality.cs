using NewServer.Models;
using NewServer.Enums;
using NewServer.Server;
using Newtonsoft.Json.Linq;
using NewServer.Database;

namespace NewServer.Services
{
    public class MessengerFunctionality
    {
        private static Response HandleUserNotFound()
        {
            Logger.Logger.Log("User not found.", LogLevel.ERROR);
            return new Response { errorMessage = "User not found. Login or password is wrong" };
        }

        private static Response HandleInvalidUserData(string message)
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
                    return HandleInvalidUserData("Invalid JSON data or empty email or password provided.");
                }

                var result = await DatabaseSuperbase.GetUserByEmailAndPassword(deserializedUser.email, deserializedUser.password);
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
                    return HandleInvalidUserData("Invalid user data provided.");
                }

                if (await DatabaseSuperbase.GetUserByUsername(deserializedUser.username) != null)
                {
                    return new Response { errorMessage = "A user with this username already exists." };
                }

                if (await DatabaseSuperbase.GetUserByEmail(deserializedUser.email!) != null)
                {
                    return new Response { errorMessage = "A user with this email already exists." };
                }

                int newCode = GlobalUtilities.GlobalUtilities.CreateRandomNumber(1000000, 9999999);
                _ = Authentication.Authentication.UpdateOrAddNewUser(deserializedUser, newCode.ToString());

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
                    return HandleInvalidUserData("Invalid user data or code.");
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
                    return HandleInvalidUserData("Invalid user data or code.");
                }

                var result = await DatabaseSuperbase.GetUserByEmail(deserializedUser.email);

                if (result == null || result.id <= 0)
                {
                    return HandleUserNotFound();
                }

                int newCode = GlobalUtilities.GlobalUtilities.CreateRandomNumber(1000000, 9999999);
                _ = Authentication.Authentication.UpdateOrAddNewUser(deserializedUser, newCode.ToString());

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
                    return HandleInvalidUserData("Invalid user data or code.");
                }

                var result = await DatabaseSuperbase.SetNewPassword(deserializedUser.email, deserializedUser.password!);

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

                var contactsData = await DatabaseSuperbase.GetChatsByUserId(deserializedUser!.id);
                if (contactsData == null)
                {
                    Logger.Logger.Log("Error from db.", LogLevel.ERROR);
                    return new Response { errorMessage = "Error from db." };
                }

                Logger.Logger.Log("Operation successfully completed.", LogLevel.INFO);
                JArray jsonArray = JArray.Parse(contactsData);
                return new Response { data = jsonArray };
            }
            catch (Exception ex)
            {
                return new Response { errorMessage = GlobalUtilities.GlobalUtilities.GetErrorMessage(ex) };
            }
        }
    }
}
