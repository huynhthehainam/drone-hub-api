
using MiSmart.Infrastructure.Controllers;
using MiSmart.Infrastructure.Responses;
using Microsoft.AspNetCore.Mvc;
using System;
using MiSmart.DAL.Models;
using MiSmart.DAL.Repositories;
using MiSmart.Infrastructure.Commands;
using MiSmart.DAL.ViewModels;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MiSmart.API.Controllers
{
    public class StreamingLinksController : AuthorizedAPIControllerBase
    {
        public StreamingLinksController(IActionResponseFactory actionResponseFactory) : base(actionResponseFactory)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetListLinks([FromQuery] PageCommand pageCommand,
        [FromServices] CustomerUserRepository customerUserRepository,
        [FromServices] ExecutionCompanyUserRepository executionCompanyUserRepository,
         [FromServices] StreamingLinkRepository streamingLinkRepository, [FromQuery] String? relation = "Owner")
        {
            ActionResponse actionResponse = actionResponseFactory.CreateInstance();

            Expression<Func<StreamingLink, Boolean>> query = ww => false;
            if (relation == "Owner")
            {

                var customerUser = await customerUserRepository.GetByPermissionAsync(CurrentUser.UUID);
                if (customerUser is null)
                {
                    actionResponse.AddNotAllowedErr();
                    return actionResponse.ToIActionResult();
                }
                query = ww => (ww.Device != null ? ww.Device.CustomerID == customerUser.CustomerID : false);
            }
            else if (relation == "Administrator")
            {
                query = ww => true;
            }
            else
            {
                var executionCompanyUser = await executionCompanyUserRepository.GetByPermissionAsync(CurrentUser.UUID);
                if (executionCompanyUser is null)
                {
                    actionResponse.AddNotAllowedErr();
                    return actionResponse.ToIActionResult();
                }

                query = ww => ww.Device != null ? ww.Device.ExecutionCompanyID == executionCompanyUser.ExecutionCompanyID : false;
            }
            var listResponse = await streamingLinkRepository.GetListResponseViewAsync<StreamingLinkViewMode>(pageCommand, query, ww => ww.CreatedTime, false);

            listResponse.SetResponse(actionResponse);


            return actionResponse.ToIActionResult();
        }
    }
}