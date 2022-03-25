using System;
using MiSmart.DAL.DatabaseContexts;
using MiSmart.DAL.Models;
using MiSmart.Infrastructure.Repositories;
using System.Collections.Generic;

namespace MiSmart.DAL.Repositories
{
    public class ExecutionCompanyUserRepository : RepositoryBase<ExecutionCompanyUser>
    {
        public ExecutionCompanyUserRepository(DatabaseContext context) : base(context)
        {
        }
        public ExecutionCompanyUser GetByPermission(Int64 userID, ExecutionCompanyUserType type = ExecutionCompanyUserType.Member)
        {
            var executionCompanyUser = Get(ww => ww.UserID == userID);
            List<ExecutionCompanyUserType> types = new List<ExecutionCompanyUserType>();
            if (executionCompanyUser is not null)
            {
                switch (type)
                {
                    case ExecutionCompanyUserType.Owner:
                        types = new List<ExecutionCompanyUserType> { ExecutionCompanyUserType.Owner };
                        break;
                    case ExecutionCompanyUserType.Member:
                        types = new List<ExecutionCompanyUserType> { ExecutionCompanyUserType.Owner, ExecutionCompanyUserType.Member };
                        break;
                    default:
                        break;
                }
                return types.Contains(executionCompanyUser.Type) ? executionCompanyUser : null;
            }
            return null;
        }
    }
}