using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(VocabularyWebApp.Startup))]
namespace VocabularyWebApp
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
