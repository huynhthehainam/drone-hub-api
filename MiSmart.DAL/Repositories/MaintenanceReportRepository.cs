using MiSmart.DAL.DatabaseContexts;
using MiSmart.DAL.Models;
using MiSmart.Infrastructure.Repositories;

namespace MiSmart.DAL.Repositories
{
    public class MaintenanceReportRepository : RepositoryBase<MaintenanceReport>
    {
        public MaintenanceReportRepository(DatabaseContext context) : base(context)
        {
        }
    }
}
