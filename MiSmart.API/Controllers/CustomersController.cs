

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
using System.Linq.Expressions;
using MiSmart.DAL.ViewModels;
using MiSmart.Infrastructure.Permissions;
using MiSmart.API.Permissions;
using MiSmart.DAL.Responses;
using MiSmart.Infrastructure.ViewModels;
using MiSmart.API.GrpcServices;

namespace MiSmart.API.Controllers
{
    public class CustomersController : AuthorizedAPIControllerBase
    {

        private readonly CustomerRepository customerRepository;
        public CustomersController(IActionResponseFactory actionResponseFactory, CustomerRepository customerRepository) : base(actionResponseFactory)
        {
            this.customerRepository = customerRepository;
        }
        [HttpPost]
        [HasPermission(typeof(AdminPermission))]
        public IActionResult Create([FromBody] AddingCustomerCommand command)
        {
            var response = actionResponseFactory.CreateInstance();

            if (!CurrentUser.IsAdmin)
            {
                response.AddNotAllowedErr();
            }
            var customer = new Customer { Name = command.Name, Address = command.Address };
            customerRepository.Create(customer);
            response.SetCreatedObject(customer);

            return response.ToIActionResult();
        }
        [HttpGet]
        [HasPermission(typeof(AdminPermission))]
        public IActionResult GetList([FromQuery] PageCommand pageCommand, [FromQuery] String search, [FromQuery] String mode = "Small")
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

        public IActionResult AssignCustomerUser([FromServices] CustomerUserRepository customerUserRepository, [FromServices] AuthGrpcClientService authGrpcClientService, [FromBody] AssigningCustomerUserCommand command, [FromRoute] Int32 id)
        {
            var response = actionResponseFactory.CreateInstance();
            var customer = customerRepository.Get(ww => ww.ID == id);
            var userExistingInformation = authGrpcClientService.GetUserExistingInformation(command.UserID.GetValueOrDefault());
            if (!userExistingInformation.IsExist)
            {
                response.AddInvalidErr("UserID");
            }
            if (customer is null)
            {
                response.AddNotFoundErr("Customer");
            }
            var existedCustomerUser = customerUserRepository.Get(ww => ww.UserID == command.UserID.GetValueOrDefault());
            if (existedCustomerUser is not null)
            {
                response.AddExistedErr("User");
            }
            CustomerUser customerUser = new CustomerUser { CustomerID = id, UserID = command.UserID.Value, Type = command.Type };
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

            var userIDs = customerUserRepository.GetListEntities(pageCommand, query).Select(ww => ww.UserID).ToList();
            response.SetData(new { UserIDs = userIDs });

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
        public IActionResult GetCustomerUsers([FromServices] CustomerUserRepository customerUserRepository, [FromQuery] PageCommand pageCommand, [FromRoute] Int32 id)
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
    }
}