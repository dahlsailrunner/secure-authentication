using Microsoft.AspNetCore.Authorization;

namespace Globomantics.Core.Authorization
{
    public class RequiresMfaAttribute : AuthorizeAttribute
    {
        public RequiresMfaAttribute()
        {
            Policy = "MfaRequirement";
        }
    }
}
