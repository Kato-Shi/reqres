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
    }
}
