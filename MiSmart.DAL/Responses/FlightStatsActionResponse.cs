using System;
using Microsoft.AspNetCore.Mvc;
using MiSmart.Infrastructure.Responses;

namespace MiSmart.DAL.Responses
{
    public class FlightStatsActionResponse : ActionResponse
    {
        public Double TotalFlightDuration { get; set; } = 0;
        public Double TotalTaskArea { get; set; } = 0;
        public Int64 TotalFlights { get; set; } = 0;
        public Double TotalCost { get; set; } = 0;
        public new IActionResult ToIActionResult()
        {
            switch (responseType)
            {
                case ResponseType.Json:
                    {
                        return new ObjectResult(new
                        {
                            TotalItems = TotalItems,
                            Type = Type,
                            Title = Title,
                            Data = Data,
                            Errors = Errors,
                            Message = Message,
                            TotalFlightDuration = TotalFlightDuration,
                            TotalTaskArea = TotalTaskArea,
                            TotalFlights = TotalFlights,
                            TotalCost = TotalCost,
                        })
                        { StatusCode = this.StatusCode };
                    }
                case ResponseType.File:
                    {
                        var result = new FileContentResult(bytes ?? new Byte[0], contentType ?? "application/text");
                        result.FileDownloadName = fileName;
                        return result;
                    }
                default:
                    {
                        return new ObjectResult(new
                        {
                            TotalItems = TotalItems,
                            Type = Type,
                            Title = Title,
                            Data = Data,
                            Errors = Errors,
                            Message = Message,
                            TotalFlightDuration = TotalFlightDuration,
                            TotalTaskArea = TotalTaskArea,
                            TotalFlights = TotalFlights
                        })
                        { StatusCode = this.StatusCode };
                    }
            }
        }
    }

}
