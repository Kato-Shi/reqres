using System.Net.Http;
using ReqResIntegratedApplication.Integration.ReqresIntegration.Entities;

namespace ReqResIntegratedApplication.Integration.ReqresIntegration.Manager
{
    public class UserManager : GenericManager<User>, IUserManager
    {
        private const string UsersEndpoint = "https://reqres.in/api/users";

        public UserManager(HttpClient httpClient) : base(httpClient, UsersEndpoint)
        {
            if (!httpClient.DefaultRequestHeaders.Contains("x-api-key"))
            {
                httpClient.DefaultRequestHeaders.Add("x-api-key", "reqres-free-v1");
            }
        }

        public Task<CreateUserResponse?> CreateUserAsync(CreateUserRequest requestBody) =>
            Post<CreateUserRequest, CreateUserResponse>(requestBody);
    }
}
