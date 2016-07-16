using System.Web.Http;
using System.Linq;
using log4net;
using System.Web.Routing;
using System.Web.Mvc;

namespace FlightNode.Service.App
{
    public class FlightNodeService : System.Web.HttpApplication
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

        protected void Application_Start()
        {
            // This method is not required when running in full IIS - it will in fact cause
            // WebApiConfig.Register to run twice. But, when running in IIS Express, we apparently
            // need to enable CORS earlier in the setup, which this effectively accomplishes.
            GlobalConfiguration.Configure(WebApiConfig.Register);

            RegisterRoutes(RouteTable.Routes);
        }

        protected void Application_BeginRequest()
        {
            if (Request.Headers.AllKeys.Contains("Origin") && Request.HttpMethod == "OPTIONS")
            {
                Response.Flush();
            }
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
