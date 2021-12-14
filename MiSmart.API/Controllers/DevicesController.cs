

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
    public class DevicesController : AuthorizedAPIControllerBase
    {

        private readonly CustomerRepository customerRepository;
        private readonly TeamRepository teamRepository;
        private readonly DeviceRepository deviceRepository;
        private readonly DeviceModelRepository deviceModelRepository;
        public DevicesController(IActionResponseFactory actionResponseFactory, CustomerRepository customerRepository, TeamRepository teamRepository, DeviceRepository deviceRepository, DeviceModelRepository deviceModelRepository) : base(actionResponseFactory)
        {
            this.teamRepository = teamRepository;
            this.deviceModelRepository = deviceModelRepository;
            this.deviceRepository = deviceRepository;
            this.customerRepository = customerRepository;
        }

        [HttpPost]
        public IActionResult Create([FromServices] CustomerUserRepository customerUserRepository, [FromServices] TeamRepository teamRepository, [FromBody] AddingDeviceCommand command)
        {
            ActionResponse response = actionResponseFactory.CreateInstance();
            Int32? customerID = command.CustomerID;
             if (!CurrentUser.IsAdmin || customerID is null)
            {
                customerID = customerUserRepository.HasOwnerPermission(CurrentUser);
            }
            if (customerID is null)
            {
                response.AddInvalidErr("CustomerID");
            }

            var team = teamRepository.Get(ww => ww.ID == command.TeamID.GetValueOrDefault() && ww.CustomerID == customerID.GetValueOrDefault());
            if (team is null)
            {
                response.AddInvalidErr("TeamID");
            }

            if (!deviceModelRepository.Any(ww => ww.ID == command.DeviceModelID))
            {
                response.AddInvalidErr("ModelDeviceID");
            }

            var device = new Device
            {
                Name = command.Name,
                CustomerID = command.CustomerID.Value,
                TeamID = command.TeamID,
                DeviceModelID = command.DeviceModelID.Value,

            };
            deviceRepository.Create(device);
            response.SetCreatedObject(device);


            return response.ToIActionResult();

        }

        [HttpGet]
        public IActionResult GetList([FromServices] CustomerUserRepository customerUserRepository, [FromQuery] PageCommand pageCommand, [FromQuery] Int32? customerID, [FromQuery] String mode = "Small")
        {
            ActionResponse response = actionResponseFactory.CreateInstance();
             if (!CurrentUser.IsAdmin || customerID is null)
            {
                customerID = customerUserRepository.HasMemberPermission(CurrentUser);
            }
            if (customerID is null)
            {
                response.AddNotAllowedErr();
            }
            Expression<Func<Device, Boolean>> query = ww => ww.CustomerID == customerID.GetValueOrDefault();
            if (mode == "Large")
            {

            }
            else if (mode == "Medium")
            {
                var listResponse = deviceRepository.GetListResponseView<MediumDeviceViewModel>(pageCommand, query);
            }
            else
            {
                var listResponse = deviceRepository.GetListResponseView<SmallDeviceViewModel>(pageCommand, query);
                listResponse.SetResponse(response);
            }
            return response.ToIActionResult();
        }
    }
}