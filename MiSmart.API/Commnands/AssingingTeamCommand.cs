




using System;
using System.ComponentModel.DataAnnotations;

namespace MiSmart.API.Commands
{
    public class AssigningDeviceExecutionCompanyCommand
    {
        [Required]
        public Int32? ExecutionCompanyID { get; set; }
    }
    public class AssigningDeviceTeamCommand
    {
        public Int32? TeamID { get; set; }
    }
}