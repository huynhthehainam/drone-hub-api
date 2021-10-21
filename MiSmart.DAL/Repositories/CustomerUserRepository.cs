


using Microsoft.EntityFrameworkCore;
using System;
using MiSmart.DAL.DatabaseContexts;
using MiSmart.DAL.Models;
using MiSmart.Infrastructure.Repositories;
using System.Linq;

namespace MiSmart.DAL.Repositories
{
    public class CustomerUserRepository : RepositoryBase<CustomerUser>
    {
        public CustomerUserRepository(DatabaseContext context) : base(context)
        {
        }
    }
}
