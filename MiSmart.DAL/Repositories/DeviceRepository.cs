


using Microsoft.EntityFrameworkCore;
using MiSmart.DAL.DatabaseContexts;
using MiSmart.DAL.Models;
using MiSmart.Infrastructure.Repositories;


namespace MiSmart.DAL.Repositories
{
    public class DeviceRepository : RepositoryBase<Device>
    {
        public DeviceRepository(DatabaseContext context) : base(context)
        {
        }
    }
}