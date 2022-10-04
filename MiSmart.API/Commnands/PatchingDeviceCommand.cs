

using System;

namespace MiSmart.API.Commands
{
    public class PatchingDeviceCommand
    {
        public String Name { get; set; }
        public Int32? DeviceModelID { get; set; }
        public Int32? ExecutionCompanyID { get; set; }
    }
}