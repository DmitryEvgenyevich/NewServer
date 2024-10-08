﻿using NewServer.Database;
using NewServer.Models;
using NewServer.Server;
using NewServer.Services;
using Newtonsoft.Json.Linq;

public class Program
{
    private static ManualResetEvent _quitEvent = new ManualResetEvent(false);
    public static async Task Main(string[] args)
    {
        await DatabaseSupabase.DatabaseInit();
        //var request = new Request
        //{
        //    data = new JObject
        //    {
        //        { "chat_id", 2 },
        //        { "user_id", 1 }
        //    },
        //};

        //await MessengerFunctionality.GetChatInfo(request, null);
        WebSocketServerManager.Start("ws://localhost:8000");

        Console.CancelKeyPress += (sender, eArgs) =>
        {
            eArgs.Cancel = true;
            _quitEvent.Set();
        };

        Console.WriteLine("Press Ctrl+C to exit...");
        _quitEvent.WaitOne();
    }
}
