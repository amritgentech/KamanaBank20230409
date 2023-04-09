using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ReconUi.Startup))]
namespace ReconUi
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
