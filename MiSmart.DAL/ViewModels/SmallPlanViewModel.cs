using System;
using System.Text.Json.Serialization;
using MiSmart.DAL.Models;
using MiSmart.Infrastructure.ViewModels;
using NetTopologySuite.Geometries;

namespace MiSmart.DAL.ViewModels
{
    public class SmallPlanViewModel : IViewModel<Plan>
    {
        public Int64 ID { get; set; }
        public String? Name { get; set; }
        public String? FileName { get; set; }
        public Double Longitude { get; set; }
        public Double Latitude { get; set; }
        [JsonIgnore]
        public Point? Point { get; set; }
        public CoordinateViewModel? Location { get; set; }
        public Double? Distance { get; set; }
        public DateTime CreatedTime { get; set; }
        public Double Area { get; set; }
        public String? DistanceString
        {
            get
            {
                String? result = null;
                if (Distance.HasValue)
                {
                    if (Distance > 1000)
                    {
                        Double d = (Distance.GetValueOrDefault() / 1000);
                        var d1 = d.ToString("0.##");
                        result = $"{d1} Km";
                    }
                    else
                    {
                        var d = Distance.GetValueOrDefault();
                        var d1 = d.ToString("0.##");
                        result = $"{d1} m";
                    }
                }
                return result;
            }
        }
        public OnlyUUIDDeviceViewModel? Device {get; set;}



        public void LoadFrom(Plan entity)
        {
            ID = entity.ID;
            Name = entity.Name;
            FileName = entity.FileName;
            Longitude = entity.Location is null ? 0 : entity.Location.X;
            Latitude = entity.Location is null ? 0 : entity.Location.Y;
            Point = entity.Location;
            CreatedTime = entity.CreatedTime;
            Area = entity.Area;
            Location = entity.Location is null ? null : new CoordinateViewModel(entity.Location.Coordinate);
            Device = entity.Device is null ? null : ViewModelHelpers.ConvertToViewModel<Device, OnlyUUIDDeviceViewModel>(entity.Device);
        }
    }
}