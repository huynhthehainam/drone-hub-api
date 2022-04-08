


using System;
using System.ComponentModel.DataAnnotations;

namespace MiSmart.API.Commands
{
    public class AddingExecutionCompanySettingCommand
    {
        [Required]
        public Double? MainPilotCostPerHectare { get; set; }
        [Required]
        public Double? SubPilotCostPerHectare { get; set; }
        [Required]
        public Double? CostPerHectare { get; set; }
    }
}