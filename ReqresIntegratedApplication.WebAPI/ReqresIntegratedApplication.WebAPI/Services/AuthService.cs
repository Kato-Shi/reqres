using System;
using System.Threading.Tasks;
using ReqResIntegratedApplication.Integration.ReqresIntegration.Entities;
using ReqResIntegratedApplication.Integration.ReqresIntegration.Services;
using ReqresIntegratedApplication.WebAPI.Helpers;

namespace ReqresIntegratedApplication.WebAPI.Services
{
    /// <summary>
    /// Handles login/logout using the ReqRes login endpoint and keeps the token in memory.
    /// </summary>
    public class AuthService
    {
        private readonly ReqResClient _client;

        public AuthService(ReqResClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task<string?> LoginAsync(string email, string password)
        {
            var response = await _client.LoginAsync(new LoginRequest(email, password));
            Session.CurrentToken = response?.Token;
            return response?.Token;
        }

        public void Logout()
        {
            Session.Clear();
        }
    }
}
