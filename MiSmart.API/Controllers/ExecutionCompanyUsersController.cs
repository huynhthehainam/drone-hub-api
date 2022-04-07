

using MiSmart.Infrastructure.Controllers;
using MiSmart.Infrastructure.Responses;
using MiSmart.API.Commands;
using Microsoft.AspNetCore.Mvc;
using System;
using MiSmart.DAL.Models;
using MiSmart.DAL.Repositories;
namespace MiSmart.API.Controllers
{
    public class ExecutionCompanyUsersController : AuthorizedAPIControllerBase
    {
        public ExecutionCompanyUsersController(IActionResponseFactory actionResponseFactory) : base(actionResponseFactory)
        {
        }
        [HttpPost("UnassignUser")]
        public IActionResult UnassignUser([FromRoute] Int32 id, [FromServices] ExecutionCompanyUserRepository executionCompanyUserRepository, [FromBody] RemovingExecutionCompanyUserCommand command)
        {
            var response = actionResponseFactory.CreateInstance();
            ExecutionCompanyUser executionCompanyUser = executionCompanyUserRepository.GetByPermission(CurrentUser.ID, ExecutionCompanyUserType.Owner);
            if (executionCompanyUser is null)
            {
                response.AddNotAllowedErr();
            }

            var targetExecutionCompanyUser = executionCompanyUserRepository.Get(ww => ww.ExecutionCompanyID == executionCompanyUser.ExecutionCompanyID && ww.UserID == command.UserID.GetValueOrDefault());
            if (targetExecutionCompanyUser is null)
            {
                response.AddInvalidErr("UserID");
            }
            executionCompanyUserRepository.Delete(targetExecutionCompanyUser);

            response.SetNoContent();

            return response.ToIActionResult();
        }
    }

}