using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReqresIntegratedApplication.WebAPI.Models;
using ReqresIntegratedApplication.WebAPI.Services;

namespace ReqresIntegratedApplication.WebAPI.Controllers
{
    [ApiController]
    [Route("api/warehouse")]
    public class WarehouseController : ControllerBase
    {
        private readonly WarehouseDashboardService _dashboardService;

        public WarehouseController(WarehouseDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("employees")]
        public async Task<ActionResult<EmployeePageDto>> GetEmployees([FromQuery] int page = 1, [FromQuery(Name = "per_page")] int perPage = 6)
        {
            var roster = await _dashboardService.GetEmployeesAsync(page, perPage);
            if (roster is null || roster.Members.Count == 0)
            {
                return NotFound("No warehouse employees were returned by ReqRes.");
            }

            return Ok(roster);
        }

        [HttpGet("workforce-summary")]
        public async Task<ActionResult<WorkforceSummaryDto>> GetWorkforce([FromQuery] int page = 1, [FromQuery(Name = "per_page")] int perPage = 6)
        {
            var summary = await _dashboardService.GetWorkforceSummaryAsync(page, perPage);
            if (summary is null)
            {
                return NotFound("Unable to calculate workforce from the ReqRes roster.");
            }

            return Ok(summary);
        }

        [HttpPost("employees")]
        public async Task<ActionResult<HiredEmployeeResponse>> Hire([FromBody] HireEmployeeRequest request)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            try
            {
                var result = await _dashboardService.HireEmployeeAsync(request);
                if (result is null)
                {
                    return StatusCode(StatusCodes.Status502BadGateway, "ReqRes did not return a payload for the created employee.");
                }

                var location = string.IsNullOrWhiteSpace(result.Id) ? "https://reqres.in/api/users" : $"https://reqres.in/api/users/{result.Id}";
                return Created(location, result);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(StatusCodes.Status502BadGateway, ex.Message);
            }
        }
    }
}
