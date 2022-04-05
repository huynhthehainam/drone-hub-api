using MiSmart.DAL.DatabaseContexts;
using MiSmart.DAL.Models;
using MiSmart.Infrastructure.Repositories;

namespace MiSmart.DAL.Repositories
{
    public class ExecutionCompanyUserFlightStatRepository : RepositoryBase<ExecutionCompanyUserFlightStat>
    {
        public ExecutionCompanyUserFlightStatRepository(DatabaseContext context) : base(context)
        {
        }
    }
}
