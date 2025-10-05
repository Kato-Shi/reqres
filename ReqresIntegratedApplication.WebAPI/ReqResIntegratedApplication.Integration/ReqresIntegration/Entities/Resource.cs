using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReqResIntegratedApplication.Integration.ReqresIntegration.Entities
{
   public class Resource
    {
        public int Page { get; set; }
        public int perPage { get; set; }
        public int Total { get; set; }

        public int TotalPages { get; set; }

        public List<ResourceData>? Data { get; set; }
    }
}
