using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReqresIntegratedApplication.Integration.ReqresIntegration.Services;

namespace ReqresIntegratedApplication.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public UserController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [HttpPost]
        public async Task<IActionResult> PostUser([FromBody] User user)
        {
            string baseURL = "https://reqres.in/api/users";
            UserServices userServices = new UserServices(_httpClient, baseURL);

            var result = await userServices.PostUser(user);
            return Ok(result);
        }
    }
}
