using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MiSmart.DAL.DatabaseContexts;
using MiSmart.DAL.Models;
using MiSmart.Infrastructure.Commands;
using MiSmart.Infrastructure.Repositories;
using MiSmart.Infrastructure.Responses;
using MiSmart.Infrastructure.ViewModels;
namespace MiSmart.DAL.Repositories
{
    public class LogFileRepository : RepositoryBase<LogFile>
    {
        public LogFileRepository(DatabaseContext context) : base(context)
        {
        }
        public override async Task<ListResponse<TView>> GetListResponseViewAsync<TView>(PageCommand pageCommand, Expression<Func<LogFile, Boolean>> expression, Func<LogFile, Object> order = null, Boolean ascending = true)
        {
            var count = await context.Set<LogFile>().CountAsync(expression);
            List<TView> data;
            var pageIndex = pageCommand.PageIndex;
            var pageSize = pageCommand.PageSize;
            var originData = context.Set<LogFile>().Where(expression).Select(ww => new LogFile
            {
                CreationTime = ww.CreationTime,
                ID = ww.ID,
                DeviceID = ww.DeviceID,
                Device = ww.Device,
                FileName = ww.FileName,
                LoggingTime = ww.LoggingTime,
                DroneStatus = ww.DroneStatus,
                Status = ww.Status,
                Errors = ww.Errors,
                LogDetail = ww.LogDetail,
                LogReport = ww.LogReport,
                LogReportResult = ww.LogReportResult,
            });
            if (pageIndex.HasValue && pageSize.HasValue)
            {

                if (order is Object)
                {
                    data = ascending ? originData.OrderBy(order).Skip(pageIndex.Value * pageSize.Value).Take(pageSize.Value).ToList().Select(ww => ViewModelHelpers.ConvertToViewModel<LogFile, TView>(ww)).ToList() :
                    originData.OrderByDescending(order).Skip(pageIndex.Value * pageSize.Value).Take(pageSize.Value).ToList().Select(ww => ViewModelHelpers.ConvertToViewModel<LogFile, TView>(ww)).ToList();
                }
                else
                {
                    data = originData.Skip(pageIndex.Value * pageSize.Value).Take(pageSize.Value).ToList().Select(ww => ViewModelHelpers.ConvertToViewModel<LogFile, TView>(ww)).ToList();
                }
            }
            else
            {

                if (order is Object)
                {
                    data = ascending ? originData.OrderBy(order).ToList().Select(ww => ViewModelHelpers.ConvertToViewModel<LogFile, TView>(ww)).ToList() :
                  originData.OrderByDescending(order).ToList().Select(ww => ViewModelHelpers.ConvertToViewModel<LogFile, TView>(ww)).ToList();
                }
                else
                {
                    data = originData.ToList().Select(ww => ViewModelHelpers.ConvertToViewModel<LogFile, TView>(ww)).ToList();
                }
            }
            return new ListResponse<TView> { Data = data, PageIndex = pageIndex, PageSize = pageSize, TotalRecords = count };
        }
    }
}