using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AprimoTaskViewer.Startup))]
namespace AprimoTaskViewer
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
