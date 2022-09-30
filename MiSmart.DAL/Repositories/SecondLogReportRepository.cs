using MiSmart.DAL.DatabaseContexts;
using MiSmart.DAL.Models;
using MiSmart.Infrastructure.Repositories;

namespace MiSmart.DAL.Repositories
{
    public class SecondLogReportRepository : RepositoryBase<SecondLogReport>
    {
        public SecondLogReportRepository(DatabaseContext context) : base(context)
        {
        }
    }
}
