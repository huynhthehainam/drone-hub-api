
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using MiSmart.DAL.Responses;
using MiSmart.DAL.ViewModels;

namespace MiSmart.API.Helpers
{
    public static class BingLocationHelper
    {
        public static async Task UpdateLocation(ListFlightStatsResponse<SmallFlightStatViewModel> listResponse, Int32 index, IHttpClientFactory httpClientFactory)
        {
            var client = httpClientFactory.CreateClient();

            var firstPoint = listResponse.Data[index].FirstPoint;
            HttpResponseMessage resp = await client.GetAsync($"http://dev.virtualearth.net/REST/v1/Locations/{firstPoint.Latitude},{firstPoint.Longitude}?key=AiZ-Nz14Iup8BQtxfTK5PM1Fv2QRHYKL_SEiZYHC7HyfBuhVI19zKy2-RsT5NzFQ");

            var content = await resp.Content.ReadAsStringAsync();

            JsonDocument document = JsonDocument.Parse(content);
            var root = document.RootElement;
            JsonElement resourceSets;
            var success = root.TryGetProperty("resourceSets", out resourceSets);
            if (success)
            {
                var array = resourceSets.EnumerateArray();
                if (array.Count() > 0)
                {
                    var firstItem = array.FirstOrDefault();
                    JsonElement resources;
                    success = firstItem.TryGetProperty("resources", out resources);
                    if (success)
                    {
                        var array2 = resources.EnumerateArray();
                        if (array2.Count() > 0)
                        {
                            var firstItem2 = array2.FirstOrDefault();
                            JsonElement address;
                            success = firstItem2.TryGetProperty("address", out address);
                            if (success)
                            {
                                List<String> locations = new List<String> { };
                                JsonElement adminDistrict2;
                                success = address.TryGetProperty("adminDistrict2", out adminDistrict2);
                                if (success)
                                {
                                    locations.Add(adminDistrict2.GetString());
                                }
                                JsonElement adminDistrict;
                                success = address.TryGetProperty("adminDistrict", out adminDistrict);
                                if (success)
                                {
                                    locations.Add(adminDistrict.GetString());
                                }
                                JsonElement countryRegion;
                                success = address.TryGetProperty("countryRegion", out countryRegion);
                                if (success)
                                {
                                    locations.Add(countryRegion.GetString());
                                }
                                listResponse.Data[index].TaskLocation = String.Join(", ", locations);
                            }
                        }
                    }
                }
            }
            Console.WriteLine("Finished");
        }
    }
}