using FlightNode.Common.Exceptions;
using FlightNode.DataCollection.Domain.Entities;
using FlightNode.DataCollection.Domain.Managers;
using FlightNode.DataCollection.Services.Controllers;
using FlightNode.DataCollection.Services.Models.Rookery;
using Moq;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using Xunit;

namespace FlightNode.DataCollection.Domain.UnitTests.Services.Controller
{
    public class WaterbirdForagingSurveyControllerTests
    {
        public class Fixture : LoggingControllerBaseFixture<WaterbirdForagingSurveyController, IWaterbirdForagingManager>
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

            protected Mock<IPrincipal> MockPrincipal;
            protected Mock<IIdentity> MockIdentity;


            protected WaterbirdForagingModel CreateDefautInput()
            {
                var input = new WaterbirdForagingModel
                {
                    AccessPointInfoId = ACCESS_POINT,
                    DisturbanceComments = DISTURBED,
                    EndDate = END_DATE,
                    LocationId = LOCATION_ID,
                    SiteTypeId = SITE_TYPE_ID,
                    StartDate = START_DATE,
                    Step = STEP,
                    SurveyComments = SURVEY_COMMENTS,
                    Temperature = TEMPERATURE,
                    TideInfoId = TIDE,
                    VantagePointInfoId = VANTAGE_POINT,
                    WeatherInfoId = WEATHER,
                    WindSpeed = WIND,
                    TimeOfLowTide = LOW_TIDE
                };
                input.Disturbances.Add(new DisturbanceModel
                {
                    Behavior = DISTURBED_BEHAVIOR,
                    DisturbanceTypeId = DISTURBED_TYPE_ID,
                    DurationMinutes = DISTURBED_DURATION,
                    Quantity = DISTURBED_QUANTITY
                });
                input.Observations.Add(new ObservationModel
                {
                    Adults = ADULTS,
                    BirdSpeciesId = SPECIES_ID,
                    FeedingId = FEEDING_ID,
                    HabitatId = HABITAT_ID,
                    Juveniles = JUVENILES,
                    PrimaryActivityId = PRIMARY_ACTIVITY_ID,
                    SecondaryActivityId = SECONDARY_ACTIVITY_ID
                });
                input.Observers.Add(OBSERVER_ID);
                return input;
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

        public class Post : Fixture
        {

            public class HappyPath : Post
            {
                [Fact]
                public void MapsAccessPoint()
                {
                    RunPositiveTest();

                    MockDomainManager.Verify(x => x.Create(It.Is<SurveyPending>(y => ACCESS_POINT == y.AccessPointId)));
                }

                [Fact]
                public void MapsDisturbanceComment()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Create(It.Is<SurveyPending>(y => DISTURBED == y.DisturbanceComments)));
                }


                [Fact]
                public void MapsEndDate()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Create(It.Is<SurveyPending>(y => END_DATE == y.EndDate)));
                }

                [Fact]
                public void MapsStartDate()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Create(It.Is<SurveyPending>(y => START_DATE == y.StartDate)));
                }

                [Fact]
                public void MapsLocationId()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Create(It.Is<SurveyPending>(y => LOCATION_ID == y.LocationId)));
                }

                [Fact]
                public void MapsLowTide()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Create(It.Is<SurveyPending>(y => LOW_TIDE == y.TimeOfLowTide)));
                }

                [Fact]
                public void MapsSiteTypeIdToAssessment()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Create(It.Is<SurveyPending>(y => SITE_TYPE_ID == y.AssessmentId)));
                }

                [Fact]
                public void MapsSurveyCommentsToGeneralComments()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Create(It.Is<SurveyPending>(y => SURVEY_COMMENTS == y.GeneralComments)));
                }

                [Fact]
                public void MapsTemperature()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Create(It.Is<SurveyPending>(y => TEMPERATURE == y.StartTemperature)));
                }

                [Fact]
                public void MapsTide()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Create(It.Is<SurveyPending>(y => TIDE == y.TideId)));
                }

                [Fact]
                public void MapsVantagePoint()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Create(It.Is<SurveyPending>(y => VANTAGE_POINT == y.VantagePointId)));
                }

                [Fact]
                public void MapsWeather()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Create(It.Is<SurveyPending>(y => WEATHER == y.WeatherId)));
                }

                [Fact]
                public void MapsWindSpeed()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Create(It.Is<SurveyPending>(y => WIND == y.WindSpeedId)));
                }

                [Fact]
                public void MapsDisturbedBehavior()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Create(It.Is<SurveyPending>(y => DISTURBED_BEHAVIOR == y.Disturbances.First().Result)));
                }

                [Fact]
                public void MapsDisturbanceTypeId()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Create(It.Is<SurveyPending>(y => DISTURBED_TYPE_ID == y.Disturbances.First().DisturbanceTypeId)));
                }

                [Fact]
                public void MapsDisturbanceDuration()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Create(It.Is<SurveyPending>(y => DISTURBED_DURATION == y.Disturbances.First().DurationMinutes)));
                }

                [Fact]
                public void MapsDisturbanceQuantity()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Create(It.Is<SurveyPending>(y => DISTURBED_QUANTITY == y.Disturbances.First().Quantity)));
                }

                [Fact]
                public void MapsIdentifierIntoDisturbance()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Create(It.Is<SurveyPending>(y => IDENTIFIER == y.Disturbances.First().SurveyIdentifier)));
                }

                [Fact]
                public void MapsObserver()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Create(It.Is<SurveyPending>(y => OBSERVER_ID == y.Observers.First())));
                }

                [Fact]
                public void MapsIdentifierIntoObservation()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Create(It.Is<SurveyPending>(y => IDENTIFIER == y.Observations.First().SurveyIdentifier)));
                }

                [Fact]
                public void MapAdultsToBin1()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Create(It.Is<SurveyPending>(y => ADULTS == y.Observations.First().Bin1)));
                }


                [Fact]
                public void MapJuvenilesToBin2()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Create(It.Is<SurveyPending>(y => JUVENILES == y.Observations.First().Bin2)));
                }

                [Fact]
                public void MapPrimaryBehavior()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Create(It.Is<SurveyPending>(y => PRIMARY_ACTIVITY_ID == y.Observations.First().PrimaryActivityId)));
                }

                [Fact]
                public void MapSecondaryBehavior()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Create(It.Is<SurveyPending>(y => SECONDARY_ACTIVITY_ID == y.Observations.First().SecondaryActivityId)));
                }

                [Fact]
                public void MapSpeciesId()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Create(It.Is<SurveyPending>(y => SPECIES_ID == y.Observations.First().BirdSpeciesId)));
                }

                [Fact]
                public void MapsFeedingId()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Create(It.Is<SurveyPending>(y => FEEDING_ID == y.Observations.First().FeedingSuccessRate)));
                }

                [Fact]
                public void MapsHabitatId()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Create(It.Is<SurveyPending>(y => HABITAT_ID == y.Observations.First().HabitatTypeId)));
                }

                [Fact]
                public void MapsSurveyTypeId()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Create(It.Is<SurveyPending>(y => SurveyType.TERN_FORAGING == y.SurveyTypeId)));
                }

                [Fact]
                public void MapsCurrentUserAsSubmittedBy()
                {
                    // Arrange
                    MockPrincipal = MockRepository.Create<IPrincipal>();
                    MockIdentity = MockRepository.Create<IIdentity>();
                    MockPrincipal.SetupGet(x => x.Identity)
                        .Returns(MockIdentity.Object);


                    MockDomainManager.Setup(x => x.NewIdentifier())
                        .Returns(IDENTIFIER);

                    MockDomainManager.Setup(x => x.Create(It.IsAny<SurveyPending>()))
                        .Returns(1);

                    var system = BuildSystem();


                    system.User = MockPrincipal.Object;

                    //
                    // Act
                    var result = ExecuteHttpAction(system.Post(new WaterbirdForagingModel()));

                    // Assert
                    MockPrincipal.VerifyAll();
                }


                [Fact]
                public void RespondsWithCreated()
                {
                    HttpResponseMessage result = RunPositiveTest();

                    //
                    // Assert
                    Assert.Equal(HttpStatusCode.Created, result.StatusCode);
                }

                [Fact]
                public void RespondsWithLocation()
                {
                    var expected = url + IDENTIFIER.ToString();

                    HttpResponseMessage result = RunPositiveTest();

                    //
                    // Assert
                    Assert.Equal(expected, result.Headers.Location.ToString());
                }

                private HttpResponseMessage RunPositiveTest()
                {
                    //
                    // Arrange
                    WaterbirdForagingModel input = CreateDefautInput();

                    MockDomainManager.Setup(x => x.NewIdentifier())
                        .Returns(IDENTIFIER);

                    MockDomainManager.Setup(x => x.Create(It.IsAny<SurveyPending>()))
                        .Returns(1);


                    var system = BuildSystem();

                    //
                    // Act
                    var result = ExecuteHttpAction(system.Post(input));
                    return result;
                }

                
            }

            public class ExceptionHandling : Post
            {

                private HttpResponseMessage RunTest(Exception ex)
                {
                    MockDomainManager.Setup(x => x.NewIdentifier())
                        .Returns(IDENTIFIER);
                    MockDomainManager.Setup(x => x.Create(It.IsAny<SurveyPending>()))
                        .Throws(ex);


                    return BuildSystem().Post(CreateDefautInput()).ExecuteAsync(new System.Threading.CancellationToken()).Result;
                }

                [Fact]
                public void ConfirmHandlingOfInvalidOperation()
                {
                    ExpectToLogToError();

                    var e = new InvalidOperationException();
                    Assert.Equal(HttpStatusCode.InternalServerError, RunTest(e).StatusCode);
                }

                [Fact]
                public void ConfirmHandlingOfServerError()
                {
                    ExpectToLogToError();

                    var e = ServerException.HandleException<ExceptionHandling>(new Exception(), "asdf");
                    Assert.Equal(HttpStatusCode.InternalServerError, RunTest(e).StatusCode);
                }

                [Fact]
                public void ConfirmHandlingOfUserError()
                {
                    ExpectToLogToDebug();

                    var e = new UserException("asdf");
                    Assert.Equal(HttpStatusCode.BadRequest, RunTest(e).StatusCode);
                }
            }
        }

        public class Put : Fixture
        {

            public class HappyPath : Put
            {

                [Fact]
                public void MapsAccessPoint()
                {
                    RunPositiveTest();

                    MockDomainManager.Verify(x => x.Update(It.Is<SurveyPending>(y => ACCESS_POINT == y.AccessPointId), It.Is<int>(y=>y== STEP)));
                }

                [Fact]
                public void MapsDisturbanceComment()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Update(It.Is<SurveyPending>(y => DISTURBED == y.DisturbanceComments), It.Is<int>(y => y == STEP)));
                }


                [Fact]
                public void MapsEndDate()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Update(It.Is<SurveyPending>(y => END_DATE == y.EndDate), It.Is<int>(y => y == STEP)));
                }

                [Fact]
                public void MapsStartDate()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Update(It.Is<SurveyPending>(y => START_DATE == y.StartDate), It.Is<int>(y => y == STEP)));
                }

                [Fact]
                public void MapsLocationId()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Update(It.Is<SurveyPending>(y => LOCATION_ID == y.LocationId), It.Is<int>(y => y == STEP)));
                }

                [Fact]
                public void MapsLowTide()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Update(It.Is<SurveyPending>(y => LOW_TIDE == y.TimeOfLowTide), It.Is<int>(y => y == STEP)));
                }

                [Fact]
                public void MapsSiteTypeIdToAssessment()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Update(It.Is<SurveyPending>(y => SITE_TYPE_ID == y.AssessmentId), It.Is<int>(y => y == STEP)));
                }

                [Fact]
                public void MapsSurveyCommentsToGeneralComments()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Update(It.Is<SurveyPending>(y => SURVEY_COMMENTS == y.GeneralComments), It.Is<int>(y => y == STEP)));
                }

                [Fact]
                public void MapsTemperature()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Update(It.Is<SurveyPending>(y => TEMPERATURE == y.StartTemperature), It.Is<int>(y => y == STEP)));
                }

                [Fact]
                public void MapsTide()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Update(It.Is<SurveyPending>(y => TIDE == y.TideId), It.Is<int>(y => y == STEP)));
                }

                [Fact]
                public void MapsVantagePoint()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Update(It.Is<SurveyPending>(y => VANTAGE_POINT == y.VantagePointId), It.Is<int>(y => y == STEP)));
                }

                [Fact]
                public void MapsWeather()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Update(It.Is<SurveyPending>(y => WEATHER == y.WeatherId), It.Is<int>(y => y == STEP)));
                }

                [Fact]
                public void MapsWindSpeed()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Update(It.Is<SurveyPending>(y => WIND == y.WindSpeedId), It.Is<int>(y => y == STEP)));
                }

                [Fact]
                public void MapsDisturbedBehavior()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Update(It.Is<SurveyPending>(y => DISTURBED_BEHAVIOR == y.Disturbances.First().Result), It.Is<int>(y => y == STEP)));
                }

                [Fact]
                public void MapsDisturbanceTypeId()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Update(It.Is<SurveyPending>(y => DISTURBED_TYPE_ID == y.Disturbances.First().DisturbanceTypeId), It.Is<int>(y => y == STEP)));
                }

                [Fact]
                public void MapsDisturbanceDuration()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Update(It.Is<SurveyPending>(y => DISTURBED_DURATION == y.Disturbances.First().DurationMinutes), It.Is<int>(y => y == STEP)));
                }

                [Fact]
                public void MapsDisturbanceQuantity()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Update(It.Is<SurveyPending>(y => DISTURBED_QUANTITY == y.Disturbances.First().Quantity), It.Is<int>(y => y == STEP)));
                }

                [Fact]
                public void MapsIdentifierIntoDisturbance()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Update(It.Is<SurveyPending>(y => IDENTIFIER == y.Disturbances.First().SurveyIdentifier), It.Is<int>(y => y == STEP)));
                }

                [Fact]
                public void MapsObserver()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Update(It.Is<SurveyPending>(y => OBSERVER_ID == y.Observers.First()), It.Is<int>(y => y == STEP)));
                }

                [Fact]
                public void MapsIdentifierIntoObservation()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Update(It.Is<SurveyPending>(y => IDENTIFIER == y.Observations.First().SurveyIdentifier), It.Is<int>(y => y == STEP)));
                }

                [Fact]
                public void MapAdultsToBin1()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Update(It.Is<SurveyPending>(y => ADULTS == y.Observations.First().Bin1), It.Is<int>(y => y == STEP)));
                }


                [Fact]
                public void MapJuvenilesToBin2()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Update(It.Is<SurveyPending>(y => JUVENILES == y.Observations.First().Bin2), It.Is<int>(y => y == STEP)));
                }

                [Fact]
                public void MapPrimaryBehavior()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Update(It.Is<SurveyPending>(y => PRIMARY_ACTIVITY_ID == y.Observations.First().PrimaryActivityId), It.Is<int>(y => y == STEP)));
                }

                [Fact]
                public void MapSecondaryBehavior()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Update(It.Is<SurveyPending>(y => SECONDARY_ACTIVITY_ID == y.Observations.First().SecondaryActivityId), It.Is<int>(y => y == STEP)));
                }

                [Fact]
                public void MapSpeciesId()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Update(It.Is<SurveyPending>(y => SPECIES_ID == y.Observations.First().BirdSpeciesId), It.Is<int>(y => y == STEP)));
                }

                [Fact]
                public void MapsFeedingId()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Update(It.Is<SurveyPending>(y => FEEDING_ID == y.Observations.First().FeedingSuccessRate), It.Is<int>(y => y == STEP)));
                }

                [Fact]
                public void MapsHabitatId()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Update(It.Is<SurveyPending>(y => HABITAT_ID == y.Observations.First().HabitatTypeId), It.Is<int>(y => y == STEP)));
                }

                [Fact]
                public void MapsSurveyTypeId()
                {
                    RunPositiveTest();
                    MockDomainManager.Verify(x => x.Update(It.Is<SurveyPending>(y => SurveyType.TERN_FORAGING == y.SurveyTypeId), It.Is<int>(y => y == STEP)));
                }


                [Fact]
                public void RespondsWithNoContent()
                {
                    HttpResponseMessage result = RunPositiveTest();

                    //
                    // Assert
                    Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
                }
                

                private HttpResponseMessage RunPositiveTest()
                {
                    //
                    // Arrange
                    WaterbirdForagingModel input = CreateDefautInput();
                    input.SurveyIdentifer = IDENTIFIER;

                    MockDomainManager.Setup(x => x.Update(It.IsAny<SurveyPending>(), It.Is<int>(y => y == STEP)));


                    var system = BuildSystem();

                    //
                    // Act
                    var result = ExecuteHttpAction(system.Put(input));

                    return result;
                }


            }

            public class ExceptionHandling : Put
            {

                private HttpResponseMessage RunTest(Exception ex)
                {
                    MockDomainManager.Setup(x => x.Update(It.IsAny<SurveyPending>(), It.Is<int>(y => y == STEP)))
                        .Throws(ex);


                    var input = CreateDefautInput();
                    input.SurveyIdentifer = IDENTIFIER;

                    return BuildSystem().Put(input).ExecuteAsync(new System.Threading.CancellationToken()).Result;
                }

                [Fact]
                public void MissingIdentifierGeneratesBadRequest()
                {

                    //
                    // Arrange
                    WaterbirdForagingModel input = CreateDefautInput();
                    input.SurveyIdentifer = null;
                    
                    //
                    // Act
                    var result = ExecuteHttpAction(BuildSystem().Put(input));

                    //
                    // Assert
                    Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
                }

                [Fact]
                public void EmptyGuidIdentifierGeneratesBadRequest()
                {

                    //
                    // Arrange
                    WaterbirdForagingModel input = CreateDefautInput();
                    input.SurveyIdentifer = Guid.Empty;

                    //
                    // Act
                    var result = ExecuteHttpAction(BuildSystem().Put(input));

                    //
                    // Assert
                    Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
                }

                [Fact]
                public void ConfirmHandlingOfInvalidOperation()
                {
                    ExpectToLogToError();

                    var e = new InvalidOperationException();
                    Assert.Equal(HttpStatusCode.InternalServerError, RunTest(e).StatusCode);
                }

                [Fact]
                public void ConfirmHandlingOfServerError()
                {
                    ExpectToLogToError();

                    var e = ServerException.HandleException<ExceptionHandling>(new Exception(), "asdf");
                    Assert.Equal(HttpStatusCode.InternalServerError, RunTest(e).StatusCode);
                }

                [Fact]
                public void ConfirmHandlingOfUserError()
                {
                    ExpectToLogToDebug();

                    var e = new UserException("asdf");
                    Assert.Equal(HttpStatusCode.BadRequest, RunTest(e).StatusCode);
                }
            }
        }

    }
}
