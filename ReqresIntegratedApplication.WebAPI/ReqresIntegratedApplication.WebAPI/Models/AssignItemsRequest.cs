using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ReqresIntegratedApplication.WebAPI.Models
{
    public class AssignItemsRequest
    {
        [Required]
        public List<int> ItemIds { get; set; } = new();
    }
}
