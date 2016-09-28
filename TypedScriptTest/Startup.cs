using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TypedScriptTest.Startup))]
namespace TypedScriptTest
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
