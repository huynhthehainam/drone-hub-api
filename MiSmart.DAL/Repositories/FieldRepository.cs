


using Microsoft.EntityFrameworkCore;
using System;
using MiSmart.DAL.DatabaseContexts;
using MiSmart.DAL.Models;
using MiSmart.Infrastructure.Repositories;
using System.Linq;

namespace MiSmart.DAL.Repositories
{
    public class FieldRepository : RepositoryBase<Field>
    {
        public FieldRepository(DatabaseContext context) : base(context)
        {
        }
    }
}
