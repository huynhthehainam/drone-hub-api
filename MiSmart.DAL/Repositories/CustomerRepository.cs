


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

       
    }
}