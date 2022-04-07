



using System;
using System.ComponentModel.DataAnnotations;
using MiSmart.DAL.Models;

namespace MiSmart.API.Commands
{
    public class AssigningTeamUserCommand
    {
        [Required]
        public Int64? UserID { get; set; }
        public TeamMemberType Type { get; set; } = TeamMemberType.Member;
    }
     public class RemovingTeamUserCommand
    {
        [Required]
        public Int64? UserID { get; set; }
    }
}