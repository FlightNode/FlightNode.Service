using FlightNode.DataCollection.Infrastructure.Customization;
using System.Data.Entity.Migrations;
using FlightNode.DataCollection.Infrastructure.Persistence;
using FlightNode.DataCollection.Domain.Entities;

namespace FlightNode.DataCollection.Domain.Migrations
{


    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public sealed class Configuration : DbMigrationsConfiguration<DataCollectionContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            SetSqlGenerator("System.Data.SqlClient", new CustomSqlServerMigrationSqlGenerator());
        }

        protected override void Seed(DataCollectionContext context)
        {
            LoadDisturbanceTypesForWaterbirdForaging(context);
            LoadTidesForWaterbirdForaging(context);
            LoadWeatherForWaterbirdForaging(context);
        }

        private void LoadWeatherForWaterbirdForaging(DataCollectionContext context)
        {
            context.Weather.AddOrUpdate(new Weather { Description = "Clear" });
            context.Weather.AddOrUpdate(new Weather { Description = "Fog or smoke" });
            context.Weather.AddOrUpdate(new Weather { Description = "Partly cloudy" });
            context.Weather.AddOrUpdate(new Weather { Description = "Drizzle" });
            context.Weather.AddOrUpdate(new Weather { Description = "Overcast" });
            context.Weather.AddOrUpdate(new Weather { Description = "Showers" });
        }

        private void LoadTidesForWaterbirdForaging(DataCollectionContext context)
        {
            context.Tides.AddOrUpdate(new Tide { Description = "Water level apepars high" });
            context.Tides.AddOrUpdate(new Tide { Description = "Water level appears low" });
            context.Tides.AddOrUpdate(new Tide { Description = "Wind-driven" });
            context.Tides.AddOrUpdate(new Tide { Description = "Non-tidal" });
        }

        private void LoadDisturbanceTypesForWaterbirdForaging(DataCollectionContext context)
        {
            context.DisturbanceType.AddOrUpdate(new DisturbanceType { Description = "Kayakers" });
            context.DisturbanceType.AddOrUpdate(new DisturbanceType { Description = "Fishermen (wading)" });
            context.DisturbanceType.AddOrUpdate(new DisturbanceType { Description = "Stationary boats" });
            context.DisturbanceType.AddOrUpdate(new DisturbanceType { Description = "Moving boats" });
            context.DisturbanceType.AddOrUpdate(new DisturbanceType { Description = "Personal watercraft (jetski, windsurfer, etc.)" });
            context.DisturbanceType.AddOrUpdate(new DisturbanceType { Description = "Humans on foot" });
            context.DisturbanceType.AddOrUpdate(new DisturbanceType { Description = "Noise (specify source)" });
            context.DisturbanceType.AddOrUpdate(new DisturbanceType { Description = "Other (list)" });

        }
    }
}
