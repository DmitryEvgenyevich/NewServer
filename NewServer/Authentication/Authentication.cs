using NewServer.Models;
using System.Collections.Concurrent;
using NewServer.Enums;
using NewServer.Database;

namespace NewServer.Authentication
{
    class Authentication
    {
        // Constant representing the waiting time in minutes before an authentication code expires.
        private const short WAIT_IN_MINUTES = 5;

        private static ConcurrentDictionary<User, (string code, DateTime expirationTime, CancellationTokenSource cts)> _authenticationList
                        = new ConcurrentDictionary<User, (string, DateTime, CancellationTokenSource)>();

        public static void UpdateOrAddNewUser(User user, string code)
        {
            if (GlobalUtilities.GlobalUtilities.isValueNull(user))
            {
                Logger.Logger.Log("User is null.", LogLevel.ERROR);
                return;
            }

            DateTime expirationTime = DateTime.Now.AddMinutes(WAIT_IN_MINUTES);

            var cts = new CancellationTokenSource();

            _authenticationList.AddOrUpdate(user,
                (code, expirationTime, cts),
                (key, oldValue) =>
                {
                    oldValue.cts.Cancel();
                    oldValue.cts.Dispose();
                    return (code, expirationTime, cts);
                });

            Logger.Logger.Log("User authentication record added or updated.", LogLevel.INFO);

            _ = Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(TimeSpan.FromMinutes(WAIT_IN_MINUTES), cts.Token);

                    if (_authenticationList.TryGetValue(user, out var entry) && DateTime.Now >= entry.expirationTime)
                    {
                        _authenticationList.TryRemove(user, out _);
                        Logger.Logger.Log("Expired user authentication record removed.", LogLevel.INFO);
                    }
                }
                catch (TaskCanceledException) { }
            }, cts.Token);
        }

        // Method to check if the provided code matches the one stored for the user and delete the entry if it does.
        public async static Task<Response> IsCodeRight_DeleteFromList(User user, string code, bool insertUserToDatabase)
        {
            // Try to retrieve the user's authentication record.
            if (_authenticationList.TryGetValue(user, out var entry))
            {
                // Check if the provided code matches the stored code.
                if (code == entry.code)
                {
                    // If the code matches, remove the record and return an empty response.
                    if (_authenticationList.TryRemove(user, out var removedEntry))
                    {
                        removedEntry.cts.Dispose();
                        Logger.Logger.Log("Expired user authentication record removed.", LogLevel.INFO);
                    }

                    if (insertUserToDatabase)
                    {
                        var newUser = await DatabaseSupabase.InsertNewUser(user);
                        if (newUser == null)
                        {
                            Logger.Logger.Log("Error from db.", LogLevel.ERROR);
                            return new Response { errorMessage = "Error from db." };
                        }
                    }
                    Logger.Logger.Log("Operation successfully completed.", LogLevel.INFO);
                    return new Response();  // Empty response indicates successful authentication.
                }
                else
                {
                    // Return an error message if the code does not match.
                    return new Response { errorMessage = "Wrong code" };
                }
            }

            Logger.Logger.Log("Operation successfully completed.", LogLevel.INFO);
            // Return an error message if the record no longer exists or is expired.
            return new Response { errorMessage = "The code is no longer valid, please click on the button to resend code." };
        }
    }
}
