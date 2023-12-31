

using MiSmart.Infrastructure.Controllers;
using MiSmart.Infrastructure.Responses;
using MiSmart.API.Commands;
using Microsoft.AspNetCore.Mvc;
using System;
using MiSmart.DAL.Models;
using MiSmart.DAL.Repositories;
using System.Linq;
using MiSmart.Infrastructure.Commands;
using System.Linq.Expressions;
using MiSmart.DAL.ViewModels;
using MiSmart.Infrastructure.Permissions;
using MiSmart.API.Permissions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using MiSmart.API.GrpcServices;
using Microsoft.Extensions.Options;
using MiSmart.API.Settings;

namespace MiSmart.API.Controllers
{
    public class CustomersController : AuthorizedAPIControllerBase
    {

        public CustomersController(IActionResponseFactory actionResponseFactory) : base(actionResponseFactory)
        {
        }
        [HttpPost]

        public async Task<IActionResult> Create([FromServices] CustomerRepository customerRepository, [FromBody] AddingCustomerCommand command)
        {
            var response = actionResponseFactory.CreateInstance();
            if (!CurrentUser.IsAdministrator && CurrentUser.RoleID != 3)
            {
                response.AddNotAllowedErr();
                return response.ToIActionResult();
            }

            var customer = await customerRepository.CreateAsync(new Customer { Name = command.Name, Address = command.Address });
            response.SetCreatedObject(customer);

            return response.ToIActionResult();
        }
        [HttpGet]
        public async Task<IActionResult> GetList([FromQuery] PageCommand pageCommand,
        [FromServices] CustomerRepository customerRepository,
        [FromQuery] String? search)
        {
            var response = actionResponseFactory.CreateInstance();
            Expression<Func<Customer, Boolean>> query = ww => (!String.IsNullOrWhiteSpace(search) ? (ww.Name ?? "").ToLower().Contains(search.ToLower()) : true);

            var listResponse = await customerRepository.GetListResponseViewAsync<SmallCustomerViewModel>(pageCommand, query);
            listResponse.SetResponse(response);


            return response.ToIActionResult();
        }

        [HttpPost("{id:int}/AssignUser")]
        [HasPermission(typeof(AdminPermission))]
        public async Task<IActionResult> AssignCustomerUser([FromServices] CustomerUserRepository customerUserRepository,
        [FromServices] CustomerRepository customerRepository,
        [FromServices] AuthGrpcClientService authGrpcClientService,
         [FromBody] AssigningCustomerUserCommand command, [FromRoute] Int32 id)
        {
            var response = actionResponseFactory.CreateInstance();
            var userInfo = authGrpcClientService.GetUserInfo(command.UserUUID.GetValueOrDefault());
            if (userInfo == null)
            {
                response.AddInvalidErr("UserUUID");
            }
            var customer = await customerRepository.GetAsync(ww => ww.ID == id);

            if (customer is null)
            {
                response.AddNotFoundErr("Customer");
            }
            var existedCustomerUser = await customerUserRepository.GetAsync(ww => ww.UserUUID == command.UserUUID.GetValueOrDefault());
            if (existedCustomerUser is not null)
            {
                response.AddExistedErr("User");
            }
            CustomerUser customerUser = await customerUserRepository.CreateAsync(new CustomerUser { CustomerID = id, UserUUID = command.UserUUID.GetValueOrDefault() });
            response.SetCreatedObject(customerUser);

            return response.ToIActionResult();
        }

        [HttpPatch("{id:int}")]
        [HasPermission(typeof(AdminPermission))]
        public async Task<IActionResult> UpdateCustomer([FromRoute] Int32 id, [FromServices] CustomerRepository customerRepository, [FromBody] UpdatingCustomerCommand command)
        {
            var response = actionResponseFactory.CreateInstance();
            var customer = await customerRepository.GetAsync(ww => ww.ID == id);

            if (customer is null)
            {
                response.AddNotFoundErr("Customer");
                return response.ToIActionResult();
            }
            customer.Name = String.IsNullOrEmpty(command.Name) ? customer.Name : command.Name;
            customer.Address = String.IsNullOrEmpty(command.Address) ? customer.Address : command.Address;


            await customerRepository.UpdateAsync(customer);
            response.SetUpdatedMessage();

            return response.ToIActionResult();
        }


        [HttpGet("AssignedUsers")]
        [HasPermission(typeof(AdminPermission))]
        public Task<IActionResult> GetCurrentCustomerUsers([FromServices] CustomerUserRepository customerUserRepository)
        {
            var response = actionResponseFactory.CreateInstance();
            var assignedUserUUIDs = customerUserRepository.GetListEntitiesAsync(new PageCommand(), ww => true).Result.Select(ww => ww.UserUUID).ToList();
            response.SetData(new { AssignedUserUUIDs = assignedUserUUIDs });
            return Task.FromResult(response.ToIActionResult());
        }

        [HttpGet("{id:int}/Users")]
        [HasPermission(typeof(AdminPermission))]
        public async Task<IActionResult> GetCustomerUsers([FromServices] CustomerUserRepository customerUserRepository, [FromServices] CustomerRepository customerRepository, [FromQuery] PageCommand pageCommand, [FromRoute] Int32 id)
        {
            var response = actionResponseFactory.CreateInstance();
            var customer = await customerRepository.GetAsync(ww => ww.ID == id);
            if (customer is null)
            {
                response.AddNotFoundErr("Customer");
                return response.ToIActionResult();
            }

            Expression<Func<CustomerUser, Boolean>> query = ww => ww.CustomerID == id;

            var listResponse = await customerUserRepository.GetListResponseViewAsync<CustomerUserViewModel>(pageCommand, query);
            listResponse.SetResponse(response);

            return response.ToIActionResult();
        }
        [HttpGet("{id:int}/Devices")]
        [HasPermission(typeof(AdminPermission))]
        public async Task<IActionResult> GetCustomerDevices([FromServices] DeviceRepository deviceRepository, [FromServices] CustomerRepository customerRepository, [FromQuery] PageCommand pageCommand, [FromRoute] Int32 id)
        {
            var response = actionResponseFactory.CreateInstance();
            var customer = await customerRepository.GetAsync(ww => ww.ID == id);
            if (customer is null)
            {
                response.AddNotFoundErr("Customer");
                return response.ToIActionResult();
            }

            var listResponse = await deviceRepository.GetListResponseViewAsync<LargeDeviceViewModel>(pageCommand, ww => ww.CustomerID == customer.ID);
            listResponse.SetResponse(response);

            return response.ToIActionResult();
        }

        [HttpDelete("{id:int}")]
        [HasPermission(typeof(AdminPermission))]
        public async Task<IActionResult> DeleteCustomer([FromServices] CustomerUserRepository customerUserRepository, [FromServices] CustomerRepository customerRepository,
        [FromQuery] PageCommand pageCommand, [FromRoute] Int32 id)
        {
            var response = actionResponseFactory.CreateInstance();
            var customer = await customerRepository.GetAsync(ww => ww.ID == id);
            if (customer is null)
            {
                response.AddNotFoundErr("Customer");
                return response.ToIActionResult();
            }
            await customerRepository.DeleteAsync(customer);
            return response.ToIActionResult();
        }

        [HttpPost("RemoveUser")]
        [HasPermission(typeof(AdminPermission))]
        public async Task<IActionResult> RemoveCustomerUser([FromServices] CustomerUserRepository customerUserRepository, [FromServices] TeamUserRepository teamUserRepository, [FromBody] RemovingCustomerUserCommand command)
        {
            var response = actionResponseFactory.CreateInstance();
            var customerUser = await customerUserRepository.GetAsync(ww => ww.UserUUID == command.UserUUID);
            if (customerUser is null)
            {
                response.AddInvalidErr("UserUUID");
                return response.ToIActionResult();
            }

            await customerUserRepository.DeleteAsync(customerUser);

            response.SetNoContent();

            return response.ToIActionResult();
        }

        [HttpPost("{id:int}/Devices")]
        public async Task<IActionResult> AddDevice([FromRoute] Int32 id, [FromServices] DeviceModelRepository deviceModelRepository, [FromServices] CustomerRepository customerRepository,
         [FromBody] AddingDeviceCommand command)

        {
            var response = actionResponseFactory.CreateInstance();
            if (!CurrentUser.IsAdministrator && CurrentUser.RoleID != 3)
            {
                response.AddNotAllowedErr();
            }
            var customer = await customerRepository.GetAsync(ww => ww.ID == id);
            if (customer is null)
            {
                response.AddNotFoundErr("Customer");
                return response.ToIActionResult();
            }
            var deviceModel = await deviceModelRepository.GetAsync(ww => ww.ID == command.DeviceModelID);
            if (deviceModel is null)
            {
                response.AddInvalidErr("DeviceModelID");
                return response.ToIActionResult();
            }
            var device = new Device
            {
                DeviceModel = deviceModel,
                Name = command.Name,
            };
            customer.Devices?.Add(device);
            await customerRepository.UpdateAsync(customer);
            response.SetCreatedObject(device);
            return response.ToIActionResult();
        }

        [HttpPost("RemoveDevice")]
        [HasPermission(typeof(AdminPermission))]
        public async Task<IActionResult> RemoveDevice([FromServices] DeviceRepository deviceRepository, [FromBody] RemovingDeviceCommand command)
        {
            var response = actionResponseFactory.CreateInstance();
            var device = await deviceRepository.GetAsync(ww => ww.ID == command.DeviceID);
            if (device is null)
            {
                response.AddInvalidErr("DeviceID");
                return response.ToIActionResult();
            }

            await deviceRepository.DeleteAsync(device);

            response.SetNoContent();
            return response.ToIActionResult();
        }

        [HttpGet("GetCustomerFromTM")]
        [AllowAnonymous]
        public async Task<IActionResult> GetListFromTM([FromQuery] PageCommand pageCommand,
        [FromServices] CustomerRepository customerRepository, [FromQuery] String? search, [FromQuery] String? secretKey,
        [FromServices] AuthGrpcClientService authGrpcClientService, [FromServices] IOptions<FarmAppSettings> options,
        [FromQuery] Int32? customerID)
        {
            ActionResponse response = actionResponseFactory.CreateInstance();
            Expression<Func<Customer, Boolean>> query = ww => (!String.IsNullOrWhiteSpace(search) ? ((ww.Name ?? "").ToLower().Contains(search.ToLower()) || (ww.Address ?? "").ToLower().Contains(search.ToLower())) : true)
                                                && (customerID.HasValue ? ww.ID == customerID : true);
            var settings = options.Value;
            if (secretKey != settings.SecretKey)
            {
                response.AddAuthorizationErr();
                return response.ToIActionResult();
            }
            var listResponse = await customerRepository.GetListResponseViewAsync<LargeCustomerViewModel>(pageCommand, query);
            listResponse.SetResponse(response);

            return response.ToIActionResult();
        }

        [HttpPost("{id:int}/AssignUserFromTM")]
        [AllowAnonymous]
        public async Task<IActionResult> AssginCustomerUserFromTM([FromServices] CustomerRepository customerRepository, [FromBody] AssigningCustomerUserFromTMCommand command,
        [FromServices] IOptions<FarmAppSettings> options, [FromServices] CustomerUserRepository customerUserRepository,
        [FromServices] AuthGrpcClientService authGrpcClientService, [FromRoute] Int32 id)
        {
            var response = actionResponseFactory.CreateInstance();
            var settings = options.Value;
            if (command.SecretKey != settings.SecretKey)
            {
                response.AddAuthorizationErr();
                return response.ToIActionResult();
            }
            var resp = authGrpcClientService.GetUserExistingInformation(command.EncryptedUUID ?? "");
            if (resp.IsExist)
            {
                try
                {
                    var uuid = Guid.Parse(resp.DecryptedUUID);
                    var existedCustomerUser = await customerUserRepository.GetAsync(ww => ww.UserUUID == uuid);
                    if (existedCustomerUser is not null)
                    {
                        response.AddExistedErr("User");
                        return response.ToIActionResult();
                    }
                    var customer = await customerRepository.GetAsync(ww => ww.ID == id);
                    if (customer is null)
                    {
                        response.AddNotFoundErr("Customer");
                        return response.ToIActionResult();
                    }
                    CustomerUser customerUser = await customerUserRepository.CreateAsync(new CustomerUser { CustomerID = customer.ID, UserUUID = uuid });
                    response.SetCreatedObject(customerUser);
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