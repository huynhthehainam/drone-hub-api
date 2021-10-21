using System;

namespace MiSmart.Infrastructure.Settings
{
    public class EmailSettings
    {
        public String SenderName { get; set; }
        public String FromEmail { get; set; }
        public String ToEmail { get; set; }
        public String CcEmail { get; set; }
        public String BccEmail { get; set; }
    }
}