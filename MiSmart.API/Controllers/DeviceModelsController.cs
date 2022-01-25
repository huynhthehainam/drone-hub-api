

using MiSmart.Infrastructure.Controllers;
using MiSmart.Infrastructure.Responses;
using MiSmart.API.Commands;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System;
using MiSmart.Infrastructure.Helpers;
using MiSmart.DAL.Models;
using MiSmart.DAL.Repositories;
using System.Linq;
using MiSmart.Infrastructure.Commands;
using MiSmart.DAL.ViewModels;
using System.Linq.Expressions;
using MiSmart.Infrastructure.Permissions;
using MiSmart.API.Permissions;

namespace MiSmart.API.Controllers
{
    public class DeviceModelsController : AuthorizedAPIControllerBase
    {
        private readonly DeviceModelRepository deviceModelRepository;
        public DeviceModelsController(IActionResponseFactory actionResponseFactory, DeviceModelRepository deviceModelRepository) : base(actionResponseFactory)
        {
            this.deviceModelRepository = deviceModelRepository;
        }

        [HttpPost]
        [HasPermission(typeof(AdminPermission))]
        public IActionResult Create([FromBody] AddingDeviceModelCommand command)
        {
            ActionResponse response = actionResponseFactory.CreateInstance();
            var model = new DeviceModel() { Name = command.Name };
            deviceModelRepository.Create(model);
            response.SetCreatedObject(model);

            return response.ToIActionResult();
        }
        [HttpGet]
        public IActionResult GetActionResult([FromQuery] PageCommand pageCommand)
        {
            var response = actionResponseFactory.CreateInstance();
            Expression<Func<DeviceModel, Boolean>> query = ww => true;
            var listResponse = deviceModelRepository.GetListResponseView<SmallDeviceModelVieModel>(pageCommand, query);
            listResponse.SetResponse(response);

            return response.ToIActionResult();
        }
        [HttpDelete("{id:int}")]
        [HasPermission(typeof(AdminPermission))]
        public IActionResult RemoveDeviceModel([FromRoute] Int32 id)
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
        [HttpPatch("{id:int}")]
        [HasPermission(typeof(AdminPermission))]
        public IActionResult UpdateDeviceModel([FromRoute] Int32 id, [FromBody] PatchingDeviceModelCommand command)
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