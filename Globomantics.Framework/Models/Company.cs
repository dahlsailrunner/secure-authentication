using System.Collections.Generic;

namespace Globomantics.Framework.Models
{
    public class Company
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<CompanyMember> Members { get; set; }
    }
}
