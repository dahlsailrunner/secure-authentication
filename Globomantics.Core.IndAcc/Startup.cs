using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using Globomantics.Core.IndAcc.Areas.Identity;
using Globomantics.Core.IndAcc.Areas.Identity.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.SqlClient;
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
            services.AddScoped<IDbConnection, SqlConnection>(db => 
                new SqlConnection(Configuration.GetConnectionString("DefaultConnection")));

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = IdentityConstants.ApplicationScheme;
                options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            }).AddIdentityCookies();

            services.AddScoped<IPasswordHasher<CustomUser>, CustomPasswordHasher>();

            services.AddIdentityCore<CustomUser>(options =>
                {
                    options.Lockout.MaxFailedAccessAttempts = 5;
                    options.Lockout.AllowedForNewUsers = true;
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
                })
                .AddSignInManager<SignInManager<CustomUser>>()
                .AddUserManager<UserManager<CustomUser>>()
                .AddUserStore<CustomUserStore>()
                .AddTokenProvider<DataProtectorTokenProvider<CustomUser>>(TokenOptions.DefaultProvider)
                .AddTokenProvider<EmailTokenProvider<CustomUser>>(TokenOptions.DefaultEmailProvider)  // not including phone number provider
                .AddTokenProvider<AuthenticatorTokenProvider<CustomUser>>(TokenOptions.DefaultAuthenticatorProvider)
                .AddDefaultUI();

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
                    //var user = httpContext.User.Identity;
                    //if (user != null && user.IsAuthenticated)
                    //{
                    //    var userInfo = GetUserInfoFromHttpContext(user as ClaimsIdentity);
                    //    diagnosticContext.Set("UserInfo", userInfo);
                    //}
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
