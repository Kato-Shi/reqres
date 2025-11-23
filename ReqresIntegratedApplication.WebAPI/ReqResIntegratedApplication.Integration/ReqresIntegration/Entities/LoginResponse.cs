using System.Text.Json.Serialization;

namespace ReqResIntegratedApplication.Integration.ReqresIntegration.Entities
{
    /// <summary>
    /// Response returned by ReqRes login.
    /// </summary>
    public class LoginResponse
    {
        [JsonPropertyName("token")]
        public string? Token { get; set; }

        /// <summary>
        /// Optional error detail captured when the ReqRes login endpoint
        /// returns a non-success status. Helps callers surface a friendly
        /// diagnostic instead of an opaque 401/403.
        /// </summary>
        public string? Error { get; set; }
    }
}
