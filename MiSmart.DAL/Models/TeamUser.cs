using Microsoft.EntityFrameworkCore.Infrastructure;
using MiSmart.Infrastructure.Data;
using System;

using System.Text.Json.Serialization;

namespace MiSmart.DAL.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TeamMemberType
    {
        Owner,
        Member
    }
    public class TeamUser : EntityBase<Int64>
    {
        public TeamUser() : base() { }
        public TeamUser(ILazyLoader lazyLoader) : base(lazyLoader) { }


        private Team team;

        [JsonIgnore]
        public Team Team
        {
            get => lazyLoader.Load(this, ref team);
            set => team = value;
        }
        public Int64 TeamID { get; set; }
        public Int64 UserID { get; set; }

        public TeamMemberType Type { get; set; } = TeamMemberType.Member;
    }
}