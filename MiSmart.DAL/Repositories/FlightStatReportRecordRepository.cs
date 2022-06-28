using MiSmart.DAL.DatabaseContexts;
using MiSmart.DAL.Models;
using MiSmart.Infrastructure.Repositories;

namespace MiSmart.DAL.Repositories;

public class FlightStatReportRecordRepository : RepositoryBase<FlightStatReportRecord>
{
    public FlightStatReportRecordRepository(DatabaseContext context) : base(context)
    {
    }
}

