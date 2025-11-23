using System.Text.Json.Serialization;

namespace ReqResIntegratedApplication.Integration.ReqresIntegration.Entities
{
    public class CreateUserRequest
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("job")]
        public string? Job { get; set; }

        public CreateUserRequest()
        {
        }

        public CreateUserRequest(string? name, string? job)
        {
            Name = name;
            Job = job;
        }
    }
}
