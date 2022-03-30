



using System;
using System.ComponentModel.DataAnnotations;
using MiSmart.DAL.Models;

namespace MiSmart.API.Commands
{
    public class AssigningCustomerUserCommand
    {
        [Required]
        public Int64? UserID { get; set; }
    }
    public class AssigningExecutionCompanyUserCommand
    {
        [Required]
        public Int64? UserID { get; set; }
        public ExecutionCompanyUserType Type { get; set; } = ExecutionCompanyUserType.Member;
    }
}