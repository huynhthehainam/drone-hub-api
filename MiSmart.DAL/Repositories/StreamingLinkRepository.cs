using MiSmart.DAL.DatabaseContexts;
using MiSmart.DAL.Models;
using MiSmart.Infrastructure.Repositories;

namespace MiSmart.DAL.Repositories
{
    public class StreamingLinkRepository : RepositoryBase<StreamingLink>
    {
        public StreamingLinkRepository(DatabaseContext context) : base(context)
        {
        }
    }
}