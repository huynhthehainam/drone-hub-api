using MiSmart.DAL.DatabaseContexts;
using MiSmart.DAL.Models;
using MiSmart.Infrastructure.Repositories;
using System.Linq;
using System;
namespace MiSmart.DAL.Repositories
{
    public class ExecutionCompanySettingRepository : RepositoryBase<ExecutionCompanySetting>
    {
        public ExecutionCompanySettingRepository(DatabaseContext context) : base(context)
        {
        }

        public ExecutionCompanySetting? GetLatestSetting(Int32 executionCompanyID)
        {
            return context.Set<ExecutionCompanySetting>().Where(ww => ww.ExecutionCompanyID == executionCompanyID).OrderByDescending(ww => ww.CreatedTime).FirstOrDefault();
        }
    }
}