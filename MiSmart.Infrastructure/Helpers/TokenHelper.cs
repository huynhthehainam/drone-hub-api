using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace MiSmart.Infrastructure.Helpers
{
    public class TokenHelper
    {
        public static String GenerateToken(Int32 size = 32, Boolean allowSpecialCharacters = false)
        {
            var allChar = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            var resultToken = new String(
               Enumerable.Repeat(allChar, size)
               .Select(token => token[random.Next(token.Length)]).ToArray());

            String token = resultToken.ToString();
            return token;
        }
    }
}
