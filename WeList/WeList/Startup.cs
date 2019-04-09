using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WeList.Startup))]
namespace WeList
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
