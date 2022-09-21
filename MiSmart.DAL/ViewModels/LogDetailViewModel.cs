using System;
using System.Text.Json;
using MiSmart.DAL.Models;
using MiSmart.Infrastructure.ViewModels;

namespace MiSmart.DAL.ViewModels
{
    public class LogDetailViewModel : IViewModel<LogDetail>
    {
        public Guid LogFileID {get; set;}
        public Double FlightDuration { get; set; }
        public JsonDocument Vibe {get; set;}
        public Double PercentBattery { get; set; }
        public Double PercentFuel {get; set;}
        public JsonDocument Edge {get; set;}
        public Double BatteryCellDeviation {get; set;}
        public Double Area {get; set;}
        public Double FlySpeed {get;set;}
        public Double Heigh {get; set;}
        public JsonDocument Accel {get;set;}
        public void LoadFrom(LogDetail entity)
        {
            LogFileID = entity.LogFileID;
            FlightDuration = entity.FlightDuration;
            Vibe = entity.Vibe;
            PercentBattery = entity.PercentBattery;
            PercentFuel = entity.PercentFuel;
            Edge = entity.Edge;
            BatteryCellDeviation = entity.BatteryCellDeviation;
            Area = entity.Area;
            FlySpeed = entity.FlySpeed;
            Heigh = entity.Heigh;
            Accel = entity.Accel;
        }
    }
}