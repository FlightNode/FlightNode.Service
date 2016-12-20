using FlightNode.DataCollection.Domain.Entities;
using FlightNode.DataCollection.Services.Models.WorkLog;
using System;
using System.Collections.Generic;
using Xunit;
using System.Linq;
using System.Net;
using System.Net.Http;
using FlightNode.Common.Exceptions;

namespace FlightNode.DataCollection.Domain.UnitTests.Services.WorkLogControllerTests
{
    public class WhenGettingAllEntries : Fixture
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
        
    }
}
