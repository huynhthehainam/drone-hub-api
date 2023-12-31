
using Microsoft.EntityFrameworkCore.Infrastructure;
using MiSmart.Infrastructure.Data;
using System;
using System.Collections.Generic;

namespace MiSmart.DAL.Models
{
    public class Team : EntityBase<Int64>
    {
        public Team() : base()
        {
        }

        public Team(ILazyLoader lazyLoader) : base(lazyLoader)
        {
        }
        public String? Name { get; set; }

        public Double TotalTaskArea { get; set; }
        public Double TotalFlightDuration { get; set; }
        public Int64 TotalFlights { get; set; }


        private ExecutionCompany? executionCompany;
        public ExecutionCompany? ExecutionCompany
        {
            get => lazyLoader.Load(this, ref executionCompany);
            set => executionCompany = value;
        }
        public Int32 ExecutionCompanyID { get; set; }



        private ICollection<TeamUser>? teamUsers;
        public ICollection<TeamUser>? TeamUsers
        {
            get => lazyLoader.Load(this, ref teamUsers);
            set => teamUsers = value;
        }

        private ICollection<Device>? devices;
        public ICollection<Device>? Devices
        {
            get => lazyLoader.Load(this, ref devices);
            set => devices = value;
        }
        private ICollection<FlightStat>? flightStats;
        public ICollection<FlightStat>? FlightStats
        {
            get => lazyLoader.Load(this, ref flightStats);
            set => flightStats = value;
        }

        public Boolean IsDisbanded { get; set; } = false;
    }
}