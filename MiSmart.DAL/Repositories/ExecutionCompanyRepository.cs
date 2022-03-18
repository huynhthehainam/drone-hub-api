using MiSmart.DAL.DatabaseContexts;
using MiSmart.DAL.Models;
using MiSmart.Infrastructure.Repositories;

namespace MiSmart.DAL.Repositories
{
    public class ExecutionCompanyRepository : RepositoryBase<ExecutionCompany>
    {
        public ExecutionCompanyRepository(DatabaseContext context) : base(context)
        {
        }


    }
}