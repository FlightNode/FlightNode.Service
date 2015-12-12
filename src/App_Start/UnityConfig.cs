using FlightNode.Api.DependencyResolution;
using identity=FlightNode.Identity.Services.Providers;
using dataCollection = FlightNode.DataCollection.Services.Providers;
using Microsoft.Practices.Unity;
using System.Web.Http.Dependencies;

namespace FlightNode.Identity.App
{
    public static class UnityConfig
    {
        public static IDependencyResolver RegisterComponents()
        {
            var container = new UnityContainer() as IUnityContainer;

            container = identity.ApiStartup.ConfigureDependencyInjection(container);
            container = dataCollection.ApiStartup.ConfigureDependencyInjection(container);
            container = RegisterIDbFactory(container);

            return new UnityDependencyResolver(container);
        }

       
        private static IUnityContainer RegisterIDbFactory(IUnityContainer container)
        {
            // TODO: might need to bring this back. Remove by end of October if not.
            //var dbFactory = new DbFactory(Properties.Settings.Default.ConnectionString);
            //container.RegisterInstance<IDbFactory>(dbFactory);

            return container;
        }
    }
}