using System;
using System.Collections.Generic;
using MiSmart.DAL.Models;
using MiSmart.Infrastructure.ViewModels;

namespace MiSmart.DAL.ViewModels
{
    public class CustomerUserViewModel : IViewModel<CustomerUser>
    {
        public Int64 UserID { get; private set; }
        public CustomerMemberType Type { get; private set; }

        public void LoadFrom(CustomerUser entity)
        {
            UserID = entity.UserID;
            Type = entity.Type;
        }
    }
}