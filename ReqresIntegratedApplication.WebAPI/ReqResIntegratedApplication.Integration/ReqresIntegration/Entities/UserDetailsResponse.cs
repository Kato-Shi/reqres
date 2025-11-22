using System.Text.Json.Serialization;

namespace ReqResIntegratedApplication.Integration.ReqresIntegration.Entities
{
    /// <summary>
    /// Wrapper for single-user lookups from ReqRes.
    /// </summary>
    public class UserDetailsResponse
    {
        [JsonPropertyName("data")]
        public UserData? Data { get; set; }
    }
}
