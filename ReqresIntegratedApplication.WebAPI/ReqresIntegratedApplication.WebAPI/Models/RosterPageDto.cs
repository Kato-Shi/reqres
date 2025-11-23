using System.Collections.Generic;

namespace ReqresIntegratedApplication.WebAPI.Models
{
    public class RosterPageDto
    {
        public int Page { get; set; }

        public int PerPage { get; set; }

        public int Total { get; set; }

        public int TotalPages { get; set; }

        public IReadOnlyCollection<RosterMemberDto> Members { get; set; }
            = new List<RosterMemberDto>();

        public HeadcountSummaryDto Summary { get; set; } = new HeadcountSummaryDto();
    }
}
