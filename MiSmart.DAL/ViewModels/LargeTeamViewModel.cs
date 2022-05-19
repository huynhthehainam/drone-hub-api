using System;
using System.Collections.Generic;
using System.Linq;
using MiSmart.DAL.Models;
using MiSmart.Infrastructure.ViewModels;

namespace MiSmart.DAL.ViewModels
{
    public class SmallExecutionCompanyUserFlightStatViewModel : IViewModel<ExecutionCompanyUserFlightStat>
    {
        public Guid ID { get; set; }
        public SuperSmallFlightStatViewModel FlightStat { get; set; }
        public TeamMemberType Type { get; set; }

        public void LoadFrom(ExecutionCompanyUserFlightStat entity)
        {
            ID = entity.ID;
            FlightStat = ViewModelHelpers.ConvertToViewModel<FlightStat, SuperSmallFlightStatViewModel>(entity.FlightStat);
            Type = entity.Type;
        }
    }
    public class TeamMemberViewModel
    {
        public Int64 UserID { get; set; }
        public TeamMemberType Type { get; set; }
        public Double TotalTaskArea { get; set; }
        public Double TotalFlightDuration { get; set; }
        public Int32 TotalFlights { get; set; }
        public List<SmallExecutionCompanyUserFlightStatViewModel> ExecutionCompanyUserFlightStats { get; set; }
    }

    public class LargeTeamViewModel : IViewModel<Team>
    {
        public Int64 ID { get; set; }
        public Int32 ExecutionCompanyID { get; set; }
        public String Name { get; set; }
        public List<TeamMemberViewModel> Members { get; set; }
        public Int32 MembersCount { get; set; }
        public Double TotalTaskArea { get; set; }
        public Double TotalFlightDuration { get; set; }
        public Int32 TotalFlights { get; set; }
        public Double TotalCost { get; set; }
        public List<SuperSmallFlightStatViewModel> FlightStats { get; set; }
        public void LoadFrom(Team entity)
        {
            ID = entity.ID;
            ExecutionCompanyID = entity.ExecutionCompanyID;
            Name = entity.Name;
            var teamUsers = new List<TeamMemberViewModel>();
            foreach (var teamUser in entity.TeamUsers)
            {
                TeamMemberViewModel teamMember = new TeamMemberViewModel
                {
                    UserID = teamUser.ExecutionCompanyUser.UserID,
                    Type = teamUser.Type,
                    ExecutionCompanyUserFlightStats = teamUser.ExecutionCompanyUser.ExecutionCompanyUserFlightStats.Select(ww => ViewModelHelpers.ConvertToViewModel<ExecutionCompanyUserFlightStat, SmallExecutionCompanyUserFlightStatViewModel>(ww)).ToList(),
                    TotalTaskArea = teamUser.ExecutionCompanyUser.ExecutionCompanyUserFlightStats.Sum(ww => ww.FlightStat.TaskArea),
                    TotalFlightDuration = teamUser.ExecutionCompanyUser.ExecutionCompanyUserFlightStats.Sum(ww => ww.FlightStat.FlightDuration),
                    TotalFlights = teamUser.ExecutionCompanyUser.ExecutionCompanyUserFlightStats.Sum(ww => ww.FlightStat.Flights),
                };
                teamUsers.Add(teamMember);
            }
            Members = teamUsers.ToList();
            MembersCount = teamUsers.Count();

            TotalTaskArea = entity.FlightStats.Sum(ww => ww.TaskArea);
            TotalFlightDuration = entity.FlightStats.Sum(ww => ww.FlightDuration);
            TotalFlights = entity.FlightStats.Sum(ww => ww.Flights);
            TotalCost = entity.FlightStats.Sum(ww => ww.Cost);
            FlightStats = entity.FlightStats.Select(ww => ViewModelHelpers.ConvertToViewModel<FlightStat, SuperSmallFlightStatViewModel>(ww)).ToList();
        }
    }
}