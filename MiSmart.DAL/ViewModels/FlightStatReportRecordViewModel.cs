

using MiSmart.DAL.Models;
using MiSmart.Infrastructure.ViewModels;
using System;
using System.Collections.Generic;

namespace MiSmart.DAL.ViewModels;

public class FlightStatReportRecordViewModel : IViewModel<FlightStatReportRecord>
{
    public Guid ID { get; set; }
    public String? Reason { get; set; }
    public List<String>? Images { get; set; }
    public void LoadFrom(FlightStatReportRecord entity)
    {
        ID = entity.ID;
        Reason = entity.Reason;
        Images = entity.Images;
    }
}