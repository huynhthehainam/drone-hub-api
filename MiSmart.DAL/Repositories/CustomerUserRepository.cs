


using Microsoft.EntityFrameworkCore;
using System;
using MiSmart.DAL.DatabaseContexts;
using MiSmart.DAL.Models;
using MiSmart.Infrastructure.Repositories;
using System.Linq;
using MiSmart.Infrastructure.ViewModels;

namespace MiSmart.DAL.Repositories
{
    public class CustomerUserRepository : RepositoryBase<CustomerUser>
    {
        public CustomerUserRepository(DatabaseContext context) : base(context)
        {
        }
        public Int32? HasOwnerPermission(UserCacheViewModel currentUser)
        {
            var customerUser = Get(ww => ww.UserID == currentUser.ID);
            if (customerUser is not null)
            {
                if (customerUser.Type == CustomerMemberType.Owner)
                {
                    return customerUser.CustomerID;
                }
            }
            return null;
        }
        public Int32? HasMemberPermission(UserCacheViewModel currentUser)
        {
            var customerUser = Get(ww => ww.UserID == currentUser.ID);
            if (customerUser is not null)
            {
                if (customerUser.Type == CustomerMemberType.Owner || customerUser.Type == CustomerMemberType.Member)
                {
                    return customerUser.CustomerID;
                }
            }
            return null;
        }
    }
}
