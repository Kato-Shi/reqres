namespace ReqresIntegratedApplication.WebAPI.Models
{
    public class WorkforceSummaryDto
    {
        public int Page { get; set; }

        public int PerPage { get; set; }

        public int TotalMembers { get; set; }

        public int TotalPages { get; set; }

        public int CountOnPage { get; set; }
    }
}
