


using Microsoft.EntityFrameworkCore;
using System;
using MiSmart.DAL.DatabaseContexts;
using MiSmart.DAL.Models;
using MiSmart.Infrastructure.Repositories;
using System.Linq;
using MiSmart.Infrastructure.ViewModels;
using System.Collections.Generic;

namespace MiSmart.DAL.Repositories
{
    public class CustomerUserRepository : RepositoryBase<CustomerUser>
    {
        public CustomerUserRepository(DatabaseContext context) : base(context)
        {
        }
        public CustomerUser GetByPermission(Int64 userID, CustomerMemberType type = CustomerMemberType.Member)
        {
            var customerUser = Get(ww => ww.UserID == userID);
            List<CustomerMemberType> types = new List<CustomerMemberType>();
            if (customerUser is not null)
            {
                switch (type)
                {
                    case CustomerMemberType.Owner:
                        types = new List<CustomerMemberType> { CustomerMemberType.Owner };
                        break;
                    case CustomerMemberType.Member:
                        types = new List<CustomerMemberType> { CustomerMemberType.Owner, CustomerMemberType.Member };
                        break;
                    default:
                        break;
                }
                return types.Contains(customerUser.Type) ? customerUser : null;
            }
            return null;
        }
    }
}
