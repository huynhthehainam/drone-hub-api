


using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MiSmart.Infrastructure.Data;

namespace MiSmart.DAL.Models
{

    public class DeviceModelParam : EntityBase<Int32>
    {
        public DeviceModelParam()
        {
        }

        public DeviceModelParam(ILazyLoader lazyLoader) : base(lazyLoader)
        {
        }
        public Boolean IsActive { get; set; }
        public String Name { get; set; }
        public DateTime CreationTime { get; set; } = DateTime.UtcNow;
        public String Description { get; set; }
        public Double YMin { get; set; }
        public Double YMax { get; set; }
        public Double YCentrifugalMin { get; set; }
        public Double YCentrifugalMax { get; set; }
        public Double FuelLevelNumber { get; set; }
        public Double FlowRateMinLimit { get; set; }
        public Double FlowRateMiddleLimit { get; set; }
        public Double FlowRateMaxLimit { get; set; }
        private DeviceModel deviceModel;
        public DeviceModel DeviceModel
        {
            get => lazyLoader.Load(this, ref deviceModel);
            set => deviceModel = value;
        }
        public Int32 DeviceModelID { get; set; }




        private ICollection<DeviceModelParamDetail> details;
        public ICollection<DeviceModelParamDetail> Details
        {
            get => lazyLoader.Load(this, ref details);
            set => details = value;
        }

        private ICollection<DeviceModelParamCentrifugalDetail> centrifugalDetails;
        public ICollection<DeviceModelParamCentrifugalDetail> CentrifugalDetails
        {
            get => lazyLoader.Load(this, ref centrifugalDetails);
            set => centrifugalDetails = value;
        }
        private ICollection<DeviceModelParamCentrifugal4Detail> centrifugal4Details;
        public ICollection<DeviceModelParamCentrifugal4Detail> Centrifugal4Details
        {
            get => lazyLoader.Load(this, ref centrifugal4Details);
            set => centrifugal4Details = value;
        }
    }

    public class DeviceModelParamDetail : EntityBase<Int32>
    {
        public DeviceModelParamDetail()
        {
        }

        public DeviceModelParamDetail(ILazyLoader lazyLoader) : base(lazyLoader)
        {
        }
        public Double XMin { get; set; }
        public Double XMax { get; set; }
        public Double A { get; set; }
        public Double B { get; set; }
        private DeviceModelParam deviceModelParam;
        public DeviceModelParam DeviceModelParam
        {
            get => lazyLoader.Load(this, ref deviceModelParam);
            set => deviceModelParam = value;
        }
        public Int32 DeviceModelParamID { get; set; }
    }

    public class DeviceModelParamCentrifugalDetail : EntityBase<Int32>
    {
        public DeviceModelParamCentrifugalDetail()
        {
        }

        public DeviceModelParamCentrifugalDetail(ILazyLoader lazyLoader) : base(lazyLoader)
        {
        }
        public Double XMin { get; set; }
        public Double XMax { get; set; }
        public Double A { get; set; }
        public Double B { get; set; }
        private DeviceModelParam deviceModelParam;
        public DeviceModelParam DeviceModelParam
        {
            get => lazyLoader.Load(this, ref deviceModelParam);
            set => deviceModelParam = value;
        }
        public Int32 DeviceModelParamID { get; set; }
    }
    public class DeviceModelParamCentrifugal4Detail : EntityBase<Int32>
    {
        public DeviceModelParamCentrifugal4Detail()
        {
        }

        public DeviceModelParamCentrifugal4Detail(ILazyLoader lazyLoader) : base(lazyLoader)
        {
        }
        public Double XMin { get; set; }
        public Double XMax { get; set; }
        public Double A { get; set; }
        public Double B { get; set; }
        private DeviceModelParam deviceModelParam;
        public DeviceModelParam DeviceModelParam
        {
            get => lazyLoader.Load(this, ref deviceModelParam);
            set => deviceModelParam = value;
        }
        public Int32 DeviceModelParamID { get; set; }
    }
}