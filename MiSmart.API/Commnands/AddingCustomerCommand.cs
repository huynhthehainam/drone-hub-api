
using System;
using System.ComponentModel.DataAnnotations;

namespace MiSmart.API.Commands
{
    public class AddingCustomerCommand
    {
        [Required]
        public String? Name { get; set; }

        public String? Address { get; set; }
    }
    public class UpdatingCustomerCommand
    {
        public String? Name { get; set; }

        public String? Address { get; set; }
    }
    public class AddingExecutionCompanyCommand
    {
        [Required]
        public String? Name { get; set; }

        public String? Address { get; set; }
    }
    public class UpdatingExecutionCompanyCommand
    {
        public String? Name { get; set; }

        public String? Address { get; set; }
    }
}