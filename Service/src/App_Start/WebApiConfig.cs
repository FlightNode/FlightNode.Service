using FlightNode.Common.Api.Filters;
using FlightNode.Identity.App;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using Newtonsoft.Json.Serialization;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.Cors;

namespace FligthNode.Service.App
{
    public static class WebApiConfig
    {
        public static object locker = new object();
        private static bool _hasAlreadyRun;

        public static void Register(HttpConfiguration config)
        {
            if (_hasAlreadyRun)
            {
                // This function runs twice when the application is executed through IIS Express.
                // That is undesirable, so simply exit early if the function has run before.
                return;
            }

            lock (locker)
            {
                _hasAlreadyRun = true;
            }

            //var policy = new EnableCorsAttribute(FlightNode.Service.Properties.Settings.Default.CorsOrigins, "*", "*");
            //config.EnableCors(policy);

            config = ConfigureRoutes(config);
            config = ConfigureFilters(config);
            config.DependencyResolver = UnityConfig.RegisterComponents();
            
            SetupJsonFormatting(config);
        }

        private static void SetupJsonFormatting(HttpConfiguration config)
        {
            var jsonFormatter = config.Formatters.OfType<JsonMediaTypeFormatter>().First();
            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        }

        private static HttpConfiguration ConfigureRoutes(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "FlightNodeApi",
                routeTemplate: "api/v1/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            return config;
        }

        private static HttpConfiguration ConfigureFilters(HttpConfiguration config)
        {
            config.Filters.Add(new NotImplementedExceptionAttribute());
            config.Filters.Add(new UnhandledExceptionFilterAttribute());

            return config;
        }


    }
}