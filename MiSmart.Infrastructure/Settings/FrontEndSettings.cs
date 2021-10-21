using System;

namespace MiSmart.Infrastructure.Settings
{
    public class FrontEndSettings
    {
        public String Domain { get; set; }
        public String ResetPasswordUrl { get; set; }
        public String ConfirmEmailUrl { get; set; }
    }
}