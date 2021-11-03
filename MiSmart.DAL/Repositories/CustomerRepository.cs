


using Microsoft.EntityFrameworkCore;
using System;
using MiSmart.DAL.DatabaseContexts;
using MiSmart.DAL.Models;
using MiSmart.Infrastructure.Repositories;
using System.Linq;
using MiSmart.Infrastructure.ViewModels;

namespace MiSmart.DAL.Repositories
{
    public class CustomerRepository : RepositoryBase<Customer>
    {
        public CustomerRepository(DatabaseContext context) : base(context)
        {
        }

        public Boolean HasOwnerPermission(Int32 customerID, UserCacheViewModel currentUser)
        {

            var customer = Get(ww => ww.ID == customerID);
            if (customer is not null)
            {
                if (currentUser.IsAdmin)
                {
                    return true;
                }
                var customerUser = customer.CustomerUsers.FirstOrDefault(ww => ww.UserID == currentUser.ID && ww.Type == CustomerMemberType.Owner);
                if (customerUser is not null)
                {
                    return true;
                }
            }
            return false;
        }
        public Boolean HasMemberPermission(Int32 customerID, UserCacheViewModel currentUser)
        {

            var customer = Get(ww => ww.ID == customerID);
            if (customer is not null)
            {
                if (currentUser.IsAdmin)
                    return true;
                var customerUser = customer.CustomerUsers.FirstOrDefault(ww => ww.UserID == currentUser.ID);
                if (customerUser is not null)
                    return true;
            }
            return false;
        }
    }
}