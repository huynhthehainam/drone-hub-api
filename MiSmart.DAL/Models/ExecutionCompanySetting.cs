


using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MiSmart.Infrastructure.Data;

namespace MiSmart.DAL.Models
{
    public class ExecutionCompanySetting : EntityBase<Int32>
    {
        public ExecutionCompanySetting()
        {
        }

        public ExecutionCompanySetting(ILazyLoader lazyLoader) : base(lazyLoader)
        {
        }

        public DateTime CreatedTime { get; set; } = DateTime.UtcNow;
        public Double MainPilotCostPerHectare { get; set; }
        public Double SubPilotCostPerHectare { get; set; }
        public Double CostPerHectare { get; set; }
        private ExecutionCompany? executionCompany;
        public ExecutionCompany? ExecutionCompany
        {
            get => lazyLoader.Load(this, ref executionCompany);
            set => executionCompany = value;
        }
        public Int32 ExecutionCompanyID { get; set; }
    }
}