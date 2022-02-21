using MiSmart.DAL.DatabaseContexts;
using MiSmart.DAL.Models;
using MiSmart.Infrastructure.Repositories;

namespace MiSmart.DAL.Repositories
{
    public class TelemetryGroupRepository : RepositoryBase<TelemetryGroup>
    {
        public TelemetryGroupRepository(DatabaseContext context) : base(context)
        {
        }
    }
}