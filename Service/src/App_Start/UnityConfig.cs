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

            return new UnityDependencyResolver(container);
        }
    }
}