using NewServer.Models;
using WebSocketSharp.Server;

namespace NewServer.Server
{
    public static class WebSocketServerManager
    {
        private static WebSocketServer? _server;
        private static readonly Dictionary<int, Echo> _authenticatedClients = new Dictionary<int, Echo>();

        public static void Start(string url)
        {
            _server = new WebSocketServer(url);
            _server.AddWebSocketService<Echo>("/echo", () => new Echo());
            _server.Start();
            Console.WriteLine("WebSocket server started at " + _server.Address + ":" + _server.Port);
        }

        public static void AddAuthenticatedClients(int userId, Echo client)
        {
            lock (_authenticatedClients)
            {
                if (!_authenticatedClients.ContainsKey(userId))
                {
                    _authenticatedClients[userId] = client;
                }
            }
        }

        public static void RemoveClient(Echo client)
        {
            lock (_authenticatedClients)
            {
                var userId = _authenticatedClients.FirstOrDefault(x => x.Value == client).Key;
                if (userId != 0)
                {
                    _authenticatedClients.Remove(userId);
                }
            }
        }

        public static void BroadcastNotification(Notification notification)
        {
            lock (_authenticatedClients)
            {
                foreach (var authenticatedClient in _authenticatedClients.Values)
                {
                    authenticatedClient.SendNotification(notification);
                }
            }
        }

        public static void SendNotificationToUser(int userId, Notification notification)
        {
            lock (_authenticatedClients)
            {
                if (_authenticatedClients.TryGetValue(userId, out var client))
                {
                    client.SendNotification(notification);
                }
            }
        }
    }
}
