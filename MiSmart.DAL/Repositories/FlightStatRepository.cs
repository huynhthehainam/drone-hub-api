


using Microsoft.EntityFrameworkCore;
using System;
using MiSmart.DAL.DatabaseContexts;
using MiSmart.DAL.Models;
using MiSmart.Infrastructure.Repositories;
using System.Linq;
using MiSmart.DAL.Responses;
using Microsoft.AspNetCore.SignalR;
using MiSmart.Infrastructure.Commands;
using System.Linq.Expressions;
using MiSmart.Infrastructure.ViewModels;
using System.Collections.Generic;

namespace MiSmart.DAL.Repositories
{
    public class FlightStatRepository : RepositoryBase<FlightStat>
    {
        public FlightStatRepository(DatabaseContext context) : base(context)
        {
        }
        public ListFlightStatsResponse<TView> GetListFlightStatsView<TView>(PageCommand pageCommand, Expression<Func<FlightStat, Boolean>> expression, Func<FlightStat, Object> order = null, Boolean ascending = true) where TView : class, IViewModel<FlightStat>, new()
        {

            var count = context.Set<FlightStat>().Count(expression);
            List<TView> data;
            var pageIndex = pageCommand.PageIndex;
            var pageSize = pageCommand.PageSize;
            Double totalFlightDuration = 0;
            Double totalTaskArea = 0;
            Int64 totalFlights = 0;
            var originData = context.Set<FlightStat>().Where(expression);
            totalFlightDuration = originData.Sum(ww => ww.FlightDuration);
            totalTaskArea = originData.Sum(ww => ww.TaskArea);
            totalFlights = originData.Sum(ww => ww.Flights);

            if (pageIndex.HasValue && pageSize.HasValue)
            {
                if (order is Object)
                {
                    data = ascending ? originData.OrderBy(order).Skip(pageIndex.Value * pageSize.Value).Take(pageSize.Value).ToList().Select(ww => ViewModelHelpers.ConvertToViewModel<FlightStat, TView>(ww)).ToList() :
                    originData.OrderByDescending(order).Skip(pageIndex.Value * pageSize.Value).Take(pageSize.Value).ToList().Select(ww => ViewModelHelpers.ConvertToViewModel<FlightStat, TView>(ww)).ToList();
                }
                else
                {
                    data = originData.Skip(pageIndex.Value * pageSize.Value).Take(pageSize.Value).ToList().Select(ww => ViewModelHelpers.ConvertToViewModel<FlightStat, TView>(ww)).ToList();
                }
            }
            else
            {

                if (order is Object)
                {
                    data = ascending ? originData.OrderBy(order).ToList().Select(ww => ViewModelHelpers.ConvertToViewModel<FlightStat, TView>(ww)).ToList() :
                  originData.OrderByDescending(order).ToList().Select(ww => ViewModelHelpers.ConvertToViewModel<FlightStat, TView>(ww)).ToList();
                }
                else
                {
                    data = originData.ToList().Select(ww => ViewModelHelpers.ConvertToViewModel<FlightStat, TView>(ww)).ToList();
                }
            }
            return new ListFlightStatsResponse<TView> { Data = data, PageIndex = pageIndex, PageSize = pageSize, TotalRecords = count, TotalFlightDuration = totalFlightDuration, TotalFlights = totalFlights, TotalTaskArea = totalTaskArea };
        }
    }
}
