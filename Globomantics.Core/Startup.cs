using System.Data;
using Globomantics.Core.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
            }).AddIdentityCookies();

            services.AddScoped<IPasswordHasher<CustomUser>, CustomPasswordHasher>();

            services.AddIdentityCore<CustomUser>()
                .AddSignInManager<SignInManager<CustomUser>>()
                .AddUserManager<UserManager<CustomUser>>()
                .AddUserStore<CustomUserStore>()
                .AddDefaultTokenProviders()
                .AddDefaultUI();

            services.AddRazorPages();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseExceptionHandler("/Error");

            app.UseHsts();
            app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseSerilogRequestLogging();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
