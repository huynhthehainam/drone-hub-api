using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MiSmart.Infrastructure.Data;

namespace MiSmart.DAL.Models
{
    public class BatteryModel : EntityBase<Int32>
    {
        public BatteryModel()
        {
        }

        public BatteryModel(ILazyLoader lazyLoader) : base(lazyLoader)
        {
        }

        public String Name { get; set; }
        public String ManufacturerName { get; set; }
        public String FileUrl { get; set; }


        private ICollection<Battery> batteries;
        public ICollection<Battery> Batteries
        {
            get => lazyLoader.Load(this, ref batteries);
            set => batteries = value;
        }

    }
}