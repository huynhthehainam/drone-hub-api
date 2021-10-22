using System;
using System.Collections.Generic;
using MiSmart.DAL.Models;
using MiSmart.Infrastructure.ViewModels;

namespace MiSmart.DAL.ViewModels
{
    public class SmallTeamViewModel : IViewModel<Team>
    {
        public Int64 ID { get; set; }
        public Int32 CustomerID { get; set; }
        public String Name { get; set; }

        public void LoadFrom(Team entity)
        {
            ID = entity.ID;
            CustomerID = entity.CustomerID;
            Name = entity.Name;
        }
    }
}