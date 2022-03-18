using System;
using MiSmart.DAL.Models;
using MiSmart.Infrastructure.ViewModels;

namespace MiSmart.DAL.ViewModels
{
    public class ExecutionCompanyUserViewModel : IViewModel<ExecutionCompanyUser>
    {
        public Int64 UserID { get; private set; }
        public ExecutionCompanyUserType Type { get; set; }

        public void LoadFrom(ExecutionCompanyUser entity)
        {
            UserID = entity.UserID;
            Type = entity.Type;
        }
    }
}