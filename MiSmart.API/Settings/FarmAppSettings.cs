using System;

namespace MiSmart.API.Settings
{
    public class FarmAppSettings
    {
        public String? SecretKey { get; set; }
        public String? FarmDomain { get; set; }
        public Double IOU { get; set; }
    }
}