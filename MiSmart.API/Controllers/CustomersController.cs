

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
using System.Linq.Expressions;
using MiSmart.DAL.ViewModels;
using MiSmart.Infrastructure.Permissions;
using MiSmart.API.Permissions;
using MiSmart.DAL.Responses;
using MiSmart.Infrastructure.ViewModels;

namespace MiSmart.API.Controllers
{
    public class CustomersController : AuthorizedAPIControllerBase
    {

        private readonly CustomerRepository customerRepository;
        public CustomersController(IActionResponseFactory actionResponseFactory, CustomerRepository customerRepository) : base(actionResponseFactory)
        {
            this.customerRepository = customerRepository;
        }
        [HttpPost]
        [HasPermission(typeof(AdminPermission))]
        public IActionResult Create([FromBody] AddingCustomerCommand command)
        {
            var response = actionResponseFactory.CreateInstance();
            var validated = true;
            if (validated)
            {
                var customer = new Customer { Name = command.Name, Address = command.Address };
                customerRepository.Create(customer);
                response.SetCreatedObject(customer);
            }
            return response.ToIActionResult();
        }
        [HttpGet]
        public IActionResult GetList([FromQuery] PageCommand pageCommand, [FromQuery] String search, [FromQuery] String mode = "Small")
        {
            var response = actionResponseFactory.CreateInstance();
            Expression<Func<Customer, Boolean>> query = ww => (!String.IsNullOrWhiteSpace(search) ? (ww.Name.ToLower().Contains(search.ToLower()) || ww.Address.ToLower().Contains(search.ToLower())) : true);
            if (mode == "Large")
            {
                // var listResponse = customerRepository.GetListResponseView<SmallCustomerViewModel>(pageCommand, query);
                // listResponse.SetResponse(response);
            }
            else
            {
                var listResponse = customerRepository.GetListResponseView<SmallCustomerViewModel>(pageCommand, query);
                listResponse.SetResponse(response);
            }

            return response.ToIActionResult();
        }

        [HttpPost("{id:int}/AssignUser")]
        public IActionResult AssignCustomerUser([FromServices] CustomerUserRepository customerUserRepository, [FromRoute] Int32 id, [FromBody] AssigningCustomerUserCommand command)
        {
            var response = actionResponseFactory.CreateInstance();

            var validated = true;
            if (!customerRepository.Any(ww => ww.ID == id))
            {
                validated = false;
                response.AddNotFoundErr("Company");
            }
            if (!customerRepository.HasOwnerPermission(id, CurrentUser))
            {
                validated = false;
                response.AddNotAllowedErr();
            }
            else
            {
                if (customerUserRepository.Any(ww => ww.CustomerID == id && ww.UserID == command.UserID))
                {
                    validated = false;
                    response.AddExistedErr("UserID");
                }
            }
            if (validated)
            {
                CustomerUser customerUser = new CustomerUser { CustomerID = id, UserID = command.UserID.Value, Type = command.Type };
                customerUserRepository.Create(customerUser);
                response.SetCreatedObject(customerUser);
            }


            return response.ToIActionResult();
        }

        [HttpPost("{id:int}/Teams/{teamID:long}/AssignUser")]
        public IActionResult AssignTeamUser([FromServices] TeamRepository teamRepository, [FromServices] TeamUserRepository teamUserRepository, [FromServices] CustomerUserRepository customerUserRepository, [FromRoute] Int32 id, [FromRoute] Int64 teamID, [FromBody] AssigningTeamUserCommand command)
        {
            var response = actionResponseFactory.CreateInstance();
            var validated = true;
            if (!customerRepository.HasOwnerPermission(id, CurrentUser))
            {
                validated = false;
                response.AddNotAllowedErr();
            }
            else
            {
                if (customerUserRepository.Any(ww => ww.CustomerID != id && ww.UserID == command.UserID))
                {
                    validated = false;
                    response.AddExistedErr("UserID");
                }
                var team = teamRepository.Get(ww => ww.ID == teamID && ww.CustomerID == id);
                if (team is not null)
                {
                    if (team.TeamUsers.Any(ww => ww.UserID == command.UserID))
                    {
                        validated = false;
                        response.AddExistedErr("UserID");
                    }
                }
                else
                {
                    validated = false;
                    response.AddInvalidErr("TeamID");
                }
            }

            if (validated)
            {

                var teamUser = new TeamUser { UserID = command.UserID.Value, TeamID = teamID, Type = command.Type };
                teamUserRepository.Create(teamUser);

                if (!customerUserRepository.Any(ww => ww.CustomerID == id && ww.UserID == command.UserID))
                {
                    var customerUser = new CustomerUser { CustomerID = id, UserID = command.UserID.Value, Type = CustomerMemberType.Member };
                    customerUserRepository.Create(customerUser);
                }
                response.SetCreatedObject(teamUser);
            }


            return response.ToIActionResult();
        }
        [HttpGet("{id:int}/FlightStats")]
        public IActionResult GetFlightStats([FromServices] FlightStatRepository flightStatRepository, [FromRoute] Int32 id, [FromQuery] PageCommand pageCommand, [FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] Int64? teamID, [FromQuery] Int32? deviceID, [FromQuery] Int32? deviceModelID, [FromQuery] String mode = "Small")
        {
            var response = new FlightStatsActionResponse();
            response.ApplySettings(actionResponseFactory.Settings);
            var validated = true;
            if (!customerRepository.HasMemberPermission(id, CurrentUser))
            {
                validated = false;
                response.AddNotAllowedErr();
            }

            if (validated)
            {
                Expression<Func<FlightStat, Boolean>> query = ww => (ww.CustomerID == id)
                    && (teamID.HasValue ? (ww.Device.TeamID == teamID.Value) : true)
                    && (deviceID.HasValue ? (ww.DeviceID == deviceID.Value) : true)
                    && (from.HasValue ? (ww.FlightTime >= from.Value) : true)
                    && (to.HasValue ? (ww.FlightTime <= to.Value) : true);
                if (mode == "Large")
                {

                }
                else
                {
                    var listResponse = flightStatRepository.GetListFlightStatsView<SmallFlightStatViewModel>(pageCommand, query, ww => ww.CreatedTime, false);
                    listResponse.SetResponse(response);
                }
            }



            return response.ToIActionResult();
        }
        [HttpGet("{id:int}/FlightStats/{flightStatID:Guid}")]
        public IActionResult GetFlightStatByID([FromServices] FlightStatRepository flightStatRepository, [FromRoute] Int32 id, [FromRoute] Guid flightStatID)
        {
            var response = actionResponseFactory.CreateInstance();
            response.ApplySettings(actionResponseFactory.Settings);
            var validated = true;
            if (!customerRepository.HasMemberPermission(id, CurrentUser))
            {
                validated = false;
                response.AddNotAllowedErr();
            }
            var flightStat = flightStatRepository.Get(ww => ww.ID == flightStatID);
            if (flightStat is null)
            {
                validated = false;
                response.AddNotFoundErr("FlightStat");
            }

            if (validated)
            {
                response.SetData(ViewModelHelpers.ConvertToViewModel<FlightStat, LargeFlightStatViewModel>(flightStat));
            }

            return response.ToIActionResult();
        }
        [HttpGet("{id:int}/Fields")]
        public IActionResult GetFields([FromServices] FieldRepository fieldRepository, [FromRoute] Int32 id, [FromQuery] PageCommand pageCommand, [FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] String search, [FromQuery] String mode = "Small")
        {
            var response = actionResponseFactory.CreateInstance();
            var validated = true;
            if (!customerRepository.HasMemberPermission(id, CurrentUser))
            {
                validated = false;
                response.AddNotAllowedErr();
            }
            if (validated)
            {
                Expression<Func<Field, Boolean>> query = ww => (ww.CustomerID == id)
                    && (from.HasValue ? (ww.CreatedTime >= from.Value) : true)
                    && (to.HasValue ? (ww.CreatedTime <= to.Value) : true)
                    && (!String.IsNullOrWhiteSpace(search) ? (ww.Name.ToLower().Contains(search.ToLower()) || ww.FieldLocation.ToLower().Contains(search.ToLower()) || ww.FieldName.ToLower().Contains(search.ToLower())) : true);
                if (mode == "Large")
                {

                }
                else
                {
                    var listResponse = fieldRepository.GetListResponseView<FieldViewModel>(pageCommand, query, ww => ww.CreatedTime, false);
                    listResponse.SetResponse(response);
                }
            }


            return response.ToIActionResult();
        }

        [HttpGet("{id:int}/Fields/{fieldID:long}")]
        public IActionResult GetFields([FromServices] FieldRepository fieldRepository, [FromRoute] Int32 id, [FromRoute] Int64 fieldID)
        {
            var response = actionResponseFactory.CreateInstance();
            var validated = true;
            if (!customerRepository.HasMemberPermission(id, CurrentUser))
            {
                validated = false;
                response.AddNotAllowedErr();
            }
            var field = fieldRepository.Get(ww => ww.ID == fieldID);
            if (field is null)
            {
                validated = false;
                response.AddNotFoundErr("Field");
            }
            if (validated)
            {
                response.SetData(ViewModelHelpers.ConvertToViewModel<Field, LargeFieldViewModel>(field));
            }
            return response.ToIActionResult();
        }

        [HttpPost("{id:int}/Teams")]
        public IActionResult CreateTeam([FromServices] TeamRepository teamRepository, [FromRoute] Int32 id, [FromBody] AddingTeamCommand command)
        {
            var response = actionResponseFactory.CreateInstance();
            var validated = true;
            if (!customerRepository.Any(ww => ww.ID == id))
            {
                validated = false;
                response.AddNotFoundErr("Customer");
            }
            if (!customerRepository.HasOwnerPermission(id, CurrentUser))
            {
                validated = false;
                response.AddNotAllowedErr();
            }

            if (validated)
            {
                var team = new Team { Name = command.Name, CustomerID = id };
                teamRepository.Create(team);
                response.SetCreatedObject(team);
            }



            return response.ToIActionResult();
        }

        [HttpGet("{id:int}/Teams")]
        public IActionResult GetTeams([FromServices] TeamRepository teamRepository, [FromRoute] Int32 id, [FromQuery] PageCommand pageCommand, [FromQuery] String search, [FromQuery] String mode = "Small")
        {
            var response = actionResponseFactory.CreateInstance();
            var validated = true;
            if (!customerRepository.Any(ww => ww.ID == id))
            {
                validated = false;
                response.AddNotFoundErr("Customer");
            }
            if (!customerRepository.HasMemberPermission(id, CurrentUser))
            {
                validated = false;
                response.AddNotAllowedErr();
            }

            if (validated)
            {

                Expression<Func<Team, Boolean>> query = ww => (ww.CustomerID == id)
                   && (!String.IsNullOrWhiteSpace(search) ? (ww.Name.ToLower().Contains(search.ToLower())) : true);
                if (mode == "Large")
                {


                }
                else
                {
                    var listResponse = teamRepository.GetListResponseView<SmallTeamViewModel>(pageCommand, query);
                    listResponse.SetResponse(response);
                }
            }



            return response.ToIActionResult();
        }
        [HttpGet("{id:int}/Teams/{teamID:long}")]
        public IActionResult GetTeamByID([FromServices] TeamRepository teamRepository, [FromRoute] Int32 id, [FromRoute] Int64 teamID)
        {
            var response = actionResponseFactory.CreateInstance();
            var validated = true;
            if (!customerRepository.Any(ww => ww.ID == id))
            {
                validated = false;
                response.AddNotFoundErr("Customer");
            }
            if (!customerRepository.HasMemberPermission(id, CurrentUser))
            {
                validated = false;
                response.AddNotAllowedErr();
            }

            if (validated)
            {
                var team = teamRepository.GetView<LargeTeamViewModel>(ww => ww.ID == teamID);
                response.SetData(team);
            }



            return response.ToIActionResult();
        }
        [HttpGet("{id:int}/Devices")]
        public IActionResult GetDevices([FromServices] DeviceRepository deviceRepository, [FromRoute] Int32 id, [FromQuery] PageCommand pageCommand, [FromQuery] String search, [FromQuery] String mode = "Small")
        {
            var response = actionResponseFactory.CreateInstance();
            var validated = true;
            if (!customerRepository.Any(ww => ww.ID == id))
            {
                validated = false;
                response.AddNotFoundErr("Customer");
            }
            if (!customerRepository.HasMemberPermission(id, CurrentUser))
            {
                validated = false;
                response.AddNotAllowedErr();
            }

            if (validated)
            {
                Expression<Func<Device, Boolean>> query = ww => (ww.CustomerID == id)
                   && (!String.IsNullOrWhiteSpace(search) ? (ww.Name.ToLower().Contains(search.ToLower())) : true);
                if (mode == "Large")
                {


                }
                else
                {
                    var listResponse = deviceRepository.GetListResponseView<SmallDeviceViewModel>(pageCommand, query);
                    listResponse.SetResponse(response);
                }
            }



            return response.ToIActionResult();
        }
        [HttpGet("{id:int}/Devices/{deviceID:int}/Records")]
        public IActionResult GetDeviceRecords([FromServices] TelemetryRecordRepository telemetryRecordRepository, [FromRoute] Int32 id, [FromRoute] Int32 deviceID, [FromQuery] PageCommand pageCommand, [FromQuery] String mode = "Small")
        {
            var response = actionResponseFactory.CreateInstance();

            var validated = true;
            if (!customerRepository.Any(ww => ww.ID == id))
            {
                validated = false;
                response.AddNotFoundErr("Customer");
            }
            if (!customerRepository.HasMemberPermission(id, CurrentUser))
            {
                validated = false;
                response.AddNotAllowedErr();
            }

            if (validated)
            {
                Expression<Func<TelemetryRecord, Boolean>> query = ww => (ww.DeviceID == deviceID && ww.Device.CustomerID == id);
                if (mode == "Large")
                {

                }
                else
                {
                    var listResponse = telemetryRecordRepository.GetListResponseView<TelemetryRecordViewModel>(pageCommand, query, ww => ww.CreatedTime, false);
                    listResponse.SetResponse(response);
                }
            }

            return response.ToIActionResult();
        }
    }
}