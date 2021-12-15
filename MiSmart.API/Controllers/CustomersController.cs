

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
            Int32? customerID = command.CustomerID;
            if (!CurrentUser.IsAdmin || customerID is null)
            {
                customerID = customerUserRepository.HasOwnerPermission(CurrentUser);
            }
            if (customerID is null)
            {
                response.AddNotFoundErr("Company");

            }

            if (customerUserRepository.Any(ww => ww.CustomerID == customerID.GetValueOrDefault() && ww.UserID == command.UserID))
            {
                response.AddExistedErr("UserID");
            }

            CustomerUser customerUser = new CustomerUser { CustomerID = customerID.GetValueOrDefault(), UserID = command.UserID.Value, Type = command.Type };
            customerUserRepository.Create(customerUser);
            response.SetCreatedObject(customerUser);



            return response.ToIActionResult();
        }







    }
}