using Microsoft.AspNetCore.Http;

namespace MiSmart.API.Commands
{
    public class UpdatingDeviceModelImageCommand
    {
        public IFormFile File { get; set; }
    }
     public class UpdatingBatteryModelImageCommand
    {
        public IFormFile File { get; set; }
    }
}