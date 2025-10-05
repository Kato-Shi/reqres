using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReqResIntegratedApplication.Integration.ReqresIntegration.Entities
{
    class ResourceData
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int Year { get; set; }
        public string? Color { get; set; }
        public string? Pantone_Value { get; set; }
    }
}
