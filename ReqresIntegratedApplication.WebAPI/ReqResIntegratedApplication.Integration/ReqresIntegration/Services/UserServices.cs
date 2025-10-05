using ReqResIntegratedApplication.Integration.ReqresIntegration.Entities;
using ReqResIntegratedApplication.Integration.ReqresIntegration.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ReqResIntegratedApplication.Integration.ReqresIntegration.Services
{
    class UserServices
    {
        public async Task<User> PostUser(User user)
        {
            User requestBody = new User(user.Page, user.perPage, user.Total, user.TotalPages, user.Data);
            UserManager userManager = new UserManager(httpClient, _baseURL, requestBody);
            return await userManager.Post();
        }
    }
}
