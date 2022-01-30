

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
            CustomerUser customerUser = customerUserRepository.GetByPermission(CurrentUser.ID, CustomerMemberType.Owner);
            if (customerUser is null)
            {
                response.AddNotAllowedErr();
            }

            var team = new Team { Name = command.Name, CustomerID = customerUser.CustomerID };
            teamRepository.Create(team);
            response.SetCreatedObject(team);




            return response.ToIActionResult();
        }

        [HttpGet]
        public IActionResult GetTeams([FromServices] TeamRepository teamRepository, [FromServices] CustomerUserRepository customerUserRepository, [FromServices] TeamUserRepository teamUserRepository, [FromQuery] PageCommand pageCommand, [FromQuery] String search, [FromQuery] String mode = "Small")
        {
            var response = actionResponseFactory.CreateInstance();

            CustomerUser customerUser = customerUserRepository.GetByPermission(CurrentUser.ID);
            if (customerUser is null)
            {
                response.AddNotAllowedErr();
            }
            var teamIDs = teamUserRepository.GetListEntities(new PageCommand(), ww => ww.CustomerUserID == customerUser.ID).Select(ww => ww.TeamID).ToList();

            Expression<Func<Team, Boolean>> query = ww => (customerUser.Type == CustomerMemberType.Owner ? ww.CustomerID == customerUser.CustomerID : (teamIDs.Contains(ww.ID)))
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
        public IActionResult GetTeamByID([FromServices] TeamRepository teamRepository, [FromServices] TeamUserRepository teamUserRepository, [FromServices] CustomerUserRepository customerUserRepository, [FromRoute] Int32 id)
        {
            var response = actionResponseFactory.CreateInstance();
            CustomerUser customerUser = customerUserRepository.GetByPermission(CurrentUser.ID);
            if (customerUser is null)
            {
                response.AddNotAllowedErr();
            }
            var teamIDs = teamUserRepository.GetListEntities(new PageCommand(), ww => ww.CustomerUserID == customerUser.ID).Select(ww => ww.TeamID).ToList();

            var team = teamRepository.GetView<LargeTeamViewModel>(ww => ww.ID == id && (customerUser.Type == CustomerMemberType.Owner ? ww.CustomerID == customerUser.CustomerID : (teamIDs.Contains(ww.ID))));
            if (team is null)
            {
                response.AddNotFoundErr("Team");

            }
            response.SetData(team);
            return response.ToIActionResult();
        }

        [HttpPost("{id:int}/Disband")]
        public IActionResult Disband([FromServices] TeamRepository teamRepository, [FromServices] CustomerUserRepository customerUserRepository, [FromRoute] Int32 id)
        {
            var response = actionResponseFactory.CreateInstance();
            CustomerUser customerUser = customerUserRepository.GetByPermission(CurrentUser.ID, CustomerMemberType.Owner);
            if (customerUser is null)
            {
                response.AddNotAllowedErr();
            }
            var team = teamRepository.Get(ww => ww.CustomerID == customerUser.CustomerID && ww.ID == id);
            if (team is null)
            {
                response.AddNotFoundErr("Team");

            }
            teamRepository.Delete(team);

            response.SetMessage("Team is disbanded", "Nhóm đã giải tán");

            return response.ToIActionResult();
        }

        [HttpPost("{id:long}/AssignUser")]
        public IActionResult AssignTeamUser([FromServices] TeamRepository teamRepository, [FromServices] TeamUserRepository teamUserRepository, [FromServices] CustomerUserRepository customerUserRepository, [FromRoute] Int64 id, [FromBody] AssigningTeamUserCommand command)
        {
            var response = actionResponseFactory.CreateInstance();

            CustomerUser customerUser = customerUserRepository.GetByPermission(CurrentUser.ID, CustomerMemberType.Owner);
            if (customerUser is null)
            {
                response.AddNotAllowedErr();
            }

            var targetCustomerUser = customerUserRepository.GetByPermission(command.UserID.GetValueOrDefault());
            if (targetCustomerUser is null)
            {
                response.AddInvalidErr("UserID");
            }
            if (!teamUserRepository.Any(ww => ww.CustomerUserID == targetCustomerUser.ID))
            {
                response.AddExistedErr("UserID");
            }
            var teamUser = new TeamUser { CustomerUserID = targetCustomerUser.ID, TeamID = id, Type = command.Type };
            teamUserRepository.Create(teamUser);
            response.SetCreatedObject(teamUser);



            return response.ToIActionResult();
        }
    }
}