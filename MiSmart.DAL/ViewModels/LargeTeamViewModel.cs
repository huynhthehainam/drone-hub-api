using System;
using System.Collections.Generic;
using System.Linq;
using MiSmart.DAL.Models;
using MiSmart.Infrastructure.ViewModels;

namespace MiSmart.DAL.ViewModels
{
    public class TeamMemberViewModel
    {
        public Int64 UserID { get; set; }
        public TeamMemberType Type { get; set; }
    }
    public class LargeTeamViewModel : IViewModel<Team>
    {
        public Int64 ID { get; set; }
        public Int32 ExecutionCompanyID { get; set; }
        public String Name { get; set; }
        public List<TeamMemberViewModel> Members { get; set; }
        public Int32 MembersCount { get; set; }

        public void LoadFrom(Team entity)
        {
            ID = entity.ID;
            ExecutionCompanyID = entity.ExecutionCompanyID;
            Name = entity.Name;
            var teamUsers = entity.TeamUsers.Select(ww => new TeamMemberViewModel { UserID = ww.ExecutionCompanyUser.UserID, Type = ww.Type });
            Members = teamUsers.ToList();
            MembersCount = teamUsers.Count();
        }
    }
}