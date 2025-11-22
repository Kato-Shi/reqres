using System.Collections.Generic;

namespace ReqresIntegratedApplication.WebAPI.Models
{
    /// <summary>
    /// Local-only representation of a warehouse employee along with their assigned item ids.
    /// </summary>
    public class WarehouseAssociate
    {
        public int UserId { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Role { get; set; } = "Associate";

        public List<int> AssignedItemIds { get; set; } = new();
    }
}
