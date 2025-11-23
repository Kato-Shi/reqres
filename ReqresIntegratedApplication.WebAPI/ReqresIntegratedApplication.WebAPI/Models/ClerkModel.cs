using System.Collections.Generic;

namespace ReqresIntegratedApplication.WebAPI.Models
{
    /// <summary>
    /// Local-only representation of a clerk enriched with assigned resources.
    /// </summary>
    public class ClerkModel
    {
        public int UserId { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Role { get; set; } = "Clerk";

        public List<int> AssignedResourceIds { get; set; } = new();
    }
}
