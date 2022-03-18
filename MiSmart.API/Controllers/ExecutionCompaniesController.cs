

using MiSmart.Infrastructure.Controllers;
using MiSmart.Infrastructure.Responses;
using MiSmart.API.Commands;
using Microsoft.AspNetCore.Mvc;
using System;
using MiSmart.DAL.Models;
using MiSmart.DAL.Repositories;
using MiSmart.Infrastructure.Commands;
using System.Linq.Expressions;
using MiSmart.DAL.ViewModels;
using MiSmart.Infrastructure.Permissions;
using MiSmart.API.Permissions;
using System.Linq;

namespace MiSmart.API.Controllers
{
    public class ExecutionCompaniesController : AuthorizedAPIControllerBase
    {

        public ExecutionCompaniesController(IActionResponseFactory actionResponseFactory) : base(actionResponseFactory)
        {
        }
        [HttpPost]
        [HasPermission(typeof(AdminPermission))]
        public IActionResult Create([FromServices] ExecutionCompanyRepository executionCompanyRepository, [FromBody] AddingExecutionCompanyCommand command)
        {
            var response = actionResponseFactory.CreateInstance();


            var executionCompany = new ExecutionCompany { Name = command.Name, Address = command.Address };
            executionCompanyRepository.Create(executionCompany);
            response.SetCreatedObject(executionCompany);

            return response.ToIActionResult();
        }

        [HttpGet]
        [HasPermission(typeof(AdminPermission))]
        public IActionResult GetList([FromQuery] PageCommand pageCommand,
        [FromServices] ExecutionCompanyRepository executionCompanyRepository,
        [FromQuery] String search)
        {
            var response = actionResponseFactory.CreateInstance();
            Expression<Func<ExecutionCompany, Boolean>> query = ww => (!String.IsNullOrWhiteSpace(search) ? (ww.Name.ToLower().Contains(search.ToLower()) || ww.Address.ToLower().Contains(search.ToLower())) : true);
            var listResponse = executionCompanyRepository.GetListResponseView<ExecutionCompanyViewModel>(pageCommand, query);
            listResponse.SetResponse(response);
            return response.ToIActionResult();
        }
        [HttpPost("{id:int}/AssignUser")]
        [HasPermission(typeof(AdminPermission))]
        public IActionResult AssignExecutionCompanyUser([FromServices] ExecutionCompanyUserRepository executionCompanyUserRepository,
     [FromServices] ExecutionCompanyRepository executionCompanyRepository,
      [FromBody] AssigningExecutionCompanyUserCommand command, [FromRoute] Int32 id)
        {
            var response = actionResponseFactory.CreateInstance();
            var executionCompany = executionCompanyRepository.Get(ww => ww.ID == id);
            if (executionCompany is null)
            {
                response.AddNotFoundErr("ExecutionCompany");
            }
            var existedExecutionCompanyUser = executionCompanyUserRepository.Get(ww => ww.UserID == command.UserID.GetValueOrDefault());
            if (existedExecutionCompanyUser is not null)
            {
                response.AddExistedErr("User");
            }
            ExecutionCompanyUser executionCompanyUser = new ExecutionCompanyUser { ExecutionCompanyID = id, UserID = command.UserID.Value };
            executionCompanyUserRepository.Create(executionCompanyUser);
            response.SetCreatedObject(executionCompanyUser);

            return response.ToIActionResult();
        }


        [HttpGet("AssignedUsers")]
        [HasPermission(typeof(AdminPermission))]
        public IActionResult GetCurrentExecutionCompanyUsers([FromServices] ExecutionCompanyUserRepository executionCompanyUserRepository)
        {
            var response = actionResponseFactory.CreateInstance();
            var assignedUserIDs = executionCompanyUserRepository.GetListEntities(new PageCommand(), ww => true).Select(ww => ww.UserID).ToList();
            response.SetData(new { AssignedUserIDs = assignedUserIDs });
            return response.ToIActionResult();
        }

        [HttpGet("{id:int}/Users")]
        [HasPermission(typeof(AdminPermission))]
        public IActionResult GetExecutionCompanyUsers([FromServices] ExecutionCompanyUserRepository executionCompanyUserRepository, [FromServices] ExecutionCompanyRepository executionCompanyRepository, [FromQuery] PageCommand pageCommand, [FromRoute] Int32 id)
        {
            var response = actionResponseFactory.CreateInstance();
            var executionCompany = executionCompanyRepository.Get(ww => ww.ID == id);
            if (executionCompany is null)
            {
                response.AddNotFoundErr("ExecutionCompany");
            }

            Expression<Func<ExecutionCompanyUser, Boolean>> query = ww => ww.ExecutionCompanyID == id;

            var listResponse = executionCompanyUserRepository.GetListResponseView<ExecutionCompanyUserViewModel>(pageCommand, query);
            listResponse.SetResponse(response);

            return response.ToIActionResult();
        }
        [HttpGet("{id:int}/Devices")]
        [HasPermission(typeof(AdminPermission))]
        public IActionResult GetExecutionCompanyDevices([FromServices] DeviceRepository deviceRepository, [FromServices] ExecutionCompanyRepository executionCompanyRepository, [FromQuery] PageCommand pageCommand, [FromRoute] Int32 id)
        {
            var response = actionResponseFactory.CreateInstance();
            var executionCompany = executionCompanyRepository.Get(ww => ww.ID == id);
            if (executionCompany is null)
            {
                response.AddNotFoundErr("ExecutionCompany");
            }

            var listResponse = deviceRepository.GetListResponseView<SmallDeviceViewModel>(pageCommand, ww => ww.ExecutionCompanyID == executionCompany.ID);
            listResponse.SetResponse(response);

            return response.ToIActionResult();
        }

        [HttpDelete("{id:int}")]
        [HasPermission(typeof(AdminPermission))]
        public IActionResult DeleteExecutionCompany([FromServices] ExecutionCompanyUserRepository executionCompanyUserRepository, [FromServices] ExecutionCompanyRepository executionCompanyRepository,
        [FromQuery] PageCommand pageCommand, [FromRoute] Int32 id)
        {
            var response = actionResponseFactory.CreateInstance();
            var executionCompany = executionCompanyRepository.Get(ww => ww.ID == id);
            if (executionCompany is null)
            {
                response.AddNotFoundErr("ExecutionCompany");
            }
            executionCompanyRepository.Delete(executionCompany);
            return response.ToIActionResult();
        }

        [HttpPost("RemoveUser")]
        [HasPermission(typeof(AdminPermission))]
        public IActionResult RemoveExecutionCompanyUser([FromServices] ExecutionCompanyUserRepository executionCompanyUserRepository, [FromServices] TeamUserRepository teamUserRepository, [FromBody] RemovingExecutionCompanyUserCommand command)
        {
            var response = actionResponseFactory.CreateInstance();
            var executionCompanyUser = executionCompanyUserRepository.Get(ww => ww.UserID == command.UserID);
            if (executionCompanyUser is null)
            {
                response.AddInvalidErr("UserID");
            }

            executionCompanyUserRepository.Delete(executionCompanyUser);

            response.SetUpdatedMessage();

            return response.ToIActionResult();
        }
    }
}