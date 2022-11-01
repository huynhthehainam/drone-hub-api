


using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MiSmart.Infrastructure.Data;

namespace MiSmart.DAL.Models
{
    public class ExecutionCompanyUserFlightStat : EntityBase<Guid>
    {
        public ExecutionCompanyUserFlightStat()
        {
        }

        public ExecutionCompanyUserFlightStat(ILazyLoader lazyLoader) : base(lazyLoader)
        {
        }
        private ExecutionCompanyUser? executionCompanyUser;
        public ExecutionCompanyUser? ExecutionCompanyUser
        {
            get => lazyLoader.Load(this, ref executionCompanyUser);
            set => executionCompanyUser = value;
        }
        public Int64 ExecutionCompanyUserID { get; set; }

        private FlightStat? flightStat;
        public FlightStat? FlightStat
        {
            get => lazyLoader.Load(this, ref flightStat);
            set => flightStat = value;
        }
        public Guid FlightStatID { get; set; }
        public TeamMemberType Type { get; set; } = TeamMemberType.Member;
    }
}