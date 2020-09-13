using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Globomantics.Core.IndAcc.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Globomantics.Core.IndAcc
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddRazorPages();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
            app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseSerilogRequestLogging(options =>
            {
                options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
                {
                    diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
                    var user = httpContext.User.Identity;
                    if (user != null && user.IsAuthenticated)
                    {
                        var userInfo = GetUserInfoFromHttpContext(user as ClaimsIdentity);
                        diagnosticContext.Set("UserInfo", userInfo);
                    }
                };
            });

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }

        private static UserInfo GetUserInfoFromHttpContext(ClaimsIdentity user)
        {
            var excluded = new List<string>
            {
                "nbf", "exp", "auth_time", "amr", "sub", "aud", "jti"
            };
            const string userIdClaimType = "sub";
            var userId = user.Claims.FirstOrDefault(a => a.Type == userIdClaimType)?.Value;
            var userInfo = new UserInfo
            {
                UserName = user.Name,
                UserId = userId,
                UserClaims = new Dictionary<string, string>()
            };
            foreach (var distinctClaimType in user.Claims
                .Where(a => excluded.All(ex => ex != a.Type))
                .Select(a => a.Type)
                .Distinct())
            {
                var claimValues = string.Join(" | ", user.Claims
                    .Where(a => a.Type == distinctClaimType)
                    .Select(c => c.Value));
                userInfo.UserClaims[distinctClaimType] = claimValues;
            }

            return userInfo;
        }
    }
}
