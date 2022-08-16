using MiSmart.DAL.DatabaseContexts;
using MiSmart.DAL.Models;
using MiSmart.Infrastructure.Repositories;

namespace MiSmart.DAL.Repositories
{
    public class LogFileRepository : RepositoryBase<LogFile>
    {
        public LogFileRepository(DatabaseContext context) : base(context)
        {
        }
    }
}