﻿using Newtonsoft.Json;
using WebSocketSharp;
using WebSocketSharp.Server;
using NewServer.Models;
using NewServer.Handlers;

namespace NewServer.Server
{
    public class Echo : WebSocketBehavior
    {
        protected override async void OnMessage(MessageEventArgs e)
        {
            Console.WriteLine("Received from client: " + e.Data);

            var request = JsonConvert.DeserializeObject<Request>(e.Data);
            request!.Socket = this;

            var response = await RequestHandler.HandleRequest(request!);
            response.requestId = request.requestId;

            Send(JsonConvert.SerializeObject(response));
        }

        protected override void OnClose(CloseEventArgs e)
        {
           WebSocketServerManager.RemoveClient(this);
            Console.WriteLine("WebSocket connection closed.");
        }

        public void SendNotification(Notification notification)
        {
            Send(JsonConvert.SerializeObject(notification));
        }
    }
}