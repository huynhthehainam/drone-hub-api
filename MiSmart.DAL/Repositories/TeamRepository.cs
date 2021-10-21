


using Microsoft.EntityFrameworkCore;
using MiSmart.DAL.DatabaseContexts;
using MiSmart.DAL.Models;
using MiSmart.Infrastructure.Repositories;
using MiSmart.Infrastructure.ViewModels;
using System;
using System.Linq;

namespace MiSmart.DAL.Repositories
{
    public class TeamRepository : RepositoryBase<Team>
    {
        public TeamRepository(DatabaseContext context) : base(context)
        {
        }
        public Boolean HasOwnerPermission(Int64 teamID, UserCacheViewModel currentUser)
        {
            if (currentUser.IsAdmin)
            {
                return true;
            }
            var team = Get(ww => ww.ID == teamID);
            if (team is not null)
            {
                var teamUser = team.TeamUsers.FirstOrDefault(ww => ww.UserID == currentUser.ID && ww.Type == TeamMemberType.Owner);
                if (teamUser is not null)
                {
                    return true;
                }
            }
            return false;
        }
    }
}