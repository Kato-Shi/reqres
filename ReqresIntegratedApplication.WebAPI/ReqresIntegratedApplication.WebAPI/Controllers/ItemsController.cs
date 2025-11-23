using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ReqResIntegratedApplication.Integration.ReqresIntegration.Entities;
using ReqresIntegratedApplication.WebAPI.Models;
using ReqresIntegratedApplication.WebAPI.Services;

namespace ReqresIntegratedApplication.WebAPI.Controllers
{
    [ApiController]
    [Route("api/items")]
    public class ItemsController : ControllerBase
    {
        private readonly ResourceService _resources;

        public ItemsController(ResourceService resources)
        {
            _resources = resources;
        }

        [HttpPost]
        public ActionResult<ResourceData> Create(ResourceUpsertRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Color))
            {
                return BadRequest("Name and color are required.");
            }

            var resource = new ResourceData
            {
                Name = request.Name,
                Color = request.Color,
                Year = request.Year,
                PantoneValue = request.PantoneValue
            };

            var created = _resources.AddResource(resource);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpGet]
        public async Task<ActionResult<Resource?>> GetAll([FromQuery] int page = 1, [FromQuery(Name = "per_page")] int perPage = 6)
        {
            var resourcePage = await _resources.GetResourcesAsync(page, perPage);
            return resourcePage is null ? NotFound() : Ok(resourcePage);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ResourceData?>> GetById(int id)
        {
            var resource = await _resources.GetResourceAsync(id);
            return resource is null ? NotFound() : Ok(resource);
        }

        [HttpPut("{id}")]
        public ActionResult<ResourceData> Update(int id, ResourceUpsertRequest request)
        {
            var update = new ResourceData
            {
                Id = id,
                Name = request.Name,
                Color = request.Color,
                Year = request.Year,
                PantoneValue = request.PantoneValue
            };

            var result = _resources.UpdateResource(id, update);
            return result is null ? NotFound() : Ok(result);
        }
    }
}
