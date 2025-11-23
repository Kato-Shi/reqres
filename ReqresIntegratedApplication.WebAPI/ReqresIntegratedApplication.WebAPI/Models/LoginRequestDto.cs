using System.ComponentModel.DataAnnotations;

namespace ReqresIntegratedApplication.WebAPI.Models
{
    /// <summary>
    /// UI-facing login payload.
    /// </summary>
    public class LoginRequestDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
