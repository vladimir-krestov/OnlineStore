using System.Security.Cryptography;
using System.Text;

namespace OnlineStore.Core.Services
{
    internal class PasswordManager
    {
        public static string GenerateSalt(int size = 16)
        {
            byte[] salt = new byte[16];
            using RandomNumberGenerator cryptoProvider = RandomNumberGenerator.Create();
            cryptoProvider.GetBytes(salt);

            return Convert.ToBase64String(salt);
        }

        public static string HashPassword(string password, string salt)
        {
            string saltedPassword = password + salt;
            byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(saltedPassword));

            return Convert.ToBase64String(hash);
        }
    }
}
