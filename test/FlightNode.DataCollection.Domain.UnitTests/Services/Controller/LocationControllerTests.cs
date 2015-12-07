using FlightNode.Common.Exceptions;
using FlightNode.DataCollection.Domain.Entities;
using FlightNode.DataCollection.Domain.Managers;
using FlightNode.DataCollection.Domain.Services.Controllers;
using FlightNode.DataCollection.Domain.Services.Models;
using Moq;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using System.Web.Http.Results;
using Xunit;

namespace FlightNode.DataCollection.Domain.UnitTests.Services
{
    public class LocationControllerTests
    {
        public class Fixture : IDisposable
        {
            protected MockRepository MockRepository = new MockRepository(MockBehavior.Strict);
            protected Mock<ILocationDomainManager> MockDomainManager;
            protected Mock<ILogger> MockLogger;
            protected const string url = "http://some/where/";

            public Fixture()
            {
                MockDomainManager = MockRepository.Create<ILocationDomainManager>();
                MockLogger = MockRepository.Create<ILogger>();
            }

            protected LocationsController BuildSystem()
            {
                var controller = new LocationsController(MockDomainManager.Object);

                controller.Logger = MockLogger.Object;

                controller.Request = new HttpRequestMessage();
                controller.Request.RequestUri = new Uri(url);

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
                BuildSystem();
            }

            [Fact]
            public void ConfirmConstructorRejectsNullArgument()
            {
                Assert.Throws<ArgumentNullException>(() => new LocationsController(null));
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
                Assert.Equal(description, RunPositiveTest().Description);
            }


            [Fact]
            public void ConfirmGetMapsId()
            {
                Assert.Equal(id, RunPositiveTest().Id);
            }

            [Fact]
            public void ConfirmGetMapsLongitude()
            {
                Assert.Equal(longitude, RunPositiveTest().Longitude);
            }

            [Fact]
            public void ConfirmMapsLatitude()
            {
                Assert.Equal(latitude, RunPositiveTest().Latitude);
            }


            private LocationModel RunPositiveTest()
            {
                // Arrange 
                var record = new Location
                {
                    Description = description,
                    Latitude = latitude,
                    Longitude = longitude,
                    Id = id,
                    WorkLogs = null
                };

                MockDomainManager.Setup(x => x.FindById(It.Is<int>(y => y == id)))
                    .Returns(record);


                // Act
                var result = BuildSystem().Get(id);

                var message = result.ExecuteAsync(new System.Threading.CancellationToken()).Result;

                Assert.Equal(HttpStatusCode.OK, message.StatusCode);

                return message.Content.ReadAsAsync<LocationModel>().Result;
            }

            public class ExceptionHandling : Fixture
            {

                private HttpResponseMessage RunTest(Exception ex)
                {
                    MockDomainManager.Setup(x => x.FindAll())
                        .Throws(ex);


                    return BuildSystem().Get().ExecuteAsync(new System.Threading.CancellationToken()).Result;
                }

                [Fact]
                public void ConfirmHandlingOfInvalidOperation()
                {
                    MockLogger.Setup(x => x.Error(It.IsAny<Exception>()));

                    var e = new InvalidOperationException();
                    Assert.Equal(HttpStatusCode.InternalServerError, RunTest(e).StatusCode);
                }

                [Fact]
                public void ConfirmHandlingOfServerError()
                {
                    MockLogger.Setup(x => x.Error(It.IsAny<Exception>()));

                    var e = ServerException.HandleException<ExceptionHandling>(new Exception(), "asdf");
                    Assert.Equal(HttpStatusCode.InternalServerError, RunTest(e).StatusCode);
                }

                [Fact]
                public void ConfirmHandlingOfUserError()
                {
                    var e = new UserException("asdf");
                    Assert.Equal(HttpStatusCode.BadRequest, RunTest(e).StatusCode);
                }
            }
        }


        public class Get_All : Fixture
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
                        Id = id,
                        WorkLogs = null
                    }
                };

                MockDomainManager.Setup(x => x.FindAll())
                    .Returns(records);

                // Act
                var result = BuildSystem().Get();

                var message = result.ExecuteAsync(new System.Threading.CancellationToken()).Result;

                Assert.Equal(HttpStatusCode.OK, message.StatusCode);

                return message.Content.ReadAsAsync<List<LocationModel>>().Result;
            }

            public class ExceptionHandling : Fixture
            {

                private HttpResponseMessage RunTest(Exception ex)
                {
                    MockDomainManager.Setup(x => x.FindAll())
                        .Throws(ex);

                    return BuildSystem().Get().ExecuteAsync(new System.Threading.CancellationToken()).Result;
                }

                [Fact]
                public void ConfirmHandlingOfInvalidOperation()
                {
                    MockLogger.Setup(x => x.Error(It.IsAny<Exception>()));

                    var e = new InvalidOperationException();
                    Assert.Equal(HttpStatusCode.InternalServerError, RunTest(e).StatusCode);
                }

                [Fact]
                public void ConfirmHandlingOfServerError()
                {
                    MockLogger.Setup(x => x.Error(It.IsAny<Exception>()));

                    var e = ServerException.HandleException<ExceptionHandling>(new Exception(), "asdf");
                    Assert.Equal(HttpStatusCode.InternalServerError, RunTest(e).StatusCode);
                }

                [Fact]
                public void ConfirmHandlingOfUserError()
                {
                    var e = new UserException("asdf");
                    Assert.Equal(HttpStatusCode.BadRequest, RunTest(e).StatusCode);
                }
            }
        }


        public class Post : Fixture
        {
            private int id = 123;
            private string description = "somewhere";
            private decimal longitude = 89.3m;
            private decimal latitude = -34.0m;

            [Fact]
            public void ConfirmMapsDescription()
            {
                RunPositiveTest();
                MockDomainManager.Verify(x => x.Create(It.Is<Location>(y => y.Description == description)));
            }

            [Fact]
            public void ConfirmMapsLongitude()
            {
                RunPositiveTest();
                MockDomainManager.Verify(x => x.Create(It.Is<Location>(y => y.Longitude == longitude)));
            }

            [Fact]
            public void ConfirmMapsLatitude()
            {
                RunPositiveTest();
                MockDomainManager.Verify(x => x.Create(It.Is<Location>(y => y.Latitude == latitude)));
            }

            [Fact]
            public void ConfirmUpdatesLocationHeader()
            {
                var expected = "http://some/where/" + id.ToString();

                Assert.Equal(expected, RunPositiveTest().Headers.Location.ToString());
            }

            [Fact]
            public void ConfirmReturnsCreatedStatus()
            {
                Assert.Equal(HttpStatusCode.Created, RunPositiveTest().StatusCode);
            }

            private LocationModel GetTestResult()
            {
                return RunPositiveTest().Content.ReadAsAsync<LocationModel>().Result;
            }

            private HttpResponseMessage RunPositiveTest()
            {
                // Arrange 
                var record = new LocationModel
                {
                    Description = description,
                    Latitude = latitude,
                    Longitude = longitude
                };

                MockDomainManager.Setup(x => x.Create(It.IsAny<Location>()))
                .Returns((Location actual) =>
                {
                    // inject an ID value so we can confirm that it is passed in the response
                    actual.Id = id;
                    return actual;
                });


                // Act
                var result = BuildSystem().Post(record);

                var message = result.ExecuteAsync(new System.Threading.CancellationToken()).Result;

                return message;

            }

            public class ExceptionHandling : Fixture
            {

                protected HttpResponseMessage RunTest(Exception ex)
                {
                    MockDomainManager.Setup(x => x.Create(It.IsAny<Location>()))
                        .Throws(ex);

                    return BuildSystem().Post(new LocationModel()).ExecuteAsync(new System.Threading.CancellationToken()).Result;
                }

                [Fact]
                public void ConfirmDoesNotAcceptNull()
                {
                    Assert.Throws<ArgumentNullException>(() => BuildSystem().Post(null));
                }

                [Fact]
                public void ConfirmHandlingOfInvalidOperation()
                {
                    MockLogger.Setup(x => x.Error(It.IsAny<Exception>()));

                    var e = new InvalidOperationException();
                    Assert.Equal(HttpStatusCode.InternalServerError, RunTest(e).StatusCode);
                }

                [Fact]
                public void ConfirmHandlingOfServerError()
                {
                    MockLogger.Setup(x => x.Error(It.IsAny<Exception>()));

                    var e = ServerException.HandleException<ExceptionHandling>(new Exception(), "asdf");
                    Assert.Equal(HttpStatusCode.InternalServerError, RunTest(e).StatusCode);
                }

                [Fact]
                public void ConfirmHandlingOfUserError()
                {
                    var e = new UserException("asdf");
                    Assert.Equal(HttpStatusCode.BadRequest, RunTest(e).StatusCode);
                }
            }

            public class ValidationExceptionHandling : Fixture
            {

                private const string message1 = "asdfasdfa";
                private const string field1 = "fieeeeeld";
                private const string message2 = "as8df7a89psdfp";
                private const string field2 = "sdk;kl;hl;";
                

                [Fact]
                public void ConfirmSendsField1InModelState()
                {
                    Assert.True(
                        RunTest().ModelState
                            .FirstOrDefault(x => x.Key == field1)
                            .Value
                            .Errors
                            .Any(x => x.ErrorMessage == message1)
                    );
                }

                [Fact]
                public void ConfirmSendsField2InModelState()
                {
                    Assert.True(
                        RunTest().ModelState
                            .FirstOrDefault(x => x.Key == field2)
                            .Value
                            .Errors
                            .Any(x => x.ErrorMessage == message2)
                    );
                }

                private InvalidModelStateResult RunTest()
                {
                    var list = new List<ValidationResult>
                    {
                        new ValidationResult(message1, new [] { field1 }),
                        new ValidationResult(message2, new [] { field2 })
                    };

                    var e = DomainValidationException.Create(list);

                    MockDomainManager.Setup(x => x.Create(It.IsAny<Location>()))
                                           .Throws(e);

                    return BuildSystem().Post(new LocationModel()) as InvalidModelStateResult;
                }
            }
        }


        public class Put : Fixture
        {
            private int id = 123;
            private string description = "somewhere";
            private decimal longitude = 89.3m;
            private decimal latitude = -34.0m;

            [Fact]
            public void ConfirmMapsDescription()
            {
                RunPositiveTest();
                MockDomainManager.Verify(x => x.Update(It.Is<Location>(y => y.Description == description)));
            }

            [Fact]
            public void ConfirmMapsLongitude()
            {
                RunPositiveTest();
                MockDomainManager.Verify(x => x.Update(It.Is<Location>(y => y.Longitude == longitude)));
            }

            [Fact]
            public void ConfirmMapsLatitude()
            {
                RunPositiveTest();
                MockDomainManager.Verify(x => x.Update(It.Is<Location>(y => y.Latitude == latitude)));
            }

            [Fact]
            public void ConfirmMapsLocationId()
            {
                RunPositiveTest();
                MockDomainManager.Verify(x => x.Update(It.Is<Location>(y => y.Id == id)));
            }

            [Fact]
            public void ConfirmReturnsNoContenttatus()
            {
                Assert.Equal(HttpStatusCode.NoContent, RunPositiveTest().StatusCode);
            }

            private LocationModel GetTestResult()
            {
                return RunPositiveTest().Content.ReadAsAsync<LocationModel>().Result;
            }

            private HttpResponseMessage RunPositiveTest()
            {
                // Arrange 
                var record = new LocationModel
                {
                    Description = description,
                    Latitude = latitude,
                    Longitude = longitude,
                    Id = id
                };

                MockDomainManager.Setup(x => x.Update(It.IsAny<Location>()));


                // Act
                var result = BuildSystem().Put(record);

                var message = result.ExecuteAsync(new System.Threading.CancellationToken()).Result;

                return message;

            }

            public class ExceptionHandling : Fixture
            {

                private HttpResponseMessage RunTest(Exception ex)
                {
                    MockDomainManager.Setup(x => x.Update(It.IsAny<Location>()))
                        .Throws(ex);

                    return BuildSystem().Put(new LocationModel()).ExecuteAsync(new System.Threading.CancellationToken()).Result;
                }

                [Fact]
                public void ConfirmDoesNotAcceptNull()
                {
                    Assert.Throws<ArgumentNullException>(() => BuildSystem().Put(null));
                }

                [Fact]
                public void ConfirmHandlingOfInvalidOperation()
                {
                    MockLogger.Setup(x => x.Error(It.IsAny<Exception>()));

                    var e = new InvalidOperationException();
                    Assert.Equal(HttpStatusCode.InternalServerError, RunTest(e).StatusCode);
                }

                [Fact]
                public void ConfirmHandlingOfServerError()
                {
                    MockLogger.Setup(x => x.Error(It.IsAny<Exception>()));

                    var e = ServerException.HandleException<ExceptionHandling>(new Exception(), "asdf");
                    Assert.Equal(HttpStatusCode.InternalServerError, RunTest(e).StatusCode);
                }

                [Fact]
                public void ConfirmHandlingOfUserError()
                {
                    var e = new UserException("asdf");
                    Assert.Equal(HttpStatusCode.BadRequest, RunTest(e).StatusCode);
                }
            }



            public class ValidationExceptionHandling : Fixture
            {

                private const string message1 = "asdfasdfa";
                private const string field1 = "fieeeeeld";
                private const string message2 = "as8df7a89psdfp";
                private const string field2 = "sdk;kl;hl;";


                [Fact]
                public void ConfirmSendsField1InModelState()
                {
                    Assert.True(
                        RunTest().ModelState
                            .FirstOrDefault(x => x.Key == field1)
                            .Value
                            .Errors
                            .Any(x => x.ErrorMessage == message1)
                    );
                }

                [Fact]
                public void ConfirmSendsField2InModelState()
                {
                    Assert.True(
                        RunTest().ModelState
                            .FirstOrDefault(x => x.Key == field2)
                            .Value
                            .Errors
                            .Any(x => x.ErrorMessage == message2)
                    );
                }

                private InvalidModelStateResult RunTest()
                {
                    var list = new List<ValidationResult>
                    {
                        new ValidationResult(message1, new [] { field1 }),
                        new ValidationResult(message2, new [] { field2 })
                    };

                    var e = DomainValidationException.Create(list);

                    MockDomainManager.Setup(x => x.Update(It.IsAny<Location>()))
                                           .Throws(e);

                    return BuildSystem().Put(new LocationModel()) as InvalidModelStateResult;
                }
            }
        }


        public class GetSimpleList : Fixture
        {
            private const int id1 = 123;
            private const int id2 = 52334;
            private const string description1 = "somewhere";
            private const string description2 = "else";

            [Fact]
            public void ConfirmReturnsOKStatus()
            {
                Assert.Equal(HttpStatusCode.OK, RunPositiveTest().StatusCode);
            }

            [Fact]
            public void ConfirmReturnsFirstItem()
            {
                Assert.Equal(description1, GetTestResults().FirstOrDefault(x => x.Id == id1).Value);
            }

            [Fact]
            public void ConfirmReturnsSecondItem()
            {
                Assert.Equal(description2, GetTestResults().FirstOrDefault(x => x.Id == id2).Value);
            }

            private List<SimpleListItem> GetTestResults()
            {

                return RunPositiveTest().Content.ReadAsAsync<List<SimpleListItem>>().Result;
            }

            private HttpResponseMessage RunPositiveTest()
            {
                // Arrange 
                var records = new List<Location>
                {
                    new Location
                    {
                        Description = description1,
                        Id = id1
                    },
                    new Location
                    {
                        Description = description2,
                        Id = id2
                    }
                };

                MockDomainManager.Setup(x => x.FindAll())
                    .Returns(records);

                // Act
                var result = BuildSystem().GetSimpleList();

                var message = result.ExecuteAsync(new System.Threading.CancellationToken()).Result;

                return message;
            }

            public class ExceptionHandling : Fixture
            {

                private HttpResponseMessage RunTest(Exception ex)
                {
                    MockDomainManager.Setup(x => x.FindAll())
                        .Throws(ex);

                    return BuildSystem().GetSimpleList().ExecuteAsync(new System.Threading.CancellationToken()).Result;
                }

                [Fact]
                public void ConfirmHandlingOfInvalidOperation()
                {
                    MockLogger.Setup(x => x.Error(It.IsAny<Exception>()));

                    var e = new InvalidOperationException();
                    Assert.Equal(HttpStatusCode.InternalServerError, RunTest(e).StatusCode);
                }

                [Fact]
                public void ConfirmHandlingOfServerError()
                {
                    MockLogger.Setup(x => x.Error(It.IsAny<Exception>()));

                    var e = ServerException.HandleException<ExceptionHandling>(new Exception(), "asdf");
                    Assert.Equal(HttpStatusCode.InternalServerError, RunTest(e).StatusCode);
                }
            }
        }
    }
}
