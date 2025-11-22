using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ReqResIntegratedApplication.Integration.ReqresIntegration.Entities;
using ReqresIntegratedApplication.WebAPI.Services;

namespace ReqresIntegratedApplication.WebAPI.Controllers
{
    [ApiController]
    [Route("api/resources")]
    public class ResourcesController : ControllerBase
    {
        private readonly ResourceService _resources;

        public ResourcesController(ResourceService resources)
        {
            _resources = resources;
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
    }
}
