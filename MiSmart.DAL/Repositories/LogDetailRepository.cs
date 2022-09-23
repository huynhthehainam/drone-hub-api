using MiSmart.DAL.DatabaseContexts;
using MiSmart.DAL.Models;
using MiSmart.Infrastructure.Repositories;

namespace MiSmart.DAL.Repositories
{
    public class LogDetailRepository : RepositoryBase<LogDetail>
    {
        public LogDetailRepository(DatabaseContext context) : base(context)
        {
        }
    }
}