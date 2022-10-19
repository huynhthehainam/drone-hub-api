


using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MiSmart.Infrastructure.Data;

namespace MiSmart.DAL.Models
{
    public class DeviceModel : EntityBase<Int32>
    {
        public DeviceModel() : base()
        {
        }

        public DeviceModel(ILazyLoader lazyLoader) : base(lazyLoader)
        {
        }
        public String Name { get; set; }
        public String FileUrl { get; set; }
        private ICollection<Device> devices;
        [JsonIgnore]
        public ICollection<Device> Devices
        {
            get => lazyLoader.Load(this, ref devices);
            set => devices = value;
        }

        private ICollection<DeviceModelParam> modelParams;
        public ICollection<DeviceModelParam> ModelParams
        {
            get => lazyLoader.Load(this, ref modelParams);
            set => modelParams = value;
        }
    }
}