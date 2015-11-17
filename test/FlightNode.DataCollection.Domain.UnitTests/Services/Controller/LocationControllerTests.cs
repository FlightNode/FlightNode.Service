using FlightNode.DataCollection.Domain.Entities;
using FlightNode.DataCollection.Domain.Managers;
using FlightNode.DataCollection.Domain.Services.Controllers;
using FlightNode.DataCollection.Domain.Services.Models;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Xunit;

namespace FlightNode.DataCollection.Domain.UnitTests.Services
{
    public class LocationControllerTests
    {
        public class Fixture : IDisposable
        {
            protected MockRepository MockRepository = new MockRepository(MockBehavior.Strict);
            protected Mock<ILocationDomainManager> MockDomainManager;

            public Fixture()
            {
                MockDomainManager = MockRepository.Create<ILocationDomainManager>();
            }

            protected LocationController Buildsystem()
            {
                var controller = new LocationController(MockDomainManager.Object);

                controller.Request = new HttpRequestMessage();
                controller.Configuration = new HttpConfiguration();

                return controller;
            }

            public void Dispose()
            {
                MockRepository.VerifyAll();
            }

        }


        public class ArgumentTests : Fixture
        {
            [Fact]
            public void ConfirmConstructorHappyPath()
            {
                Buildsystem();
            }

            [Fact]
            public void ConfirmConstructorRejectsNullArgument()
            {
                Assert.Throws<ArgumentNullException>(() => new LocationController(null));
            }
        }

        public class Get : Fixture
        {
            private int id = 123;
            private string description = "somewhere";
            private decimal longitude = 89.3m;
            private decimal latitude = -34.0m;

            [Fact]
            public void ConfirmGetMapsDescription()
            {
                Assert.Equal(description, RunPositiveTest().First().Description);
            }


            [Fact]
            public void ConfirmGetMapsId()
            {
                Assert.Equal(id, RunPositiveTest().First().Id);
            }

            [Fact]
            public void ConfirmGetMapsLongitude()
            {
                Assert.Equal(longitude, RunPositiveTest().First().Longitude);
            }

            [Fact]
            public void ConfirmMapsLatitude()
            {
                Assert.Equal(latitude, RunPositiveTest().First().Latitude);
            }


            private List<LocationModel> RunPositiveTest()
            {
                // Arrange 
                var records = new List<Location>
                {
                    new Location
                    {
                        Description = description,
                        Latitude = latitude,
                        Longitude= longitude,
                        LocationId = id,
                        WorkLogs = null
                    }
                };

                MockDomainManager.Setup(x => x.FindAll())
                    .Returns(records);

                var expected = new List<LocationModel> {
                    new LocationModel
                    {
                        Description = description,
                        Id = id,
                        Latitude = latitude,
                        Longitude  = longitude
                    }
                };

                // Act
                var result = Buildsystem().Get();

                var message = result.ExecuteAsync(new System.Threading.CancellationToken()).Result;

                Assert.Equal(HttpStatusCode.OK, message.StatusCode);

                return message.Content.ReadAsAsync<List<LocationModel>>().Result;
            }
        }

    }
}
