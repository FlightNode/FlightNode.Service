using FligthNode.Service.App;
using log4net;
using Microsoft.Owin;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using Owin;
using System.Web.Http;
using dataCollection = FlightNode.DataCollection.Services.Providers;
using identity = FlightNode.Identity.Services.Providers;

[assembly: OwinStartup(typeof(FlightNode.Service.App.Startup))]

namespace FlightNode.Service.App
{
    public class Startup
    {
        private ILog _logger;

        public ILog Logger
        {
            get
            {
                return _logger ?? (_logger = LogManager.GetLogger(GetType().FullName));
            }
            set
            {
                _logger = value;
            }
        }

        public void Configuration(IAppBuilder app)
        {
            // Don't like this hard-coded value, need to determine a better solution
            const string tokenUrl = "http://localhost:50323";


            log4net.Config.XmlConfigurator.Configure();
            Logger.Info("FlightNode application starting from Startup.Configuration.");

            //GlobalConfiguration.Configure(WebApiConfig.Register);

            app = identity.ApiStartup.Configure(app, tokenUrl);
            app = dataCollection.ApiStartup.Configure(app, tokenUrl);


            HttpConfiguration config = new HttpConfiguration();
            WebApiConfig.Register(config);
            app.UseWebApi(config);


            var physicalFileSystem = new PhysicalFileSystem(@"./");
            var options = new FileServerOptions
            {
                EnableDefaultFiles = true,
                FileSystem = physicalFileSystem,
                EnableDirectoryBrowsing = false,
            };
            options.StaticFileOptions.FileSystem = physicalFileSystem;
            options.StaticFileOptions.ServeUnknownFileTypes = false;
            options.DefaultFilesOptions.DefaultFileNames = new[]
            {
                "index.html"
            };

            app.UseFileServer(options);
        }


    }
}