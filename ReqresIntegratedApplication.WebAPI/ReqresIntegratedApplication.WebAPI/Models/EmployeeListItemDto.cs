namespace ReqresIntegratedApplication.WebAPI.Models
{
    public class EmployeeListItemDto
    {
        public int Id { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string? Avatar { get; set; }
    }
}
