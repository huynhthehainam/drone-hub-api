

using MiSmart.Infrastructure.Controllers;
using MiSmart.Infrastructure.Responses;
using MiSmart.API.Commands;
using Microsoft.AspNetCore.Mvc;
using System;
using MiSmart.DAL.Models;
using MiSmart.DAL.Repositories;
using System.Threading.Tasks;

namespace MiSmart.API.Controllers
{
    public class ExecutionCompanyUsersController : AuthorizedAPIControllerBase
    {
        public ExecutionCompanyUsersController(IActionResponseFactory actionResponseFactory) : base(actionResponseFactory)
        {
        }
        [HttpPost("UnassignUser")]
        public async Task<IActionResult> UnassignUser([FromRoute] Int32 id, [FromServices] ExecutionCompanyUserRepository executionCompanyUserRepository, [FromBody] RemovingExecutionCompanyUserCommand command)
        {
            var response = actionResponseFactory.CreateInstance();
            ExecutionCompanyUser executionCompanyUser = await executionCompanyUserRepository.GetByPermissionAsync(CurrentUser.ID, ExecutionCompanyUserType.Owner);
            if (executionCompanyUser is null)
            {
                response.AddNotAllowedErr();
            }

            var targetExecutionCompanyUser = await executionCompanyUserRepository.GetAsync(ww => ww.ExecutionCompanyID == executionCompanyUser.ExecutionCompanyID && ww.UserID == command.UserID.GetValueOrDefault());
            if (targetExecutionCompanyUser is null)
            {
                response.AddInvalidErr("UserID");
            }
            await executionCompanyUserRepository.DeleteAsync(targetExecutionCompanyUser);

            response.SetNoContent();

            return response.ToIActionResult();
        }
    }

}