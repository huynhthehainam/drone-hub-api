
using MiSmart.DAL.Models;
using MiSmart.Infrastructure.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MiSmart.DAL.ViewModels
{
    public class CustomerDeviceSpecificQuantity
    {
        public Int32 CustomerID { get; set; }
        public String CustomerName { get; set; }
        public Int32 Count { get; set; }
        public List<OnlyNameDeviceViewModel> Devices { get; set; }
        public String ListNames { get; set; }
    }
    public class SmallDeviceModelVieModel : IViewModel<DeviceModel>
    {
        public DeviceModel Entity;
        public Int32 ID { get; set; }
        public String Name { get; set; }
        public Int32 DevicesCount { get; set; }
        public String FileUrl { get; set; }
        public List<CustomerDeviceSpecificQuantity> SpecificQuantities { get; set; }
        public void LoadFrom(DeviceModel entity)
        {
            ID = entity.ID;
            Name = entity.Name;
            DevicesCount = entity.Devices.Count;
            Entity = entity;
            FileUrl = entity.FileUrl;
            var group = entity.Devices.GroupBy(ww => ww.Customer).ToList();
            SpecificQuantities = group.Select(ww => new CustomerDeviceSpecificQuantity
            {
                Count = ww.Count(),
                CustomerID = ww.FirstOrDefault().CustomerID,
                CustomerName = ww.FirstOrDefault().Customer.Name,
                Devices = ww.Select(d => ViewModelHelpers.ConvertToViewModel<Device, OnlyNameDeviceViewModel>(d)).ToList(),
                ListNames = String.Join(",", ww.Select(d => d.Name).ToList()),
            }).ToList();
        }
    }
}