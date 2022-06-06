



using System;
using System.ComponentModel.DataAnnotations;
using MiSmart.DAL.Models;

namespace MiSmart.API.Commands
{
    public class AssigningTeamUserCommand
    {
        [Required]
        public Guid? UserUUID { get; set; }
        public TeamMemberType Type { get; set; } = TeamMemberType.Member;
    }
     public class RemovingTeamUserCommand
    {
        [Required]
        public Guid? UserUUID { get; set; }
    }
}