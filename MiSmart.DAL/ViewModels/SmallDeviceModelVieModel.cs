
using MiSmart.DAL.Models;
using MiSmart.Infrastructure.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MiSmart.DAL.ViewModels
{
    public class CustomerDeviceSpecificQuantity
    {
        public Int32? CustomerID { get; set; }
        public String? CustomerName { get; set; }
        public Int32 Count { get; set; }
        public List<OnlyNameDeviceViewModel>? Devices { get; set; }
        public String? ListNames { get; set; }
    }
    public class SmallDeviceModelViewModel : IViewModel<DeviceModel>
    {
        public DeviceModel? Entity;
        public Int32 ID { get; set; }
        public String? Name { get; set; }
        public Int32 DevicesCount { get; set; }
        public String? FileUrl { get; set; }
        public DeviceModelType Type { get; set; }
        public List<CustomerDeviceSpecificQuantity>? SpecificQuantities { get; set; }
        public List<String>? SprayingModes { get; set; }
        public void LoadFrom(DeviceModel entity)
        {
            ID = entity.ID;
            Name = entity.Name;
            DevicesCount = entity.Devices?.Count ?? 0;
            Entity = entity;
            FileUrl = entity.FileUrl;
            Type = entity.Type;
            SprayingModes = entity.SprayingModes;
            var group = entity.Devices?.GroupBy(ww => ww.Customer).ToList();
            SpecificQuantities = group?.Select(ww => new CustomerDeviceSpecificQuantity
            {
                Count = ww.Count(),
                CustomerID = ww.FirstOrDefault()?.CustomerID,
                CustomerName = ww.FirstOrDefault()?.Customer?.Name,
                Devices = ww.Select(d => ViewModelHelpers.ConvertToViewModel<Device, OnlyNameDeviceViewModel>(d)).ToList(),
                ListNames = String.Join(",", ww.Select(d => d.Name).ToList()),
            }).ToList();
        }
    }

    public class GCSDeviceModelViewModel : IViewModel<DeviceModel>
    {
        public Int32 ID { get; set; }
        public String? Name { get; set; }
        public DeviceModelType Type { get; set; }
        public DeviceModelParamViewModel? DeviceModelParam { get; set; }
        public List<String>? SprayingModes { get; set; }
        public void LoadFrom(DeviceModel entity)
        {
            ID = entity.ID;
            Name = entity.Name;
            Type = entity.Type;
            SprayingModes = entity.SprayingModes;
            var activeParam = entity.ModelParams?.FirstOrDefault(ww => ww.IsActive);
            if (activeParam != null)
            {
                DeviceModelParam = ViewModelHelpers.ConvertToViewModel<DeviceModelParam, DeviceModelParamViewModel>(activeParam);
            }
        }
    }
}