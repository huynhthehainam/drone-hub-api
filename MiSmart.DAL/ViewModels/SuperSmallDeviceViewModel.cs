
using System;
using MiSmart.DAL.Models;
using MiSmart.Infrastructure.ViewModels;

namespace MiSmart.DAL.ViewModels
{
    public class SuperSmallDeviceViewModel : IViewModel<Device>
    {
        public Int32 ID { get; set; }
        public String? Token { get; set; }


        public void LoadFrom(Device entity)
        {
            ID = entity.ID;
            Token = entity.Token;

        }
    }
    public class OnlyNameDeviceViewModel : IViewModel<Device>
    {
        public Int32 ID { get; set; }
        public String? Name { get; set; }

        public void LoadFrom(Device entity)
        {
            ID = entity.ID;
            Name = entity.Name;
        }
    }
    public class OnlyUUIDDeviceViewModel : IViewModel<Device>
    {
        public Int32 ID { get; set; }
        public Guid UUID { get; set; }

        public void LoadFrom(Device entity)
        {
            ID = entity.ID;
            UUID = entity.UUID;
        }
    }
}