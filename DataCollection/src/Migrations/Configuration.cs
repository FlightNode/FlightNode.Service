using FlightNode.DataCollection.Infrastructure.Customization;
using System.Data.Entity.Migrations;
using FlightNode.DataCollection.Infrastructure.Persistence;
using FlightNode.DataCollection.Domain.Entities;
using System;

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
            LoadHabitatTypes(context);
            LoadFeedingSuccessRates(context);
            LoadActivityTypes(context);
            LoadSiteAssessments(context);
            LoadVantagePoints(context);
            LoadAccessPoints(context);
        }

        private void LoadAccessPoints(DataCollectionContext context)
        {
            context.AccessPoints.AddOrUpdate(new AccessPoint { Description = "On Foot" });
            context.AccessPoints.AddOrUpdate(new AccessPoint { Description = "Vessel (motor)" });
            context.AccessPoints.AddOrUpdate(new AccessPoint { Description = "Kayak/Canoe" });
        }

        private void LoadVantagePoints(DataCollectionContext context)
        {
            context.VantagePoints.AddOrUpdate(new VantagePoint { Description = "On-Site Visit" });
            context.VantagePoints.AddOrUpdate(new VantagePoint { Description = "View from Adjacent Area by Vehicle/Boat/Foot" });
        }

        private void LoadSiteAssessments(DataCollectionContext context)
        {
            context.SiteAssessments.AddOrUpdate(new SiteAssessment { Description = "New Site" });
            context.SiteAssessments.AddOrUpdate(new SiteAssessment { Description = "Previously Submitted Site" });
            context.SiteAssessments.AddOrUpdate(new SiteAssessment { Description = "Unknown" });
        }

        private void LoadActivityTypes(DataCollectionContext context)
        {
            context.SurveyActivities.AddOrUpdate(new SurveyActivity { Description = "Feeding" });
            context.SurveyActivities.AddOrUpdate(new SurveyActivity { Description = "Preening" });
            context.SurveyActivities.AddOrUpdate(new SurveyActivity { Description = "Loafing" });
            context.SurveyActivities.AddOrUpdate(new SurveyActivity { Description = "Fly Over" });
        }

        private void LoadFeedingSuccessRates(DataCollectionContext context)
        {
            context.FeedingSuccessRates.AddOrUpdate(new FeedingSuccessRate { Description = "0-25% capture/strikes low success" });
            context.FeedingSuccessRates.AddOrUpdate(new FeedingSuccessRate { Description = "25-50% capture/strikes medium success" });
            context.FeedingSuccessRates.AddOrUpdate(new FeedingSuccessRate { Description = "50-75% capture/strikes high success" });
        }

        private void LoadHabitatTypes(DataCollectionContext context)
        {
            context.HabitatTypes.AddOrUpdate(new HabitatType { Description = "Open Water Below Knee" });
            context.HabitatTypes.AddOrUpdate(new HabitatType { Description = "Open Water Above Knee" });
            context.HabitatTypes.AddOrUpdate(new HabitatType { Description = "Manmade (ditch, culvert, etc.)" });
            context.HabitatTypes.AddOrUpdate(new HabitatType { Description = "Forest" });
            context.HabitatTypes.AddOrUpdate(new HabitatType { Description = "Stream" });
            context.HabitatTypes.AddOrUpdate(new HabitatType { Description = "Low Marsh" });
            context.HabitatTypes.AddOrUpdate(new HabitatType { Description = "High Marsh" });
            context.HabitatTypes.AddOrUpdate(new HabitatType { Description = "Scrub-Shrub" });
            context.HabitatTypes.AddOrUpdate(new HabitatType { Description = "Tall Grass" });
            context.HabitatTypes.AddOrUpdate(new HabitatType { Description = "OpenWater" });
            context.HabitatTypes.AddOrUpdate(new HabitatType { Description = "Mudflat" });
            context.HabitatTypes.AddOrUpdate(new HabitatType { Description = "Pond" });
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
