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
        public async Task<CustomerUser?> GetByPermissionAsync(Guid userUUID)
        {
            var customerUser = await GetAsync(ww => ww.UserUUID == userUUID);

            return customerUser;
        }
    }
}
