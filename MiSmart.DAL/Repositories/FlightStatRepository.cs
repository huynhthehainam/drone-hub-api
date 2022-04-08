using System;
using MiSmart.DAL.DatabaseContexts;
using MiSmart.DAL.Models;
using MiSmart.Infrastructure.Repositories;
using System.Linq;
using MiSmart.DAL.Responses;
using MiSmart.Infrastructure.Commands;
using System.Linq.Expressions;
using MiSmart.Infrastructure.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MiSmart.DAL.Repositories
{
    public class FlightStatRepository : RepositoryBase<FlightStat>
    {
        public FlightStatRepository(DatabaseContext context) : base(context)
        {
        }
        public async Task<ListFlightStatsResponse<TView>> GetListFlightStatsViewAsync<TView>(PageCommand pageCommand, Expression<Func<FlightStat, Boolean>> expression, Func<FlightStat, Object> order = null, Boolean ascending = true) where TView : class, IViewModel<FlightStat>, new()
        {

            var taskCount = context.Set<FlightStat>().CountAsync(expression);
            List<TView> data;
            var pageIndex = pageCommand.PageIndex;
            var pageSize = pageCommand.PageSize;
            Double totalFlightDuration = 0;
            Double totalTaskArea = 0;
            Int64 totalFlights = 0;
            Double totalCost = 0;
            var originData = context.Set<FlightStat>().Where(expression);
            var taskTotalFlightDuration = originData.SumAsync(ww => ww.FlightDuration);
            var taskTotalTaskArea = originData.SumAsync(ww => ww.TaskArea);
            var taskTotalFlights = originData.SumAsync(ww => ww.Flights);
            var taskTotalCost = originData.SumAsync(ww => ww.Cost);

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
            var count = await taskCount;
            totalFlightDuration = await taskTotalFlightDuration;
            totalTaskArea = await taskTotalTaskArea;
            totalFlights = await taskTotalFlights;
            totalCost = await taskTotalCost;
            return new ListFlightStatsResponse<TView>
            {
                Data = data,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalRecords = count,
                TotalFlightDuration = totalFlightDuration,
                TotalFlights = totalFlights,
                TotalTaskArea = totalTaskArea,
                TotalCost = totalCost,
            };
        }
    }
}
