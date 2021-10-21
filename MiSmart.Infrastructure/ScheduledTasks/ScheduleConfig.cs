using System;
namespace MiSmart.Infrastructure.ScheduledTasks
{
    public interface IScheduleConfig<out T> where T : CronJobService
    {
        String CronExpression { get; set; }
        TimeZoneInfo TimeZoneInfo { get; set; }
    }
    public class ScheduleConfig<T> : IScheduleConfig<T> where T : CronJobService
    {
        public String CronExpression { get; set; }
        public TimeZoneInfo TimeZoneInfo { get; set; }
    }
}