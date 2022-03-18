using System;
using MiSmart.DAL.Models;
using MiSmart.Infrastructure.ViewModels;

namespace MiSmart.DAL.ViewModels
{
    public class CustomerUserViewModel : IViewModel<CustomerUser>
    {
        public Int64 UserID { get; private set; }

        public void LoadFrom(CustomerUser entity)
        {
            UserID = entity.UserID;
        }
    }
}