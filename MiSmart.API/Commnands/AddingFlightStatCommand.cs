



using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MiSmart.API.Commands
{
    public class LocationPoint
    {
        [Required]
        public Double? Longitude { get; set; }
        [Required]
        public Double? Latitude { get; set; }
    }
    public class AddingFlightStatCommand
    {
        public DateTime? FlightTime { get; set; }
        [Required(AllowEmptyStrings = false)]
        public String TaskLocation { get; set; }
        [Required]
        public Int32? Flights { get; set; }
        [Required]
        public String FieldName { get; set; }
        [Required]
        public Double? TaskArea { get; set; }
        [Required]
        public Double? FlightDuration { get; set; }
        [Required(AllowEmptyStrings = false)]
        public String PilotName { get; set; }
        public List<LocationPoint> FlywayPoints { get; set; } = new List<LocationPoint>();
        public List<Int32> SprayedIndexes { get; set; } = new List<Int32>();
        public String TeamName { get; set; }
    }
    public class AddingOfflineFlightStatCommand : AddingFlightStatCommand
    {
        public String DeviceAccessToken { get; set; }
    }

    public class AddingBulkOfflineFlightStatsCommand
    {
        public List<AddingOfflineFlightStatCommand> Data { get; set; }
    }
}