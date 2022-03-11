

using MiSmart.Infrastructure.Controllers;
using MiSmart.Infrastructure.Responses;
using Microsoft.AspNetCore.Mvc;
using System;
using MiSmart.DAL.Models;
using MiSmart.DAL.Repositories;
using System.Linq;
using MiSmart.Infrastructure.Commands;
using MiSmart.DAL.ViewModels;
using MiSmart.Infrastructure.ViewModels;
using System.Collections.Generic;

namespace MiSmart.API.Controllers
{
    public class UsersController : AuthorizedAPIControllerBase
    {
        public UsersController(IActionResponseFactory actionResponseFactory) : base(actionResponseFactory)
        {
        }
        [HttpGet("me")]
        public IActionResult GetProfile([FromServices] CustomerUserRepository customerUserRepository)
        {
            var response = actionResponseFactory.CreateInstance();
            Console.WriteLine(CurrentUser);

            Console.WriteLine(CurrentUser.ID);
            var customerUser = customerUserRepository.Get(ww => ww.UserID == CurrentUser.ID);
            if (customerUser is null)
            {
                response.AddNotFoundErr("User");
            }
            response.SetData(new
            {
                ID = customerUser.UserID,
                Customer = ViewModelHelpers.ConvertToViewModel<Customer, SmallCustomerViewModel>(customerUser.Customer),
                Type = customerUser.Type,
            });




            return response.ToIActionResult();
        }
        [HttpGet]
        public IActionResult GetList([FromServices] CustomerUserRepository customerUserRepository, [FromQuery] Boolean? isAssigned)
        {
            var response = actionResponseFactory.CreateInstance();

            if (isAssigned is not null && isAssigned.GetValueOrDefault() == true)
            {
                var customerUsers = customerUserRepository.GetListEntities(new PageCommand(), ww => true);
                List<Int64> ids = customerUsers.Select(ww => ww.UserID).ToList().Distinct().ToList();
                response.SetData(ids);
            }
            else
            {
                response.SetData(new List<Object> { });
            }



            return response.ToIActionResult();
        }
    }
}