using FlightNode.DataCollection.Domain.Entities;
using FlightNode.DataCollection.Domain.Managers;
using FlightNode.DataCollection.Services.Controllers;
using FlightNode.DataCollection.Services.Models.Survey;
using System;
using Xunit;

namespace FlightNode.DataCollection.Domain.UnitTests.Services.Controller.WaterbirdForagingSurveyControllerTests
{

    public class Fixture : LoggingControllerBaseFixture<WaterbirdForagingSurveyController, ISurveyManager>
    {
        protected readonly Guid IDENTIFIER = new Guid("a507f681-c111-447a-bc1f-195916891226");
        protected const int ACCESS_POINT = 1;
        protected const string DISTURBED = "disturbed";
        protected const string DISTURBED_BEHAVIOR = "!#%#@#%";
        protected const int DISTURBED_TYPE_ID = 2;
        protected const int DISTURBED_DURATION = 234;
        protected const int DISTURBED_QUANTITY = 3;
        protected readonly DateTime END_DATE = new DateTime(2016, 6, 11, 20, 05, 0);
        protected const string EndDateString = "2016-06-11T05:00:00.000Z";
        protected const string EndDateStringShort = "2016-06-11";
        protected const string EndTimeString = "1970-01-01T20:05:00.000Z";
        protected const string EndTimeStringShort = "8:05 PM";
        protected readonly DateTime START_DATE = new DateTime(2016, 6, 11, 18, 01, 0);
        protected const string StartDateStringUsFormat = "06/11/2016";
        protected const string StartDateString = "2016-06-11T05:00:00.000Z";
        protected const string StartDateStringShort = "2016-06-11";
        protected const string StartTimeString = "1970-01-01T18:01:00.000Z";
        protected const string StartTimeStringShort = "6:01 PM";
        protected const int LOCATION_ID = 4;
        protected const int ADULTS = 6;
        protected const int SPECIES_ID = 7;
        protected const int FEEDING_ID = 8;
        protected const int HABITAT_ID = 9;
        protected const int JUVENILES = 10;
        protected const int PRIMARY_ACTIVITY_ID = 11;
        protected const int SECONDARY_ACTIVITY_ID = 12;
        protected const int OBSERVER_ID = 13;
        protected const string Observers = "a, b, and c";
        protected const int SITE_TYPE_ID = 14;
        protected const int STEP = 1;
        protected const string SURVEY_COMMENTS = "Survey comments";
        protected const decimal TEMPERATURE = 16.0m;
        protected const bool WindDrivenTide = true;
        protected const int VANTAGE_POINT = 18;
        protected const int WEATHER = 19;
        protected const int WindSpeed = 20;
        protected const int WindDirection = 201;
        protected const int SURVEY_ID = 21;
        protected const int OBSERVATION_ID = 22;
        protected const int DISTURBANCE_ID = 23;
        protected const string LOCATION_NAME = "Charlie's Pasture";
        protected const int WaterHeightId = 24;
        protected const int SubmittedBy = 25;
        protected const decimal PrepTime = 23.99m;

        protected ISurvey BuildDefaultSurvey()
        {
            var domainResult = new SurveyPending();
            domainResult.AccessPointId = ACCESS_POINT;
            domainResult.AssessmentId = SITE_TYPE_ID;
            domainResult.DisturbanceComments = DISTURBED;
            domainResult.Disturbances.Add(new Disturbance
            {
                Id = DISTURBANCE_ID,
                DisturbanceTypeId = DISTURBED_TYPE_ID,
                DurationMinutes = DISTURBED_DURATION,
                Quantity = DISTURBED_QUANTITY,
                Result = DISTURBED_BEHAVIOR,
                SurveyIdentifier = IDENTIFIER
            });
            domainResult.EndDate = END_DATE;
            domainResult.GeneralComments = SURVEY_COMMENTS;
            domainResult.Id = SURVEY_ID;
            domainResult.LocationId = LOCATION_ID;
            domainResult.Observations.Add(new Observation
            {
                Id = OBSERVATION_ID,
                Bin1 = ADULTS,
                Bin2 = JUVENILES,
                BirdSpeciesId = SPECIES_ID,
                FeedingSuccessRate = FEEDING_ID,
                HabitatTypeId = HABITAT_ID,
                PrimaryActivityId = PRIMARY_ACTIVITY_ID,
                SecondaryActivityId = SECONDARY_ACTIVITY_ID,
                SurveyIdentifier = IDENTIFIER
            });
            domainResult.Observers = Observers;
            domainResult.StartDate = START_DATE;
            domainResult.Temperature = TEMPERATURE;
            domainResult.SubmittedBy = 14;
            domainResult.SurveyIdentifier = IDENTIFIER;
            domainResult.SurveyTypeId = 15;
            domainResult.WindDrivenTide = WindDrivenTide;
            domainResult.VantagePointId = VANTAGE_POINT;
            domainResult.WeatherId = WEATHER;
            domainResult.WindSpeed = WindSpeed;
            domainResult.LocationName = LOCATION_NAME;
            domainResult.WaterHeightId = WaterHeightId;
            domainResult.WindDirection = WindDirection;
            domainResult.PrepTimeHours = PrepTime;

            return domainResult;
        }

        protected WaterbirdForagingModel CreateDefautInput()
        {
            var input = new WaterbirdForagingModel
            {
                AccessPointId = ACCESS_POINT,
                DisturbanceComments = DISTURBED,
                LocationId = LOCATION_ID,
                SiteTypeId = SITE_TYPE_ID,
                SurveyComments = SURVEY_COMMENTS,
                Temperature = TEMPERATURE,
                WindDrivenTide = WindDrivenTide,
                VantagePointId = VANTAGE_POINT,
                WeatherId = WEATHER,
                WindSpeed = WindSpeed,
                SurveyId = SURVEY_ID,
                WaterHeightId = WaterHeightId,
                StartDate = StartDateStringUsFormat,
                StartTime = StartTimeString,
                EndTime = EndTimeString,
                Observers = Observers,
                WindDirection = WindDirection,
                SubmittedBy = SubmittedBy,
                PrepTimeHours = PrepTime
            };
            input.Disturbances.Add(new DisturbanceModel
            {
                Behavior = DISTURBED_BEHAVIOR,
                DisturbanceTypeId = DISTURBED_TYPE_ID,
                DurationMinutes = DISTURBED_DURATION,
                Quantity = DISTURBED_QUANTITY,
                DisturbanceId = DISTURBANCE_ID
            });
            input.Observations.Add(new ObservationModel
            {
                Adults = ADULTS,
                BirdSpeciesId = SPECIES_ID,
                FeedingId = FEEDING_ID,
                HabitatId = HABITAT_ID,
                Juveniles = JUVENILES,
                PrimaryActivityId = PRIMARY_ACTIVITY_ID,
                SecondaryActivityId = SECONDARY_ACTIVITY_ID,
                ObservationId = OBSERVATION_ID
            });
            return input;
        }
        protected SurveyPending CreateDefaultEntity()
        {
            var input = new SurveyPending
            {
                AccessPointId = ACCESS_POINT,
                DisturbanceComments = DISTURBED,
                LocationId = LOCATION_ID,
                AssessmentId = SITE_TYPE_ID,
                GeneralComments = SURVEY_COMMENTS,
                Temperature = TEMPERATURE,
                WindDrivenTide = WindDrivenTide,
                VantagePointId = VANTAGE_POINT,
                WeatherId = WEATHER,
                WindSpeed = WindSpeed,
                Id = SURVEY_ID,
                WaterHeightId = WaterHeightId,
                StartDate = START_DATE,
                EndDate = END_DATE,
                Observers = Observers,
                WindDirection = WindDirection,
                SubmittedBy = SubmittedBy,
                PrepTimeHours = PrepTime
            };
            input.Disturbances.Add(new Disturbance
            {
                Result = DISTURBED_BEHAVIOR,
                DisturbanceTypeId = DISTURBED_TYPE_ID,
                DurationMinutes = DISTURBED_DURATION,
                Quantity = DISTURBED_QUANTITY,
                Id = DISTURBANCE_ID
            });
            input.Observations.Add(new Observation
            {
                Bin1 = ADULTS,
                BirdSpeciesId = SPECIES_ID,
                FeedingSuccessRate = FEEDING_ID,
                HabitatTypeId = HABITAT_ID,
                Bin2 = JUVENILES,
                PrimaryActivityId = PRIMARY_ACTIVITY_ID,
                SecondaryActivityId = SECONDARY_ACTIVITY_ID,
                Id = OBSERVATION_ID
            });
            return input;
        }

        protected bool DateValuesAreClose(DateTime one, DateTime two)
        {
            return Math.Abs((one - two).Milliseconds) < 1000;
        }

    }

    public class Constructor : Fixture
    {
        [Fact]
        public void HappyPath()
        {
            BuildSystem();
        }

        [Fact]
        public void RejectsNullArgument()
        {
            Assert.Throws<ArgumentNullException>(() => new WaterbirdForagingSurveyController(null));
        }
    }



}
