

using MiSmart.Infrastructure.Controllers;
using MiSmart.Infrastructure.Responses;
using MiSmart.API.Commands;
using Microsoft.AspNetCore.Mvc;
using System;
using MiSmart.DAL.Models;
using MiSmart.DAL.Repositories;
using System.Linq;
using MiSmart.Infrastructure.Commands;
using MiSmart.DAL.ViewModels;
using System.Linq.Expressions;

namespace MiSmart.API.Controllers
{
    public class TeamsController : AuthorizedAPIControllerBase
    {
        public TeamsController(IActionResponseFactory actionResponseFactory) : base(actionResponseFactory)
        {
        }

        [HttpPost]
        public IActionResult CreateTeam([FromServices] TeamRepository teamRepository, [FromServices] ExecutionCompanyUserRepository executionCompanyUserRepository, [FromBody] AddingTeamCommand command)
        {
            var response = actionResponseFactory.CreateInstance();
            ExecutionCompanyUser executionCompanyUser = executionCompanyUserRepository.GetByPermission(CurrentUser.ID, ExecutionCompanyUserType.Owner);
            if (executionCompanyUser is null)
            {
                response.AddNotAllowedErr();
            }

            var team = new Team { Name = command.Name, ExecutionCompanyID = executionCompanyUser.ExecutionCompanyID };
            teamRepository.Create(team);
            response.SetCreatedObject(team);




            return response.ToIActionResult();
        }

        [HttpGet]
        public IActionResult GetTeams([FromServices] TeamRepository teamRepository, [FromServices] ExecutionCompanyUserRepository executionCompanyUserRepository, [FromServices] TeamUserRepository teamUserRepository, [FromQuery] PageCommand pageCommand, [FromQuery] String search, [FromQuery] String mode = "Small")
        {
            var response = actionResponseFactory.CreateInstance();

            ExecutionCompanyUser executionCompanyUser = executionCompanyUserRepository.GetByPermission(CurrentUser.ID);
            if (executionCompanyUser is null)
            {
                response.AddNotAllowedErr();
            }
            var teamIDs = teamUserRepository.GetListEntities(new PageCommand(), ww => ww.ExecutionCompanyUserID == executionCompanyUser.ID).Select(ww => ww.TeamID).ToList();

            Expression<Func<Team, Boolean>> query = ww => (executionCompanyUser.Type == ExecutionCompanyUserType.Owner ? ww.ExecutionCompanyID == executionCompanyUser.ExecutionCompanyID : (teamIDs.Contains(ww.ID)))
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
        public IActionResult GetTeamByID([FromServices] TeamRepository teamRepository, [FromServices] TeamUserRepository teamUserRepository, [FromServices] ExecutionCompanyUserRepository executionCompanyUserRepository, [FromRoute] Int32 id)
        {
            var response = actionResponseFactory.CreateInstance();
            ExecutionCompanyUser executionCompanyUser = executionCompanyUserRepository.GetByPermission(CurrentUser.ID);
            if (executionCompanyUser is null)
            {
                response.AddNotAllowedErr();
            }
            var teamIDs = teamUserRepository.GetListEntities(new PageCommand(), ww => ww.ExecutionCompanyUserID == executionCompanyUser.ID).Select(ww => ww.TeamID).ToList();


            var team = teamRepository.GetView<LargeTeamViewModel>(ww => ww.ID == id && (executionCompanyUser.Type == ExecutionCompanyUserType.Owner ? ww.ExecutionCompanyID == executionCompanyUser.ExecutionCompanyID : (teamIDs.Contains(ww.ID))));
            if (team is null)
            {
                response.AddNotFoundErr("Team");

            }
            response.SetData(team);
            return response.ToIActionResult();
        }
        [HttpPatch("{id:int}")]
        public IActionResult PatchTeam([FromServices] TeamRepository teamRepository, [FromRoute] Int32 id
        , [FromServices] ExecutionCompanyUserRepository executionCompanyUserRepository, [FromBody] UpdatingTeamCommand command)
        {
            var response = actionResponseFactory.CreateInstance();
            ExecutionCompanyUser executionCompanyUser = executionCompanyUserRepository.GetByPermission(CurrentUser.ID);
            if (executionCompanyUser is null)
            {
                response.AddNotAllowedErr();
            }
            var team = teamRepository.Get(ww => ww.ExecutionCompanyID == executionCompanyUser.ExecutionCompanyID && ww.ID == id);
            if (team is null)
            {
                response.AddNotFoundErr("Team");
            }
            team.Name = String.IsNullOrEmpty(command.Name) ? team.Name : command.Name;
            teamRepository.Update(team);
            response.SetUpdatedMessage();
            return response.ToIActionResult();
        }

        [HttpPost("{id:int}/Disband")]
        public IActionResult Disband([FromServices] TeamRepository teamRepository, [FromServices] ExecutionCompanyUserRepository executionCompanyUserRepository, [FromRoute] Int32 id)
        {
            var response = actionResponseFactory.CreateInstance();
            ExecutionCompanyUser executionCompanyUser = executionCompanyUserRepository.GetByPermission(CurrentUser.ID, ExecutionCompanyUserType.Owner);
            if (executionCompanyUser is null)
            {
                response.AddNotAllowedErr();
            }
            var team = teamRepository.Get(ww => ww.ExecutionCompanyID == executionCompanyUser.ExecutionCompanyID && ww.ID == id);
            if (team is null)
            {
                response.AddNotFoundErr("Team");

            }
            teamRepository.Delete(team);

            response.SetMessage("Team is disbanded", "Nhóm đã giải tán");

            return response.ToIActionResult();
        }

        [HttpPost("{id:long}/AssignUser")]
        public IActionResult AssignTeamUser([FromServices] TeamRepository teamRepository, [FromServices] TeamUserRepository teamUserRepository,
         [FromServices] ExecutionCompanyUserRepository executionCompanyUserRepository, [FromRoute] Int64 id, [FromBody] AssigningTeamUserCommand command)
        {
            var response = actionResponseFactory.CreateInstance();

            ExecutionCompanyUser executionCompanyUser = executionCompanyUserRepository.GetByPermission(CurrentUser.ID, ExecutionCompanyUserType.Owner);
            if (executionCompanyUser is null)
            {
                response.AddNotAllowedErr();
            }

            var targetExecutionCompanyUser = executionCompanyUserRepository.Get(ww => ww.UserID == command.UserID.GetValueOrDefault() && ww.ExecutionCompanyID == executionCompanyUser.ExecutionCompanyID);
            if (targetExecutionCompanyUser is null)
            {
                targetExecutionCompanyUser = new ExecutionCompanyUser { ExecutionCompanyID = executionCompanyUser.ExecutionCompanyID, UserID = command.UserID.GetValueOrDefault(), Type = ExecutionCompanyUserType.Member };
                executionCompanyUserRepository.Create(targetExecutionCompanyUser);
            }
            var existedTeamUser = teamUserRepository.Get(ww => ww.ExecutionCompanyUserID == targetExecutionCompanyUser.ID && ww.TeamID == id);
            if (existedTeamUser is not null)
            {
                response.AddExistedErr("TeamUser");
            }
            var teamUser = new TeamUser { ExecutionCompanyUserID = targetExecutionCompanyUser.ID, TeamID = id, Type = command.Type };
            teamUserRepository.Create(teamUser);
            response.SetCreatedObject(teamUser);
            return response.ToIActionResult();
        }
    }
}