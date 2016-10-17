using System;
using System.Collections.Generic;
using System.Linq;
using FlightNode.Common.Exceptions;
using FlightNode.DataCollection.Domain.Entities;
using FlightNode.DataCollection.Services.Models.WorkLog;
using Moq;
using System.Net;
using System.Net.Http;
using Xunit;
using FlightNode.DataCollection.Services.Models;
using System.Web.Http.Results;
using System.ComponentModel.DataAnnotations;

namespace FlightNode.DataCollection.Domain.UnitTests.Services.WorkLogControllerTests
{

    public class WhenCreatingAWorkLogEntry : Fixture
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
            MockDomainManager.Verify(x => x.Create(It.Is<WorkLog>(y => y.TasksCompleted == tasksCompletedSafe)));
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
            MockDomainManager.Verify(x => x.Create(It.Is<WorkLog>(y => y.LocationId == locationId)));
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




    }

}
