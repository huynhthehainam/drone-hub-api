



using System;
using System.ComponentModel.DataAnnotations;
using MiSmart.DAL.Models;

namespace MiSmart.API.Commands
{
    public class AssigningCustomerUserCommand
    {
        [Required]
        public Int64? UserID { get; set; }

        public CustomerMemberType Type { get; set; } = CustomerMemberType.Member;
    }
}