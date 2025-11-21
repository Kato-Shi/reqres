using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReqresIntegratedApplication.WebAPI.Models;
using ReqresIntegratedApplication.WebAPI.Services;

namespace ReqresIntegratedApplication.WebAPI.Controllers
{
    [ApiController]
    [Route("api/teamshift")]
    public class TeamShiftController : ControllerBase
    {
        private readonly TeamShiftDashboardService _dashboardService;

        public TeamShiftController(TeamShiftDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("roster")]
        public async Task<ActionResult<RosterPageDto>> GetRoster([FromQuery] int page = 1, [FromQuery(Name = "per_page")] int perPage = 6)
        {
            var roster = await _dashboardService.GetRosterAsync(page, perPage);
            if (roster is null || roster.Members.Count == 0)
            {
                return NotFound("No roster entries were returned by ReqRes.");
            }

            return Ok(roster);
        }

        [HttpGet("headcount")]
        public async Task<ActionResult<HeadcountSummaryDto>> GetHeadcount([FromQuery] int page = 1, [FromQuery(Name = "per_page")] int perPage = 6)
        {
            var summary = await _dashboardService.GetHeadcountAsync(page, perPage);
            if (summary is null)
            {
                return NotFound("Unable to calculate headcount from the ReqRes roster.");
            }

            return Ok(summary);
        }

        [HttpPost("roster")]
        public async Task<ActionResult<OnboardedMemberResponse>> Onboard([FromBody] OnboardMemberRequest request)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            try
            {
                var result = await _dashboardService.OnboardMemberAsync(request);
                if (result is null)
                {
                    return StatusCode(StatusCodes.Status502BadGateway, "ReqRes did not return a payload for the created member.");
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
