using NewServer.Database;
using NewServer.Server;

public class Program
{
    private static ManualResetEvent _quitEvent = new ManualResetEvent(false);
    public static async Task Main(string[] args)
    {
        await Database.DatabaseInit();
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
