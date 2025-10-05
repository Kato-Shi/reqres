using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReqResIntegratedApplication.Integration.ReqresIntegration.Entities
{
    class User
    {
        public int Page { get; set; }
        public int perPage { get; set; }

        public int Total { get; set; }
        public int TotalPages { get; set; }
        public List<UserData>? Data { get; set; }
         
        public User(int page, int perpage, int total, int totalpage, List<UserData> data)
        {
            Page = page;
            perPage = perpage;
            Total = total;
            TotalPages = totalpage;
            Data = data;
        }
    }
}
