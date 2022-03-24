using MiSmart.DAL.DatabaseContexts;
using MiSmart.DAL.Models;
using MiSmart.Infrastructure.Repositories;

namespace MiSmart.DAL.Repositories
{
    public class BatteryGroupLogRepository : RepositoryBase<BatteryGroupLog>
    {
        public BatteryGroupLogRepository(DatabaseContext context) : base(context)
        {
        }
    }
}