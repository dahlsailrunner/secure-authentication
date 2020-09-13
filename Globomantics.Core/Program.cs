using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Globomantics.Core
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
            Log.CloseAndFlush();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog((context, provider, loggerConfiguration) =>
                {
                    var name = Assembly.GetEntryAssembly()?.GetName();
                    loggerConfiguration
                        .ReadFrom.Configuration(context.Configuration)
                        .Enrich.FromLogContext()
                        .Enrich.WithProperty("Assembly", name?.Name)
                        .WriteTo.Seq("http://localhost:5341");
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
