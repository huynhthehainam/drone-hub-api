using System;
using MiSmart.DAL.Models;
using MiSmart.Infrastructure.ViewModels;

namespace MiSmart.DAL.ViewModels
{
    public class LogDetailViewModel : IViewModel<LogDetail>
    {
        public Guid LogFileID {get; set;}
        public Double FlightDuration { get; set; }
        public Double PercentBattery { get; set; }
        public Double PercentFuel {get; set;}
        public Double BatteryCellDeviation {get; set;}
        public Double FlySpeed {get;set;}
        public Double Height {get; set; }
        public Double VibeX {get; set; }
        public Double VibeY {get; set; }
        public Double VibeZ {get; set; }
        public Double Roll {get; set; }
        public Double Pitch{get; set; }
        public Double AccelX {get; set; }
        public Double AccelY {get; set; }
        public Double AccelZ {get; set; }
        public String Location {get; set; }
        public void LoadFrom(LogDetail entity)
        {
            LogFileID = entity.LogFileID;
            FlightDuration = entity.FlightDuration;
            PercentBattery = entity.PercentBattery;
            PercentFuel = entity.PercentFuel;
            BatteryCellDeviation = entity.BatteryCellDeviation;
            FlySpeed = entity.FlySpeed;
            Height = entity.Height;
            VibeX = entity.VibeX;
            VibeY = entity.VibeY;
            VibeZ = entity.VibeZ;
            Roll = entity.Roll;
            Pitch = entity.Pitch;
            AccelX = entity.AccelX;
            AccelY = entity.AccelY;
            AccelZ = entity.AccelZ;
            Location = entity.Location;
        }
    }
}