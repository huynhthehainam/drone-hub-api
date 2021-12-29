using System;
using System.Collections.Generic;
using System.Linq;
using MiSmart.DAL.Models;
using MiSmart.Infrastructure.ViewModels;

namespace MiSmart.DAL.ViewModels
{
    public class SmallTeamViewModel : IViewModel<Team>
    {
        public Int64 ID { get; set; }
        // public Int32 CustomerID { get; set; }
        public String Name { get; set; }
        public Int32 MembersCount { get; set; }

        public void LoadFrom(Team entity)
        {
            ID = entity.ID;
            MembersCount = entity.TeamUsers.Select(ww => ww.UserID).ToList().Count;
            Name = entity.Name;
        }
    }
}