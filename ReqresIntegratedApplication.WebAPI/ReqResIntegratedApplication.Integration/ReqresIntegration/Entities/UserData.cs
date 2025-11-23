using System.Text.Json.Serialization;

namespace ReqResIntegratedApplication.Integration.ReqresIntegration.Entities
{
    public class UserData
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("first_name")]
        public string? FirstName { get; set; }

        [JsonPropertyName("last_name")]
        public string? LastName { get; set; }

        [JsonPropertyName("avatar")]
        public string? Avatar { get; set; }

        /// <summary>
        /// Local-only job title used to keep PUT/PATCH changes visible in subsequent reads.
        /// </summary>
        [JsonPropertyName("job")]
        public string? Job { get; set; }

        public UserData()
        {
        }

        public UserData(int id, string? email, string? firstName, string? lastName, string? avatar, string? job = null)
        {
            Id = id;
            Email = email;
            FirstName = firstName;
            LastName = lastName;
            Avatar = avatar;
            Job = job;
        }
    }
}
