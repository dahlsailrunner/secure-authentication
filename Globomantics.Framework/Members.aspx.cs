using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Security;
using System.Web.UI;
using Dapper;
using Globomantics.Framework.Models;

namespace Globomantics.Framework
{
    public partial class Members : Page
    {
        public Company Company { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!User.Identity.IsAuthenticated)
            {
                FormsAuthentication.RedirectToLoginPage();
            }
            var companyId = Convert.ToInt32(Request.QueryString["companyId"]);
            GetCompanyDetails(ConfigurationManager.ConnectionStrings["GlobomanticsDb"]?.ConnectionString, companyId);
        }

        private void GetCompanyDetails(string connStr, int companyId)
        {
            var compDict = new Dictionary<int, Company>();
            var sql = @"
SELECT * 
FROM dbo.Companies c 
JOIN dbo.CompanyMembers cm 
    ON c.Id = cm.CompanyId
WHERE c.Id = @CompanyId";

            using (var db = new SqlConnection(connStr))
            {
                Company = db.Query<Company, CompanyMember, Company>(sql, (c, cm) =>
                {
                    if (!compDict.TryGetValue(c.Id, out var currComp))
                    {
                        currComp = c;
                        currComp.Members = new List<CompanyMember>();
                        compDict.Add(currComp.Id, currComp);
                    }
                    currComp.Members.Add(cm);
                    return currComp;

                }, new { companyId }).FirstOrDefault();
            }
        }
    }
}