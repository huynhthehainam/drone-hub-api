
using System;
using System.ComponentModel.DataAnnotations;

namespace MiSmart.API.Commands
{
    public class AddingPartCommand
    {
        [Required]
        public String? Group {get; set; }
        [Required]
        public String? Name {get; set; }
    }
}