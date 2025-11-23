using System;
using System.Threading.Tasks;
using ReqresIntegratedApplication.WebAPI.Helpers;

namespace ReqresIntegratedApplication.WebAPI.Services
{
    /// <summary>
    /// Handles login/logout using an in-memory credential check to avoid upstream ReqRes 401s.
    /// </summary>
    public class AuthService
    {
        private const string DemoEmail = "eve.holt@reqres.in";
        private const string DemoPassword = "cityslicka";
        private const string DemoToken = "local-demo-token";

        public Task<(string? Token, string? Error)> LoginAsync(string email, string password)
        {
            var isMatch = email.Equals(DemoEmail, StringComparison.OrdinalIgnoreCase)
                && password == DemoPassword;

            if (!isMatch)
            {
                Session.Clear();
                return Task.FromResult<(string?, string?)>((null, "Invalid credentials. Use eve.holt@reqres.in / cityslicka for this demo."));
            }

            Session.CurrentToken = DemoToken;
            return Task.FromResult<(string?, string?)>((DemoToken, null));
        }

        public void Logout()
        {
            Session.Clear();
        }
    }
}
