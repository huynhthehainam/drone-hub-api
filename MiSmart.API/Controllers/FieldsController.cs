
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
using MiSmart.Infrastructure.ViewModels;
using System.Linq.Expressions;
using MiSmart.DAL.Responses;

namespace MiSmart.API.Controllers
{
    public class FieldsController : AuthorizedAPIControllerBase
    {
        public FieldsController(IActionResponseFactory actionResponseFactory) : base(actionResponseFactory)
        {
        }
        [HttpGet]
        public IActionResult GetFields([FromServices] FieldRepository fieldRepository, [FromServices] CustomerUserRepository customerUserRepository, [FromQuery] PageCommand pageCommand, [FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] String search, [FromQuery] String mode = "Small")
        {
            var response = actionResponseFactory.CreateInstance();
            CustomerUserPermission customerUserPermission = customerUserRepository.GetMemberPermission(CurrentUser);
            if (customerUserPermission is null)
            {
                response.AddNotAllowedErr();
            }

            Expression<Func<Field, Boolean>> query = ww => (ww.CustomerID == customerUserPermission.CustomerID)
                && (from.HasValue ? (ww.CreatedTime >= from.Value) : true)
                && (to.HasValue ? (ww.CreatedTime <= to.Value) : true)
                && (!String.IsNullOrWhiteSpace(search) ? (ww.Name.ToLower().Contains(search.ToLower()) || ww.FieldLocation.ToLower().Contains(search.ToLower()) || ww.FieldName.ToLower().Contains(search.ToLower())) : true);
            if (mode == "Large")
            {

            }
            else
            {
                var listResponse = fieldRepository.GetListResponseView<FieldViewModel>(pageCommand, query, ww => ww.CreatedTime, false);
                listResponse.SetResponse(response);
            }



            return response.ToIActionResult();
        }

        [HttpGet("{id:int}")]
        public IActionResult GetFields([FromServices] FieldRepository fieldRepository, [FromServices] CustomerUserRepository customerUserRepository, [FromRoute] Int64 id)
        {
            var response = actionResponseFactory.CreateInstance();

            CustomerUserPermission customerUserPermission = customerUserRepository.GetMemberPermission(CurrentUser);
            if (customerUserPermission is null)
            {
                response.AddNotAllowedErr();
            }
            var field = fieldRepository.Get(ww => ww.ID == id && ww.CustomerID == customerUserPermission.CustomerID);
            if (field is null)
            {
                response.AddNotFoundErr("Field");
            }

            response.SetData(ViewModelHelpers.ConvertToViewModel<Field, LargeFieldViewModel>(field));

            return response.ToIActionResult();
        }
    }
}