using MiSmart.DAL.DatabaseContexts;
using MiSmart.DAL.Models;
using MiSmart.Infrastructure.Repositories;

namespace MiSmart.DAL.Repositories
{
    public class PartRepository : RepositoryBase<Part>
    {
        public PartRepository(DatabaseContext context) : base(context)
        {
        }
    }
}