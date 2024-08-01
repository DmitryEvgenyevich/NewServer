﻿using System.Runtime.CompilerServices;
using NewServer.Enums;

namespace NewServer.Logger
{
    public static class Logger
    {
        // The file name for the log.
        private static readonly string _fileName = "Log.txt";

        // The path to the directory where the log file will be stored.
        private static readonly string _logPath = @"C:\Server\log\"; // Ensure there is a backslash at the end of the path

        // Logs a message with a specified log level.
        public static void Log(string message, LogLevel level, [CallerMemberName] string functionName = "")
        {
            var formattedMessage = $"{DateTime.Now} [{level}] {functionName}: {message}";
            _writeToFile(formattedMessage);
        }

        // Writes a message to the log file. Creates the file if it does not exist.
        private static void _writeToFile(string message)
        {
            // Full path to the log file
            string fullPath = Path.Combine(_logPath, _fileName);

            try
            {
                // Check for the existence of the file and create it if it does not exist
                if (!File.Exists(fullPath))
                {
                    // Create the file and immediately close the handle to allow subsequent writes
                    using (var stream = File.Create(fullPath))
                    {
                        // Optionally write an initial message or setup headers
                    }
                }

                // Write the log message to the file
                using (StreamWriter writer = new StreamWriter(fullPath, true))
                {
                    writer.WriteLine(message);
                }
            }
            catch (Exception ex)
            {
                // Output the exception to the console if the log write fails
                Console.WriteLine($"Failed to write to log file: {ex.Message}");
            }
        }
    }
}