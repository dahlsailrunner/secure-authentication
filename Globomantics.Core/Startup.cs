using System;
using System.Data;
using System.Net.Http;
using Globomantics.Core.Authorization;
using Globomantics.Core.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Polly;
using Serilog;

namespace Globomantics.Core
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
                new SqlConnection(Configuration.GetConnectionString("GlobomanticsDb")));

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = IdentityConstants.ApplicationScheme;
                options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            }).AddIdentityCookies(o =>
            {
                o.ApplicationCookie.Configure(opts =>
                {
                    opts.LoginPath = "/Identity/Account/Login";
                    opts.AccessDeniedPath = "/Identity/Account/AccessDenied";
                });
                o.ExternalCookie.Configure(opts =>
                {
                    opts.LoginPath = "/Identity/Account/Login";
                    opts.AccessDeniedPath = "/Identity/Account/AccessDenied";
                });
            });

            services.AddSingleton<IAuthorizationPolicyProvider, CustomPolicyProvider>();
            services.AddScoped<IAuthorizationHandler, RightRequirementHandler>();
            services.AddScoped<IAuthorizationHandler, MfaChallengeRequirementHandler>();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("MfaRequired",
                    p =>
                    {
                        p.RequireClaim("CompanyId");
                        p.RequireClaim("MfaEnabled", "True");
                    });
            });

            const int considerPwned = 1000;
            services.AddPwnedPasswordHttpClient(minimumFrequencyToConsiderPwned: considerPwned)
                .AddTransientHttpErrorPolicy(p => p.RetryAsync(3))
                .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(1)));
            services.AddScoped<IPasswordHasher<CustomUser>, CustomPasswordHasher>();

            services.AddTransient<IConfigureOptions<IdentityOptions>, CustomIdentityOptions>();

            services.AddIdentityCore<CustomUser>()
                .AddSignInManager<SignInManager<CustomUser>>()
                //.AddUserManager<UserManager<CustomUser>>()
                .AddUserManager<CustomUserManager>()
                .AddUserStore<CustomUserStore>()
                // not including phone number provider
                .AddTokenProvider<DataProtectorTokenProvider<CustomUser>>(TokenOptions.DefaultProvider)
                .AddTokenProvider<EmailTokenProvider<CustomUser>>(TokenOptions.DefaultEmailProvider)
                .AddTokenProvider<AuthenticatorTokenProvider<CustomUser>>(TokenOptions.DefaultAuthenticatorProvider)
                .AddDefaultUI()
                .AddPwnedPasswordValidator<CustomUser>(options =>
                {
                    options.ErrorMessage = 
                        $"Cannot use passwords that have been pwned more than {considerPwned} times.";
                })
                .AddPasswordValidator<CustomPasswordValidator>()
                .AddUserValidator<CustomUserValidator>();

            // You should likely read the hard-coded string values below from configuration
            services.AddTransient<IEmailSender>(s => new CustomEmailSender(
                "localhost", 25, "donotreply@globomantics.com"));

            services.Configure<DataProtectionTokenProviderOptions>(options =>
            {
                options.TokenLifespan = TimeSpan.FromHours(1);
            });

            services.AddRazorPages();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseExceptionHandler("/Error");

            app.UseHsts();
            app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseSerilogRequestLogging(opts =>
            {
                opts.EnrichDiagnosticContext = (diagCtx, httpCtx) =>
                {
                    diagCtx.Set("ClientIP", httpCtx.Connection.RemoteIpAddress);
                    diagCtx.Set("UserAgent", httpCtx.Request.Headers["User-Agent"]);
                    if (httpCtx.User.Identity.IsAuthenticated)
                    {
                        diagCtx.Set("UserName", httpCtx.User.Identity?.Name);
                    }
                };
            });

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages().RequireAuthorization();
            });
        }
    }
}
