using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(email2sms.Startup))]
namespace email2sms
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
