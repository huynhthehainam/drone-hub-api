

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
using System.Threading.Tasks;

namespace MiSmart.API.Controllers
{
    public class TeamsController : AuthorizedAPIControllerBase
    {
        public TeamsController(IActionResponseFactory actionResponseFactory) : base(actionResponseFactory)
        {
        }

        [HttpPost]
        public async Task<IActionResult> CreateTeam([FromServices] TeamRepository teamRepository, [FromServices] ExecutionCompanyUserRepository executionCompanyUserRepository, [FromBody] AddingTeamCommand command)
        {
            var response = actionResponseFactory.CreateInstance();
            ExecutionCompanyUser executionCompanyUser = await executionCompanyUserRepository.GetByPermissionAsync(CurrentUser.ID, ExecutionCompanyUserType.Owner);
            if (executionCompanyUser is null)
            {
                response.AddNotAllowedErr();
            }

            var team = await teamRepository.CreateAsync(new Team { Name = command.Name, ExecutionCompanyID = executionCompanyUser.ExecutionCompanyID });
            response.SetCreatedObject(team);




            return response.ToIActionResult();
        }

        [HttpGet]
        public async Task<IActionResult> GetTeams([FromServices] TeamRepository teamRepository, [FromServices] ExecutionCompanyUserRepository executionCompanyUserRepository,
         [FromServices] TeamUserRepository teamUserRepository, [FromQuery] PageCommand pageCommand, [FromQuery] String search, [FromQuery] String relation = "Executor")
        {
            var response = actionResponseFactory.CreateInstance();
            Expression<Func<Team, Boolean>> query = ww => false;
            if (relation == "Executor")
            {
                ExecutionCompanyUser executionCompanyUser = await executionCompanyUserRepository.GetByPermissionAsync(CurrentUser.ID);
                if (executionCompanyUser is null)
                {
                    response.AddNotAllowedErr();
                }
                var teamIDs = teamUserRepository.GetListEntitiesAsync(new PageCommand(), ww => ww.ExecutionCompanyUserID == executionCompanyUser.ID).Result.Select(ww => ww.TeamID).ToList();

                query = ww => (executionCompanyUser.Type == ExecutionCompanyUserType.Owner ? ww.ExecutionCompanyID == executionCompanyUser.ExecutionCompanyID : (teamIDs.Contains(ww.ID)))
                   && (!String.IsNullOrWhiteSpace(search) ? (ww.Name.ToLower().Contains(search.ToLower())) : true) && !ww.IsDisbanded;

            }
            else
            {
                if (!CurrentUser.IsAdmin && CurrentUser.RoleID != 1)
                {
                    response.AddNotAllowedErr();
                }
                query = ww => true;
            }

            var listResponse = await teamRepository.GetListResponseViewAsync<SmallTeamViewModel>(pageCommand, query);
            listResponse.SetResponse(response);




            return response.ToIActionResult();
        }
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetTeamByID([FromServices] TeamRepository teamRepository, [FromServices] TeamUserRepository teamUserRepository, [FromServices] ExecutionCompanyUserRepository executionCompanyUserRepository, [FromRoute] Int32 id)
        {
            var response = actionResponseFactory.CreateInstance();
            ExecutionCompanyUser executionCompanyUser = await executionCompanyUserRepository.GetByPermissionAsync(CurrentUser.ID);
            if (executionCompanyUser is null)
            {
                response.AddNotAllowedErr();
            }
            var teamIDs = teamUserRepository.GetListEntitiesAsync(new PageCommand(), ww => ww.ExecutionCompanyUserID == executionCompanyUser.ID).Result.Select(ww => ww.TeamID).ToList();


            var team = await teamRepository.GetViewAsync<LargeTeamViewModel>(ww => ww.ID == id
            && (executionCompanyUser.Type == ExecutionCompanyUserType.Owner ? ww.ExecutionCompanyID == executionCompanyUser.ExecutionCompanyID : (teamIDs.Contains(ww.ID)))
            && !ww.IsDisbanded);
            if (team is null)
            {
                response.AddNotFoundErr("Team");

            }
            response.SetData(team);
            return response.ToIActionResult();
        }
        [HttpPatch("{id:int}")]
        public async Task<IActionResult> PatchTeam([FromServices] TeamRepository teamRepository, [FromRoute] Int32 id
        , [FromServices] ExecutionCompanyUserRepository executionCompanyUserRepository, [FromBody] UpdatingTeamCommand command)
        {
            var response = actionResponseFactory.CreateInstance();
            ExecutionCompanyUser executionCompanyUser = await executionCompanyUserRepository.GetByPermissionAsync(CurrentUser.ID, ExecutionCompanyUserType.Owner);
            if (executionCompanyUser is null)
            {
                response.AddNotAllowedErr();
            }
            var team = await teamRepository.GetAsync(ww => ww.ExecutionCompanyID == executionCompanyUser.ExecutionCompanyID && ww.ID == id && !ww.IsDisbanded);
            if (team is null)
            {
                response.AddNotFoundErr("Team");
            }
            team.Name = String.IsNullOrEmpty(command.Name) ? team.Name : command.Name;
            await teamRepository.UpdateAsync(team);
            response.SetUpdatedMessage();
            return response.ToIActionResult();
        }

        [HttpPost("{id:int}/Disband")]
        public async Task<IActionResult> Disband([FromServices] TeamRepository teamRepository, [FromServices] ExecutionCompanyUserRepository executionCompanyUserRepository, [FromRoute] Int32 id)
        {
            var response = actionResponseFactory.CreateInstance();
            ExecutionCompanyUser executionCompanyUser = await executionCompanyUserRepository.GetByPermissionAsync(CurrentUser.ID, ExecutionCompanyUserType.Owner);
            if (executionCompanyUser is null)
            {
                response.AddNotAllowedErr();
            }
            var team = await teamRepository.GetAsync(ww => ww.ExecutionCompanyID == executionCompanyUser.ExecutionCompanyID && ww.ID == id && !ww.IsDisbanded);
            if (team is null)
            {
                response.AddNotFoundErr("Team");

            }

            team.IsDisbanded = true;
            await teamRepository.UpdateAsync(team);

            response.SetMessage("Team is disbanded", "Nhóm đã giải tán");

            return response.ToIActionResult();
        }
        [HttpPost("{id:long}/UnassignUser")]
        public async Task<IActionResult> UnassignUser([FromRoute] Int64 id, [FromServices] TeamUserRepository teamUserRepository, [FromServices] ExecutionCompanyUserRepository executionCompanyUserRepository, [FromBody] RemovingTeamUserCommand command)
        {
            var response = actionResponseFactory.CreateInstance();
            ExecutionCompanyUser executionCompanyUser = await executionCompanyUserRepository.GetByPermissionAsync(CurrentUser.ID, ExecutionCompanyUserType.Owner);
            if (executionCompanyUser is null)
            {
                response.AddNotAllowedErr();
            }


            var teamUser = await teamUserRepository.GetAsync(ww => ww.ExecutionCompanyUser.UserID == command.UserID.GetValueOrDefault() && ww.TeamID == id && ww.Team.ExecutionCompanyID == executionCompanyUser.ExecutionCompanyID);

            if (teamUser is null)
            {
                response.AddInvalidErr("UserID");
            }
            await teamUserRepository.DeleteAsync(teamUser);
            response.SetNoContent();
            return response.ToIActionResult();
        }

        [HttpPost("{id:long}/AssignUser")]
        public async Task<IActionResult> AssignTeamUser([FromServices] TeamRepository teamRepository, [FromServices] TeamUserRepository teamUserRepository,
         [FromServices] ExecutionCompanyUserRepository executionCompanyUserRepository, [FromRoute] Int64 id, [FromBody] AssigningTeamUserCommand command)
        {
            var response = actionResponseFactory.CreateInstance();

            ExecutionCompanyUser executionCompanyUser = await executionCompanyUserRepository.GetByPermissionAsync(CurrentUser.ID, ExecutionCompanyUserType.Owner);
            if (executionCompanyUser is null)
            {
                response.AddNotAllowedErr();
            }

            ExecutionCompanyUser targetExecutionCompanyUser = null;

            var existedExecutionCompanyUser = await executionCompanyUserRepository.GetAsync(ww => ww.UserID == command.UserID.GetValueOrDefault());
            if (existedExecutionCompanyUser is not null)
            {
                if (existedExecutionCompanyUser.ExecutionCompanyID == executionCompanyUser.ExecutionCompanyID)
                {
                    targetExecutionCompanyUser = existedExecutionCompanyUser;
                }
                else
                {
                    response.AddInvalidErr("UserID");
                }
            }

            if (targetExecutionCompanyUser is null)
            {
                targetExecutionCompanyUser = new ExecutionCompanyUser { ExecutionCompanyID = executionCompanyUser.ExecutionCompanyID, UserID = command.UserID.GetValueOrDefault(), Type = ExecutionCompanyUserType.Member };
                await executionCompanyUserRepository.CreateAsync(targetExecutionCompanyUser);
            }
            var team = await teamRepository.GetAsync(ww => ww.ExecutionCompanyID == executionCompanyUser.ExecutionCompanyID && ww.ID == id && !ww.IsDisbanded);
            if (team is null)
            {
                response.AddNotFoundErr("Team");

            }
            var existedTeamUser = await teamUserRepository.GetAsync(ww => ww.ExecutionCompanyUserID == targetExecutionCompanyUser.ID && ww.TeamID == team.ID);
            if (existedTeamUser is not null)
            {
                response.AddExistedErr("TeamUser");
            }
            var teamUser = new TeamUser { ExecutionCompanyUserID = targetExecutionCompanyUser.ID, TeamID = id, Type = command.Type };
            await teamUserRepository.CreateAsync(teamUser);
            response.SetCreatedObject(teamUser);
            return response.ToIActionResult();
        }
        [HttpGet("{id:long}/UnassignedUsers")]
        public async Task<IActionResult> GetUnassignedUsers([FromRoute] Int64 id, [FromServices] ExecutionCompanyUserRepository executionCompanyUserRepository, [FromServices] TeamRepository teamRepository)
        {
            var response = actionResponseFactory.CreateInstance();
            ExecutionCompanyUser executionCompanyUser = await executionCompanyUserRepository.GetByPermissionAsync(CurrentUser.ID, ExecutionCompanyUserType.Owner);
            if (executionCompanyUser is null)
            {
                response.AddNotAllowedErr();
            }

            var team = await teamRepository.GetAsync(ww => ww.ExecutionCompanyID == executionCompanyUser.ExecutionCompanyID && ww.ID == id && !ww.IsDisbanded);
            if (team is null)
            {
                response.AddNotFoundErr("Team");
            }

            var userIDs = executionCompanyUser.ExecutionCompany.ExecutionCompanyUsers.Select(ww => ww.UserID).ToList();

            var teamUserIDs = team.TeamUsers.Select(ww => ww.ExecutionCompanyUser.UserID).ToList();

            var unassignedIDs = userIDs.Except(teamUserIDs).ToList();
            response.SetData(unassignedIDs);
            return response.ToIActionResult();
        }
    }
}