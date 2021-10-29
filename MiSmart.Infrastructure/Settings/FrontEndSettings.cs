using System;

namespace MiSmart.Infrastructure.Settings
{
    public class FrontEndSettings
    {
        public String Domain { get; set; }
        public String ResetPasswordUrl { get; set; }
        public String ConfirmEmailUrl { get; set; }
        public String GetResetPasswordFullUrl()
        {
            return $"{Domain}{ResetPasswordUrl}";
        }
        public String GetResetPasswordFullUrlWithToken(String token)
        {
            return $"{GetResetPasswordFullUrl()}?token={token}";
        }
        public String GetConfirmEmailFullUrl()
        {
            return $"{Domain}{ConfirmEmailUrl}";
        }
        public String GetConfirmEmailFullUrlWithToken(String token)
        {
            return $"{GetConfirmEmailFullUrl()}?token={token}";

        }
    }
}