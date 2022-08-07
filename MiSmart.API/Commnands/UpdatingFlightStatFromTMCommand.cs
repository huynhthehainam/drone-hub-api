
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MiSmart.DAL.Models;

namespace MiSmart.API.Commands;

public class PlantFromTM
{
    public String ID { get; set; }
    public String Name { get; set; }
}
public class UpdatingSingleFlightStatFromTMCommand
{
    public String ID { get; set; }
    public List<LocationPoint> FieldPoints { get; set; }
    public PlantFromTM Plant { get; set; }
    public TMUser User { get; set; }
}
public class UpdatingFlightStatsFromTMCommand
{
    [Required]
    public String SecretKey { get; set; }
    public List<UpdatingSingleFlightStatFromTMCommand> Data { get; set; }
}