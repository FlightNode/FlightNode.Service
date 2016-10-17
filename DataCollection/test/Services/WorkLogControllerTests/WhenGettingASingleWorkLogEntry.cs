using FlightNode.Common.Exceptions;
using FlightNode.DataCollection.Domain.Entities;
using FlightNode.DataCollection.Services.Models.WorkLog;
using Moq;
using System;
using System.Net;
using System.Net.Http;
using Xunit;

namespace FlightNode.DataCollection.Domain.UnitTests.Services.WorkLogControllerTests
{
   public class WhenGettingASingleWorkLogEntry : Fixture
    {
        private const int id = 123;
        private const string description = "somewhere";
        private const decimal travelHours = 3.53m;
        private const decimal workHours = 23.32m;
        private readonly DateTime workDay = DateTime.Parse("2015-12-07 13:45");
        private const int userId = 223;
        private const int workTypeId = 3;
        private const int locationId = 4;
        private const string VolunteerName = "J. Doe";

        [Fact]
        public void ConfirmGetMapsVolunteerName()
        {
            Assert.Equal(VolunteerName, RunPositiveTest().VolunteerName);
        }

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
            var record = new WorkLogWithVolunteerName
            {
                LocationId = locationId,
                TravelTimeHours = travelHours,
                WorkTypeId = workTypeId,
                WorkHours = workHours,
                UserId = userId,
                WorkDate = workDay,
                Id = id,
                VolunteerName = VolunteerName
            };

            MockDomainManager.Setup(x => x.FindById(It.Is<int>(y => y == id)))
                .Returns(record);


            // Act
            var result = BuildSystemWithSanitizer().Get(id);

            var message = result.ExecuteAsync(new System.Threading.CancellationToken()).Result;

            Assert.Equal(HttpStatusCode.OK, message.StatusCode);

            return message.Content.ReadAsAsync<WorkLogModel>().Result;
        }

    }
}
