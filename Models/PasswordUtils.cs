using System.Security.Cryptography;
using System.Text;

namespace finder_work.Models
{
    public static class PasswordUtils
    {
        public static string Hash(string password)
        {
            using var rng = RandomNumberGenerator.Create();
            var salt = new byte[16];
            rng.GetBytes(salt);
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256);
            var hash = pbkdf2.GetBytes(32);
            var result = Convert.ToBase64String(salt) + ":" + Convert.ToBase64String(hash);
            return result;
        }

        public static bool Verify(string password, string stored)
        {
            var parts = stored.Split(':');
            if (parts.Length != 2) return false;
            var salt = Convert.FromBase64String(parts[0]);
            var hash = Convert.FromBase64String(parts[1]);
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256);
            var test = pbkdf2.GetBytes(32);
            return CryptographicOperations.FixedTimeEquals(test, hash);
        }
    }
}
