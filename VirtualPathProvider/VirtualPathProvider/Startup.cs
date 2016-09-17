using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(VirtualPathProvider.Startup))]
namespace VirtualPathProvider
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
