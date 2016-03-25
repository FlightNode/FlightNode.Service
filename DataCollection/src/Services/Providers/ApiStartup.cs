using Microsoft.Practices.Unity;
using Owin;
using System.Reflection;
using System.Linq;
using FlightNode.DataCollection.Domain.Entities;

namespace FlightNode.DataCollection.Services.Providers
{
    public static class ApiStartup
    {
        public static IAppBuilder Configure(IAppBuilder app)
        {
            // No action to take. Leaving this stub as a reminder that we *could*
            // customize API startup for DataCollection, if needed.

            return app;
        }


        public static IUnityContainer ConfigureDependencyInjection(IUnityContainer container)
        {
            container = RegisterAllTypesIn(container, Assembly.GetExecutingAssembly());

            //container.RegisterType<IdentityDbContext>();
            //container.RegisterType(typeof(IUserStore<User, int>), typeof(UserStore));

            return container;
        }

        private static IUnityContainer RegisterAllTypesIn(IUnityContainer container, Assembly repoAssembly)
        {
            //container.RegisterTypes(AllClasses.FromAssemblies(repoAssembly),
            //                                     WithMappings.FromAllInterfacesInSameAssembly,
            //                                     WithName.Default,
            //                                     WithLifetime.PerResolve);

            var typesToRegister = AllClasses.FromAssemblies(repoAssembly)
                .Where(t => !typeof(ISurvey).IsAssignableFrom(t))
                .ToList();

            container.RegisterTypes(typesToRegister,
                WithMappings.FromAllInterfacesInSameAssembly,
                WithName.Default,
                WithLifetime.PerResolve);

            return container;
        }

    }
}
