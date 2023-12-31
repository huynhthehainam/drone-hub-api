
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MiSmart.Infrastructure.Data;
namespace MiSmart.DAL.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ExecutionCompanyUserType
    {
        Owner,
        Member
    }
    public class ExecutionCompanyUser : EntityBase<Int64>
    {
        public ExecutionCompanyUser() : base()
        {
        }

        public ExecutionCompanyUser(ILazyLoader lazyLoader) : base(lazyLoader)
        {
        }

        public Guid UserUUID { get; set; }
        private ExecutionCompany? executionCompany;
        public ExecutionCompany? ExecutionCompany
        {
            get => lazyLoader.Load(this, ref executionCompany);
            set => executionCompany = value;
        }
        public Int32 ExecutionCompanyID { get; set; }
        public ExecutionCompanyUserType Type { get; set; } = ExecutionCompanyUserType.Member;
        private ICollection<TeamUser>? teamUsers;
        public ICollection<TeamUser>? TeamUsers
        {
            get => lazyLoader.Load(this, ref teamUsers);
            set => teamUsers = value;
        }

        private ICollection<ExecutionCompanyUserFlightStat>? executionCompanyUserFlightStats;
        public ICollection<ExecutionCompanyUserFlightStat>? ExecutionCompanyUserFlightStats
        {
            get => lazyLoader.Load(this, ref executionCompanyUserFlightStats);
            set => executionCompanyUserFlightStats = value;
        }
    }
}