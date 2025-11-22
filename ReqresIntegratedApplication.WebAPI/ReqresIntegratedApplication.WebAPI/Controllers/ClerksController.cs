using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ReqresIntegratedApplication.WebAPI.Models;
using ReqresIntegratedApplication.WebAPI.Services;

namespace ReqresIntegratedApplication.WebAPI.Controllers
{
    [ApiController]
    [Route("api/clerks")]
    public class ClerksController : ControllerBase
    {
        private readonly ClerkService _clerks;

        public ClerksController(ClerkService clerks)
        {
            _clerks = clerks;
        }

        [HttpGet]
        public IActionResult GetAll() => Ok(_clerks.GetClerks());

        [HttpGet("{userId}")]
        public IActionResult Get(int userId)
        {
            var clerk = _clerks.GetClerk(userId);
            return clerk is null ? NotFound() : Ok(clerk);
        }

        [HttpPost("{userId}/promote")]
        public async Task<IActionResult> Promote(int userId, [FromQuery] string role = "Clerk")
        {
            var clerk = await _clerks.PromoteAsync(userId, role);
            return clerk is null ? NotFound("Employee not found in ReqRes.") : Ok(clerk);
        }

        [HttpPost("{userId}/assign-resources")]
        public IActionResult AssignResources(int userId, [FromBody] AssignResourcesRequest request)
        {
            var success = _clerks.AssignResources(userId, request.ResourceIds);
            return success ? Ok(_clerks.GetClerk(userId)) : NotFound("Clerk not found. Promote the employee first.");
        }
    }
}
