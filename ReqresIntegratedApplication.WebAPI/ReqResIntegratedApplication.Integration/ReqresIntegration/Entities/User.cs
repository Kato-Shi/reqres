using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ReqResIntegratedApplication.Integration.ReqresIntegration.Entities
{
    public class User
    {
        [JsonPropertyName("page")]
        public int Page { get; set; }

        [JsonPropertyName("per_page")]
        public int PerPage { get; set; }

        [JsonPropertyName("total")]
        public int Total { get; set; }

        [JsonPropertyName("total_pages")]
        public int TotalPages { get; set; }

        [JsonPropertyName("data")]
        public List<UserData>? Data { get; set; }

        public User()
        {
        }

        public User(int page, int perPage, int total, int totalPages, List<UserData>? data)
        {
            Page = page;
            PerPage = perPage;
            Total = total;
            TotalPages = totalPages;
            Data = data;
        }
    }
}
