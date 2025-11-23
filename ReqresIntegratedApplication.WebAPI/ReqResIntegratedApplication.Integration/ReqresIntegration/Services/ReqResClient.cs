using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ReqResIntegratedApplication.Integration.ReqresIntegration.Entities;

namespace ReqResIntegratedApplication.Integration.ReqresIntegration.Services
{
    /// <summary>
    /// Thin client around ReqRes endpoints used by TeamShift Lite. Keeps HttpClient reuse centralized.
    /// </summary>
    public class ReqResClient
    {
        private static readonly JsonSerializerOptions CaseInsensitive = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private readonly HttpClient _httpClient;

        public ReqResClient(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            if (_httpClient.BaseAddress is null)
            {
                _httpClient.BaseAddress = new Uri("https://reqres.in/api/");
            }
        }

        public Task<User?> GetUsersAsync(int page, int perPage) =>
            _httpClient.GetFromJsonAsync<User>($"users?page={page}&per_page={perPage}");

        public Task<UserDetailsResponse?> GetUserAsync(int id) =>
            _httpClient.GetFromJsonAsync<UserDetailsResponse>($"users/{id}");

        public Task<CreateUserResponse?> CreateUserAsync(CreateUserRequest request) =>
            PostAsync<CreateUserRequest, CreateUserResponse>("users", request);

        public Task<UpdateUserResponse?> PutUserAsync(int id, UpdateUserRequest request) =>
            SendUserUpdateAsync(HttpMethod.Put, id, request);

        public Task<UpdateUserResponse?> PatchUserAsync(int id, UpdateUserRequest request) =>
            SendUserUpdateAsync(HttpMethod.Patch, id, request);

        public async Task<bool> DeleteUserAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"users/{id}");
            if (response.IsSuccessStatusCode)
            {
                return true;
            }

            // Treat ReqRes not-found or proxy blocks as failures but allow callers to fall back locally.
            return false;
        }

        public Task<Resource?> GetResourcesAsync(int page, int perPage) =>
            _httpClient.GetFromJsonAsync<Resource>($"unknown?page={page}&per_page={perPage}");

        public Task<ResourceData?> GetResourceAsync(int id) =>
            _httpClient.GetFromJsonAsync<ResourceData>($"unknown/{id}");

        private async Task<TResult?> PostAsync<TRequest, TResult>(string relativeUrl, TRequest request, bool ensureSuccess = true)
            where TRequest : class
            where TResult : class
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            using var content = BuildJsonContent(request);
            var response = await _httpClient.PostAsync(relativeUrl, content);
            if (ensureSuccess)
            {
                response.EnsureSuccessStatusCode();
            }
            else if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<TResult>(CaseInsensitive);
        }

        private async Task<UpdateUserResponse?> SendUserUpdateAsync(HttpMethod method, int id, UpdateUserRequest request)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            using var content = BuildJsonContent(request);
            var requestMessage = new HttpRequestMessage(method, $"users/{id}")
            {
                Content = content
            };

            var response = await _httpClient.SendAsync(requestMessage);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<UpdateUserResponse>(CaseInsensitive);
        }

        private static StringContent BuildJsonContent<T>(T payload)
        {
            var json = JsonSerializer.Serialize(payload);
            return new StringContent(json, Encoding.UTF8, "application/json");
        }
    }
}
