using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin;
using Owin;
using FlightNode.Identity.Services.Providers;

[assembly: OwinStartup(typeof(FlightNode.DataCollection.Api.Startup))]

namespace FlightNode.DataCollection.Api
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app = ApiStartup.Configure(app, string.Empty);
        }
    }
}
