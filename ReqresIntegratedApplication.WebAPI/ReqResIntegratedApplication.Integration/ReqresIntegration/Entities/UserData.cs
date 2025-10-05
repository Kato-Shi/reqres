using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReqResIntegratedApplication.Integration.ReqresIntegration.Entities
{
    public class UserData
    {
        public int Id { get; set; }
        public string? Email { get; set; }
        public string? First_Name { get; set; }
        public string? Last_Name { get; set; }
        public string? Avatar { get; set; }

    public UserData(int id, string email, string first_name, string last_name, string avatar)
        {
            Id = id;
            Email = email;
            First_Name = first_name;
            Last_Name = last_name;
            Avatar = avatar;
        }
    }
}
