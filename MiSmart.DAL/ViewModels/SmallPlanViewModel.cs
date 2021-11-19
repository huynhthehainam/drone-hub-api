using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using MiSmart.DAL.Extensions;
using MiSmart.DAL.Models;
using MiSmart.Infrastructure.ViewModels;
using NetTopologySuite.Geometries;

namespace MiSmart.DAL.ViewModels
{
    public class SmallPlanViewModel : IViewModel<Plan>
    {
        public Int64 ID { get; set; }
        public String Name { get; set; }
        public String FileName { get; set; }
        public Double Longitude { get; set; }
        public Double Latitude { get; set; }
        [JsonIgnore]
        public Point Location { get; set; }
        public Double? Distance { get; set; }
        public String DistanceString
        {
            get
            {
                String result = null;
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

        public void CalculateDistance(Point point)
        {
            this.Distance = Location.ProjectTo(3857).Distance(point.ProjectTo(3857));
        }

        public void LoadFrom(Plan entity)
        {
            ID = entity.ID;
            Name = entity.Name;
            FileName = entity.FileName;
            Longitude = entity.Location.X;
            Latitude = entity.Location.Y;
            Location = entity.Location;
        }
    }
}