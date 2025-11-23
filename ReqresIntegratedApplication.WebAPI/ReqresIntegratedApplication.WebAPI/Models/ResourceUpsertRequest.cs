namespace ReqresIntegratedApplication.WebAPI.Models
{
    /// <summary>
    /// Represents the data needed to create or update a resource/tool item.
    /// </summary>
    public class ResourceUpsertRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public int Year { get; set; }
        public string PantoneValue { get; set; } = string.Empty;
    }
}
