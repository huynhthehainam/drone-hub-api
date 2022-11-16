



using System;
using System.ComponentModel.DataAnnotations;
using MiSmart.DAL.Models;

namespace MiSmart.API.Commands
{
    public class AssigningCustomerUserCommand
    {
        [Required]
        public Guid? UserUUID { get; set; }
    }
    public class AssigningExecutionCompanyUserCommand
    {
        [Required]
        public Guid? UserUUID { get; set; }
        public ExecutionCompanyUserType Type { get; set; } = ExecutionCompanyUserType.Member;
    }
    public class AssigningExecutionCompanyUserFromTMCommand
    {
        [Required]
        public String? SecretKey { get; set; }
        public String? EncryptedUUID { get; set; }
        public ExecutionCompanyUserType Type { get; set; } = ExecutionCompanyUserType.Member;
    }
    public class AssigningCustomerUserFromTMCommand
    {
        [Required]
        public String? SecretKey { get; set; }
        [Required]
        public String? EncryptedUUID { get; set; }
    }

}