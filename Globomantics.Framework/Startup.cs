using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Globomantics.Framework.Startup))]
namespace Globomantics.Framework
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
