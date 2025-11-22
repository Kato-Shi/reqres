using System;
using System.Text.Json.Serialization;

namespace ReqResIntegratedApplication.Integration.ReqresIntegration.Entities
{
    public class CreateUserResponse
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("job")]
        public string? Job { get; set; }

        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("createdAt")]
        public DateTimeOffset CreatedAt { get; set; }

        public CreateUserResponse()
        {
        }

        public CreateUserResponse(string? name, string? job, string? id, DateTimeOffset createdAt)
        {
            Name = name;
            Job = job;
            Id = id;
            CreatedAt = createdAt;
        }
    }
}
