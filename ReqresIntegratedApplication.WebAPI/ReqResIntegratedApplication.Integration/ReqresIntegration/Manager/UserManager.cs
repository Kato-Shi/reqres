using ReqResIntegratedApplication.Integration.ReqresIntegration.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ReqResIntegratedApplication.Integration.ReqresIntegration.Manager
{
    public class UserManager : GenericManager<User>
    {
        public UserManager(HttpClient httpClient, string url, User requestBody) : base(httpClient, url, requestBody)
        {
            _httpClient.DefaultRequestHeaders.Add("x-api-key", "reqres-free-v1");   
        }
    }
}
