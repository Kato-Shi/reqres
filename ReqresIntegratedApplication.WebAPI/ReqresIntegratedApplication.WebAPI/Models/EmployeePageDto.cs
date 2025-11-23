using System.Collections.Generic;

namespace ReqresIntegratedApplication.WebAPI.Models
{
    public class EmployeePageDto
    {
        public int Page { get; set; }

        public int PerPage { get; set; }

        public int Total { get; set; }

        public int TotalPages { get; set; }

        public IReadOnlyCollection<EmployeeListItemDto> Members { get; set; }
            = new List<EmployeeListItemDto>();

        public WorkforceSummaryDto Summary { get; set; } = new WorkforceSummaryDto();
    }
}
