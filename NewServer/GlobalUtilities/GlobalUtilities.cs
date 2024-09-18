using System.Text;
using System.Text.Json;
using System.Security.Cryptography;

namespace NewServer.GlobalUtilities
{
    static class GlobalUtilities
    {
        public static string HashString(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                string hash = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
                return hash;
            }
        }
        public static int CreateRandomNumber(int num1, int num2)
        {
            return new Random().Next(num1, num2);
        }

        public static bool isValueNull<T>(T value)
        {
            return value == null;
        }
    }
}
