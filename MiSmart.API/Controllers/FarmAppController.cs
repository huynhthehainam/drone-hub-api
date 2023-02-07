

using MiSmart.Infrastructure.Controllers;
using MiSmart.Infrastructure.Responses;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using Microsoft.Extensions.Options;
using MiSmart.API.Settings;
using MiSmart.Infrastructure.Constants;
using MiSmart.API.Models;
using System;
using Microsoft.AspNetCore.Authorization;
using MiSmart.Infrastructure.Commands;
using MiSmart.DAL.Repositories;
using System.Linq.Expressions;
using MiSmart.DAL.Models;
using MiSmart.DAL.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace MiSmart.API.Controllers
{
    public class FarmAppController : AuthorizedAPIControllerBase
    {
        public FarmAppController(IActionResponseFactory actionResponseFactory) : base(actionResponseFactory)
        {
        }

        [HttpGet("AllFarmers")]
        public Task<IActionResult> GetAllFarmers([FromServices] IHttpClientFactory httpClientFactory, [FromServices] IOptions<FarmAppSettings> options)
        {
            var actionResponse = actionResponseFactory.CreateInstance();
            //             using (var client = httpClientFactory.CreateClient())
            //             {

            //                 var contentJson = JsonSerializer.Serialize(new { Query = @"
            //         query {
            //   getAllFarmerFromDroneHub(q: {}) {
            //     data {
            //       id
            //       uid
            //       name
            //       phone

            //       email
            //     }
            //   }
            // }

            //             " }, JsonSerializerDefaultOptions.CamelOptions);
            //                 StringContent content = new StringContent(contentJson, Encoding.UTF8, "application/json");
            //                 client.DefaultRequestHeaders.TryAddWithoutValidation("x-token", options.Value.SecretKey);
            //                 var url = options.Value.FarmDomain + "/graphql";
            //                 var response = await client.PostAsync(url, content);
            //                 var body = await response.Content.ReadAsStringAsync();
            //                 GettingAllFarmersResponse gettingAllMedicinesResponse = JsonSerializer.Deserialize<GettingAllFarmersResponse>(body, JsonSerializerDefaultOptions.CamelOptions);
            //                 if (gettingAllMedicinesResponse.Data.GetAllFarmerFromDroneHub == null)
            //                 {
            //                     actionResponse.AddNotFoundErr("AllFarmers");
            //                 }

            //                 actionResponse.SetData(gettingAllMedicinesResponse.Data.GetAllFarmerFromDroneHub.Data);
            //             }

            actionResponse.SetData(new Object[] { });



            return Task.FromResult(actionResponse.ToIActionResult());
        }

        [HttpGet("AllMedicines")]
        public async Task<IActionResult> GetAllMedichines([FromServices] IHttpClientFactory httpClientFactory,
        [FromServices] IOptions<FarmAppSettings> options)
        {
            var actionResponse = actionResponseFactory.CreateInstance();
            using (var client = httpClientFactory.CreateClient())
            {
                var contentJson = JsonSerializer.Serialize(new { Query = @"
                              query {
  getAllMedicineFromDroneHub(q: {}) {
    data {
      id
      name
      code

      thumbnail
    }
  }
}


                " }, JsonSerializerDefaultOptions.CamelOptions);
                StringContent content = new StringContent(contentJson, Encoding.UTF8, "application/json");

                client.DefaultRequestHeaders.TryAddWithoutValidation("x-token", options.Value.SecretKey);
                var url = options.Value.FarmDomain + "/graphql";
                var response = await client.PostAsync(url, content);

                var body = await response.Content.ReadAsStringAsync();

                var gettingAllMedicinesResponse = JsonSerializer.Deserialize<GettingAllMedicinesResponse>(body, JsonSerializerDefaultOptions.CamelOptions);
                if (gettingAllMedicinesResponse == null || gettingAllMedicinesResponse.Data == null || gettingAllMedicinesResponse.Data.GetAllMedicineFromDroneHub == null || gettingAllMedicinesResponse.Data.GetAllMedicineFromDroneHub.Data == null)
                {
                    actionResponse.AddNotFoundErr("AllMedicines");
                    return actionResponse.ToIActionResult();
                }

                actionResponse.SetData(gettingAllMedicinesResponse.Data.GetAllMedicineFromDroneHub.Data);
            }

            return actionResponse.ToIActionResult();
        }

        [HttpGet("Plans")]
        [AllowAnonymous]
        public async Task<IActionResult> GetListPlan([FromQuery] PageCommand pageCommand, [FromServices] PlanRepository planRepository, [FromQuery] String? ids,
        [FromServices] IOptions<FarmAppSettings> options, [FromQuery] String? secretKey)
        {
            ActionResponse actionResponse = actionResponseFactory.CreateInstance();
            var settings = options.Value;
            if (secretKey != settings.SecretKey)
            {
                actionResponse.AddAuthorizationErr();
                return actionResponse.ToIActionResult();
            }
            List<Int64>? planIds = null;
            if (!String.IsNullOrEmpty(ids))
            {
                var words = ids.Split(",").ToList();
                planIds = new List<Int64>();
                foreach (var word in words)
                {
                    planIds.Add(Int64.Parse(word));
                }
            }
            Expression<Func<Plan, Boolean>> query = ww => (planIds != null ? (!planIds.Contains(ww.ID)) : true) ; 
            var listResponse = await planRepository.GetListResponseViewAsync<SmallPlanViewModel>(pageCommand, query, ww => ww.CreatedTime, false);

            listResponse.SetResponse(actionResponse);
            return actionResponse.ToIActionResult();
        }
        [HttpGet("{id:long}/PlanFile")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPlanFileFromTM([FromRoute] Int64 id, [FromServices] PlanRepository planRepository, [FromServices] ExecutionCompanyUserRepository executionCompanyUserRepository,
        [FromServices] IOptions<FarmAppSettings> options, [FromQuery] String? secretKey)
        {
            ActionResponse actionResponse = actionResponseFactory.CreateInstance();
            var settings = options.Value;
            if (secretKey != settings.SecretKey)
            {
                actionResponse.AddAuthorizationErr();
                return actionResponse.ToIActionResult();
            }
            Expression<Func<Plan, Boolean>> query = ww => (ww.ID == id);
            var plan = await planRepository.GetAsync(query);
            if (plan is null)
            {
                actionResponse.AddNotFoundErr("Plan");
                return actionResponse.ToIActionResult();
            }

            actionResponse.SetFile(plan.FileBytes ?? new Byte[0], "application/octet-stream", plan.FileName ?? "example.plan");
            return actionResponse.ToIActionResult();
        }
    }
}