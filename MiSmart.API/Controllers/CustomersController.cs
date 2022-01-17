

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

        [HttpPost("AssignUser")]
        public IActionResult AssignCustomerUser([FromServices] CustomerUserRepository customerUserRepository, [FromBody] AssigningCustomerUserCommand command)
        {
            var response = actionResponseFactory.CreateInstance();
            CustomerUserPermission customerUserPermission = customerUserRepository.GetMemberPermission(CurrentUser, CustomerMemberType.Owner);

            if (customerUserPermission is null)
            {
                response.AddNotAllowedErr();
            }

            if (customerUserRepository.Any(ww => ww.CustomerID == customerUserPermission.CustomerID && ww.UserID == command.UserID))
            {
                response.AddExistedErr("UserID");
            }

            CustomerUser customerUser = new CustomerUser { CustomerID = customerUserPermission.CustomerID, UserID = command.UserID.Value, Type = command.Type };
            customerUserRepository.Create(customerUser);
            response.SetCreatedObject(customerUser);



            return response.ToIActionResult();
        }

        [HttpGet("AssignedUsers")]
        [HasPermission(typeof(AdminPermission))]
        public IActionResult GetCurrentCustomerUsers([FromServices] CustomerUserRepository customerUserRepository, [FromServices] TeamUserRepository teamUserRepository)
        {
            var response = actionResponseFactory.CreateInstance();
            var assignedUserIDs = teamUserRepository.GetListEntities(new PageCommand(), ww => true).Select(ww => ww.UserID).ToList();
            response.SetData(new { AssignedUserIDs = assignedUserIDs });
            return response.ToIActionResult();
        }

        [HttpGet("{id:int}/Users")]
        [HasPermission(typeof(AdminPermission))]
        public IActionResult GetCustomerUsers([FromServices] CustomerUserRepository customerUserRepository, [FromQuery] PageCommand pageCommand, [FromRoute] Int32 id)
        {
            var response = actionResponseFactory.CreateInstance();

            Expression<Func<CustomerUser, Boolean>> query = ww => ww.CustomerID == id;

            var userIDs = customerUserRepository.GetListEntities(pageCommand, query).Select(ww => ww.UserID).ToList();
            response.SetData(new { UserIDs = userIDs });

            return response.ToIActionResult();
        }
    }
}