using System.Linq;
using System.Threading.Tasks;
using Globomantics.Core.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;

namespace Globomantics.Core.Authorization
{
    public class MfaAuthorizationHandler : AuthorizationHandler<MfaRequirement>
    {
        private readonly UserManager<CustomUser> _userManager;
        private readonly SignInManager<CustomUser> _signInManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        

        public MfaAuthorizationHandler(UserManager<CustomUser> userManager, SignInManager<CustomUser> signInManager, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _httpContextAccessor = httpContextAccessor;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MfaRequirement requirement)
        {
            var amr = context.User.Claims.FirstOrDefault(c => c.Type == "amr")?.Value;

            if (!string.Equals(amr, "mfa"))
            {
                var x = context.Resource;
                var t = x.GetType();
                
                if (context.Resource is RouteEndpoint)
                {
                    _httpContextAccessor.HttpContext.Response.Redirect("/Identity/Account/LoginWith2fa");
                    context.Succeed(requirement);
                }
                
            }
            context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
