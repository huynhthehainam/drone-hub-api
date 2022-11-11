

using MiSmart.Infrastructure.Controllers;
using MiSmart.Infrastructure.Responses;
using MiSmart.API.Commands;
using Microsoft.AspNetCore.Mvc;
using System;
using MiSmart.DAL.Models;
using MiSmart.DAL.Repositories;
using MiSmart.Infrastructure.Commands;
using MiSmart.DAL.ViewModels;
using System.Linq.Expressions;
using MiSmart.Infrastructure.Permissions;
using MiSmart.API.Permissions;
using MiSmart.Infrastructure.Minio;
using System.Threading.Tasks;

namespace MiSmart.API.Controllers
{
    public class BatteryModelsController : AuthorizedAPIControllerBase
    {
        public BatteryModelsController(IActionResponseFactory actionResponseFactory) : base(actionResponseFactory)
        {
        }

        [HttpPost]
        [HasPermission(typeof(AdminPermission))]
        public async Task<IActionResult> Create([FromBody] AddingBatteryModelCommand command,
        [FromServices] BatteryModelRepository batteryModelRepository)
        {
            ActionResponse response = actionResponseFactory.CreateInstance();
            Console.WriteLine("go to permission");
            var model = await batteryModelRepository.CreateAsync(new BatteryModel() { Name = command.Name, ManufacturerName = command.ManufacturerName });
            response.SetCreatedObject(model);
            return response.ToIActionResult();
        }
        [HttpGet]
        public async Task<IActionResult> GetList([FromQuery] PageCommand pageCommand, [FromServices] BatteryModelRepository batteryModelRepository)
        {
            var response = actionResponseFactory.CreateInstance();
            Expression<Func<BatteryModel, Boolean>> query = ww => true;
            var listResponse = await batteryModelRepository.GetListResponseViewAsync<SmallBatteryModelViewModel>(pageCommand, query);

            listResponse.SetResponse(response);


            return response.ToIActionResult();
        }
        [HttpDelete("{id:int}")]
        [HasPermission(typeof(AdminPermission))]
        public async Task<IActionResult> RemoveBatteryModel([FromRoute] Int32 id, [FromServices] BatteryModelRepository batteryModelRepository)
        {
            var response = actionResponseFactory.CreateInstance();
            var batteryModel = await batteryModelRepository.GetAsync(ww => ww.ID == id);
            if (batteryModel is null)
            {
                response.AddNotFoundErr("BatteryModel");
                return response.ToIActionResult();
            }


            await batteryModelRepository.DeleteAsync(batteryModel);
            response.SetNoContent();
            return response.ToIActionResult();
        }
        [HttpPost("{id:int}/UploadImage")]
        [HasPermission(typeof(AdminPermission))]
        public async Task<IActionResult> UpdateImage([FromRoute] Int32 id, [FromServices] BatteryModelRepository batteryModelRepository, [FromServices] MinioService minioService, [FromForm] UpdatingBatteryModelImageCommand command)
        {
            var response = actionResponseFactory.CreateInstance();
            var batteryModel = await batteryModelRepository.GetAsync(ww => ww.ID == id);
            if (batteryModel is null)
            {
                response.AddNotFoundErr("BatteryModel");
                return response.ToIActionResult();
            }

            if (batteryModel.FileUrl is not null)
            {
                await minioService.RemoveFileByUrlAsync(batteryModel.FileUrl);
            }
            if (command.File != null)
            {
                batteryModel.FileUrl = await minioService.PutFileAsync(command.File, new String[] { "drone-hub-api", "battery-model" });
                await batteryModelRepository.UpdateAsync(batteryModel);
                response.SetUpdatedMessage();
            }


            return response.ToIActionResult();
        }
        [HttpPatch("{id:int}")]
        [HasPermission(typeof(AdminPermission))]
        public async Task<IActionResult> Update([FromRoute] Int32 id, [FromServices] BatteryModelRepository batteryModelRepository, [FromBody] PatchingBatterModelCommand command)
        {
            var response = actionResponseFactory.CreateInstance();
            var batteryModel = await batteryModelRepository.GetAsync(ww => ww.ID == id);
            if (batteryModel is null)
            {
                response.AddNotFoundErr("BatteryModel");
                return response.ToIActionResult();
            }

            batteryModel.Name = String.IsNullOrWhiteSpace(command.Name) ? batteryModel.Name : command.Name;
            batteryModel.ManufacturerName = String.IsNullOrWhiteSpace(command.ManufacturerName) ? batteryModel.ManufacturerName : command.ManufacturerName;

            await batteryModelRepository.UpdateAsync(batteryModel);

            response.SetUpdatedMessage();
            return response.ToIActionResult();
        }
    }
}