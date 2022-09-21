using MiSmart.DAL.DatabaseContexts;
using MiSmart.DAL.Models;
using MiSmart.Infrastructure.Repositories;

namespace MiSmart.DAL.Repositories
{
    public class LogReportResultRepository : RepositoryBase<Field>
    {
        public LogReportResultRepository(DatabaseContext context) : base(context)
        {
        }
    }
}