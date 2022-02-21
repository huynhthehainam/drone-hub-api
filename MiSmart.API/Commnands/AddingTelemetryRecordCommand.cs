

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MiSmart.API.Commands
{
    public class AddingTelemetryRecordCommand
    {
        [Required]
        public Double? Latitude { get; set; }
        [Required]
        public Double? Longitude { get; set; }

        [Required]
        [Range(0, 360)]
        public Double? Direction { get; set; }
        public Object AdditionalInformation { get; set; } = new Object();
    }
    public class AddingBulkTelemetryRecordCommand
    {
        [Required]
        public List<AddingTelemetryRecordCommand> Data { get; set; }
    }
}