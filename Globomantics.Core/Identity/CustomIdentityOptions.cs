using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Globomantics.Core.Identity
{
    public class CustomIdentityOptions : IConfigureOptions<IdentityOptions>
    {
        public void Configure(IdentityOptions options)
        {
            options.Password.RequiredLength = 6;
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequiredUniqueChars = 1;
        }
    }
}
