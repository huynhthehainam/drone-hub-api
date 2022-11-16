

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
using MiSmart.API.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using MiSmart.API.Settings;
using MiSmart.API.GrpcServices;

namespace MiSmart.API.Controllers
{
    public class ExecutionCompaniesController : AuthorizedAPIControllerBase
    {
        public ExecutionCompaniesController(IActionResponseFactory actionResponseFactory) : base(actionResponseFactory)
        {
        }
        [HttpPost]
        [HasPermission(typeof(AdminPermission))]
        public async Task<IActionResult> Create([FromServices] ExecutionCompanyRepository executionCompanyRepository, [FromBody] AddingExecutionCompanyCommand command)
        {
            var response = actionResponseFactory.CreateInstance();


            var executionCompany = await executionCompanyRepository.CreateAsync(new ExecutionCompany { Name = command.Name, Address = command.Address });
            response.SetCreatedObject(executionCompany);

            return response.ToIActionResult();
        }

        [HttpGet]
        public async Task<IActionResult> GetList([FromQuery] PageCommand pageCommand,
        [FromServices] ExecutionCompanyRepository executionCompanyRepository,
        [FromQuery] String? search)
        {
            var response = actionResponseFactory.CreateInstance();
            Expression<Func<ExecutionCompany, Boolean>> query = ww => (!String.IsNullOrWhiteSpace(search) ? ((ww.Name ?? "").ToLower().Contains(search.ToLower()) || (ww.Address ?? "").ToLower().Contains(search.ToLower())) : true);
            var listResponse = await executionCompanyRepository.GetListResponseViewAsync<ExecutionCompanyViewModel>(pageCommand, query);
            listResponse.SetResponse(response);
            return response.ToIActionResult();
        }

        [HttpPatch("{id:int}")]
        [HasPermission(typeof(AdminPermission))]
        public async Task<IActionResult> UpdateExecutionCompany([FromServices] ExecutionCompanyRepository executionCompanyRepository, [FromRoute] Int32 id, [FromBody] UpdatingExecutionCompanyCommand command)
        {
            var response = actionResponseFactory.CreateInstance();
            var company = await executionCompanyRepository.GetAsync(ww => ww.ID == id);

            if (company is null)
            {
                response.AddNotFoundErr("ExecutionCompany");
                return response.ToIActionResult();
            }

            company.Name = String.IsNullOrWhiteSpace(command.Name) ? company.Name : command.Name;
            company.Address = String.IsNullOrWhiteSpace(command.Address) ? company.Address : command.Address;
            await executionCompanyRepository.UpdateAsync(company);
            response.SetUpdatedMessage();
            return response.ToIActionResult();
        }

        [HttpPost("{id:int}/AssignUser")]
        [HasPermission(typeof(AdminPermission))]
        public async Task<IActionResult> AssignExecutionCompanyUser([FromServices] ExecutionCompanyUserRepository executionCompanyUserRepository,
     [FromServices] ExecutionCompanyRepository executionCompanyRepository, [FromServices] AuthSystemService authSystemService,
      [FromBody] AssigningExecutionCompanyUserCommand command, [FromRoute] Int32 id)
        {
            var response = actionResponseFactory.CreateInstance();
            var userExists = authSystemService.CheckUserUUIDExists(command.UserUUID.GetValueOrDefault());
            if (!userExists)
            {
                response.AddInvalidErr("UserUUID");
                return response.ToIActionResult();
            }

            var executionCompany = await executionCompanyRepository.GetAsync(ww => ww.ID == id);
            if (executionCompany is null)
            {

                response.AddNotFoundErr("ExecutionCompany");
                return response.ToIActionResult();
            }
            var existedExecutionCompanyUser = await executionCompanyUserRepository.GetAsync(ww => ww.UserUUID == command.UserUUID.GetValueOrDefault());
            if (existedExecutionCompanyUser is not null)
            {
                response.AddExistedErr("User");
                return response.ToIActionResult();
            }
            ExecutionCompanyUser executionCompanyUser = await executionCompanyUserRepository.CreateAsync(new ExecutionCompanyUser { ExecutionCompanyID = id, UserUUID = command.UserUUID.GetValueOrDefault(), Type = command.Type });
            response.SetCreatedObject(executionCompanyUser);

            return response.ToIActionResult();
        }


        [HttpGet("AssignedUsers")]
        [HasPermission(typeof(AdminPermission))]
        public Task<IActionResult> GetCurrentExecutionCompanyUsers([FromServices] ExecutionCompanyUserRepository executionCompanyUserRepository)
        {
            var response = actionResponseFactory.CreateInstance();
            var assignedUserIDs = executionCompanyUserRepository.GetListEntitiesAsync(new PageCommand(), ww => true).Result.Select(ww => ww.UserUUID).ToList();
            response.SetData(new { AssignedUserIDs = assignedUserIDs });
            return Task.FromResult(response.ToIActionResult());
        }

        [HttpGet("{id:int}/Users")]
        [HasPermission(typeof(AdminPermission))]
        public async Task<IActionResult> GetExecutionCompanyUsers([FromServices] ExecutionCompanyUserRepository executionCompanyUserRepository, [FromServices] ExecutionCompanyRepository executionCompanyRepository, [FromQuery] PageCommand pageCommand, [FromRoute] Int32 id)
        {
            var response = actionResponseFactory.CreateInstance();
            var executionCompany = await executionCompanyRepository.GetAsync(ww => ww.ID == id);
            if (executionCompany is null)
            {
                response.AddNotFoundErr("ExecutionCompany");
                return response.ToIActionResult();
            }

            Expression<Func<ExecutionCompanyUser, Boolean>> query = ww => ww.ExecutionCompanyID == id;

            var listResponse = await executionCompanyUserRepository.GetListResponseViewAsync<ExecutionCompanyUserViewModel>(pageCommand, query);
            listResponse.SetResponse(response);

            return response.ToIActionResult();
        }
        [HttpGet("{id:int}/Devices")]
        [HasPermission(typeof(AdminPermission))]
        public async Task<IActionResult> GetExecutionCompanyDevices([FromServices] DeviceRepository deviceRepository, [FromServices] ExecutionCompanyRepository executionCompanyRepository, [FromQuery] PageCommand pageCommand, [FromRoute] Int32 id)
        {
            var response = actionResponseFactory.CreateInstance();
            var executionCompany = await executionCompanyRepository.GetAsync(ww => ww.ID == id);
            if (executionCompany is null)
            {
                response.AddNotFoundErr("ExecutionCompany");
                return response.ToIActionResult();
            }

            var listResponse = await deviceRepository.GetListResponseViewAsync<LargeDeviceViewModel>(pageCommand, ww => ww.ExecutionCompanyID == executionCompany.ID);
            listResponse.SetResponse(response);

            return response.ToIActionResult();
        }


        [HttpGet("{id:int}/Batteries")]
        [HasPermission(typeof(AdminPermission))]
        public async Task<IActionResult> GetBatteries([FromServices] BatteryRepository batteryRepository, [FromServices] ExecutionCompanyRepository executionCompanyRepository, [FromQuery] PageCommand pageCommand, [FromRoute] Int32 id)
        {
            var response = actionResponseFactory.CreateInstance();
            var executionCompany = await executionCompanyRepository.GetAsync(ww => ww.ID == id);
            if (executionCompany is null)
            {
                response.AddNotFoundErr("ExecutionCompany");
                return response.ToIActionResult();
            }

            var listResponse = await batteryRepository.GetListResponseViewAsync<BatteryViewModel>(pageCommand, ww => ww.ExecutionCompanyID == executionCompany.ID);
            listResponse.SetResponse(response);

            return response.ToIActionResult();
        }

        [HttpDelete("{id:int}")]
        [HasPermission(typeof(AdminPermission))]
        public async Task<IActionResult> DeleteExecutionCompany([FromServices] ExecutionCompanyUserRepository executionCompanyUserRepository, [FromServices] ExecutionCompanyRepository executionCompanyRepository,
        [FromQuery] PageCommand pageCommand, [FromRoute] Int32 id)
        {
            var response = actionResponseFactory.CreateInstance();
            var executionCompany = await executionCompanyRepository.GetAsync(ww => ww.ID == id);
            if (executionCompany is null)
            {
                response.AddNotFoundErr("ExecutionCompany");
                return response.ToIActionResult();
            }
            await executionCompanyRepository.DeleteAsync(executionCompany);
            return response.ToIActionResult();
        }

        [HttpPost("RemoveUser")]
        [HasPermission(typeof(AdminPermission))]
        public async Task<IActionResult> RemoveExecutionCompanyUser([FromServices] ExecutionCompanyUserRepository executionCompanyUserRepository, [FromServices] TeamUserRepository teamUserRepository, [FromBody] RemovingExecutionCompanyUserCommand command)
        {
            var response = actionResponseFactory.CreateInstance();
            var executionCompanyUser = await executionCompanyUserRepository.GetAsync(ww => ww.UserUUID == command.UserUUID);
            if (executionCompanyUser is null)
            {
                response.AddInvalidErr("UserUUID");
                return response.ToIActionResult();
            }

            await executionCompanyUserRepository.DeleteAsync(executionCompanyUser);

            response.SetUpdatedMessage();

            return response.ToIActionResult();
        }
        [HttpGet("GetExecutionCompanyFromTM")]
        [AllowAnonymous]
        public async Task<IActionResult> GetListFromTM([FromQuery] PageCommand pageCommand,
        [FromServices] ExecutionCompanyRepository executionCompanyRepository,
        [FromQuery] String? search, [FromQuery] String? secretKey, [FromServices] AuthGrpcClientService authGrpcClientService,
        [FromServices] IOptions<FarmAppSettings> options)
        {
            var response = actionResponseFactory.CreateInstance();
            var settings = options.Value;
            if (secretKey != settings.SecretKey)
            {
                response.AddAuthorizationErr();
                return response.ToIActionResult();
            }
            Expression<Func<ExecutionCompany, Boolean>> query = ww => (!String.IsNullOrWhiteSpace(search) ? ((ww.Name ?? "").ToLower().Contains(search.ToLower()) || (ww.Address ?? "").ToLower().Contains(search.ToLower())) : true);
            var listResponse = await executionCompanyRepository.GetListResponseViewAsync<ExecutionCompanyViewModel>(pageCommand, query);
            listResponse.SetResponse(response);

            return response.ToIActionResult();
        }
        [HttpPost("{id:int}/AssignUserFromTM")]
        [AllowAnonymous]
        public async Task<IActionResult> AssignExecutionCompanyUserFromTM([FromServices] ExecutionCompanyUserRepository executionCompanyUserRepository,
        [FromServices] ExecutionCompanyRepository executionCompanyRepository, [FromServices] AuthGrpcClientService authGrpcClientService, [FromBody] AssigningExecutionCompanyUserFromTMCommand command,
        [FromRoute] Int32 id, [FromServices] IOptions<FarmAppSettings> options)
        {
            var response = actionResponseFactory.CreateInstance();
            var resp = authGrpcClientService.GetUserExistingInformation(command.EncryptedUUID ?? "");
            var settings = options.Value;
            if (command.SecretKey != settings.SecretKey)
            {
                response.AddAuthorizationErr();
                return response.ToIActionResult();
            }
            if (resp.IsExist)
            {
                try
                {
                    var uuid = Guid.Parse(resp.DecryptedUUID);
                    ExecutionCompanyUser? executionCompanyUser = await executionCompanyUserRepository.GetAsync(ww => ww.UserUUID == uuid);
                    if (executionCompanyUser != null)
                    {
                        response.AddExistedErr("User");
                        return response.ToIActionResult();
                    }
                    ExecutionCompany? company = await executionCompanyRepository.GetAsync(ww => ww.ID == id);
                    if (company == null)
                    {
                        response.AddNotFoundErr("ExecutionCompany");
                        return response.ToIActionResult();
                    }
                    var user = await executionCompanyUserRepository.CreateAsync(new ExecutionCompanyUser { ExecutionCompany = company, UserUUID = uuid, Type = command.Type });
                    response.SetCreatedObject(user);
                }
                catch (Exception)
                {
                    response.AddInvalidErr("EncryptedUUID");
                    return response.ToIActionResult();
                }
            }
            else
            {
                response.AddInvalidErr("EncryptedUUID");
                return response.ToIActionResult();
            }
            return response.ToIActionResult();
        }

    }
}