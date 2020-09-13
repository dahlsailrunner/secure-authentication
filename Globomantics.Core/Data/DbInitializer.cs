using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Globomantics.Core.Data
{
    public static class DbInitializer
    {
        public static void InitializeAndUpdate(this CompanyContext context)
        {
            context.Database.EnsureCreated();
            context.Database.Migrate();

            if (!context.Companies.Any())
            {
                InsertSeedData(context);
            }
        }

        private static void InsertSeedData(CompanyContext context)
        {
            var companies = new List<Company>
            {
                new Company { Name = "Acme Blasting" },
                new Company { Name = "Mars Exploration" }

            };
            context.Companies.AddRange(companies);
            context.SaveChanges();

            var companyMembers = new List<CompanyMember>
            {
                new CompanyMember { CompanyId = 1, MemberEmail = "wile@acme.com" },
                new CompanyMember { CompanyId = 1, MemberEmail = "rr@acme.com" },
                new CompanyMember { CompanyId = 2, MemberEmail = "kim@mars.com" },
                new CompanyMember { CompanyId = 2, MemberEmail = "stanley@mars.com" }

            };
            context.CompanyMembers.AddRange(companyMembers);
            context.SaveChanges();
        }
    }
}
