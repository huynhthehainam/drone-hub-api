
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using MiSmart.DAL.Models;

namespace MiSmart.API.Commands;


public class UpdatingSingleFlightStatFromTMCommand
{
    public String? ID { get; set; }
    public List<LocationPoint> FieldPoints { get; set; } = new List<LocationPoint>();
    public TMPlant? Plant { get; set; }
    public TMUser? User { get; set; }
}
public class UpdatingFlightStatsFromTMCommand
{
    [Required]
    public String? SecretKey { get; set; }
    public List<UpdatingSingleFlightStatFromTMCommand>? Data { get; set; }
}
public class UpdatingFlightStatsFromTMUserCommand
{
    [Required]
    public String? SecretKey { get; set; }
    public Guid FlightStatID { get; set; }
    public JsonDocument? Medicines { get; set; }
}