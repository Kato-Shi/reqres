using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ReqresIntegratedApplication.WebAPI.Models
{
    public class AssignResourcesRequest
    {
        [Required]
        public List<int> ResourceIds { get; set; } = new();
    }
}
