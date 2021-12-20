

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
using Microsoft.AspNetCore.Authorization;
using System.IO;
using System.Linq.Expressions;
using NetTopologySuite.Geometries;
using NetTopologySuite;
using MiSmart.DAL.Extensions;

namespace MiSmart.API.Controllers
{
    public class TeamsController : AuthorizedAPIControllerBase
    {
        public TeamsController(IActionResponseFactory actionResponseFactory) : base(actionResponseFactory)
        {
        }

        [HttpPost]
        public IActionResult CreateTeam([FromServices] TeamRepository teamRepository, [FromServices] CustomerUserRepository customerUserRepository, [FromBody] AddingTeamCommand command)
        {
            var response = actionResponseFactory.CreateInstance();
            Int32? customerID = command.CustomerID;
            if (!CurrentUser.IsAdmin || customerID is null)
            {
                customerID = customerUserRepository.HasMemberPermission(CurrentUser);
            }
            if (customerID is null)
            {
                response.AddNotAllowedErr();
            }



            var team = new Team { Name = command.Name, CustomerID = customerID.GetValueOrDefault() };
            teamRepository.Create(team);
            response.SetCreatedObject(team);




            return response.ToIActionResult();
        }

        [HttpGet]
        public IActionResult GetTeams([FromServices] TeamRepository teamRepository, [FromServices] CustomerUserRepository customerUserRepository, [FromQuery] PageCommand pageCommand, [FromQuery] Int32? customerID, [FromQuery] String search, [FromQuery] String mode = "Small")
        {
            var response = actionResponseFactory.CreateInstance();

            if (!CurrentUser.IsAdmin || customerID is null)
            {
                customerID = customerUserRepository.HasMemberPermission(CurrentUser);
            }
            if (customerID is null)
            {
                response.AddNotAllowedErr();
            }

            Expression<Func<Team, Boolean>> query = ww => (ww.CustomerID == customerID.GetValueOrDefault())
              && (!String.IsNullOrWhiteSpace(search) ? (ww.Name.ToLower().Contains(search.ToLower())) : true);
            if (mode == "Large")
            {


            }
            else
            {
                var listResponse = teamRepository.GetListResponseView<SmallTeamViewModel>(pageCommand, query);
                listResponse.SetResponse(response);
            }



            return response.ToIActionResult();
        }
        [HttpGet("{id:int}")]
        public IActionResult GetTeamByID([FromServices] TeamRepository teamRepository, [FromServices] CustomerUserRepository customerUserRepository, [FromRoute] Int32 id)
        {
            var response = actionResponseFactory.CreateInstance();
            Int32? customerID = null;
            if (!CurrentUser.IsAdmin || customerID is null)
            {
                customerID = customerUserRepository.HasMemberPermission(CurrentUser);
            }
            if (customerID is null)
            {
                response.AddNotAllowedErr();
            }
            var team = teamRepository.GetView<LargeTeamViewModel>(ww => ww.ID == id && ww.CustomerID == customerID.GetValueOrDefault());
            response.SetData(team);
            return response.ToIActionResult();
        }

        [HttpPost("{id:long}/AssignUser")]
        public IActionResult AssignTeamUser([FromServices] TeamRepository teamRepository, [FromServices] TeamUserRepository teamUserRepository, [FromServices] CustomerUserRepository customerUserRepository, [FromRoute] Int64 id, [FromBody] AssigningTeamUserCommand command)
        {
            var response = actionResponseFactory.CreateInstance();

            var customerID = customerUserRepository.HasMemberPermission(CurrentUser);
            if (customerID is null)
            {
                response.AddNotAllowedErr();
            }
            if (!teamUserRepository.Any(ww => ww.UserID == CurrentUser.ID))
            {
                response.AddExistedErr("UserID");
            }

            var teamUser = new TeamUser { UserID = command.UserID.Value, TeamID = id, Type = command.Type };
            teamUserRepository.Create(teamUser);

            if (!customerUserRepository.Any(ww => ww.CustomerID == id && ww.UserID == command.UserID))
            {
                var customerUser = new CustomerUser { CustomerID = customerID.GetValueOrDefault(), UserID = command.UserID.Value, Type = CustomerMemberType.Member };
                customerUserRepository.Create(customerUser);
            }
            response.SetCreatedObject(teamUser);



            return response.ToIActionResult();
        }
    }
}