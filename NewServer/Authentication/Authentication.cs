using NewServer.Models;
using System.Collections.Concurrent;
using NewServer.Enums;
using NewServer.Database;

namespace NewServer.Authentication
{
    class Authentication
    {
        // Using ConcurrentDictionary to ensure thread safety.
        private static ConcurrentDictionary<User, (string, DateTime)> _authenticationList = new ConcurrentDictionary<User, (string, DateTime)>();

        // Constant representing the waiting time in minutes before an authentication code expires.
        private const short WAIT_IN_MINUTES = 5;

        // Method to add or update a user's authentication record using a thread-safe dictionary.
        public static async Task UpdateOrAddNewUser(User user, string code)
        {
            // Calculate the expiration time for the authentication code.
            DateTime expirationTime = DateTime.Now.AddMinutes(WAIT_IN_MINUTES);
            // Add or update the user's code and expiration time in the dictionary.
            _authenticationList.AddOrUpdate(user, (code, expirationTime), (key, oldValue) => (code, expirationTime));

            Logger.Logger.Log("Operation successfully completed.", LogLevel.INFO);
            // Start a timer to automatically remove the record after the expiration period has elapsed.
            await Task.Delay(TimeSpan.FromMinutes(WAIT_IN_MINUTES));
            // After the delay, check if the record still exists and if the current time is past the expiration time.
            if (_authenticationList.TryGetValue(user, out var entry) && DateTime.Now >= entry.Item2)
            {
                // If the record is expired, remove it from the dictionary.
                _authenticationList.TryRemove(user, out _);
            }
        }

        // Method to check if the provided code matches the one stored for the user and delete the entry if it does.
        public async static Task<Response> IsCodeRight_DeleteFromList(User user, string code, bool insertUserToDatabase)
        {
            // Try to retrieve the user's authentication record.
            if (_authenticationList.TryGetValue(user, out var entry))
            {
                // Check if the provided code matches the stored code.
                if (code == entry.Item1)
                {
                    // If the code matches, remove the record and return an empty response.
                    _authenticationList.TryRemove(user, out _);

                    if (insertUserToDatabase)
                    {
                        var newUser = await DatabaseSuperbase.InsertUserToTableUsers(user);
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
            return new Response { errorMessage = "The code is no longer valid" };
        }
    }
}
