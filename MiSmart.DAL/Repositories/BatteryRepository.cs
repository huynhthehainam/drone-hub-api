using MiSmart.DAL.DatabaseContexts;
using MiSmart.DAL.Models;
using MiSmart.Infrastructure.Repositories;

namespace MiSmart.DAL.Repositories
{
    public class BatteryRepository : RepositoryBase<Battery>
    {
        public BatteryRepository(DatabaseContext context) : base(context)
        {
        }
    }
}