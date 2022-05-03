using System;

namespace MiSmart.API.Commands
{
    public class UpdatingFlightStatFromExecutorCommand
    {
        public Int64? TeamID { get; set; }
        public String FieldName { get; set; }
        public String TaskLocation { get; set; }
        public String TMUserUID { get; set; }
    }
}