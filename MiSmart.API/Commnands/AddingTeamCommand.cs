using System;
using System.ComponentModel.DataAnnotations;

namespace MiSmart.API.Commands
{
    public class AddingTeamCommand
    {
        [Required(AllowEmptyStrings = false)]
        public String Name { get; set; }

        public Int32? CustomerID { get; set; }
    }
}