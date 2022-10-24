using MiSmart.DAL.DatabaseContexts;
using MiSmart.DAL.Models;
using MiSmart.Infrastructure.Repositories;

namespace MiSmart.DAL.Repositories
{
    public class DeviceModelParamRepository : RepositoryBase<DeviceModelParam>
    {
        public DeviceModelParamRepository(DatabaseContext context) : base(context)
        {
        }
    }
}
