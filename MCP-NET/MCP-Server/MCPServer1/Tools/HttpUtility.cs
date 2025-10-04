using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using static MCPServer1.Program;

namespace Tools
{
    public class HttpUtility
    {
        
        private readonly IHttpClientFactory _httpClientFactory;

        public HttpUtility()
        {
            _httpClientFactory = ServiceLocator.ServiceProvider.GetRequiredService<IHttpClientFactory>();
        }

        public async Task<TOutPut> GetHttpCallAsync<T, TOutPut>(Dictionary<string, string> headers, string contentType, string url, T model, string type)
        {
            string content = string.Empty;
            var response = await GetCustomHttpCallAsync(headers, contentType, url, model, type);
            if (response != null)
            {
                Stream responseStream = await response.Content.ReadAsStreamAsync();
                content = await new StreamReader(responseStream).ReadToEndAsync();
            }
            return JsonConvert.DeserializeObject<TOutPut>(content);
        }

        //public async Task<HttpResponseMessage> GetCustomHttpCallAsync<T>(Dictionary<string, string> headers, string contentType, string url, T model, string type)
        //{
        //    using var httpClient = _httpClientFactory.CreateClient();
        //    foreach (var item in headers)
        //    {
        //        httpClient.DefaultRequestHeaders.Add(item.Key, item.Value);
        //    }
        //    httpClient.DefaultRequestHeaders.Add("appName", "EVA");
        //    HttpResponseMessage response = null;
        //    string json = string.Empty;
        //    StringContent httpContent;

        //    switch (type)
        //    {
        //        case "GET":
        //            response = await httpClient.GetAsync(url);
        //            break;

        //        case "POST":
        //            json = contentType == "application/json"
        //                ? JsonConvert.SerializeObject(model, Formatting.Indented)
        //                : "grant_type=client_credentials&scope=read write serviceapi_all";
        //            httpContent = new StringContent(json.ToString(), Encoding.UTF8, contentType);
        //            response = await httpClient.PostAsync(url, httpContent);
        //            break;

        //        case "PUT":
        //            json = JsonConvert.SerializeObject(model, Formatting.Indented);
        //            httpContent = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
        //            response = await httpClient.PutAsync(url, httpContent);
        //            break;

        //        case "PATCH":
        //            break;

        //        case "DELETE":
        //            response = await httpClient.DeleteAsync(url);
        //            break;
        //    }
        //    return response;
        //}


        public async Task<HttpResponseMessage> GetCustomHttpCallAsync<T>(Dictionary<string, string> headers, string contentType, string url, T model, string type)
        {
            var client = _httpClientFactory.CreateClient();

            // Determine HTTP method
            HttpMethod httpMethod = type.ToUpper() switch
            {
                "GET" => HttpMethod.Get,
                "POST" => HttpMethod.Post,
                "PUT" => HttpMethod.Put,
                "PATCH" => new HttpMethod("PATCH"),
                "DELETE" => HttpMethod.Delete,
                _ => throw new NotSupportedException($"HTTP method '{type}' is not supported.")
            };

            var request = new HttpRequestMessage(httpMethod, url);

            // Add headers
            foreach (var item in headers)
            {
                request.Headers.TryAddWithoutValidation(item.Key, item.Value);
            }

            request.Headers.TryAddWithoutValidation("appName", "EVA");

            // Add JSON body (for POST, PUT, PATCH)
            if (httpMethod == HttpMethod.Post || httpMethod == HttpMethod.Put || httpMethod.Method == "PATCH")
            {
                if (contentType == "application/json")
                {
                    string json = JsonConvert.SerializeObject(model);
                    request.Content = new StringContent(json, Encoding.UTF8, "application/json");
                }
                else
                {
                    throw new NotSupportedException($"Only 'application/json' contentType is supported for this call.");
                }
            }

            // Send the request
            return await client.SendAsync(request);
        }

    }
}

