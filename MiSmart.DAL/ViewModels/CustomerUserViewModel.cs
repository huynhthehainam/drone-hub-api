using System;
using MiSmart.DAL.Models;
using MiSmart.Infrastructure.ViewModels;

namespace MiSmart.DAL.ViewModels
{
    public class CustomerUserViewModel : IViewModel<CustomerUser>
    {
        public Guid UserUUID { get; private set; }

        public void LoadFrom(CustomerUser entity)
        {
            UserUUID = entity.UserUUID;
        }
    }
}