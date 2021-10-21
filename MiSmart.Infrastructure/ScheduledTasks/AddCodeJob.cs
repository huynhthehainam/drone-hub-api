using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
namespace MiSmart.Infrastructure.ScheduledTasks
{
    // public class AddCodeJob : CronJobService
    // {
    //     private IServiceProvider serviceProvider;
    //     public AddCodeJob(IScheduleConfig<AddCodeJob> options, IServiceProvider serviceProvider) : base(options)
    //     {
    //         this.serviceProvider = serviceProvider;
    //     }
    //     public override Task DoWork(CancellationToken cancellationToken)
    //     {
    //         using (var scope = serviceProvider.CreateScope())
    //         {
    //             var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
    //             var users = context.Users.ToList();
    //             foreach (var user in users)
    //             {
    //                 Console.WriteLine(user.Email);
    //             }
    //         }
    //         return Task.CompletedTask;
    //     }
    // }
}