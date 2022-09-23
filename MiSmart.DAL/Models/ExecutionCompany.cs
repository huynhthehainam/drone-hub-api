


using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MiSmart.Infrastructure.Data;

namespace MiSmart.DAL.Models
{
    public class ExecutionCompany : EntityBase<Int32>
    {
        public ExecutionCompany() : base()
        {
        }

        public ExecutionCompany(ILazyLoader lazyLoader) : base(lazyLoader)
        {
        }
        public String Name { get; set; }
        public String Address { get; set; }

        private ICollection<Team> teams;
        public ICollection<Team> Teams
        {
            get => lazyLoader.Load(this, ref teams);
            set => teams = value;

        }
        private ICollection<Device> devices;
        public ICollection<Device> Devices
        {
            get => lazyLoader.Load(this, ref devices);
            set => devices = value;
        }
        private ICollection<ExecutionCompanyUser> executionCompanyUsers;
        public ICollection<ExecutionCompanyUser> ExecutionCompanyUsers
        {
            get => lazyLoader.Load(this, ref executionCompanyUsers);
            set => executionCompanyUsers = value;
        }
        private ICollection<Field> fields;
        public ICollection<Field> Fields
        {
            get => lazyLoader.Load(this, ref fields);
            set => fields = value;
        }

        private ICollection<FlightStat> flightStats;
        public ICollection<FlightStat> FlightStats
        {
            get => lazyLoader.Load(this, ref flightStats);
            set => flightStats = value;
        }


        private ICollection<Battery> batteries;
        public ICollection<Battery> Batteries
        {
            get => lazyLoader.Load(this, ref batteries);
            set => batteries = value;
        }

        private ICollection<ExecutionCompanySetting> settings;
        public ICollection<ExecutionCompanySetting> Settings
        {
            get => lazyLoader.Load(this, ref settings);
            set => settings = value;
        }

        private ICollection<LogReportResult> logReportResults;
        public ICollection<LogReportResult> LogReportResults
        {
            get => lazyLoader.Load(this, ref logReportResults);
            set => logReportResults = value;
        }
    }
}