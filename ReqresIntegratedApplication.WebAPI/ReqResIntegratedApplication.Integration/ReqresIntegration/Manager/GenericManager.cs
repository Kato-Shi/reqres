using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace ReqResIntegratedApplication.Integration.ReqresIntegration.Manager
{
    public class GenericManager<T> : IGenericManager<T> where T : class
    {
        protected readonly HttpClient HttpClient;
        protected readonly string BaseUrl;

        public GenericManager(HttpClient httpClient, string baseUrl)
        {
            HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                throw new ArgumentException("Base URL must be provided.", nameof(baseUrl));
            }

            BaseUrl = baseUrl.TrimEnd('/');
        }

        public virtual Task<T?> Get(int id)
        {
            return HttpClient.GetFromJsonAsync<T>($"{BaseUrl}/{id}");
        }

        public virtual Task<T?> GetAll(int page = 1, int perPage = 6)
        {
            var url = $"{BaseUrl}?page={page}&per_page={perPage}";
            return HttpClient.GetFromJsonAsync<T>(url);
        }

        public virtual async Task<TResult?> Post<TRequest, TResult>(TRequest requestBody)
            where TRequest : class
            where TResult : class
        {
            if (requestBody is null)
            {
                throw new ArgumentNullException(nameof(requestBody));
            }

            var response = await HttpClient.PostAsJsonAsync(BaseUrl, requestBody);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<TResult>();
        }
    }
}
