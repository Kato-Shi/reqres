using System;

namespace ReqresIntegratedApplication.WebAPI.Models
{
    public class HiredEmployeeResponse
    {
        public string Id { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public DateTimeOffset CreatedAt { get; set; }
            = DateTimeOffset.UtcNow;

        public string ResourceUrl { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;
    }
}
