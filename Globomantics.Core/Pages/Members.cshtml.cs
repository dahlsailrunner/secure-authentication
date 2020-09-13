using System.Collections.Generic;
using System.Linq;
using Globomantics.Core.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Globomantics.Core.Pages
{
    public class MembersModel : PageModel
    {
        private readonly CompanyContext _context;
        public  List<CompanyMember> Members { get; set; }
        public Company Company { get; set; }

        public MembersModel(CompanyContext context)
        {
            _context = context;
        }

        public void OnGet(int companyId)
        {
            Company = _context.Companies.Find(companyId);
            Members = _context.CompanyMembers.Where(a => a.CompanyId == companyId)?.ToList();
        }
    }
}
