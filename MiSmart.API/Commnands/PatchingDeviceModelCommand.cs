

using System;
using System.Collections.Generic;

namespace MiSmart.API.Commands
{
    public class PatchingDeviceModelCommand
    {
        public String? Name { get; set; }
        public List<String>? SprayingModes { get; set; }
    }
    public class PatchingBatterModelCommand
    {
        public String? Name { get; set; }
        public String? ManufacturerName { get; set; }
    }
}