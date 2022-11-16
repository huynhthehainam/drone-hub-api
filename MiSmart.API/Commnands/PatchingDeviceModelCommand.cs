

using System;
using System.Collections.Generic;
using MiSmart.DAL.Models;

namespace MiSmart.API.Commands
{
    public class PatchingDeviceModelCommand
    {
        public String? Name { get; set; }
        public DeviceModelType? Type { get; set; }
        public List<String>? SprayingModes { get; set; }
    }
    public class PatchingBatterModelCommand
    {
        public String? Name { get; set; }
        public String? ManufacturerName { get; set; }
    }
}