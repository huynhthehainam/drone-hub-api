using MiSmart.DAL.DatabaseContexts;
using MiSmart.DAL.Models;
using MiSmart.Infrastructure.Repositories;

namespace MiSmart.DAL.Repositories
{
    public class BatteryModelRepository : RepositoryBase<BatteryModel>
    {
        public BatteryModelRepository(DatabaseContext context) : base(context)
        {
        }
    }
}
