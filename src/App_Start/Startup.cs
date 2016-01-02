using identity=FlightNode.Identity.Services.Providers;
using dataCollection=FlightNode.DataCollection.Services.Providers;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(FlightNode.Service.App.Startup))]

namespace FlightNode.Service.App
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Don't like this hard-coded value, need to determine a better solution
            const string tokenUrl = "http://localhost:50323";

            app = identity.ApiStartup.Configure(app, tokenUrl);
            app = dataCollection.ApiStartup.Configure(app, tokenUrl);
        }
        
        
    }
}