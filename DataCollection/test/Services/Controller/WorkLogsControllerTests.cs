using FlightNode.Common.Exceptions;
using FlightNode.Common.Utility;
using FlightNode.DataCollection.Domain.Entities;
using FlightNode.DataCollection.Domain.Managers;
using FlightNode.DataCollection.Domain.Services.Controllers;
using FlightNode.DataCollection.Services.Models;
using FlightNode.DataCollection.Services.Models.WorkLog;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Web.Http.Results;
using Xunit;

namespace FlightNode.DataCollection.Domain.UnitTests.Services.Controller
{
    public class WorkLogControllerTests
    {
        public class Fixture : LoggingControllerBaseFixture<WorkLogsController, IWorkLogDomainManager>
        {
        }

        public class ArgumentTests : Fixture
        {
            [Fact]
            public void ConfirmConstructorHappyPath()
            {
                BuildSystemWithSanitizer();
            }

            [Fact]
            public void ConfirmConstructorRejectsNullFirstArgument()
            {
                Assert.Throws<ArgumentNullException>(() => new WorkLogsController(null, new Sanitizer()));
            }

            [Fact]
            public void ConfirmConstructorRejectsNullSecondArgument()
            {
                Assert.Throws<ArgumentNullException>(() => new WorkLogsController(Mock.Of<IWorkLogDomainManager>(), null));
            }
        }

        public class Get : Fixture
        {
            private const int id = 123;
            private const string description = "somewhere";
            private const decimal travelHours = 3.53m;
            private const decimal workHours = 23.32m;
            private readonly DateTime workDay = DateTime.Parse("2015-12-07 13:45");
            private const int userId = 223;
            private const int workTypeId = 3;
            private const int locationId = 4;

            [Fact]
            public void ConfirmGetMapsWorkTypeId()
            {
                Assert.Equal(workTypeId, RunPositiveTest().WorkTypeId);
            }


            [Fact]
            public void ConfirmGetMapsWorkHours()
            {
                Assert.Equal(workHours, RunPositiveTest().WorkHours);
            }

            [Fact]
            public void ConfirmGetMapsWorkDate()
            {
                Assert.Equal(workDay, RunPositiveTest().WorkDate);
            }

            [Fact]
            public void ConfirmGetMapsUserId()
            {
                Assert.Equal(userId, RunPositiveTest().UserId);
            }

            [Fact]
            public void ConfirmGetMapsTravelTimeHours()
            {
                Assert.Equal(travelHours, RunPositiveTest().TravelTimeHours);
            }

            [Fact]
            public void ConfirmGetMapsLocationId()
            {
                Assert.Equal(locationId, RunPositiveTest().LocationId);
            }

            [Fact]
            public void ConfirmGetMapsId()
            {
                Assert.Equal(id, RunPositiveTest().Id);
            }

            private WorkLogModel RunPositiveTest()
            {
                // Arrange 
                var record = new WorkLog
                {
                    LocationId = locationId,
                    TravelTimeHours = travelHours,
                    WorkTypeId = workTypeId,
                    WorkHours = workHours,
                    UserId = userId,
                    WorkDate = workDay,
                    Id = id,
                };

                MockDomainManager.Setup(x => x.FindById(It.Is<int>(y => y == id)))
                    .Returns(record);


                // Act
                var result = BuildSystemWithSanitizer().Get(id);

                var message = result.ExecuteAsync(new System.Threading.CancellationToken()).Result;

                Assert.Equal(HttpStatusCode.OK, message.StatusCode);

                return message.Content.ReadAsAsync<WorkLogModel>().Result;
            }

            public class ExceptionHandling : Fixture
            {

                private HttpResponseMessage RunTest(Exception ex)
                {
                    MockDomainManager.Setup(x => x.FindAll())
                        .Throws(ex);


                    return BuildSystemWithSanitizer().Get().ExecuteAsync(new System.Threading.CancellationToken()).Result;
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
            private const int id = 123;
            private const string description = "somewhere";
            private const decimal travelHours = 3.53m;
            private const decimal workHours = 23.32m;
            private readonly DateTime workDay = DateTime.Parse("2015-12-07 13:45");
            private const int userId = 223;
            private const int workTypeId = 3;
            private const int locationId = 4;

            [Fact]
            public void ConfirmGetMapsWorkTypeId()
            {
                Assert.Equal(workTypeId, RunPositiveTest().First().WorkTypeId);
            }


            [Fact]
            public void ConfirmGetMapsWorkHours()
            {
                Assert.Equal(workHours, RunPositiveTest().First().WorkHours);
            }

            [Fact]
            public void ConfirmGetMapsWorkDate()
            {
                Assert.Equal(workDay, RunPositiveTest().First().WorkDate);
            }

            [Fact]
            public void ConfirmGetMapsUserId()
            {
                Assert.Equal(userId, RunPositiveTest().First().UserId);
            }

            [Fact]
            public void ConfirmGetMapsTravelTimeHours()
            {
                Assert.Equal(travelHours, RunPositiveTest().First().TravelTimeHours);
            }

            [Fact]
            public void ConfirmGetMapsLocationId()
            {
                Assert.Equal(locationId, RunPositiveTest().First().LocationId);
            }

            [Fact]
            public void ConfirmGetMapsId()
            {
                Assert.Equal(id, RunPositiveTest().First().Id);
            }

            private List<WorkLogModel> RunPositiveTest()
            {
                // Arrange 
                var records = new List<WorkLog>
                {
                    new WorkLog
                    {
                        LocationId = locationId,
                        TravelTimeHours = travelHours,
                        WorkTypeId = workTypeId,
                        WorkHours = workHours,
                        UserId = userId,
                        WorkDate = workDay,
                        Id = id
                    }
                };

                MockDomainManager.Setup(x => x.FindAll())
                    .Returns(records);

                // Act
                var result = BuildSystemWithSanitizer().Get();

                var message = result.ExecuteAsync(new System.Threading.CancellationToken()).Result;

                Assert.Equal(HttpStatusCode.OK, message.StatusCode);

                return message.Content.ReadAsAsync<List<WorkLogModel>>().Result;
            }

            public class ExceptionHandling : Fixture
            {

                private HttpResponseMessage RunTest(Exception ex)
                {
                    MockDomainManager.Setup(x => x.FindAll())
                        .Throws(ex);

                    return BuildSystemWithSanitizer().Get().ExecuteAsync(new System.Threading.CancellationToken()).Result;
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



        public class GetMyLogs : Fixture
        {
            // Mapping of data types is tested elsewhere.
            // Here, let's just confirm that all records are mapped

            private const int id = 123;
            private const int id2 = 34234;
            private const int userId = 223;            

            [Fact]
            public void ConfirmMapsFirstRecord()
            {
                Assert.Equal(id, RunPositiveTest().First().Id);
            }

            [Fact]
            public void ConfirmMapsSecondRecord()
            {
                Assert.Equal(id2, RunPositiveTest().Skip(1).First().Id);
            }

            protected WorkLogsController SetCurrentUserId(WorkLogsController system)
            {
                var identity = MockRepository.Create<IIdentity>();

                var principal = MockRepository.Create<IPrincipal>();
                principal.SetupGet(x => x.Identity)
                    .Returns(identity.Object);

                system.User = principal.Object;

                return system;
            }

            private List<WorkLogReportRecord> RunPositiveTest()
            {
                // Arrange 
                var records = new List<WorkLogReportRecord>
                {
                    new WorkLogReportRecord
                    {
                        Id = id,
                        WorkDate = "01/10/2016"
                    },

                    new WorkLogReportRecord
                    {
                        Id = id2,
                        WorkDate = "01/10/2016"
                    }
                };

                MockDomainManager.Setup(x => x.GetForUser(It.Is<int>(y => y == 0)))
                    .Returns(records);

                // Act
                var result = SetCurrentUserId(BuildSystemWithSanitizer())
                    .GetMyLogs();

                var message = result.ExecuteAsync(new System.Threading.CancellationToken()).Result;

                Assert.Equal(HttpStatusCode.OK, message.StatusCode);

                return message.Content.ReadAsAsync<List<WorkLogReportRecord>>().Result;
            }

            public class ExceptionHandling : GetMyLogs
            {

                private HttpResponseMessage RunTest(Exception ex)
                {
                    MockDomainManager.Setup(x => x.GetForUser(It.Is<int>(y => y == 0)))
                            .Throws(ex);

                    return SetCurrentUserId(BuildSystemWithSanitizer())
                        .GetMyLogs()
                        .ExecuteAsync(new System.Threading.CancellationToken())
                        .Result;
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



        public class GetExport : Fixture
        {
            // Mapping of data types is tested elsewhere.
            // Here, let's just confirm that all records are mapped

            private const int id = 123;
            private const int id2 = 34234;
            private const int userId = 223;

            [Fact]
            public void ConfirmMapsFirstRecord()
            {
                Assert.Equal(id, RunPositiveTest().First().Id);
            }

            [Fact]
            public void ConfirmMapsSecondRecord()
            {
                Assert.Equal(id2, RunPositiveTest().Skip(1).First().Id);
            }
            
            private List<WorkLogModel> RunPositiveTest()
            {
                // Arrange 
                var records = new List<WorkLogReportRecord>
                {
                    new WorkLogReportRecord
                    {
                        Id = id,
                        WorkDate = "01/10/2016"
                    },

                    new WorkLogReportRecord
                    {
                        Id = id2,
                        WorkDate = "01/10/2016"
                    }
                };

                MockDomainManager.Setup(x => x.GetReport())
                    .Returns(records);

                // Act
                var result = BuildSystemWithSanitizer()
                    .GetExport();

                var message = result.ExecuteAsync(new System.Threading.CancellationToken()).Result;

                Assert.Equal(HttpStatusCode.OK, message.StatusCode);

                return message.Content.ReadAsAsync<List<WorkLogModel>>().Result;
            }

            public class ExceptionHandling : GetExport
            {

                private HttpResponseMessage RunTest(Exception ex)
                {
                    MockDomainManager.Setup(x => x.GetReport())
                                .Throws(ex);

                    return BuildSystemWithSanitizer()
                        .GetExport()
                        .ExecuteAsync(new System.Threading.CancellationToken())
                        .Result;
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
            private const int id = 123;
            private const string description = "somewhere";
            private const decimal travelHours = 3.53m;
            private const decimal workHours = 23.32m;
            private readonly DateTime workDay = DateTime.Parse("2015-12-07 13:45");
            private const int userId = 223;
            private const int workTypeId = 3;
            private const int locationId = 4;
            private const int numberOfVolunteers = 234;
            private const string tasksCompletedSafe = "asdf asdfasdfasdf";
            private const string tasksCompletedWithHtml = "asdf asdfa<i>something</i>sdfasdf";

            [Fact]
            public void ConfirmMapsNumberOfVolunteers()
            {
                RunPositiveTest();
                MockDomainManager.Verify(x => x.Create(It.Is<WorkLog>(y => y.NumberOfVolunteers == numberOfVolunteers)));
            }

            [Fact]
            public void ConfirmMapsTasksCompleted()
            {
                RunPositiveTest();
                MockDomainManager.Verify(x => x.Create(It.Is<WorkLog>(y => y.TasksCompleted== tasksCompletedSafe)));
            }


            [Fact]
            public void ConfirmStripsHtmlFromTasksCompleted()
            {

                // Arrange 
                var record = new WorkLogModel
                {
                    LocationId = locationId,
                    TravelTimeHours = travelHours,
                    WorkTypeId = workTypeId,
                    WorkHours = workHours,
                    UserId = userId,
                    WorkDate = workDay,
                    Id = id,
                    NumberOfVolunteers = numberOfVolunteers,
                    TasksCompleted = tasksCompletedWithHtml
                };

                MockDomainManager.Setup(x => x.Create(It.IsAny<WorkLog>()))
                .Returns((WorkLog actual) =>
                {
                    // inject an ID value so we can confirm that it is passed in the response
                    actual.Id = id;
                    return actual;
                });

                MockSanitizer.Setup(x => x.RemoveAllHtml(tasksCompletedWithHtml))
                    .Returns(tasksCompletedSafe);


                // Act
                var result = BuildSystemWithSanitizer().Post(record);

                var message = result.ExecuteAsync(new System.Threading.CancellationToken()).Result;

                //Assert
                MockDomainManager.Verify(x => x.Create(It.Is<WorkLog>(y => y.TasksCompleted == tasksCompletedSafe)));
            }

            [Fact]
            public void ConfirmMapsId()
            {
                RunPositiveTest();
                MockDomainManager.Verify(x => x.Create(It.Is<WorkLog>(y => y.Id == id)));
            }

            [Fact]
            public void ConfirmMapsLocationId()
            {
                RunPositiveTest();
                MockDomainManager.Verify(x => x.Create(It.Is<WorkLog>(y => y.LocationId == locationId )));
            }

            [Fact]
            public void ConfirmMapsTravelTimeHours()
            {
                RunPositiveTest();
                MockDomainManager.Verify(x => x.Create(It.Is<WorkLog>(y => y.TravelTimeHours == travelHours)));
            }

            [Fact]
            public void ConfirmMapsUserId()
            {
                RunPositiveTest();
                MockDomainManager.Verify(x => x.Create(It.Is<WorkLog>(y => y.UserId == userId)));
            }

            [Fact]
            public void ConfirmMapsWorkDate()
            {
                RunPositiveTest();
                MockDomainManager.Verify(x => x.Create(It.Is<WorkLog>(y => y.WorkDate == workDay)));
            }

            [Fact]
            public void ConfirmMapsWorkHours()
            {
                RunPositiveTest();
                MockDomainManager.Verify(x => x.Create(It.Is<WorkLog>(y => y.WorkHours == workHours)));
            }

            [Fact]
            public void ConfirmMapsWorkTypeId()
            {
                RunPositiveTest();
                MockDomainManager.Verify(x => x.Create(It.Is<WorkLog>(y => y.WorkTypeId == workTypeId)));
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
                var record = new WorkLogModel
                {
                    LocationId = locationId,
                    TravelTimeHours = travelHours,
                    WorkTypeId = workTypeId,
                    WorkHours = workHours,
                    UserId = userId,
                    WorkDate = workDay,
                    Id = id,
                    NumberOfVolunteers = numberOfVolunteers,
                    TasksCompleted = tasksCompletedSafe
                };

                MockDomainManager.Setup(x => x.Create(It.IsAny<WorkLog>()))
                .Returns((WorkLog actual) =>
                {
                    // inject an ID value so we can confirm that it is passed in the response
                    actual.Id = id;
                    return actual;
                });

                MockSanitizer.Setup(x => x.RemoveAllHtml(It.IsAny<string>()))
                    .Returns((string input) => input);


                // Act
                var result = BuildSystemWithSanitizer().Post(record);

                var message = result.ExecuteAsync(new System.Threading.CancellationToken()).Result;

                return message;

            }

            public class ExceptionHandling : Fixture
            {

                protected HttpResponseMessage RunTest(Exception ex)
                {
                    MockDomainManager.Setup(x => x.Create(It.IsAny<WorkLog>()))
                        .Throws(ex);

                    MockSanitizer.Setup(x => x.RemoveAllHtml(It.IsAny<string>()))
                        .Returns(string.Empty);

                    return BuildSystemWithSanitizer().Post(new WorkLogModel()).ExecuteAsync(new System.Threading.CancellationToken()).Result;
                }

                [Fact]
                public void ConfirmDoesNotAcceptNull()
                {
                    Assert.Throws<ArgumentNullException>(() => BuildSystemWithSanitizer().Post(null));
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

                    MockDomainManager.Setup(x => x.Create(It.IsAny<WorkLog>()))
                                           .Throws(e);


                    MockSanitizer.Setup(x => x.RemoveAllHtml(It.IsAny<string>()))
                        .Returns(string.Empty);

                    ExpectToLogToDebug();

                    return BuildSystemWithSanitizer().Post(new WorkLogModel()) as InvalidModelStateResult;
                }
            }
        }


        public class Put : Fixture
        {
            private const int id = 123;
            private const string description = "somewhere";
            private const decimal travelHours = 3.53m;
            private const decimal workHours = 23.32m;
            private readonly DateTime workDay = DateTime.Parse("2015-12-07 13:45");
            private const int userId = 223;
            private const int workTypeId = 3;
            private const int locationId = 4;

            [Fact]
            public void ConfirmMapsId()
            {
                RunPositiveTest();
                MockDomainManager.Verify(x => x.Update(It.Is<WorkLog>(y => y.Id == id)));
            }

            [Fact]
            public void ConfirmMapsLocationId()
            {
                RunPositiveTest();
                MockDomainManager.Verify(x => x.Update(It.Is<WorkLog>(y => y.LocationId == locationId)));
            }

            [Fact]
            public void ConfirmMapsTravelTimeHours()
            {
                RunPositiveTest();
                MockDomainManager.Verify(x => x.Update(It.Is<WorkLog>(y => y.TravelTimeHours == travelHours)));
            }

            [Fact]
            public void ConfirmMapsUserId()
            {
                RunPositiveTest();
                MockDomainManager.Verify(x => x.Update(It.Is<WorkLog>(y => y.UserId == userId)));
            }

            [Fact]
            public void ConfirmMapsWorkDate()
            {
                RunPositiveTest();
                MockDomainManager.Verify(x => x.Update(It.Is<WorkLog>(y => y.WorkDate == workDay)));
            }

            [Fact]
            public void ConfirmMapsWorkHours()
            {
                RunPositiveTest();
                MockDomainManager.Verify(x => x.Update(It.Is<WorkLog>(y => y.WorkHours == workHours)));
            }

            [Fact]
            public void ConfirmMapsWorkTypeId()
            {
                RunPositiveTest();
                MockDomainManager.Verify(x => x.Update(It.Is<WorkLog>(y => y.WorkTypeId == workTypeId)));
            }

           
            [Fact]
            public void ConfirmReturnsNoContenttatus()
            {
                Assert.Equal(HttpStatusCode.NoContent, RunPositiveTest().StatusCode);
            }

            private WorkLogModel GetTestResult()
            {
                return RunPositiveTest().Content.ReadAsAsync<WorkLogModel>().Result;
            }

            private HttpResponseMessage RunPositiveTest()
            {
                // Arrange 
                var record = new WorkLogModel
                {
                    LocationId = locationId,
                    TravelTimeHours = travelHours,
                    WorkTypeId = workTypeId,
                    WorkHours = workHours,
                    UserId = userId,
                    WorkDate = workDay,
                    Id = id
                };

                MockDomainManager.Setup(x => x.Update(It.IsAny<WorkLog>()))
                    .Returns(1);

                MockSanitizer.Setup(x => x.RemoveAllHtml(It.IsAny<string>()))
                    .Returns((string input) => input);


                // Act
                var result = BuildSystemWithSanitizer().Put(record);

                var message = result.ExecuteAsync(new System.Threading.CancellationToken()).Result;

                return message;

            }

            public class ExceptionHandling : Fixture
            {

                private HttpResponseMessage RunTest(Exception ex)
                {
                    MockDomainManager.Setup(x => x.Update(It.IsAny<WorkLog>()))
                        .Throws(ex);

                    MockSanitizer.Setup(x => x.RemoveAllHtml(It.IsAny<string>()))
                        .Returns(string.Empty);

                    return BuildSystemWithSanitizer().Put(new WorkLogModel()).ExecuteAsync(new System.Threading.CancellationToken()).Result;
                }

                [Fact]
                public void ConfirmDoesNotAcceptNull()
                {
                    Assert.Throws<ArgumentNullException>(() => BuildSystemWithSanitizer().Put(null));
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

                    MockDomainManager.Setup(x => x.Update(It.IsAny<WorkLog>()))
                                           .Throws(e);

                    MockSanitizer.Setup(x => x.RemoveAllHtml(It.IsAny<string>()))
                        .Returns(string.Empty);

                    ExpectToLogToDebug();

                    return BuildSystemWithSanitizer().Put(new WorkLogModel()) as InvalidModelStateResult;
                }
            }
        }
        
    }
}
