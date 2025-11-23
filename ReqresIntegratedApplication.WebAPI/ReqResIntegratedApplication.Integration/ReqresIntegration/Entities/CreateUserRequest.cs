using System.Text.Json.Serialization;

namespace ReqResIntegratedApplication.Integration.ReqresIntegration.Entities
{
        public class CreateUserRequest
        {
            [JsonPropertyName("name")]
            public string? Name { get; set; }

            [JsonPropertyName("job")]
            public string? Job { get; set; }

            [JsonPropertyName("email")]
            public string? Email { get; set; }

            public CreateUserRequest()
            {
            }

            public CreateUserRequest(string? name, string? job, string? email = null)
            {
                Name = name;
                Job = job;
                Email = email;
            }
        }
    }
