



using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MiSmart.DAL.Models;

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
        [Required]
        public DateTime? FlightTime { get; set; }
        [Required(AllowEmptyStrings = false)]
        public String TaskLocation { get; set; }
        [Required]
        public Int32? Flights { get; set; }
        [Required]
        public String FieldName { get; set; }
        [Required]
        public Double? TaskArea { get; set; }
        public AreaUnit TaskAreaUnit { get; set; } = AreaUnit.Hectare;
        [Required]
        public Double? FlightDuration { get; set; }
        [Required(AllowEmptyStrings = false)]
        public String PilotName { get; set; }
        public List<LocationPoint> FlywayPoints { get; set; } = new List<LocationPoint>();
        public String TeamName { get; set; }
    }
}