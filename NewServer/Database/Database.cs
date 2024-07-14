using Supabase;
using NewServer.Models;

namespace NewServer.Database
{
    public class Database
    {
        private static Client? _database;
        private static SupabaseOptions options = new SupabaseOptions
        {
            AutoConnectRealtime = true
        };

        public static async Task DatabaseInit()
        {
            _database = new Client(Environment.GetEnvironmentVariable("SUPABASE_URL")!, Environment.GetEnvironmentVariable("SUPABASE_KEY"), options);
            await _database.InitializeAsync();
        }

        public static async Task<User?> GetUserByEmailAndPassword(string email, string password)
        {
            var response = await _database!.From<User>()
                .Select(x => new object[] { x.id, x.username!, x.email! })
                .Where(x => x.email == email && x.password == password)
                .Single();

            return response;
        }
    }
}
