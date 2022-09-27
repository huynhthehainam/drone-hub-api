using MiSmart.DAL.DatabaseContexts;
using MiSmart.DAL.Models;
using MiSmart.Infrastructure.Repositories;

namespace MiSmart.DAL.Repositories
{
    public class LogResultDetailRepository : RepositoryBase<LogResultDetail>
    {
        public LogResultDetailRepository(DatabaseContext context) : base(context)
        {
        }
    }
}