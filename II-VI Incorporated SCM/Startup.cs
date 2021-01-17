using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(II_VI_Incorporated_SCM.Startup))]
namespace II_VI_Incorporated_SCM
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
