

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
using System.Threading.Tasks;

namespace MiSmart.API.Controllers
{
    public class UsersController : AuthorizedAPIControllerBase
    {
        public UsersController(IActionResponseFactory actionResponseFactory) : base(actionResponseFactory)
        {
        }
        [HttpGet("me")]
        public async Task<IActionResult> GetProfile([FromServices] CustomerUserRepository customerUserRepository, [FromServices] ExecutionCompanyUserRepository executionCompanyUserRepository)
        {
            var response = actionResponseFactory.CreateInstance();
            var customerUser = await customerUserRepository.GetAsync(ww => ww.UserID == CurrentUser.ID);
            ExecutionCompanyUser executionCompanyUser = await executionCompanyUserRepository.GetAsync(ww => ww.UserID == CurrentUser.ID);
            if (customerUser is null && executionCompanyUser is null)
            {
                response.AddNotFoundErr("User");
            }
            response.SetData(new
            {
                CustomerUser = customerUser is not null ? new
                {
                    Customer = ViewModelHelpers.ConvertToViewModel<Customer, SmallCustomerViewModel>(customerUser.Customer)
                } : null,
                ExecutionCompanyUser = executionCompanyUser is not null ? new
                {
                    ExecutionCompany = ViewModelHelpers.ConvertToViewModel<ExecutionCompany, ExecutionCompanyViewModel>(executionCompanyUser.ExecutionCompany),
                    Type = executionCompanyUser.Type,
                } : null
            });




            return response.ToIActionResult();
        }
        [HttpGet]
        public async Task<IActionResult> GetList([FromServices] CustomerUserRepository customerUserRepository, [FromQuery] Boolean? isAssigned)
        {
            var response = actionResponseFactory.CreateInstance();

            if (isAssigned is not null && isAssigned.GetValueOrDefault() == true)
            {
                var customerUsers = await customerUserRepository.GetListEntitiesAsync(new PageCommand(), ww => true);
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