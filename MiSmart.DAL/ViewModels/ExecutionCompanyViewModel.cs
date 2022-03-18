using System;
using MiSmart.DAL.Models;
using MiSmart.Infrastructure.ViewModels;

namespace MiSmart.DAL.ViewModels
{
    public class ExecutionCompanyViewModel : IViewModel<ExecutionCompany>
    {
        public Int32 ID { get; set; }
        public String Name { get; set; }
        public void LoadFrom(ExecutionCompany entity)
        {
            ID = entity.ID;
            Name = entity.Name;
        }
    }
}