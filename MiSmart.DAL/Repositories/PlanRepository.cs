


using Microsoft.EntityFrameworkCore;
using System;
using MiSmart.DAL.DatabaseContexts;
using MiSmart.DAL.Models;
using MiSmart.Infrastructure.Repositories;
using System.Linq;

namespace MiSmart.DAL.Repositories
{
    public class PlanRepository : RepositoryBase<Plan>
    {
        public PlanRepository(DatabaseContext context) : base(context)
        {
        }
    }
}
