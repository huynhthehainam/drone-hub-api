

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
using MiSmart.DAL.ViewModels;




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
            var customerUser = customerUserRepository.Get(ww => ww.UserID == CurrentUser.ID);
            if (customerUser is not null)
            {
                response.Data = new { ID = CurrentUser.ID, CustomerID = customerUser.CustomerID, CustomerName = customerUser.Customer.Name };
            }
            else
            {
                response.AddNotFoundErr("User");
            }



            return response.ToIActionResult();
        }
    }
}