using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(VocabularySite.Startup))]
namespace VocabularySite
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
