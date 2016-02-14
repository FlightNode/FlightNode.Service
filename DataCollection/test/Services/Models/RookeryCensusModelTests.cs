using FlightNode.DataCollection.Services.Models.Rookery;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FlightNode.DataCollection.Domain.UnitTests.Services.Models
{
    public class RookeryCensusModelTests
    {

        public class Constructor : RookeryCensusModelTests
        {
            [Fact]
            public void InitializesSurveyIdentifier()
            {
                Assert.Equal(Guid.Empty, new RookeryCensusModel().SurveyIdentifer);
            }

            [Fact]
            public void InitializesObservers()
            {
                Assert.Equal(0, new RookeryCensusModel().Observers.Count());
            }

            [Fact]
            public void InitializesDisturbances()
            {
                Assert.Equal(0, new RookeryCensusModel().Disturbances.Count());
            }

            [Fact]
            public void InitializesObservations()
            {
                Assert.Equal(0, new RookeryCensusModel().Observations.Count());
            }

            [Fact]
            public void Json()
            {
                var system = new RookeryCensusModel
                {
                    AccessPointInfoId = 1,
                    DisturbanceComments = "disturbance comments",
                    Disturbances = new List<DisturbanceModel>
                    {
                        new DisturbanceModel
                        {
                            Behavior = "disturbed behavior 1",
                            DisturbanceId = 0,
                            DisturbanceTypeId = 3,
                            DurationMinutes = 40,
                            Quantity = 34
                        },
                        new DisturbanceModel
                        {
                            Behavior = "disturbed behavior 2",
                            DisturbanceId = 0,
                            DisturbanceTypeId = 4,
                            DurationMinutes = 5,
                            Quantity = 6
                        }
                    },
                    EndDate = new DateTime(2016, 2, 9, 15, 12, 0),
                    LocationId = 7,
                    Observations = new List<ObservationModel>
                    {
                        new ObservationModel
                        {
                            Adults = 8,
                            BirdSpeciesId =9 ,
                            FeedingId = 10,
                            HabitatId = 11,
                            Juveniles = 12,
                            PrimaryActivityId = 13,
                            SecondaryActivityId = 14
                        },
                        new ObservationModel
                        {
                            Adults = 15,
                            BirdSpeciesId =16 ,
                            FeedingId = 17,
                            HabitatId = 18,
                            Juveniles = 19,
                            PrimaryActivityId = 20,
                            SecondaryActivityId = 21
                        }
                    },
                    SiteTypeId = 22,
                    StartDate = new DateTime(2016, 2,9 ,12,0,0),
                    Step = 2,
                    SurveyComments = "Survey comments",
                    SurveyIdentifer = Guid.NewGuid(),
                    TideInfoId = 23,
                    VantagePointInfoId = 24,
                    WeatherInfoId = 25
                };
                system.Observers.Add(987);
                system.Observers.Add(999);

                var settings = new JsonSerializerSettings();
                settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                var json = JsonConvert.SerializeObject(system, Formatting.Indented, settings);
            }
        }
    }

}
