

using System;
using MiSmart.DAL.Models;
using MiSmart.Infrastructure.ViewModels;

namespace MiSmart.DAL.ViewModels
{
    public class BatteryViewModel : IViewModel<Battery>
    {
        public Int32 ID { get; set; }
        public String ActualID { get; set; }
        public Int32? ExecutionCompanyID { get; set; }
        public BatteryGroupLogViewModel LastGroup { get; set; }
        public void LoadFrom(Battery entity)
        {
            ID = entity.ID;
            ActualID = entity.ActualID;
            ExecutionCompanyID = entity.ExecutionCompanyID;
            LastGroup = entity.LastGroup is null ? null : ViewModelHelpers.ConvertToViewModel<BatteryGroupLog, BatteryGroupLogViewModel>(entity.LastGroup);
        }
    }
}