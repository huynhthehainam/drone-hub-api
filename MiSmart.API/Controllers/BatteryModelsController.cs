

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

namespace MiSmart.API.Controllers
{
    public class BatteryModelsController : AuthorizedAPIControllerBase
    {
        public BatteryModelsController(IActionResponseFactory actionResponseFactory) : base(actionResponseFactory)
        {
        }

        [HttpPost]
        [HasPermission(typeof(AdminPermission))]
        public IActionResult Create([FromBody] AddingBatteryModelCommand command,
        [FromServices] BatteryModelRepository batteryModelRepository)
        {
            ActionResponse response = actionResponseFactory.CreateInstance();
            var model = new BatteryModel() { Name = command.Name, ManufacturerName = command.ManufacturerName };
            batteryModelRepository.Create(model);
            response.SetCreatedObject(model);

            return response.ToIActionResult();
        }
        [HttpGet]
        public IActionResult GetList([FromQuery] PageCommand pageCommand, [FromServices] BatteryModelRepository batteryModelRepository)
        {
            var response = actionResponseFactory.CreateInstance();
            Expression<Func<BatteryModel, Boolean>> query = ww => true;
            var listResponse = batteryModelRepository.GetListResponseView<SmallBatteryModelViewModel>(pageCommand, query);

            listResponse.SetResponse(response);


            return response.ToIActionResult();
        }
        [HttpDelete("{id:int}")]
        [HasPermission(typeof(AdminPermission))]
        public IActionResult RemoveBatteryModel([FromRoute] Int32 id, [FromServices] BatteryModelRepository batteryModelRepository)
        {
            var response = actionResponseFactory.CreateInstance();
            var batteryModel = batteryModelRepository.Get(ww => ww.ID == id);
            if (batteryModel is null)
            {
                response.AddNotFoundErr("BatteryModel");
            }


            batteryModelRepository.Delete(batteryModel);
            response.SetNoContent();

            return response.ToIActionResult();
        }
        [HttpPost("{id:int}/UploadImage")]
        [HasPermission(typeof(AdminPermission))]
        public IActionResult UpdateImage([FromRoute] Int32 id, [FromServices] BatteryModelRepository batteryModelRepository, [FromServices] MinioService minioService, [FromForm] UpdatingBatteryModelImageCommand command)
        {
            var response = actionResponseFactory.CreateInstance();
            var batteryModel = batteryModelRepository.Get(ww => ww.ID == id);
            if (batteryModel is null)
            {
                response.AddNotFoundErr("BatteryModel");
            }

            if (batteryModel.FileUrl is not null)
            {
                minioService.RemoveFileByUrl(batteryModel.FileUrl);
            }

            batteryModel.FileUrl = minioService.PutFile(command.File, new String[] { "drone-hub-api", "battery-model" });
            batteryModelRepository.Update(batteryModel);
            response.SetUpdatedMessage();

            return response.ToIActionResult();
        }
        [HttpPatch("{id:int}")]
        [HasPermission(typeof(AdminPermission))]
        public IActionResult Update([FromRoute] Int32 id, [FromServices] BatteryModelRepository batteryModelRepository, [FromBody] PatchingBatterModelCommand command)
        {
            var response = actionResponseFactory.CreateInstance();
            var batteryModel = batteryModelRepository.Get(ww => ww.ID == id);
            if (batteryModel is null)
            {
                response.AddNotFoundErr("BatteryModel");
            }

            batteryModel.Name = String.IsNullOrWhiteSpace(command.Name) ? batteryModel.Name : command.Name;
            batteryModel.ManufacturerName = String.IsNullOrWhiteSpace(command.ManufacturerName) ? batteryModel.ManufacturerName : command.ManufacturerName;

            batteryModelRepository.Update(batteryModel);


            return response.ToIActionResult();
        }
    }
}