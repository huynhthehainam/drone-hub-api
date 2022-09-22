using MiSmart.DAL.DatabaseContexts;
using MiSmart.DAL.Models;
using MiSmart.Infrastructure.Repositories;

namespace MiSmart.DAL.Repositories
{
    public class LogReportRepository : RepositoryBase<LogReport>
    {
        public LogReportRepository(DatabaseContext context) : base(context)
        {
        }
    }
}