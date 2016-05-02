using FlightNode.DataCollection.Domain.Entities;
using System;
using System.Linq;
using Xunit;

namespace FlightNode.DataCollection.Domain.UnitTests.Domain.Entities
{
    public class SurveyPendingTests
    {

        public class Constructor
        {
            [Fact]
            public void InitializesObservationsList()
            {
                var system = new SurveyPending();

                Assert.Equal(0, system.Observations.Count());
            }

            [Fact]
            public void InitializesDisturbancesList()
            {
                var system = new SurveyPending();

                Assert.Equal(0, system.Disturbances.Count());
            }
        }

        public class AddObservation
        {
            [Fact]
            public void AddsToCollection()
            {
                var item = new Observation();

                var system = new SurveyPending();

                system.Add(item);

                Assert.True(system.Observations.Contains(item));
            }

            [Fact]
            public void ReturnsSelf()
            {
                var item = new Observation();

                var system = new SurveyPending();

                var actual = system.Add(item);

                Assert.Same(system, actual);
            }

            [Fact]
            public void RejectsNullArgument()
            {
                Assert.Throws<ArgumentNullException>(() => new SurveyPending().Add(null as Observation));
            }
        }

        public class AddDisturbance
        {
            [Fact]
            public void AddsToDisturbance()
            {
                var item = new Disturbance();

                var system = new SurveyPending();

                system.Add(item);

                Assert.True(system.Disturbances.Contains(item));
            }

            [Fact]
            public void ReturnsSelf()
            {
                var item = new Disturbance();

                var system = new SurveyPending();

                var actual = system.Add(item);

                Assert.Same(system, actual);
            }

            [Fact]
            public void RejectsNullArgument()
            {
                Assert.Throws<ArgumentNullException>(() => new SurveyPending().Add(null as Disturbance));
            }
        }

        public class ToSurveyCompleted
        {

            protected readonly Guid IDENTIFIER = new Guid("a507f681-c111-447a-bc1f-195916891226");
            protected const int ACCESS_POINT = 1;
            protected const string DISTURBED = "disturbed";
            protected const string DISTURBED_BEHAVIOR = "!#%#@#%";
            protected const int DISTURBED_TYPE_ID = 2;
            protected const int DISTURBED_DURATION = 234;
            protected const int DISTURBED_QUANTITY = 3;
            protected readonly DateTime END_DATE = new DateTime(2013, 2, 3, 5, 2, 6);
            protected readonly DateTime START_DATE = new DateTime(2013, 2, 3, 2, 2, 6);
            protected readonly DateTime LOW_TIDE = new DateTime(2013, 2, 3, 2, 2, 8);
            protected const int LOCATION_ID = 4;
            protected const int ADULTS = 6;
            protected const int SPECIES_ID = 7;
            protected const int FEEDING_ID = 8;
            protected const int HABITAT_ID = 9;
            protected const int JUVENILES = 10;
            protected const int PRIMARY_ACTIVITY_ID = 11;
            protected const int SECONDARY_ACTIVITY_ID = 12;
            protected const int OBSERVER_ID = 13;
            protected const int SITE_TYPE_ID = 14;
            protected const int STEP = 15;
            protected const string SURVEY_COMMENTS = "Survey comments";
            protected const int TEMPERATURE = 16;
            protected const int TIDE = 17;
            protected const int VANTAGE_POINT = 18;
            protected const int WEATHER = 19;
            protected const int WIND = 20;
            protected const int END_TEMPERATURE = 21;
            protected const int USER_ID = 22;
            protected const int SURVEY_TYPE_ID = 23;
            protected const string Observers = "a, b, and c";


            protected SurveyPending CreateDefault()
            {
                var result = new SurveyPending
                {
                    AccessPointId = ACCESS_POINT,
                    AssessmentId = SITE_TYPE_ID,
                    DisturbanceComments = DISTURBED,
                    EndDate = END_DATE,
                    EndTemperature = END_TEMPERATURE,
                    GeneralComments = SURVEY_COMMENTS,
                    LocationId = LOCATION_ID,
                    StartDate = START_DATE,
                    StartTemperature = TEMPERATURE,
                    SubmittedBy = USER_ID,
                    SurveyIdentifier = IDENTIFIER,
                    SurveyTypeId = SURVEY_TYPE_ID,
                    TideId = TIDE,
                    TimeOfLowTide = LOW_TIDE,
                    VantagePointId = VANTAGE_POINT,
                    WeatherId = WEATHER,
                    WindSpeed = WIND,
                    Observers = Observers
                };
                
                result.Add(new Disturbance
                {
                    DisturbanceTypeId = DISTURBED_TYPE_ID,
                    DurationMinutes = DISTURBED_DURATION,
                    Quantity = DISTURBED_QUANTITY,
                    Result = DISTURBED_BEHAVIOR,
                    SurveyIdentifier = IDENTIFIER
                });
                result.Add(new Observation
                {
                    SurveyIdentifier = IDENTIFIER,
                    Bin1 = ADULTS,
                    Bin2 = JUVENILES,
                    FeedingSuccessRate = FEEDING_ID,
                    HabitatTypeId = HABITAT_ID,
                    PrimaryActivityId = PRIMARY_ACTIVITY_ID,
                    SecondaryActivityId = SECONDARY_ACTIVITY_ID
                });

                return result;
            }

            public class ValidInput : ToSurveyCompleted
            {
                private SurveyCompleted RunTest()
                {
                    return CreateDefault().ToSurveyCompleted();
                }

                [Fact]
                public void MapsAccessPoint()
                {
                    Assert.Equal(ACCESS_POINT, RunTest().AccessPointId);
                }

                [Fact]
                public void MapsAssessment()
                {
                    Assert.Equal(SITE_TYPE_ID, RunTest().AssessmentId);
                }

                [Fact]
                public void MapsDisturbanceComments()
                {
                    Assert.Equal(DISTURBED, RunTest().DisturbanceComments);
                }

                [Fact]
                public void MapsDisturbance()
                {
                    Assert.Equal(DISTURBED_BEHAVIOR, RunTest().Disturbances[0].Result);
                }

                [Fact] 
                public void MapsEndDate()
                {
                    Assert.Equal(END_DATE, RunTest().EndDate);
                }

                [Fact]
                public void MapsEndTemperature()
                {
                    Assert.Equal(END_TEMPERATURE, RunTest().EndTemperature);
                }

                [Fact]
                public void MapsGeneralComments()
                {
                    Assert.Equal(SURVEY_COMMENTS, RunTest().GeneralComments);
                }

                [Fact]
                public void MapsLocationId()
                {
                    Assert.Equal(LOCATION_ID, RunTest().LocationId);
                }

                [Fact]
                public void MapsStartDate()
                {
                    Assert.Equal(START_DATE, RunTest().StartDate);
                }

                [Fact]
                public void MapsStartTemperature()
                {
                    Assert.Equal(TEMPERATURE, RunTest().StartTemperature);
                }

                [Fact]
                public void MapsSubmittedBy()
                {
                    Assert.Equal(USER_ID, RunTest().SubmittedBy);
                }

                [Fact]
                public void MapsSurveyIdentifier()
                {
                    Assert.Equal(IDENTIFIER, RunTest().SurveyIdentifier);
                }

                [Fact]
                public void MapsSurveyTypeId()
                {
                    Assert.Equal(SURVEY_TYPE_ID, RunTest().SurveyTypeId);
                }

                [Fact]
                public void MapsTideId()
                {
                    Assert.Equal(TIDE, RunTest().TideId);
                }

                [Fact]
                public void MapsTimeOfLowTide()
                {
                    Assert.Equal(LOW_TIDE, RunTest().TimeOfLowTide);
                }

                [Fact]
                public void MapsVantagePoint()
                {
                    Assert.Equal(VANTAGE_POINT, RunTest().VantagePointId);
                }

                [Fact]
                public void MapsWeatherId()
                {
                    Assert.Equal(WEATHER, RunTest().WeatherId);
                }

                [Fact]
                public void MapsWindSpeed()
                {
                    Assert.Equal(WIND, RunTest().WindSpeed);
                }

                [Fact]
                public void MapsObservers()
                {
                    Assert.Equal(Observers, RunTest().Observers);
                }
            }
        }
    }
}
