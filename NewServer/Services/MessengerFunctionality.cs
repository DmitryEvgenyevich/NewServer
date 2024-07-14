using NewServer.Models;
using Newtonsoft.Json;
using NewServer.Enums;
using NewServer.Server;

namespace NewServer.Services
{
    public class MessengerFunctionality
    {
        public static async Task<Response> SignIn(Request request)
        {
            try
            {
                var deserializedUser = request.data!.ToObject<User>();
                if (deserializedUser == null || string.IsNullOrWhiteSpace(deserializedUser.email) || string.IsNullOrWhiteSpace(deserializedUser.password))
                {
                    Logger.Logger.Log("Invalid JSON data or empty email or password provided.", LogLevel.ERROR);
                    return new Response { ErrorMessage = "Invalid JSON data or empty email or password provided." };
                }

                var result = await Database.Database.GetUserByEmailAndPassword(deserializedUser.email, deserializedUser.password);

                if (result == null)
                {
                    Logger.Logger.Log("Error from db.", LogLevel.ERROR);
                    return new Response { ErrorMessage = "Error from db." };
                }

                if (result.id == 0 || result.id <= 0)
                {
                    Logger.Logger.Log("User not found.", LogLevel.ERROR);
                    return new Response { ErrorMessage = "User not found." };
                }

                WebSocketServerManager.AddAuthenticatedClients(result.id, request.Socket!);
                Logger.Logger.Log("Operation successfully completed.", LogLevel.INFO);
                return new Response { Data = JsonConvert.SerializeObject(result) };
            }
            catch (Exception ex)
            {
                Logger.Logger.Log($"Error from server: {ex.Message}", LogLevel.ERROR);
                return new Response { ErrorMessage = $"Error from server {ex.Message}" };
            }
        }
    }
}
