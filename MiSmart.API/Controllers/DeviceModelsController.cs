

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
    public class DeviceModelsController : AuthorizedAPIControllerBase
    {
        public DeviceModelsController(IActionResponseFactory actionResponseFactory) : base(actionResponseFactory)
        {
        }

        [HttpPost]
        [HasPermission(typeof(AdminPermission))]
        public IActionResult Create([FromBody] AddingDeviceModelCommand command,
        [FromServices] DeviceModelRepository deviceModelRepository)
        {
            ActionResponse response = actionResponseFactory.CreateInstance();
            var model = new DeviceModel() { Name = command.Name };
            deviceModelRepository.Create(model);
            response.SetCreatedObject(model);

            return response.ToIActionResult();
        }
        [HttpGet]
        public IActionResult GetActionResult([FromQuery] PageCommand pageCommand, [FromServices] MinioService minioService, [FromServices] DeviceModelRepository deviceModelRepository)
        {
            var response = actionResponseFactory.CreateInstance();
            Expression<Func<DeviceModel, Boolean>> query = ww => true;
            var listResponse = deviceModelRepository.GetListResponseView<SmallDeviceModelVieModel>(pageCommand, query);

            listResponse.SetResponse(response);


            return response.ToIActionResult();
        }
        [HttpDelete("{id:int}")]
        [HasPermission(typeof(AdminPermission))]
        public IActionResult RemoveDeviceModel([FromRoute] Int32 id, [FromServices] DeviceModelRepository deviceModelRepository)
        {
            var response = actionResponseFactory.CreateInstance();
            var deviceModel = deviceModelRepository.Get(ww => ww.ID == id);
            if (deviceModel is null)
            {
                response.AddNotFoundErr("DeviceModel");
            }


            deviceModelRepository.Delete(deviceModel);
            response.SetNoContent();

            return response.ToIActionResult();
        }
        [HttpPost("{id:int}/UploadImage")]
        [HasPermission(typeof(AdminPermission))]
        public IActionResult UpdateImage([FromRoute] Int32 id, [FromServices] DeviceModelRepository deviceModelRepository, [FromServices] MinioService minioService, [FromForm] UpdatingDeviceModelImageCommand command)
        {
            var response = actionResponseFactory.CreateInstance();
            var deviceModel = deviceModelRepository.Get(ww => ww.ID == id);
            if (deviceModel is null)
            {
                response.AddNotFoundErr("DeviceModel");
            }

            if (deviceModel.FileUrl is not null)
            {
                minioService.RemoveFileByUrl(deviceModel.FileUrl);
            }

            deviceModel.FileUrl = minioService.PutFile(command.File, new String[] { "drone-hub-api", "device-model" });
            deviceModelRepository.Update(deviceModel);
            response.SetUpdatedMessage();

            return response.ToIActionResult();
        }
        [HttpPatch("{id:int}")]
        [HasPermission(typeof(AdminPermission))]
        public IActionResult UpdateDeviceModel([FromRoute] Int32 id, [FromServices] DeviceModelRepository deviceModelRepository, [FromBody] PatchingDeviceModelCommand command)
        {
            var response = actionResponseFactory.CreateInstance();
            var deviceModel = deviceModelRepository.Get(ww => ww.ID == id);
            if (deviceModel is null)
            {
                response.AddNotFoundErr("DeviceModel");
            }

            deviceModel.Name = String.IsNullOrWhiteSpace(command.Name) ? deviceModel.Name : command.Name;

            deviceModelRepository.Update(deviceModel);


            return response.ToIActionResult();
        }
    }
}