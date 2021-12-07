

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
        public IActionResult Create([FromBody] AddingDeviceCommand command)
        {
            ActionResponse response = actionResponseFactory.CreateInstance();
            var validated = true;

            var customer = customerRepository.Get(ww => ww.ID == command.CustomerID.Value);
            if (customer is not null)
            {
                if (command.TeamID.HasValue)
                {
                    var team = customer.Teams.FirstOrDefault(ww => ww.ID == command.TeamID.Value);
                    if (team is null)
                    {
                        validated = false;
                        response.AddInvalidErr("TeamID");
                    }
                }
            }
            else
            {
                validated = false;
                response.AddInvalidErr("CustomerID");
            }

            if (!deviceModelRepository.Any(ww => ww.ID == command.DeviceModelID))
            {
                validated = false;
                response.AddInvalidErr("ModelDeviceID");
            }
            if (validated)
            {
                var device = new Device
                {
                    Name = command.Name,
                    CustomerID = command.CustomerID.Value,
                    TeamID = command.TeamID,
                    DeviceModelID = command.DeviceModelID.Value,

                };
                deviceRepository.Create(device);
                response.SetCreatedObject(device);
            }

            return response.ToIActionResult();

        }

        [HttpGet]
        public IActionResult GetList([FromQuery] PageCommand pageCommand, [FromQuery] String mode = "Small")
        {
            ActionResponse response = actionResponseFactory.CreateInstance();
            Expression<Func<Device, Boolean>> query = ww => true;
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