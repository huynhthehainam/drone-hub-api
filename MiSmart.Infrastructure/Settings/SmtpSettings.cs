using System;

namespace MiSmart.Infrastructure.Settings
{
    public class SmtpSettings
    {
        public String Server { get; set; }
        public Int32 Port { get; set; }
        public String UserName { get; set; }
        public String Password { get; set; }
        public Boolean EnableSsl { get; set; }
    }
}