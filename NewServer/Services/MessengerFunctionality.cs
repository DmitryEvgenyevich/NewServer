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
                    return new Response { errorMessage = "Invalid JSON data or empty email or password provided." };
                }

                var result = await Database.Database.GetUserByEmailAndPassword(deserializedUser.email, deserializedUser.password);

                if (result == null)
                {
                    Logger.Logger.Log("User not found.", LogLevel.ERROR);
                    return new Response { errorMessage = "User not found." };
                }

                WebSocketServerManager.AddAuthenticatedClients(result.id, request.Socket!);
                Logger.Logger.Log("Operation successfully completed.", LogLevel.INFO);
                return new Response { data = JsonConvert.SerializeObject(result) };
            }
            catch (Exception ex)
            {
                Logger.Logger.Log($"Error from server: {ex.Message}", LogLevel.ERROR);
                return new Response { errorMessage = $"Error from server {ex.Message}" };
            }
        }
    }
}
