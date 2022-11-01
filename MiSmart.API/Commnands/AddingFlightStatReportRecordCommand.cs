using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace MiSmart.API.Commands;
public class AddingFlightStatReportRecordCommand
{
    public String? Reason { get; set; }
    public List<IFormFile> Files { get; set; } = new List<IFormFile> { };
}