using System;
using System.ComponentModel.DataAnnotations;

namespace MiSmart.API.Commands
{
    public class AddingTeamCommand
    {
        [Required(AllowEmptyStrings = false)]
        public String Name { get; set; }
    }
    public class UpdatingTeamCommand
    {
        public String Name { get; set; }
    }
}