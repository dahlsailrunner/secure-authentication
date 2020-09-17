using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web;
using System.Web.Optimization;
using System.Web.Routing;
using Serilog;

namespace Globomantics.Framework
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            var name = Assembly.GetExecutingAssembly().GetName().Name;

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.AppSettings()
                .Enrich.WithRequest()
                .Enrich.WithProperty("Assembly", name)
                .WriteTo.Seq("http://localhost:5341")
                .CreateLogger();
        }
    }
}