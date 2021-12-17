

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
    }
}