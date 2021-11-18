using System;
using System.Collections.Generic;
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
        public Point Location { get; set; }


        public void LoadFrom(Plan entity)
        {
            ID = entity.ID;
            Name = entity.Name;
            FileName = entity.FileName;
            Location = entity.Location;
        }
    }
}