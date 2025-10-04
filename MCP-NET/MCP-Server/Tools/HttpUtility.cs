using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Tools
{
    public class HttpUtility
    {
        
        private readonly HttpClient _httpClient;

        public HttpUtility()
        {
            _httpClient = new HttpClient();
        }

        public async Task<T?> GetAsync<T>(string url)
        {
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var responseStream = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(responseStream);
        }

        public async Task<TResponse?> PostAsync<TRequest, TResponse>(string url, TRequest data)
        {
            var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content);
            response.EnsureSuccessStatusCode();

            var responseStream = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TResponse>(responseStream);
        }

        public async Task<TResponse?> PutAsync<TRequest, TResponse>(string url, TRequest data)
        {
            var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync(url, content);
            response.EnsureSuccessStatusCode();

            var responseStream = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TResponse>(responseStream);
        }

        public async Task<bool> DeleteAsync(string url)
        {
            var response = await _httpClient.DeleteAsync(url);
            return response.IsSuccessStatusCode;
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}

