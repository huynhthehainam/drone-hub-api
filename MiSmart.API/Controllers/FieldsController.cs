
using MiSmart.Infrastructure.Controllers;
using MiSmart.Infrastructure.Responses;
using Microsoft.AspNetCore.Mvc;
using System;
using MiSmart.DAL.Models;
using MiSmart.DAL.Repositories;
using MiSmart.Infrastructure.Commands;
using MiSmart.DAL.ViewModels;
using MiSmart.Infrastructure.ViewModels;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MiSmart.API.Controllers
{
    public class FieldsController : AuthorizedAPIControllerBase
    {
        public FieldsController(IActionResponseFactory actionResponseFactory) : base(actionResponseFactory)
        {
        }
        [HttpGet]
        public async Task<IActionResult> GetFields([FromServices] FieldRepository fieldRepository,
            [FromServices] ExecutionCompanyUserRepository executionCompanyUserRepository,
         [FromServices] CustomerUserRepository customerUserRepository, [FromQuery] PageCommand pageCommand, [FromQuery] DateTime? from,
         [FromQuery] DateTime? to, [FromQuery] String search,
         [FromQuery] String relation = "Owner")
        {
            var response = actionResponseFactory.CreateInstance();

            Expression<Func<Field, Boolean>> query = ww => false;
            if (relation == "Owner")
            {
                CustomerUser customerUser = await customerUserRepository.GetByPermissionAsync(CurrentUser.UUID);
                if (customerUser is null)
                {
                    response.AddNotAllowedErr();
                }
                query = ww => (ww.CustomerID == customerUser.CustomerID)
                    && (from.HasValue ? (ww.CreatedTime >= from.Value) : true)
                    && (to.HasValue ? (ww.CreatedTime <= to.Value) : true)
                    && (!String.IsNullOrWhiteSpace(search) ? (ww.Name.ToLower().Contains(search.ToLower()) || ww.FieldLocation.ToLower().Contains(search.ToLower()) || ww.FieldName.ToLower().Contains(search.ToLower())) : true);

            }
            else
            {
                ExecutionCompanyUser executionCompanyUser = await executionCompanyUserRepository.GetByPermissionAsync(CurrentUser.UUID);
                if (executionCompanyUser is null)
                {
                    response.AddNotAllowedErr();
                }
                query = ww => (ww.ExecutionCompanyID == executionCompanyUser.ExecutionCompanyID)
                                   && (from.HasValue ? (ww.CreatedTime >= from.Value) : true)
                                   && (to.HasValue ? (ww.CreatedTime <= to.Value) : true)
                                   && (!String.IsNullOrWhiteSpace(search) ? (ww.Name.ToLower().Contains(search.ToLower()) || ww.FieldLocation.ToLower().Contains(search.ToLower()) || ww.FieldName.ToLower().Contains(search.ToLower())) : true);
            }
            var listResponse = await fieldRepository.GetListResponseViewAsync<FieldViewModel>(pageCommand, query, ww => ww.CreatedTime, false);
            listResponse.SetResponse(response);




            return response.ToIActionResult();
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetFields([FromServices] FieldRepository fieldRepository, [FromServices] CustomerUserRepository customerUserRepository, [FromRoute] Int64 id)
        {
            var response = actionResponseFactory.CreateInstance();

            CustomerUser customerUser = await customerUserRepository.GetByPermissionAsync(CurrentUser.UUID);
            if (customerUser is null)
            {
                response.AddNotAllowedErr();
            }
            var field = await fieldRepository.GetAsync(ww => ww.ID == id && ww.CustomerID == customerUser.CustomerID);
            if (field is null)
            {
                response.AddNotFoundErr("Field");
            }

            response.SetData(ViewModelHelpers.ConvertToViewModel<Field, LargeFieldViewModel>(field));

            return response.ToIActionResult();
        }
    }
}