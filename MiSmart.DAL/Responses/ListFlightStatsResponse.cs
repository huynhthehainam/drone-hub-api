using System;
using MiSmart.Infrastructure.Responses;

namespace MiSmart.DAL.Responses
{
    public class ListFlightStatsResponse<T> : ListResponse<T> where T : class
    {
        public Double TotalFlightDuration { get; set; }
        public Double TotalTaskArea { get; set; }
        public Int64 TotalFlights { get; set; }
        public Double TotalCost { get; set; } = 0;

        public FlightStatsActionResponse SetResponse(FlightStatsActionResponse response)
        {
            response.TotalItems = this.TotalRecords;
            response.Data = Data;
            response.TotalFlightDuration = TotalFlightDuration;
            response.TotalTaskArea = TotalTaskArea;
            response.TotalFlights = TotalFlights;
            response.TotalCost = TotalCost == 0 ? 0.1 : TotalCost;
            return response;
        }
    }
}