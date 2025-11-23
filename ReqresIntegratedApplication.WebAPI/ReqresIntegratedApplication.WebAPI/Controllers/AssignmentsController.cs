using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ReqresIntegratedApplication.WebAPI.Models;
using ReqresIntegratedApplication.WebAPI.Services;

namespace ReqresIntegratedApplication.WebAPI.Controllers
{
    [ApiController]
    [Route("api/warehouse/assignments")]
    public class AssignmentsController : ControllerBase
    {
        private readonly WarehouseAssignmentService _assignments;

        public AssignmentsController(WarehouseAssignmentService assignments)
        {
            _assignments = assignments;
        }

        [HttpGet]
        public IActionResult GetAll() => Ok(_assignments.GetAssociates());

        [HttpGet("{userId:int}")]
        public IActionResult Get(int userId)
        {
            var associate = _assignments.GetAssociate(userId);
            return associate is null ? NotFound("Associate not found. Promote the employee first.") : Ok(associate);
        }

        [HttpPost("promote/{userId:int}")]
        public async Task<IActionResult> Promote(int userId, [FromQuery] string role = "Associate")
        {
            var associate = await _assignments.PromoteAsync(userId, role);
            return associate is null ? NotFound("Employee not found in ReqRes.") : Ok(associate);
        }

        [HttpPost("{userId:int}/items")]
        public IActionResult AssignItems(int userId, [FromBody] AssignItemsRequest request)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var success = _assignments.AssignItems(userId, request.ItemIds);
            return success ? Ok(_assignments.GetAssociate(userId)) : NotFound("Associate not found. Promote the employee first.");
        }
    }
}
