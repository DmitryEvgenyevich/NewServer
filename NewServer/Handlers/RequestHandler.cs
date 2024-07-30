using NewServer.Services;
using NewServer.Models;
using System.Reflection;

namespace NewServer.Handlers
{
    public class RequestHandler
    {
        public static async Task<Response> HandleRequest(Request request)
        {
            MethodInfo method = typeof(MessengerFunctionality).GetMethod(request.command!, BindingFlags.Public | BindingFlags.Static)!;

            if (method == null)
            {
                return new Response { errorMessage = $"Method '{request.command}' not found in MessengerFunctionality" };
            }

            var messengerFunctionalityInstance = new MessengerFunctionality();

            object[] parameters = { request };

            Task<Response> resultTask = (Task<Response>)method.Invoke(messengerFunctionalityInstance, parameters)!;

            return await resultTask.ConfigureAwait(false);
        }
    }
}
