using System.Text.Json.Serialization;

namespace ReqResIntegratedApplication.Integration.ReqresIntegration.Entities
{
    /// <summary>
    /// Payload used for PUT or PATCH operations against ReqRes users.
    /// </summary>
    public class UpdateUserRequest
    {
        public UpdateUserRequest()
        {
        }

        public UpdateUserRequest(string name, string job)
        {
            Name = name;
            Job = job;
        }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("job")]
        public string Job { get; set; } = string.Empty;
    }
}
