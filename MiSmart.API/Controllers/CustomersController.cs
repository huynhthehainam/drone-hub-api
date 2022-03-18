

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
using MiSmart.API.GrpcServices;

namespace MiSmart.API.Controllers
{
    public class CustomersController : AuthorizedAPIControllerBase
    {

        public CustomersController(IActionResponseFactory actionResponseFactory) : base(actionResponseFactory)
        {
        }
        [HttpPost]
        [HasPermission(typeof(AdminPermission))]
        public IActionResult Create([FromServices] CustomerRepository customerRepository, [FromBody] AddingCustomerCommand command)
        {
            var response = actionResponseFactory.CreateInstance();


            var customer = new Customer { Name = command.Name, Address = command.Address };
            customerRepository.Create(customer);
            response.SetCreatedObject(customer);

            return response.ToIActionResult();
        }
        [HttpGet]
        [HasPermission(typeof(AdminPermission))]
        public IActionResult GetList([FromQuery] PageCommand pageCommand,
        [FromServices] CustomerRepository customerRepository,
        [FromQuery] String search, [FromQuery] String mode = "Small")
        {
            var response = actionResponseFactory.CreateInstance();
            Expression<Func<Customer, Boolean>> query = ww => (!String.IsNullOrWhiteSpace(search) ? (ww.Name.ToLower().Contains(search.ToLower()) || ww.Address.ToLower().Contains(search.ToLower())) : true);
            if (mode == "Large")
            {
                // var listResponse = customerRepository.GetListResponseView<SmallCustomerViewModel>(pageCommand, query);
                // listResponse.SetResponse(response);
            }
            else
            {
                var listResponse = customerRepository.GetListResponseView<SmallCustomerViewModel>(pageCommand, query);
                listResponse.SetResponse(response);
            }

            return response.ToIActionResult();
        }

        [HttpPost("{id:int}/AssignUser")]
        [HasPermission(typeof(AdminPermission))]
        public IActionResult AssignCustomerUser([FromServices] CustomerUserRepository customerUserRepository,
        [FromServices] CustomerRepository customerRepository,
        [FromServices] AuthGrpcClientService authGrpcClientService, [FromBody] AssigningCustomerUserCommand command, [FromRoute] Int32 id)
        {
            var response = actionResponseFactory.CreateInstance();
            var customer = customerRepository.Get(ww => ww.ID == id);
            if (customer is null)
            {
                response.AddNotFoundErr("Customer");
            }
            var existedCustomerUser = customerUserRepository.Get(ww => ww.UserID == command.UserID.GetValueOrDefault());
            if (existedCustomerUser is not null)
            {
                response.AddExistedErr("User");
            }
            CustomerUser customerUser = new CustomerUser { CustomerID = id, UserID = command.UserID.Value };
            customerUserRepository.Create(customerUser);
            response.SetCreatedObject(customerUser);



            return response.ToIActionResult();
        }


        [HttpGet("AssignedUsers")]
        [HasPermission(typeof(AdminPermission))]
        public IActionResult GetCurrentCustomerUsers([FromServices] CustomerUserRepository customerUserRepository)
        {
            var response = actionResponseFactory.CreateInstance();
            var assignedUserIDs = customerUserRepository.GetListEntities(new PageCommand(), ww => true).Select(ww => ww.UserID).ToList();
            response.SetData(new { AssignedUserIDs = assignedUserIDs });
            return response.ToIActionResult();
        }

        [HttpGet("{id:int}/Users")]
        [HasPermission(typeof(AdminPermission))]
        public IActionResult GetCustomerUsers([FromServices] CustomerUserRepository customerUserRepository, [FromServices] CustomerRepository customerRepository, [FromQuery] PageCommand pageCommand, [FromRoute] Int32 id)
        {
            var response = actionResponseFactory.CreateInstance();
            var customer = customerRepository.Get(ww => ww.ID == id);
            if (customer is null)
            {
                response.AddNotFoundErr("Customer");
            }

            Expression<Func<CustomerUser, Boolean>> query = ww => ww.CustomerID == id;

            var listResponse = customerUserRepository.GetListResponseView<CustomerUserViewModel>(pageCommand, query);
            listResponse.SetResponse(response);

            return response.ToIActionResult();
        }
        [HttpGet("{id:int}/Devices")]
        [HasPermission(typeof(AdminPermission))]
        public IActionResult GetCustomerDevices([FromServices] DeviceRepository deviceRepository, [FromServices] CustomerRepository customerRepository, [FromQuery] PageCommand pageCommand, [FromRoute] Int32 id)
        {
            var response = actionResponseFactory.CreateInstance();
            var customer = customerRepository.Get(ww => ww.ID == id);
            if (customer is null)
            {
                response.AddNotFoundErr("Customer");
            }

            var listResponse = deviceRepository.GetListResponseView<SmallDeviceViewModel>(pageCommand, ww => ww.CustomerID == customer.ID);
            listResponse.SetResponse(response);

            return response.ToIActionResult();
        }

        [HttpDelete("{id:int}")]
        [HasPermission(typeof(AdminPermission))]
        public IActionResult DeleteCustomer([FromServices] CustomerUserRepository customerUserRepository, [FromServices] CustomerRepository customerRepository,
        [FromQuery] PageCommand pageCommand, [FromRoute] Int32 id)
        {
            var response = actionResponseFactory.CreateInstance();
            var customer = customerRepository.Get(ww => ww.ID == id);
            if (customer is null)
            {
                response.AddNotFoundErr("Customer");
            }
            customerRepository.Delete(customer);
            return response.ToIActionResult();
        }

        [HttpPost("RemoveUser")]
        [HasPermission(typeof(AdminPermission))]
        public IActionResult RemoveCustomerUser([FromServices] CustomerUserRepository customerUserRepository, [FromServices] TeamUserRepository teamUserRepository, [FromBody] RemovingCustomerUserCommand command)
        {
            var response = actionResponseFactory.CreateInstance();
            var customerUser = customerUserRepository.Get(ww => ww.UserID == command.UserID);
            if (customerUser is null)
            {
                response.AddInvalidErr("UserID");
            }

            customerUserRepository.Delete(customerUser);

            response.SetNoContent();

            return response.ToIActionResult();
        }

        [HttpPost("{id:int}/Devices")]
        [HasPermission(typeof(AdminPermission))]
        public IActionResult AddDevice([FromRoute] Int32 id, [FromServices] DeviceModelRepository deviceModelRepository, [FromServices] CustomerRepository customerRepository,
         [FromBody] AddingDeviceCommand command)

        {
            var response = actionResponseFactory.CreateInstance();
            var customer = customerRepository.Get(ww => ww.ID == id);
            if (customer is null)
            {
                response.AddNotFoundErr("Customer");
            }
            var deviceModel = deviceModelRepository.Get(ww => ww.ID == command.DeviceModelID);
            if (deviceModel is null)
            {
                response.AddInvalidErr("DeviceModelID");
            }
            var device = new Device
            {
                DeviceModel = deviceModel,
                Name = command.Name,
            };
            customer.Devices.Add(device);
            customerRepository.Update(customer);
            response.SetCreatedObject(device);
            return response.ToIActionResult();
        }

        [HttpPost("RemoveDevice")]
        [HasPermission(typeof(AdminPermission))]
        public IActionResult RemoveDevice([FromServices] DeviceRepository deviceRepository, [FromBody] RemovingDeviceCommand command)
        {
            var response = actionResponseFactory.CreateInstance();
            var device = deviceRepository.Get(ww => ww.ID == command.DeviceID);
            if (device is null)
            {
                response.AddInvalidErr("DeviceID");
            }

            deviceRepository.Delete(device);

            response.SetNoContent();
            return response.ToIActionResult();
        }
    }
}