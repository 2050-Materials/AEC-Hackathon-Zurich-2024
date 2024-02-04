using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TwentyFiftyMaterialsCore.Models;

namespace TwentyFiftyMaterialsCore
{
    internal class RequestMaterialData
    {
        private static readonly HttpClient client = new HttpClient();
        private static readonly string baseApiUrl = "https://app.2050-materials.com/"; // Replace with your actual base API URL
        private static readonly string apiToken = ""; // Replace with your actual API token
        TFMaterial material;

        public static void RequestMaterialDataFromApi()
        {
            // Configure HttpClient instance
            client.BaseAddress = new Uri(baseApiUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiToken);
        }

        public static async Task<string> RequestMaterialDataFromAPI(Dictionary<string, object> filters, int page = 1)
        {
            try
            {
                var url = BuildUrlWithFilters(filters, page);
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode(); // Throws an exception if the HTTP response status is an error code.
                var jsonString = await response.Content.ReadAsStringAsync();
                return jsonString;
                //JsonConvert.DeserializeObject<string>(jsonString); // Adjust the deserialization according to your needs

                //Console.WriteLine(products); // Or handle the data as needed
            }
            catch (HttpRequestException e)
            {
                return null;
                //Console.WriteLine($"Request exception: {e.Message}");
            }
        }

        private static string BuildUrlWithFilters(Dictionary<string, object> filters, int page)
        {
            var queryComponents = new List<string>();

            foreach (var filter in filters)
            {
                if (filter.Key == "product_url" && filter.Value is List<string> urls)
                {
                    foreach (var url in urls)
                    {
                        queryComponents.Add($"{filter.Key}={Uri.EscapeDataString(url)}");
                    }
                }
                else
                {
                    queryComponents.Add($"{filter.Key}={Uri.EscapeDataString(filter.Value.ToString())}");
                }
            }

            var filterQuery = string.Join("&", queryComponents);
            var urlWithFilters = $"{baseApiUrl}developer/api/get_products_open_api?page={page}&{filterQuery}";
            return urlWithFilters;
        }
    }
}
