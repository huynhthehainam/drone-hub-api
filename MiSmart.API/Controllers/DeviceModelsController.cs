

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
using System.Linq;

namespace MiSmart.API.Controllers
{
    public class DeviceModelsController : AuthorizedAPIControllerBase
    {
        public DeviceModelsController(IActionResponseFactory actionResponseFactory) : base(actionResponseFactory)
        {
        }

        [HttpPost]
        [HasPermission(typeof(AdminPermission))]
        public async Task<IActionResult> Create([FromBody] AddingDeviceModelCommand command,
        [FromServices] DeviceModelRepository deviceModelRepository)
        {
            ActionResponse response = actionResponseFactory.CreateInstance();
            var model = await deviceModelRepository.CreateAsync(new DeviceModel() { Name = command.Name, Type = command.Type });
            response.SetCreatedObject(model);

            return response.ToIActionResult();
        }
        [HttpGet]
        public async Task<IActionResult> GetActionResult([FromQuery] PageCommand pageCommand, [FromServices] MinioService minioService, [FromServices] DeviceModelRepository deviceModelRepository)
        {
            var response = actionResponseFactory.CreateInstance();
            Expression<Func<DeviceModel, Boolean>> query = ww => true;
            var listResponse = await deviceModelRepository.GetListResponseViewAsync<SmallDeviceModelViewModel>(pageCommand, query);

            listResponse.SetResponse(response);


            return response.ToIActionResult();
        }
        [HttpDelete("{id:int}")]
        [HasPermission(typeof(AdminPermission))]
        public async Task<IActionResult> RemoveDeviceModel([FromRoute] Int32 id, [FromServices] DeviceModelRepository deviceModelRepository)
        {
            var response = actionResponseFactory.CreateInstance();
            var deviceModel = await deviceModelRepository.GetAsync(ww => ww.ID == id);
            if (deviceModel is null)
            {
                response.AddNotFoundErr("DeviceModel");
                return response.ToIActionResult();
            }


            await deviceModelRepository.DeleteAsync(deviceModel);
            response.SetNoContent();

            return response.ToIActionResult();
        }
        [HttpPost("{id:int}/UploadImage")]
        [HasPermission(typeof(AdminPermission))]
        public async Task<IActionResult> UpdateImage([FromRoute] Int32 id, [FromServices] DeviceModelRepository deviceModelRepository, [FromServices] MinioService minioService, [FromForm] UpdatingDeviceModelImageCommand command)
        {
            var response = actionResponseFactory.CreateInstance();
            var deviceModel = await deviceModelRepository.GetAsync(ww => ww.ID == id);
            if (deviceModel is null)
            {
                response.AddNotFoundErr("DeviceModel");
                return response.ToIActionResult();
            }
            if (command.File != null)
            {
                if (deviceModel.FileUrl is not null)
                {
                    await minioService.RemoveFileByUrlAsync(deviceModel.FileUrl);
                }

                deviceModel.FileUrl = await minioService.PutFileAsync(command.File, new String[] { "drone-hub-api", "device-model" });
                await deviceModelRepository.UpdateAsync(deviceModel);
                response.SetUpdatedMessage();
            }

            return response.ToIActionResult();
        }
        [HttpPatch("{id:int}")]
        [HasPermission(typeof(AdminPermission))]
        public async Task<IActionResult> UpdateDeviceModel([FromRoute] Int32 id, [FromServices] DeviceModelRepository deviceModelRepository, [FromBody] PatchingDeviceModelCommand command)
        {
            var response = actionResponseFactory.CreateInstance();
            var deviceModel = await deviceModelRepository.GetAsync(ww => ww.ID == id);
            if (deviceModel is null)
            {
                response.AddNotFoundErr("DeviceModel");
                return response.ToIActionResult();
            }

            deviceModel.Name = String.IsNullOrWhiteSpace(command.Name) ? deviceModel.Name : command.Name;

            await deviceModelRepository.UpdateAsync(deviceModel);

            response.SetUpdatedMessage();

            return response.ToIActionResult();
        }

        [HttpPost("{id:int}/ModelParams")]
        [HasPermission(typeof(AdminPermission))]
        public async Task<IActionResult> CreateModelParam([FromRoute] Int32 id, [FromServices] DeviceModelRepository deviceModelRepository, [FromServices] DeviceModelParamRepository deviceModelParamRepository, [FromBody] AddingDeviceModelParamCommand command)
        {
            ActionResponse response = actionResponseFactory.CreateInstance();

            var deviceModel = await deviceModelRepository.GetAsync(ww => ww.ID == id);
            if (deviceModel is null)
            {
                response.AddNotFoundErr("DeviceModel");
                return response.ToIActionResult();
            }

            var activeParams = await deviceModelParamRepository.GetListEntitiesAsync(new PageCommand(), ww => ww.IsActive && ww.DeviceModelID == id);
            foreach (var param in activeParams)
            {
                param.IsActive = false;
                await deviceModelParamRepository.UpdateAsync(param);
            }

            DeviceModelParam deviceModelParam = await deviceModelParamRepository.CreateAsync(new DeviceModelParam()
            {
                CreationTime = DateTime.UtcNow,
                Description = command.Description,
                DeviceModel = deviceModel,
                FuelLevelNumber = command.FuelLevelNumber.GetValueOrDefault(),
                IsActive = true,
                Name = command.Name,
                YMax = command.YMax.GetValueOrDefault(),
                YMin = command.YMin.GetValueOrDefault(),
                FlowRateMaxLimit = command.FlowRateMaxLimit.GetValueOrDefault(),
                FlowRateMiddleLimit = command.FlowRateMiddleLimit.GetValueOrDefault(),
                FlowRateMinLimit = command.FlowRateMinLimit.GetValueOrDefault(),
                YCentrifugal4Min = command.YCentrifugal4Min.GetValueOrDefault(),
                YCentrifugal4Max = command.YCentrifugal4Max.GetValueOrDefault(),
                YCentrifugalMax = command.YCentrifugalMax.GetValueOrDefault(),
                YCentrifugalMin = command.YCentrifugalMin.GetValueOrDefault(),
                Details = command.Details.Select(ww => new DeviceModelParamDetail()
                {
                    A = ww.A.GetValueOrDefault(),
                    B = ww.B.GetValueOrDefault(),
                    XMax = ww.XMax.GetValueOrDefault(),
                    XMin = ww.XMin.GetValueOrDefault(),
                }).ToArray(),
                CentrifugalDetails = command.CentrifugalDetails.Select(ww => new DeviceModelParamCentrifugalDetail()
                {
                    A = ww.A.GetValueOrDefault(),
                    B = ww.B.GetValueOrDefault(),
                    XMax = ww.XMax.GetValueOrDefault(),
                    XMin = ww.XMin.GetValueOrDefault(),
                }).ToArray(),
                Centrifugal4Details = command.Centrifugal4Details.Select(ww => new DeviceModelParamCentrifugal4Detail()
                {
                    A = ww.A.GetValueOrDefault(),
                    B = ww.B.GetValueOrDefault(),
                    XMax = ww.XMax.GetValueOrDefault(),
                    XMin = ww.XMin.GetValueOrDefault(),
                }).ToArray(),
            });

            response.SetCreatedObject(deviceModelParam);



            return response.ToIActionResult();
        }
        [HttpGet("{id:int}/ModelParams")]
        [HasPermission(typeof(AdminPermission))]
        public async Task<IActionResult> GetListModelParams([FromRoute] Int32 id, [FromServices] DeviceModelRepository deviceModelRepository,
        [FromServices] DeviceModelParamRepository deviceModelParamRepository, [FromQuery] PageCommand pageCommand)
        {
            ActionResponse response = actionResponseFactory.CreateInstance();

            var deviceModel = await deviceModelRepository.GetAsync(ww => ww.ID == id);
            if (deviceModel is null)
            {
                response.AddNotFoundErr("DeviceModel");
                return response.ToIActionResult();
            }

            Expression<Func<DeviceModelParam, Boolean>> query = ww => (ww.DeviceModelID == deviceModel.ID);
            var listResponse = await deviceModelParamRepository.GetListResponseViewAsync<DeviceModelParamViewModel>(pageCommand, query, ww => ww.CreationTime, false);
            listResponse.SetResponse(response);

            return response.ToIActionResult();
        }
        [HttpPatch("{id:int}/DeviceModelParams/{modelParamId:int}/ActiveModelParam")]
        [HasPermission(typeof(AdminPermission))]
        public async Task<IActionResult> ActiveModelParam([FromRoute] Int32 id, [FromRoute] Int32 modelParamId, [FromServices] DeviceModelRepository deviceModelRepository,
        [FromServices] DeviceModelParamRepository deviceModelParamRepository)
        {
            ActionResponse response = actionResponseFactory.CreateInstance();
            var deviceModel = await deviceModelRepository.GetAsync(ww => ww.ID == id);
            if (deviceModel is null)
            {
                response.AddNotFoundErr("DeviceModel");
                return response.ToIActionResult();
            }

            var deviceModelParam = await deviceModelParamRepository.GetAsync(ww => ww.ID == modelParamId);
            if (deviceModelParam is null)
            {
                response.AddNotFoundErr("DeviceModelParam");
                return response.ToIActionResult();
            }
            var activeParams = await deviceModelParamRepository.GetListEntitiesAsync(new PageCommand(), ww => ww.IsActive && ww.DeviceModelID == id);
            foreach (var param in activeParams)
            {
                param.IsActive = false;
                await deviceModelParamRepository.UpdateAsync(param);
            }

            deviceModelParam.IsActive = true;
            await deviceModelParamRepository.UpdateAsync(deviceModelParam);

            return response.ToIActionResult();
        }
    }
}