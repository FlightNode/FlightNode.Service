using FlightNode.Common.Api.Models;
using FlightNode.Common.Exceptions;
using FlightNode.DataCollection.Domain.Entities;
using FlightNode.DataCollection.Domain.Managers;
using FlightNode.DataCollection.Domain.Services.Controllers;
using FlightNode.DataCollection.Services.Models;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Results;
using Xunit;

namespace FlightNode.DataCollection.Domain.UnitTests.Services.Controller
{
    public class LocationsControllerTests
    {
        public class Fixture : LoggingControllerBaseFixture<LocationsController, ILocationDomainManager>
        {
            protected const int ID = 123;
            protected const decimal LONGITUDE = 89.3m;
            protected const decimal LATITUDE = -34.0m;
            protected const string SITE_CODE = "code1";
            protected const string SITE_NAME = "name1";
            protected const string CITY = "city";
            protected const string COUNTY = "county";
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


            [Fact]
            public void ConfirmGetMapsCity()
            {
                Assert.Equal(CITY, RunPositiveTest().City);
            }

            [Fact]
            public void ConfirmGetMapsCounty()
            {
                Assert.Equal(COUNTY, RunPositiveTest().County);
            }

            [Fact]
            public void ConfirmGetMapsSiteCode()
            {
                Assert.Equal(SITE_CODE, RunPositiveTest().SiteCode);
            }

            [Fact]
            public void ConfirmGetMapsSiteName()
            {
                Assert.Equal(SITE_NAME, RunPositiveTest().SiteName);
            }

            [Fact]
            public void ConfirmGetMapsId()
            {
                Assert.Equal(ID, RunPositiveTest().Id);
            }

            [Fact]
            public void ConfirmGetMapsLongitude()
            {
                Assert.Equal(LONGITUDE, RunPositiveTest().Longitude);
            }

            [Fact]
            public void ConfirmMapsLatitude()
            {
                Assert.Equal(LATITUDE, RunPositiveTest().Latitude);
            }


            private LocationModel RunPositiveTest()
            {
                // Arrange 
                var record = new Location
                {
                    SiteCode = SITE_CODE,
                    SiteName = SITE_NAME,
                    Latitude = LATITUDE,
                    Longitude = LONGITUDE,
                    Id = ID,
                    WorkLogs = null,
                    City = CITY,
                    County = COUNTY
                };

                MockDomainManager.Setup(x => x.FindById(It.Is<int>(y => y == ID)))
                    .Returns(record);


                // Act
                var result = BuildSystem().Get(ID);

                HttpResponseMessage message = ExecuteHttpAction(result);

                Assert.Equal(HttpStatusCode.OK, message.StatusCode);

                return ReadResult<LocationModel>(message);
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


        public class Get_All : Fixture
        {

            [Fact]
            public void ConfirmGetMapsCity()
            {
                Assert.Equal(CITY, RunPositiveTest().First().City);
            }

            [Fact]
            public void ConfirmGetMapsCounty()
            {
                Assert.Equal(COUNTY, RunPositiveTest().First().County);
            }

            [Fact]
            public void ConfirmGetMapsSiteCode()
            {
                Assert.Equal(SITE_CODE, RunPositiveTest().First().SiteCode);
            }

            [Fact]
            public void ConfirmGetMapsSiteName()
            {
                Assert.Equal(SITE_NAME, RunPositiveTest().First().SiteName);
            }

            [Fact]
            public void ConfirmGetMapsId()
            {
                Assert.Equal(ID, RunPositiveTest().First().Id);
            }

            [Fact]
            public void ConfirmGetMapsLongitude()
            {
                Assert.Equal(LONGITUDE, RunPositiveTest().First().Longitude);
            }

            [Fact]
            public void ConfirmMapsLatitude()
            {
                Assert.Equal(LATITUDE, RunPositiveTest().First().Latitude);
            }


            private List<LocationModel> RunPositiveTest()
            {
                // Arrange 
                var records = new List<Location>
                {
                    new Location
                    {
                        SiteCode=SITE_CODE,
                        SiteName=SITE_NAME,
                        Latitude = LATITUDE,
                        Longitude= LONGITUDE,
                        Id = ID,
                        WorkLogs = null,
                        City = CITY,
                        County = COUNTY
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


        public class Post : Fixture
        {
            [Fact]
            public void ConfirmMapsCity()
            {
                RunPositiveTest();
                MockDomainManager.Verify(x => x.Create(It.Is<Location>(y => y.City  == CITY)));
            }

            [Fact]
            public void ConfirmMapCounty()
            {
                RunPositiveTest();
                MockDomainManager.Verify(x => x.Create(It.Is<Location>(y => y.County == COUNTY)));
            }


            [Fact]
            public void ConfirmMapsSiteCode()
            {
                RunPositiveTest();
                MockDomainManager.Verify(x => x.Create(It.Is<Location>(y => y.SiteCode == SITE_CODE)));
            }

            [Fact]
            public void ConfirmMapSiteName()
            {
                RunPositiveTest();
                MockDomainManager.Verify(x => x.Create(It.Is<Location>(y => y.SiteName == SITE_NAME)));
            }

            [Fact]
            public void ConfirmMapsLongitude()
            {
                RunPositiveTest();
                MockDomainManager.Verify(x => x.Create(It.Is<Location>(y => y.Longitude == LONGITUDE)));
            }

            [Fact]
            public void ConfirmMapsLatitude()
            {
                RunPositiveTest();
                MockDomainManager.Verify(x => x.Create(It.Is<Location>(y => y.Latitude == LATITUDE)));
            }

            [Fact]
            public void ConfirmUpdatesLocationHeader()
            {
                var expected = "http://some/where/" + ID.ToString();

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
                    SiteCode = SITE_CODE,
                    SiteName = SITE_NAME,
                    Latitude = LATITUDE,
                    Longitude = LONGITUDE,
                    City = CITY,
                    County = COUNTY
                };

                MockDomainManager.Setup(x => x.Create(It.IsAny<Location>()))
                .Returns((Location actual) =>
                {
                    // inject an ID value so we can confirm that it is passed in the response
                    actual.Id = ID;
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

                    ExpectToLogToDebug();

                    return BuildSystem().Post(new LocationModel()) as InvalidModelStateResult;
                }
            }
        }


        public class Put : Fixture
        {

            [Fact]
            public void ConfirmMapsCity()
            {
                RunPositiveTest();
                MockDomainManager.Verify(x => x.Update(It.Is<Location>(y => y.City == CITY)));
            }

            [Fact]
            public void ConfirmMapCounty()
            {
                RunPositiveTest();
                MockDomainManager.Verify(x => x.Update(It.Is<Location>(y => y.County == COUNTY)));
            }

            [Fact]
            public void ConfirmMapsSiteCode()
            {
                RunPositiveTest();
                MockDomainManager.Verify(x => x.Update(It.Is<Location>(y => y.SiteCode == SITE_CODE)));
            }

            [Fact]
            public void ConfirmMapsSiteName()
            {
                RunPositiveTest();
                MockDomainManager.Verify(x => x.Update(It.Is<Location>(y => y.SiteName == SITE_NAME)));
            }

            [Fact]
            public void ConfirmMapsLongitude()
            {
                RunPositiveTest();
                MockDomainManager.Verify(x => x.Update(It.Is<Location>(y => y.Longitude == LONGITUDE)));
            }

            [Fact]
            public void ConfirmMapsLatitude()
            {
                RunPositiveTest();
                MockDomainManager.Verify(x => x.Update(It.Is<Location>(y => y.Latitude == LATITUDE)));
            }

            [Fact]
            public void ConfirmMapsLocationId()
            {
                RunPositiveTest();
                MockDomainManager.Verify(x => x.Update(It.Is<Location>(y => y.Id == ID)));
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
                    SiteCode = SITE_CODE,
                    SiteName = SITE_NAME,
                    Latitude = LATITUDE,
                    Longitude = LONGITUDE,
                    Id = ID,
                    City = CITY,
                    County = COUNTY
                };

                MockDomainManager.Setup(x => x.Update(It.IsAny<Location>()))
                    .Returns(1);


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

                    ExpectToLogToDebug();

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
                        SiteName = description1,
                        Id = id1
                    },
                    new Location
                    {
                        SiteName = description2,
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
            }
        }
    }
}
