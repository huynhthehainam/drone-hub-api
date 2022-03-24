

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MiSmart.API.Commands
{
    public class AddingBatteryLogCommand
    {
        public String ActualID { get; set; }
        public Double PercentRemaining { get; set; }
        public Double Temperature { get; set; }
        public String TemperatureUnit { get; set; }
        public Double CellMinimumVoltage { get; set; }
        public String CellMinimumVoltageUnit { get; set; }
        public Double CellMaximumVoltage { get; set; }
        public String CellMaximumVoltageUnit { get; set; }
        public Int32 CycleCount { get; set; }
        public Double Current { get; set; }
        public String CurrentUnit { get; set; }
    }

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
        public List<AddingBatteryLogCommand> BatteryLogs { get; set; } = new List<AddingBatteryLogCommand>();

    }
}