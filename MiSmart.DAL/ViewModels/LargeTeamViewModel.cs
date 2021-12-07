using System;
using System.Collections.Generic;
using System.Linq;
using MiSmart.DAL.Models;
using MiSmart.Infrastructure.ViewModels;

namespace MiSmart.DAL.ViewModels
{
    public class LargeTeamViewModel : IViewModel<Team>
    {
        public Int64 ID { get; set; }
        public Int32 CustomerID { get; set; }
        public String Name { get; set; }
        public List<Int64> UserIDs { get; set; }

        public void LoadFrom(Team entity)
        {
            ID = entity.ID;
            CustomerID = entity.CustomerID;
            Name = entity.Name;
            UserIDs = entity.TeamUsers.Select(ww => ww.UserID).ToList();
        }
    }
}