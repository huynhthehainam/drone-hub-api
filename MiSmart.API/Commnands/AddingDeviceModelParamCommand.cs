
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

    public class AddingDeviceModelCentrifugalParamDetailCommand
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
    public class AddingDeviceModelCentrifugalParam4DetailCommand
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
        public String? Name { get; set; }
        public String? Description { get; set; }
        [Required]
        public Double? YMin { get; set; }
        [Required]
        public Double? YMax { get; set; }
        [Required]
        public Double? YCentrifugalMin { get; set; }
        [Required]
        public Double? YCentrifugalMax { get; set; }
        [Required]
        public Double? FuelLevelNumber { get; set; }
        [Required]
        public Double? FlowRateMinLimit { get; set; }
        [Required]
        public Double? FlowRateMiddleLimit { get; set; }
        [Required]
        public Double? FlowRateMaxLimit { get; set; }
        [Required]
        public Double? YCentrifugal4Min { get; set; }
        [Required]
        public Double? YCentrifugal4Max { get; set; }
        [Required]
        public List<AddingDeviceModelParamDetailCommand> Details { get; set; } = new List<AddingDeviceModelParamDetailCommand>();
        [Required]
        public List<AddingDeviceModelCentrifugalParamDetailCommand> CentrifugalDetails { get; set; } = new List<AddingDeviceModelCentrifugalParamDetailCommand>();
        [Required]
        public List<AddingDeviceModelCentrifugalParam4DetailCommand> Centrifugal4Details { get; set; } = new List<AddingDeviceModelCentrifugalParam4DetailCommand>();

    }

}