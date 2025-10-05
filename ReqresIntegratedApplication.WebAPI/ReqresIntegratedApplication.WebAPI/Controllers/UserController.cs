using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReqResIntegratedApplication.Integration.ReqresIntegration.Entities;
using ReqResIntegratedApplication.Integration.ReqresIntegration.Services;

namespace ReqresIntegratedApplication.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserServices _userServices;

        public UserController(UserServices userServices)
        {
            _userServices = userServices;
        }

        [HttpGet]
        public async Task<ActionResult<User>> GetUsers([FromQuery] int page = 1, [FromQuery(Name = "per_page")] int perPage = 6)
        {
            var response = await _userServices.GetUsers(page, perPage);
            if (response is null || response.Data is null || response.Data.Count == 0)
            {
                return NotFound("No users found.");
            }

            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<CreateUserResponse>> PostUser([FromBody] CreateUserRequest request)
        {
            if (request is null)
            {
                return BadRequest("Request body is required.");
            }

            try
            {
                var result = await _userServices.PostUser(request);
                if (result is null)
                {
                    return StatusCode(StatusCodes.Status502BadGateway, "The ReqRes API returned an empty response.");
                }

                const string baseUrl = "https://reqres.in/api/users";
                var location = !string.IsNullOrWhiteSpace(result.Id) ? $"{baseUrl}/{result.Id}" : baseUrl;
                return Created(location, result);
            }
            catch (HttpRequestException httpRequestException)
            {
                return StatusCode(StatusCodes.Status502BadGateway, httpRequestException.Message);
            }
        }
    }
}
