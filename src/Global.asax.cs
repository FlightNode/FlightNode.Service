using FligthNode.Identity.App;
using System.Web.Http;
using System.Linq;
using log4net;

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
            log4net.Config.XmlConfigurator.Configure();
            Logger.Info("FlightNode application starting.");

            GlobalConfiguration.Configure(WebApiConfig.Register);
        }

        protected void Application_BeginRequest()
        {
            if (Request.Headers.AllKeys.Contains("Origin") && Request.HttpMethod == "OPTIONS")
            {
                Response.Flush();
            }
        }
    }
}
