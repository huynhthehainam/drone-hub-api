
using System;
using MiSmart.DAL.Models;
using MiSmart.Infrastructure.ViewModels;

namespace MiSmart.DAL.ViewModels
{
    public class MediumDeviceViewModel : IViewModel<Device>
    {
        public Int32 ID { get; set; }
        public CoordinateViewModel LastPoint { get; set; }

        public void LoadFrom(Device entity)
        {
            ID = entity.ID;
            LastPoint = new CoordinateViewModel(entity.LastPoint.Coordinate);
        }
    }
}