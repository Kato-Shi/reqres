using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ReqResIntegratedApplication.Integration.ReqresIntegration.Manager
{
    public class GenericManager<T> : IGenericManager<T> where T : class
    {
        protected readonly HttpClient _httpClient;
        protected readonly string _baseUrl;
        protected readonly T _requestBody;

        public async Task<T> Get(int id)
        {
            return null;
        }
        public async Task<List<T>> GetAll()
        {
            return null;
        }
        public GenericManager(HttpClient httpClient, string baseUrl, T requestBody)
        {
            _httpClient = httpClient;
            _baseUrl = baseUrl;
            _requestBody = requestBody;
        }

        public async Task<T> Post()
        {
            var json = JsonSerializer.Serialize(_requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(_baseUrl, content);
            var responseString = await response.Content.ReadAsStringAsync();
            var responseJson = JsonSerializer.Deserialize<T>(responseString);
            return responseJson;
        }
    }
}
