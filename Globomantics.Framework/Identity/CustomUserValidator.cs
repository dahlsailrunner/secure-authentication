using System.Data;
using System.Threading.Tasks;
using Dapper;
using Globomantics.Framework.Models;
using Microsoft.AspNet.Identity;

namespace Globomantics.Framework.Identity
{
    public class CustomUserValidator : UserValidator<CustomUser, int>
    {
        private readonly IDbConnection _db;

        public CustomUserValidator(UserManager<CustomUser, int> manager, IDbConnection db) 
            : base(manager)
        {
            _db = db;
            AllowOnlyAlphanumericUserNames = true;
            RequireUniqueEmail = true;
        }

        public override async Task<IdentityResult> ValidateAsync(CustomUser user)
        {
            var baseResult = await base.ValidateAsync(user);
            if (baseResult != IdentityResult.Success)
            {
                return baseResult;
            }

            var companyMemberResult = await ValidateUserIsCompanyMember(user.LoginName);
            if (companyMemberResult != IdentityResult.Success)
            {
                return companyMemberResult;
            }

            return IdentityResult.Success;
        }

        private async Task<IdentityResult> ValidateUserIsCompanyMember(string memberEmail)
        {
            var companyMember = await _db.QuerySingleOrDefaultAsync<CompanyMember>(
                "SELECT * FROM dbo.CompanyMembers WHERE MemberEmail = @MemberEmail",
                new {memberEmail});

            return companyMember == null 
                ? IdentityResult.Failed("Provided username is not a valid company member.") 
                : IdentityResult.Success;
        }
    }
}
