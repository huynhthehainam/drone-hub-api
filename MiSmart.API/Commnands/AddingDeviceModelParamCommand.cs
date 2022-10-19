
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MiSmart.API.Commands
{
    public class AddingDeviceModelParamDetailCommand
    {
        [Required]
        public Double? XMin { get; set; }
        [Required]
        public Double? XMax { get; set; }
        [Required]
        public Double? A { get; set; }
        [Required]
        public Double? B { get; set; }
    }
    public class AddingDeviceModelParamCommand
    {
        [Required]
        public String Name { get; set; }
        public String Description { get; set; }
        [Required]
        public Double? YMin { get; set; }
        [Required]
        public Double? YMax { get; set; }
        [Required]
        public Double? FuelLevelNumber { get; set; }
        [Required]
        public List<AddingDeviceModelParamDetailCommand> Details { get; set; }

    }

}