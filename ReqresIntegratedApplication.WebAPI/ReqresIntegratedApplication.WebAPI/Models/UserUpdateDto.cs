using System.ComponentModel.DataAnnotations;

namespace ReqresIntegratedApplication.WebAPI.Models
{
    public class UserUpdateDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Job { get; set; } = string.Empty;
    }
}
