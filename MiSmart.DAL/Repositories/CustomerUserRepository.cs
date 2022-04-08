using System;
using System.Threading.Tasks;
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
        public async Task<CustomerUser> GetByPermissionAsync(Int64 userID)
        {
            var customerUser = await GetAsync(ww => ww.UserID == userID);

            return customerUser;
        }
    }
}
