


using Microsoft.EntityFrameworkCore;
using MiSmart.DAL.DatabaseContexts;
using MiSmart.DAL.Models;
using MiSmart.Infrastructure.Repositories;
using MiSmart.Infrastructure.ViewModels;
using System;
using System.Linq;

namespace MiSmart.DAL.Repositories
{
    public class TeamRepository : RepositoryBase<Team>
    {
        public TeamRepository(DatabaseContext context) : base(context)
        {
        }
    }
}