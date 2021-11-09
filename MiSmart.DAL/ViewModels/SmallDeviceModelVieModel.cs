
using MiSmart.DAL.Models;
using MiSmart.Infrastructure.ViewModels;
using System;
namespace MiSmart.DAL.ViewModels
{
    public class SmallDeviceModelVieModel : IViewModel<DeviceModel>
    {
        public Int32 ID { get; set; }
        public String Name { get; set; }
        public void LoadFrom(DeviceModel entity)
        {
            ID = entity.ID;
            Name = entity.Name;
        }
    }
}