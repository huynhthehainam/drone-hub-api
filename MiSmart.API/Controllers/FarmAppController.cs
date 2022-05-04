

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

namespace MiSmart.API.Controllers
{
    public class FarmAppController : AuthorizedAPIControllerBase
    {
        public FarmAppController(IActionResponseFactory actionResponseFactory) : base(actionResponseFactory)
        {
        }

        [HttpGet("AllMedicines")]
        public async Task<IActionResult> GetAllMedichines([FromServices] IHttpClientFactory httpClientFactory,
        [FromServices] IOptions<FarmAppSettings> options)
        {
            var actionResponse = actionResponseFactory.CreateInstance();
            using (var client = httpClientFactory.CreateClient())
            {
                var aa = JsonSerializer.Serialize(new { Query = @"
                query {
                    getAllMedicineFromDroneHub(q:{}){
                        data {
                            id
                            name
                            code
                            thumbnail
                        }
                    }
                }" }, JsonSerializerDefaultOptions.CamelOptions);
                StringContent content = new StringContent(aa, Encoding.UTF8, "application/json");

                client.DefaultRequestHeaders.TryAddWithoutValidation("x-token", options.Value.SecretKey);
                var url = options.Value.FarmDomain + "/graphql";
                var response = await client.PostAsync(url, content);

                var body = await response.Content.ReadAsStringAsync();

                GettingAllMedicinesResponse gettingAllMedicinesResponse = JsonSerializer.Deserialize<GettingAllMedicinesResponse>(body, JsonSerializerDefaultOptions.CamelOptions);
                if (gettingAllMedicinesResponse.Data.GetAllMedicineFromDroneHub == null)
                {
                    actionResponse.AddNotFoundErr("AllMedicines");
                }

                actionResponse.SetData(gettingAllMedicinesResponse.Data.GetAllMedicineFromDroneHub.Data);
            }

            return actionResponse.ToIActionResult();
        }
    }
}