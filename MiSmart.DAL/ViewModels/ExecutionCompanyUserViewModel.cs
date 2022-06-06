using System;
using MiSmart.DAL.Models;
using MiSmart.Infrastructure.ViewModels;

namespace MiSmart.DAL.ViewModels
{
    public class ExecutionCompanyUserViewModel : IViewModel<ExecutionCompanyUser>
    {
        public Guid UserUUID { get; private set; }
        public ExecutionCompanyUserType Type { get; set; }

        public void LoadFrom(ExecutionCompanyUser entity)
        {
            UserUUID = entity.UserUUID;
            Type = entity.Type;
        }
    }
}