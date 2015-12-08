using FlightNode.Api.DependencyResolution;
using FlightNode.Identity.Services.Providers;
using Microsoft.Practices.Unity;
using System.Web.Http.Dependencies;

namespace FlightNode.DataCollection.Api
{
    public static class UnityConfig
    {
        public static IDependencyResolver RegisterComponents()
        {
            var container = new UnityContainer() as IUnityContainer;

            container = ApiStartup.ConfigureDependencyInjection(container);

            return new UnityDependencyResolver(container);
        }
    }
}
