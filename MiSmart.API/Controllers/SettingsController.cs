
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
using MiSmart.API.Settings;

namespace MiSmart.API.Controllers
{
    public class SettingsController : AuthorizedAPIControllerBase
    {
        public SettingsController(IActionResponseFactory actionResponseFactory) : base(actionResponseFactory)
        {
        }

        [HttpGet("ConversionSettings")]
        public IActionResult GetConversionSettings([FromServices] IOptions<ConversionSettings> options)
        {
            var response = actionResponseFactory.CreateInstance();
            ConversionSettings settings = options.Value;
            response.Data = settings;
            return response.ToIActionResult();
        }
    }
}