using System;

namespace MiSmart.Infrastructure.Settings
{
    public class HashSettings
    {
        public Int32 SaltSize { get; set; }
        public Int32 HashSize { get; set; }
        public String PrivateKey { get; set; }
    }
}