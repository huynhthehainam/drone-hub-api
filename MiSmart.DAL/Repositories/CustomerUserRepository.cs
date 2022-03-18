using System;
using MiSmart.DAL.DatabaseContexts;
using MiSmart.DAL.Models;
using MiSmart.Infrastructure.Repositories;

namespace MiSmart.DAL.Repositories
{
    public class CustomerUserRepository : RepositoryBase<CustomerUser>
    {
        public CustomerUserRepository(DatabaseContext context) : base(context)
        {
        }
        public CustomerUser GetByPermission(Int64 userID)
        {
            var customerUser = Get(ww => ww.UserID == userID);

            return customerUser;
        }
    }
}
