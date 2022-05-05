using System;
using System.Collections.Generic;
using MiSmart.DAL.Models;

namespace MiSmart.API.Commands
{


    public class UpdatingFlightStatFromExecutorCommand
    {
        public Int64? TeamID { get; set; }
        public String FieldName { get; set; }
        public String TaskLocation { get; set; }
        public TMUser TMUser { get; set; }
        public List<Medicine> Medicines { get; set; } = new List<Medicine>();
    }
}