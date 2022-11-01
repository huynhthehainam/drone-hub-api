


using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MiSmart.Infrastructure.Data;
namespace MiSmart.DAL.Models;

public class FlightStatReportRecord : EntityBase<Guid>
{
    public FlightStatReportRecord()
    {
    }
    public FlightStatReportRecord(ILazyLoader lazyLoader) : base(lazyLoader)
    {
    }

    public String? Reason { get; set; }
    public List<String>? Images { get; set; }

    private FlightStat? flightStat;
    public FlightStat? FlightStat
    {
        get => lazyLoader.Load(this, ref flightStat);
        set => flightStat = value;
    }
    public Guid FlightStatID { get; set; }
}