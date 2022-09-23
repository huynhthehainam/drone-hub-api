using MiSmart.DAL.DatabaseContexts;
using MiSmart.DAL.Models;
using MiSmart.Infrastructure.Repositories;
using System.Linq;

namespace MiSmart.DAL.Repositories
{
    public class LogTokenRepository : RepositoryBase<LogToken>
    {
        public LogTokenRepository(DatabaseContext context) : base(context)
        {
        }
    }
}