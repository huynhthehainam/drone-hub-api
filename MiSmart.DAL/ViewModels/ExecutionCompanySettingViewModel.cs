
using System;
using MiSmart.DAL.Models;
using MiSmart.Infrastructure.ViewModels;

namespace MiSmart.DAL.ViewModels
{
    public class ExecutionCompanySettingViewModel : IViewModel<ExecutionCompanySetting>
    {
        public Int32 ID { get; set; }
        public Double CostPerHectare { get; set; }
        public DateTime CreatedTime { get; set; }
        public Double MainPilotCostPerHectare { get; set; }
        public Double SubPitlotCostPerHectare { get; set; }

        public void LoadFrom(ExecutionCompanySetting entity)
        {
            ID = entity.ID;
            CostPerHectare = entity.CostPerHectare;
            CreatedTime = entity.CreatedTime;
            MainPilotCostPerHectare = entity.MainPilotCostPerHectare;
            SubPitlotCostPerHectare = entity.SubPitlotCostPerHectare;
        }
    }
}