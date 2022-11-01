using System;
using System.Linq;
using System.Threading.Tasks;
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

        public async Task<Battery?> GetOrCreateBySerialNumberAsync(String serialNumber)
        {
            var batteryModel = context.Set<BatteryModel>().FirstOrDefault();
            if (batteryModel is null)
            {
                return null;
            }
            var battery = await GetAsync(ww => ww.ActualID == serialNumber);
            if (battery is not null)
            {
                return battery;
            }

            battery = await CreateAsync(new Battery { ActualID = serialNumber, BatteryModel = batteryModel });

            return battery;

        }
    }
}