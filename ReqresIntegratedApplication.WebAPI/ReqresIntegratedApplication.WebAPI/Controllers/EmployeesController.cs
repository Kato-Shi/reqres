using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ReqResIntegratedApplication.Integration.ReqresIntegration.Entities;
using ReqresIntegratedApplication.WebAPI.Models;
using ReqresIntegratedApplication.WebAPI.Services;

namespace ReqresIntegratedApplication.WebAPI.Controllers
{
    [ApiController]
    [Route("api/employees")]
    public class EmployeesController : ControllerBase
    {
        private readonly EmployeeService _employees;

        public EmployeesController(EmployeeService employees)
        {
            _employees = employees;
        }

        [HttpGet]
        public async Task<ActionResult<User?>> GetAll([FromQuery] int page = 1, [FromQuery(Name = "per_page")] int perPage = 6)
        {
            var users = await _employees.GetUsersAsync(page, perPage);
            return users is null ? NotFound("No employees returned from ReqRes.") : Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserData?>> GetById(int id)
        {
            var user = await _employees.GetUserAsync(id);
            return user is null ? NotFound() : Ok(user);
        }

        [HttpPost]
        public async Task<ActionResult<CreateUserResponse?>> Create([FromBody] CreateUserRequest request)
        {
            var created = await _employees.CreateUserAsync(request);
            if (created is null)
            {
                return StatusCode(502, "ReqRes did not create the user.");
            }

            return Created($"https://reqres.in/api/users/{created.Id}", created);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<UpdateUserResponse?>> Put(int id, [FromBody] UserUpdateDto request)
        {
            var response = await _employees.UpdateUserAsync(id, new UpdateUserRequest(request.Name, request.Job), false);
            return response is null ? StatusCode(502, "ReqRes did not return an update payload.") : Ok(response);
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult<UpdateUserResponse?>> Patch(int id, [FromBody] UserUpdateDto request)
        {
            var response = await _employees.UpdateUserAsync(id, new UpdateUserRequest(request.Name, request.Job), true);
            return response is null ? StatusCode(502, "ReqRes did not return an update payload.") : Ok(response);
        }

        [HttpGet("cache")]
        public ActionResult GetCache()
        {
            return Ok(_employees.GetLocalUsers());
        }
    }
}
