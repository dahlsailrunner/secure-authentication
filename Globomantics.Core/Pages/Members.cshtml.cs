using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Globomantics.Core.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Globomantics.Core.Pages
{
    public class MembersModel : PageModel
    {
        private readonly IDbConnection _db;
        public Company Company { get; set; }

        public MembersModel(IDbConnection db)
        {
            _db = db;
        }

        public void OnGet(int companyId)
        {
            var compDict = new Dictionary<int, Company>();
            var sql = @"
SELECT * 
FROM dbo.Companies c 
JOIN dbo.CompanyMembers cm 
    ON c.Id = cm.CompanyId
WHERE c.Id = @CompanyId";

            Company = _db.Query<Company, CompanyMember, Company>(sql, (c, cm) =>
            {
                if (!compDict.TryGetValue(c.Id, out var currComp))
                {
                    currComp = c;
                    currComp.Members = new List<CompanyMember>();
                    compDict.Add(currComp.Id, currComp);
                }
                currComp.Members.Add(cm);
                return currComp;

            }, new {companyId}).FirstOrDefault();
           
        }
    }
}
