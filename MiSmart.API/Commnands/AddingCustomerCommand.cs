
using System;
using System.ComponentModel.DataAnnotations;

namespace MiSmart.API.Commands
{
    public class AddingCustomerCommand
    {
        [Required]
        public String Name { get; set; }

        public String Address { get; set; }
    }
}