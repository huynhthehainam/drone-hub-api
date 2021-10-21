using System;
using System.Security.Cryptography;
using MiSmart.Infrastructure.Settings;
using Microsoft.Extensions.Options;
namespace MiSmart.Infrastructure.Services
{
    public class HashService
    {
        private HashSettings hashSettings;
        public HashService(IOptions<HashSettings> options1)
        {
            hashSettings = options1.Value;
        }
        public String Hash(String text, Int32 iterations)
        {
            // Create salt
            Byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new Byte[hashSettings.SaltSize]);
            // Create hash
            var pbkdf2 = new Rfc2898DeriveBytes(text, salt, iterations);
            var hash = pbkdf2.GetBytes(hashSettings.HashSize);
            // Combine salt and hash
            var hashBytes = new Byte[hashSettings.SaltSize + hashSettings.HashSize];
            Array.Copy(salt, 0, hashBytes, 0, hashSettings.SaltSize);
            Array.Copy(hash, 0, hashBytes, hashSettings.SaltSize, hashSettings.HashSize);
            // Convert to base64
            var base64Hash = Convert.ToBase64String(hashBytes);
            // Format hash with extra information
            return String.Format(hashSettings.PrivateKey + "{0}${1}", iterations, base64Hash);
        }
        public String Hash(String text)
        {
            return Hash(text, 10000);
        }
        public Boolean IsHashSupported(String hashString)
        {
            return hashString.Contains(hashSettings.PrivateKey);
        }
        public Boolean Verify(String password, String hashedPassword)
        {
            // Check hash
            if (!IsHashSupported(hashedPassword))
            {
                throw new NotSupportedException("The hashtype is not supported");
            }
            // Extract iteration and Base64 String
            var splittedHashString = hashedPassword.Replace(hashSettings.PrivateKey, "").Split('$');
            var iterations = Int32.Parse(splittedHashString[0]);
            var base64Hash = splittedHashString[1];
            // Get hash bytes
            var hashBytes = Convert.FromBase64String(base64Hash);
            // Get salt
            var salt = new Byte[hashSettings.SaltSize];
            Array.Copy(hashBytes, 0, salt, 0, hashSettings.SaltSize);
            // Create hash with given salt
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations);
            Byte[] hash = pbkdf2.GetBytes(hashSettings.HashSize);
            // Get result
            for (var i = 0; i < hashSettings.HashSize; i++)
            {
                if (hashBytes[i + hashSettings.SaltSize] != hash[i])
                {
                    return false;
                }
            }
            return true;
        }
    }
}
