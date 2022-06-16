
using System;
using System.ComponentModel.DataAnnotations;

namespace MiSmart.API.Commands
{
    public class AddingBatteryCommand
    {
        [Required]
        public Int32? BatteryModelID { get; set; }
        [Required]
        public String ActualID { get; set; }
        public Int32? ExecutionCompanyID { get; set; }
        public String Name { get; set; }
    }
    public class PatchingBatteryCommand
    {
        public String Name { get; set; }
        public Int32? BatteryModelID { get; set; }
        public Int32? ExecutionCompanyID { get; set; }
    }
    public class AddingBatteryModelCommand
    {
        [Required]
        public String Name { get; set; }
        [Required]
        public String ManufacturerName { get; set; }
    }
}