


using Microsoft.EntityFrameworkCore;
using System;
using MiSmart.DAL.DatabaseContexts;
using MiSmart.DAL.Models;
using MiSmart.Infrastructure.Repositories;
using System.Linq;
using MiSmart.Infrastructure.ViewModels;

namespace MiSmart.DAL.Repositories
{
    public class CustomerUserPermission
    {
        public Int32 CustomerID { get; set; }
        public CustomerMemberType Type { get; set; }

    }
    public class CustomerUserRepository : RepositoryBase<CustomerUser>
    {
        public CustomerUserRepository(DatabaseContext context) : base(context)
        {
        }
        public CustomerUserPermission GetMemberPermission(UserCacheViewModel currentUser, CustomerMemberType type = CustomerMemberType.Member)
        {
            var customerUser = Get(ww => ww.UserID == currentUser.ID);
            if (customerUser is not null)
            {
                switch (type)
                {
                    case CustomerMemberType.Owner:
                        if (customerUser.Type == CustomerMemberType.Owner)
                        {
                            return new CustomerUserPermission { CustomerID = customerUser.CustomerID, Type = customerUser.Type };
                        }
                        break;
                    case CustomerMemberType.Member:
                        if (customerUser.Type == CustomerMemberType.Owner || customerUser.Type == CustomerMemberType.Member)
                        {
                            return new CustomerUserPermission { CustomerID = customerUser.CustomerID, Type = customerUser.Type };
                        }
                        break;
                    default:
                        break;
                }

            }
            return null;
        }
    }
}
