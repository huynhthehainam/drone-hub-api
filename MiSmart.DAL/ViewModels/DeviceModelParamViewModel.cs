using System;
using System.Linq;
using MiSmart.DAL.Models;
using MiSmart.Infrastructure.ViewModels;

namespace MiSmart.DAL.ViewModels
{
    public class DeviceModelParamViewModel : IViewModel<DeviceModelParam>
    {
        public Int32 ID { get; set; }
        public Boolean IsActive { get; set; }
        public DateTime CreationTime { get; set; }
        public String? Description { get; set; }
        public String? Name { get; set; }
        public Double YMin { get; set; }
        public Double YMax { get; set; }
        public Double YCentrifugalMin { get; set; }
        public Double YCentrifugalMax { get; set; }
        public Double YCentrifugal4Min { get; set; }
        public Double YCentrifugal4Max { get; set; }
        public Double FuelLevelNumber { get; set; }
        public Double FlowRateMinLimit { get; set; }
        public Double FlowRateMiddleLimit { get; set; }
        public Double FlowRateMaxLimit { get; set; }
        public DeviceModelParamDetailViewModel[]? Details { get; set; }
        public DeviceModelParamCentrifugalDetailViewModel[]? CentrifugalDetails { get; set; }
        public DeviceModelParamCentrifugal4DetailViewModel[]? Centrifugal4Details { get; set; }

        public void LoadFrom(DeviceModelParam entity)
        {
            ID = entity.ID;
            IsActive = entity.IsActive;
            CreationTime = entity.CreationTime;
            Description = entity.Description;

            YMin = entity.YMin;
            Name = entity.Name;
            YMax = entity.YMax;
            YCentrifugalMin = entity.YCentrifugalMin;
            YCentrifugalMax = entity.YCentrifugalMax;
            YCentrifugal4Min = entity.YCentrifugal4Min;
            YCentrifugal4Max = entity.YCentrifugal4Max;
            FuelLevelNumber = entity.FuelLevelNumber;
            FlowRateMinLimit = entity.FlowRateMinLimit;
            FlowRateMiddleLimit = entity.FlowRateMiddleLimit;
            FlowRateMaxLimit = entity.FlowRateMaxLimit;
            Details = entity.Details is null ? null : entity.Details.OrderBy(ww => ww.XMin).Select(ww => ViewModelHelpers.ConvertToViewModel<DeviceModelParamDetail, DeviceModelParamDetailViewModel>(ww)).ToArray();
            CentrifugalDetails = entity.CentrifugalDetails is null ? null : entity.CentrifugalDetails.OrderBy(ww => ww.XMin).Select(ww => ViewModelHelpers.ConvertToViewModel<DeviceModelParamCentrifugalDetail, DeviceModelParamCentrifugalDetailViewModel>(ww)).ToArray();
            Centrifugal4Details = entity.Centrifugal4Details is null ? null : entity.Centrifugal4Details.OrderBy(ww => ww.XMin).Select(ww => ViewModelHelpers.ConvertToViewModel<DeviceModelParamCentrifugal4Detail, DeviceModelParamCentrifugal4DetailViewModel>(ww)).ToArray();
        }
    }

    public class DeviceModelParamDetailViewModel : IViewModel<DeviceModelParamDetail>
    {
        public Int32 ID { get; set; }
        public Double XMin { get; set; }
        public Double XMax { get; set; }
        public Double A { get; set; }
        public Double B { get; set; }
        public void LoadFrom(DeviceModelParamDetail entity)
        {
            ID = entity.ID;
            XMin = entity.XMin;
            XMax = entity.XMax;
            A = entity.A;
            B = entity.B;
        }
    }
    public class DeviceModelParamCentrifugalDetailViewModel : IViewModel<DeviceModelParamCentrifugalDetail>
    {
        public Int32 ID { get; set; }
        public Double XMin { get; set; }
        public Double XMax { get; set; }
        public Double A { get; set; }
        public Double B { get; set; }
        public void LoadFrom(DeviceModelParamCentrifugalDetail entity)
        {
            ID = entity.ID;
            XMin = entity.XMin;
            XMax = entity.XMax;
            A = entity.A;
            B = entity.B;
        }
    }
    public class DeviceModelParamCentrifugal4DetailViewModel : IViewModel<DeviceModelParamCentrifugal4Detail>
    {
        public Int32 ID { get; set; }
        public Double XMin { get; set; }
        public Double XMax { get; set; }
        public Double A { get; set; }
        public Double B { get; set; }
        public void LoadFrom(DeviceModelParamCentrifugal4Detail entity)
        {
            ID = entity.ID;
            XMin = entity.XMin;
            XMax = entity.XMax;
            A = entity.A;
            B = entity.B;
        }
    }
}