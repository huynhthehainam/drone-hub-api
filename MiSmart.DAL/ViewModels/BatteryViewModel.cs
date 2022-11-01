

using System;
using MiSmart.DAL.Models;
using MiSmart.Infrastructure.ViewModels;

namespace MiSmart.DAL.ViewModels
{
    public class BatteryViewModel : IViewModel<Battery>
    {
        public Int32 ID { get; set; }
        public String? ActualID { get; set; }
        public Int32? ExecutionCompanyID { get; set; }
        public String? ExecutionCompanyName { get; set; }
        public String? BatteryModel { get; set; }
        public Int32 BatteryModelID { get; set; }
        public String? BatteryModelName { get; set; }
        public String? Name { get; set; }
        public BatteryGroupLogViewModel? LastGroup { get; set; }
        public void LoadFrom(Battery entity)
        {
            ID = entity.ID;
            ActualID = entity.ActualID;
            ExecutionCompanyID = entity.ExecutionCompanyID;
            ExecutionCompanyName = entity.ExecutionCompany?.Name;
            LastGroup = entity.LastGroup is null ? null : ViewModelHelpers.ConvertToViewModel<BatteryGroupLog, BatteryGroupLogViewModel>(entity.LastGroup);
            BatteryModelID = entity.BatteryModelID;
            BatteryModelName = entity.BatteryModel is null ? null : entity.BatteryModel.Name;
            Name = entity.Name;
        }
    }
    public class SmallBatteryModelViewModel : IViewModel<BatteryModel>
    {
        public Int32 ID { get; set; }
        public String? Name { get; set; }
        public String? ManufacturerName { get; set; }
        public Int32 BatteriesCount { get; set; }
        public String? FileUrl { get; set; }
        public void LoadFrom(BatteryModel entity)
        {
            ID = entity.ID;
            Name = entity.Name;
            ManufacturerName = entity.ManufacturerName;
            BatteriesCount = entity.Batteries == null ? 0 : entity.Batteries.Count;
            FileUrl = entity.FileUrl;
        }
    }
}