using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Reference.Startup))]
namespace Reference
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
