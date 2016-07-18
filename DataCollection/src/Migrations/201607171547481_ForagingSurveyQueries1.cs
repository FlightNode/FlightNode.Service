using FlightNode.DataCollection.Infrastructure.Customization;
using System.Data.Entity.Migrations;

namespace FlightNode.DataCollection.Domain.Migrations
{

    public partial class ForagingSurveyQueries1 : DbMigration
    {
        public override void Up()
        {

            
            //CreateTable(
            //    "dbo.Users",
            //    c => new
            //        {
            //            Id = c.Int(nullable: false, identity: true),
            //            GivenName = c.String(),
            //            FamilyName = c.String(),
            //        })
            //    .PrimaryKey(t => t.Id);


            this.CreateView("dbo.WaterbirdForagingExport", @"WITH observation as (
	SELECT 
        obs.Id,
		svy.SurveyIdentifier,
		loc.SiteCode,
		loc.SiteName,
		loc.City,
		loc.County,
		loc.Longitude,
		loc.Latitude,
		assess.Description as Assessment,
		svy.StartDate,
		svy.EndDate,
		svy.StartTemperature,
		weather.Description as Weather,
		tide.Description as Tide,
		svy.WindSpeed,
		vntg.Description as VantagePoint,
		access.Description as AccessPoint,
		usr.GivenName + ' ' + usr.FamilyName as SubmittedBy,
		ISNULL(svy.Observers,'') as Observers,
		ISNULL(svy.GeneralComments, '') as GeneralComments,
		bs.Genus,
		bs.Species,
		bs.CommonAlphaCode,
		bs.CommonName,
		obs.Bin1 as NumberOfAdults,
		obs.Bin2 as NumberOfJuveniles,
		actp.Description as PrimaryActivity,
		acts.Description as SecondaryActivity,
		fr.Description as FeedingSuccessRate,
		hab.Description as HabitatType,
		ISNULL(svy.DisturbanceComments, '') as DisturbanceComments
	FROM dbo.SurveyCompleted svy
	LEFT OUTER JOIN dbo.Observations obs ON svy.SurveyIdentifier = obs.SurveyIdentifier
										AND (obs.Bin1 > 0 OR obs.Bin2 > 0)
	LEFT OUTER JOIN dbo.BirdSpecies bs ON obs.BirdSpeciesId = bs.Id
	LEFT OUTER JOIN dbo.FeedingSuccessRates fr ON obs.FeedingSuccessRate = fr.Id
	LEFT OUTER JOIN dbo.SurveyActivities actp ON obs.PrimaryActivityId = actp.Id
	LEFT OUTER JOIN dbo.SurveyActivities acts ON obs.SecondaryActivityId = acts.Id
	LEFT OUTER JOIN dbo.HabitatTypes hab ON obs.HabitatTypeId = hab.Id
	INNER JOIN dbo.Locations loc ON svy.LocationId = loc.Id
	INNER JOIN dbo.AccessPoints access ON svy.AccessPointId = access.Id
	INNER JOIN dbo.SiteAssessments assess ON svy.AssessmentId = assess.Id
	INNER JOIN dbo.Tides tide ON svy.TideId = tide.Id
	INNER JOIN dbo.Users usr ON svy.SubmittedBy = usr.Id
	INNER JOIN dbo.VantagePoints vntg ON svy.VantagePointId = vntg.Id
	INNER JOIN dbo.Weather weather ON svy.WeatherId = weather.Id
	-- Not used for foraging
	--INNER JOIN dbo.WaterHeights water ON svy.WaterHeightId = water.Id
	WHERE svy.SurveyTypeId = 2
)
SELECT o.*,
	ISNULL(kayak.Quantity,0) as KayakerQuantity,
	ISNULL(kayak.DurationMinutes,0) as KayakerDurationMinutes,
	ISNULL(kayak.Result,'') as KayakResult,
	ISNULL(wading.Quantity,0) as FishermenWadingQuantity,
	ISNULL(wading.DurationMinutes,0) as FishermenWaidingDurationMinutes,
	ISNULL(wading.Result,'') as FishermenWaidingResult,
	ISNULL(stationary.Quantity,0) as StationaryBoatsQuantity,
	ISNULL(stationary.DurationMinutes,0) as StationaryBoatsDurationMinutes,
	ISNULL(stationary.Result,'') as StationaryBoatsResult,
	ISNULL(moving.Quantity,0) as MovingBoatsQuantity,
	ISNULL(moving.DurationMinutes,0) as MovingBoatsDurationMinutes,
	ISNULL(moving.Result,'') as MovingBoatsResult,
	ISNULL(personal.Quantity,0) as PersonalWatercraftQuantity,
	ISNULL(personal.DurationMinutes,0) as PersonalWatercraftDurationMinutes,
	ISNULL(personal.Result,'') as PersonalWatercraftResult,
	ISNULL(humans.Quantity,0) as HumansQuantity,
	ISNULL(humans.DurationMinutes,0) as HumansMinutes,
	ISNULL(humans.Result,'') as HumansResult,
	ISNULL(noise.Quantity,0) as NoiseQuantity,
	ISNULL(noise.DurationMinutes,0) as NoiseMinutes,
	ISNULL(noise.Result,'') as NoiseResult,
	ISNULL(other.Quantity,0) as OtherDisturbanceQuantity,
	ISNULL(other.DurationMinutes,0) as OtherDisturbanceMinutes,
	ISNULL(other.Result,'') as OtherDisturbanceResult
FROM Observation o
LEFT OUTER JOIN dbo.Disturbances kayak ON o.SurveyIdentifier = kayak.SurveyIdentifier AND kayak.DisturbanceTypeId = 1
LEFT OUTER JOIN dbo.Disturbances wading ON o.SurveyIdentifier = wading.SurveyIdentifier AND wading.DisturbanceTypeId = 2
LEFT OUTER JOIN dbo.Disturbances stationary ON o.SurveyIdentifier = stationary.SurveyIdentifier AND stationary.DisturbanceTypeId = 3
LEFT OUTER JOIN dbo.Disturbances moving ON o.SurveyIdentifier = moving.SurveyIdentifier AND moving.DisturbanceTypeId = 4
LEFT OUTER JOIN dbo.Disturbances personal ON o.SurveyIdentifier = personal.SurveyIdentifier AND personal.DisturbanceTypeId = 5
LEFT OUTER JOIN dbo.Disturbances humans ON o.SurveyIdentifier = humans.SurveyIdentifier AND humans.DisturbanceTypeId = 6
LEFT OUTER JOIN dbo.Disturbances noise ON o.SurveyIdentifier = noise.SurveyIdentifier AND noise.DisturbanceTypeId = 7
LEFT OUTER JOIN dbo.Disturbances other ON o.SurveyIdentifier = other.SurveyIdentifier AND other.DisturbanceTypeId = 8");

        }
        
        public override void Down()
        {
            this.DropView("dbo.WaterbirdForagingExport");
        }
    }
}
