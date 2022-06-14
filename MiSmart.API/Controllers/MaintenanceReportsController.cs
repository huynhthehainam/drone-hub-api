


using MiSmart.Infrastructure.Controllers;
using MiSmart.Infrastructure.Responses;
using MiSmart.API.Commands;
using Microsoft.AspNetCore.Mvc;
using System;
using MiSmart.DAL.Repositories;
using MiSmart.Infrastructure.Permissions;
using MiSmart.API.Permissions;
using MiSmart.Infrastructure.Minio;
using System.Threading.Tasks;

namespace MiSmart.API.Controllers
{
    public class MaintenanceReportsController : AuthorizedAPIControllerBase
    {
        public MaintenanceReportsController(IActionResponseFactory actionResponseFactory) : base(actionResponseFactory)
        {
        }

        [HttpPost("{id:int}/UploadFile")]
        [HasPermission(typeof(MaintainerPermission))]
        public async Task<IActionResult> UpdateFile([FromRoute] Int32 id, [FromServices] MaintenanceReportRepository maintenanceReportRepository, [FromServices] MinioService minioService, [FromForm] AddingMaintenanceReportAttachmentCommand command)
        {
            ActionResponse actionResponse = actionResponseFactory.CreateInstance();
            var report = await maintenanceReportRepository.GetAsync(ww => ww.ID == id && ww.UserUUID == CurrentUser.UUID);
            if (report is null)
            {
                actionResponse.AddNotFoundErr("Report");
            }

            var fileLink = await minioService.PutFileAsync(command.File, new String[] { "drone-hub-api", "maintenance-report", $"{report.UUID}" });

            report.AttachmentLinks.Add(fileLink);

            await maintenanceReportRepository.UpdateAsync(report);

            actionResponse.SetUpdatedMessage();
            return actionResponse.ToIActionResult();
        }
    }
}