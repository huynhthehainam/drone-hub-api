
using MiSmart.Infrastructure.Controllers;
using MiSmart.Infrastructure.Responses;
using Microsoft.AspNetCore.Mvc;
using System;
using MiSmart.DAL.Models;
using MiSmart.DAL.Repositories;
using MiSmart.Infrastructure.Commands;
using MiSmart.DAL.ViewModels;
using MiSmart.Infrastructure.ViewModels;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Linq;
using MiSmart.DAL.Responses;
using Microsoft.Extensions.Options;
using MiSmart.Infrastructure.Permissions;
using MiSmart.API.Permissions;
using MiSmart.API.Commands;

namespace MiSmart.API.Controllers
{
    public class FlightStatsController : AuthorizedAPIControllerBase
    {
        public FlightStatsController(IActionResponseFactory actionResponseFactory) : base(actionResponseFactory)
        {
        }

        [HttpGet]
        public IActionResult GetFlightStats([FromServices] FlightStatRepository flightStatRepository,
        [FromServices] TeamUserRepository teamUserRepository,
        [FromServices] IOptions<ActionResponseSettings> options,
        [FromServices] ExecutionCompanyUserRepository executionCompanyUserRepository,
        [FromServices] CustomerUserRepository customerUserRepository, [FromQuery] PageCommand pageCommand,
         [FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] Int32? executionCompanyID,
         [FromQuery] Int32? customerID,
         [FromQuery] Int64? teamID, [FromQuery] Int32? deviceID, [FromQuery] Int32? deviceModelID,
         [FromQuery] String relation = "Owner",
         [FromQuery] String mode = "Small")
        {
            FlightStatsActionResponse response = new FlightStatsActionResponse();
            response.ApplySettings(options.Value);
            Expression<Func<FlightStat, Boolean>> query = ww => false;

            if (relation == "Owner")
            {

                CustomerUser customerUser = customerUserRepository.GetByPermission(CurrentUser.ID);
                if (customerUser is null)
                {
                    response.AddNotAllowedErr();
                }

                query = ww => (ww.CustomerID == customerUser.CustomerID)
                    && (teamID.HasValue ? (ww.Device.TeamID == teamID.Value) : true)
                    && (deviceID.HasValue ? (ww.DeviceID == deviceID.Value) : true)
                    && (from.HasValue ? (ww.FlightTime >= from.Value) : true)
                    && (to.HasValue ? (ww.FlightTime <= to.Value.AddDays(1)) : true)
                    && (executionCompanyID.HasValue ? (ww.ExecutionCompanyID == executionCompanyID.GetValueOrDefault()) : true)
                    && (true);
            }
            else if (relation == "Administrator")
            {
                if (!CurrentUser.IsAdmin && CurrentUser.RoleID != 1)
                {
                    response.AddNotAllowedErr();
                }
                query = fl => true;
            }
            else
            {
                ExecutionCompanyUser executionCompanyUser = executionCompanyUserRepository.GetByPermission(CurrentUser.ID);
                if (executionCompanyUser is null)
                {
                    response.AddNotAllowedErr();
                }
                List<Int64> teamIDs = teamUserRepository.GetListEntities(new PageCommand(), ww => ww.ExecutionCompanyUserID == executionCompanyUser.ID).Select(ww => ww.TeamID).ToList();
                query = ww => (ww.ExecutionCompanyID == executionCompanyUser.ExecutionCompanyID)
                   && (teamID.HasValue ? (ww.Device.TeamID == teamID.Value) : true)
                   && (deviceID.HasValue ? (ww.DeviceID == deviceID.Value) : true)
                   && (from.HasValue ? (ww.FlightTime >= from.Value) : true)
                   && (to.HasValue ? (ww.FlightTime <= to.Value.AddDays(1)) : true)
                   && (customerID.HasValue ? (ww.CustomerID == customerID.GetValueOrDefault()) : true)
                   && (executionCompanyUser.Type == ExecutionCompanyUserType.Member ? (teamIDs.Contains(ww.TeamID.GetValueOrDefault())) : true);
            }
            var listResponse = flightStatRepository.GetListFlightStatsView<SmallFlightStatViewModel>(pageCommand, query, ww => ww.FlightTime, false);
            listResponse.SetResponse(response);
            return response.ToIActionResult();
        }

        [HttpGet("{id:Guid}")]
        public IActionResult GetByID([FromServices] FlightStatRepository flightStatRepository, [FromServices] CustomerUserRepository customerUserRepository,
        [FromServices] ExecutionCompanyUserRepository executionCompanyUserRepository, [FromRoute] Guid id,
        [FromQuery] String relation = "Owner")
        {
            var response = actionResponseFactory.CreateInstance();
            Expression<Func<FlightStat, Boolean>> query = ww => false;
            if (relation == "Owner")
            {
                CustomerUser customerUser = customerUserRepository.GetByPermission(CurrentUser.ID);
                if (customerUser is null)
                {
                    response.AddNotAllowedErr();
                }
                query = ww => ww.ID == id && ww.CustomerID == customerUser.CustomerID;
            }
            else if (relation == "Administrator")
            {
                if (!CurrentUser.IsAdmin && CurrentUser.RoleID != 1)
                {
                    response.AddNotAllowedErr();
                }
                query = ww => ww.ID == id;
            }
            else
            {
                ExecutionCompanyUser executionCompanyUser = executionCompanyUserRepository.GetByPermission(CurrentUser.ID);
                if (executionCompanyUser is null)
                {
                    response.AddNotAllowedErr();
                }
                query = ww => ww.ID == id && ww.ExecutionCompanyID == executionCompanyUser.ExecutionCompanyID;
            }
            var flightStat = flightStatRepository.Get(query);

            if (flightStat is null)
            {
                response.AddNotFoundErr("FlightStat");
            }
            response.SetData(ViewModelHelpers.ConvertToViewModel<FlightStat, LargeFlightStatViewModel>(flightStat));
            return response.ToIActionResult();
        }
        [HttpPost("{id:Guid}/UpdateFromExecutor")]
        public IActionResult UpdateFromExecutor([FromRoute] Guid id, [FromServices] TeamRepository teamRepository, [FromServices] ExecutionCompanyUserRepository executionCompanyUserRepository, [FromServices] FlightStatRepository flightStatRepository, [FromBody] UpdatingFlightStatFromExecutorCommand command)
        {
            var response = actionResponseFactory.CreateInstance();
            ExecutionCompanyUser executionCompanyUser = executionCompanyUserRepository.GetByPermission(CurrentUser.ID, ExecutionCompanyUserType.Owner);
            if (executionCompanyUser is null)
            {
                response.AddNotAllowedErr();
            }


            var flightStat = flightStatRepository.Get(ww => ww.ID == id && ww.ExecutionCompanyID == executionCompanyUser.ExecutionCompanyID);
            if (flightStat is null)
            {
                response.AddNotFoundErr("FlightStat");
            }

            if (command.TeamID.HasValue)
            {
                var team = teamRepository.Get(ww => ww.ID == command.TeamID.GetValueOrDefault() && ww.ExecutionCompanyID == executionCompanyUser.ExecutionCompanyID);
                if (team is null)
                {
                    response.AddInvalidErr("TeamID");
                }
                flightStat.Team = team;
            }


            flightStat.FieldName = String.IsNullOrEmpty(command.FieldName) ? flightStat.FieldName : command.FieldName;
            flightStat.TaskLocation = String.IsNullOrEmpty(command.TaskLocation) ? flightStat.TaskLocation : command.TaskLocation;

            flightStatRepository.Update(flightStat);
            response.SetUpdatedMessage();

            return response.ToIActionResult();
        }

        [HttpDelete("{id:Guid}")]
        [HasPermission(typeof(AdminPermission))]
        public IActionResult DeleteByID([FromServices] FlightStatRepository flightStatRepository, [FromServices] CustomerUserRepository customerUserRepository, [FromRoute] Guid id)
        {
            var response = actionResponseFactory.CreateInstance();
            var flightStat = flightStatRepository.Get(ww => ww.ID == id);

            if (flightStat is null)
            {
                response.AddNotFoundErr("FlightStat");
            }
            flightStatRepository.Delete(flightStat);
            response.SetNoContent();
            return response.ToIActionResult();
        }
    }
}