
using System;
using MiSmart.DAL.Models;
using MiSmart.Infrastructure.ViewModels;

namespace MiSmart.DAL.ViewModels
{
    public class SuperSmallDeviceViewModel : IViewModel<Device>
    {
        public Int32 ID { get; set; }
        public String Token { get; set; }


        public void LoadFrom(Device entity)
        {
            ID = entity.ID;
            Token = entity.Token;
        }
    }
}