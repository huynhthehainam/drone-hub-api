using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiSmart.API.Commands;
using MiSmart.DAL.Models;
using MiSmart.DAL.Repositories;
using MiSmart.DAL.ViewModels;
using MiSmart.Infrastructure.Commands;
using MiSmart.Infrastructure.Controllers;
using MiSmart.Infrastructure.Responses;

namespace MiSmart.API.Controllers
{
    public class PartsController : AuthorizedAPIControllerBase
    {
        public PartsController(IActionResponseFactory actionResponseFactory) : base(actionResponseFactory)
        {
        }
        [HttpPost]
        public async Task<IActionResult> CreatePart([FromBody] AddingPartCommand command, [FromServices] PartRepository partRepository){
            ActionResponse response = actionResponseFactory.CreateInstance();
            if (!CurrentUser.IsAdministrator || CurrentUser.RoleID != 3){
                response.AddNotAllowedErr();
            }
            var part = new Part(){
                Group = command.Group,
                Name = command.Name,
            };
            await partRepository.CreateAsync(part);
            response.SetCreatedObject(part);
            return response.ToIActionResult();    
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetPart([FromServices] PartRepository partRepository, [FromQuery] PageCommand pageCommand){
            ActionResponse response = actionResponseFactory.CreateInstance();
            var listPart = await partRepository.GetListResponseViewAsync<PartViewModel>(pageCommand, ww => true, ww => ww.ID);
            listPart.SetResponse(response);
            return response.ToIActionResult();
        }
    }
}