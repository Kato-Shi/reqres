using System.ComponentModel.DataAnnotations;

namespace ReqresIntegratedApplication.WebAPI.Models
{
    public class OnboardMemberRequest
    {
        [Required]
        [MinLength(2)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MinLength(2)]
        public string Role { get; set; } = string.Empty;
    }
}
