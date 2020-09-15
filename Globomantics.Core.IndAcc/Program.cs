using System;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Globomantics.Core.IndAcc
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            //EnsureDatabasesCurrent(host.Services);
            host.Run();
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

        //private static void EnsureDatabasesCurrent(IServiceProvider hostServices)
        //{
        //    using (var scope = hostServices.CreateScope())
        //    {
        //        var services = scope.ServiceProvider;
        //        try
        //        {
        //            var ctx = services.GetRequiredService<ApplicationDbContext>();
        //            ctx.InitializeAndUpdate();
        //        }
        //        catch (Exception e)
        //        {
        //            Log.Error(e, "Error occurred initializing the database.");
        //        }
        //    }
        //}
    }
}
