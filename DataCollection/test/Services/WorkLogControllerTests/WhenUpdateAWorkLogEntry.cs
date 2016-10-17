using FlightNode.Common.Exceptions;
using FlightNode.DataCollection.Domain.Entities;
using FlightNode.DataCollection.Services.Models.WorkLog;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Results;
using Xunit;

namespace FlightNode.DataCollection.Domain.UnitTests.Services.WorkLogControllerTests
{
    public class WhenUpdateAWorkLogEntry : Fixture
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





        private const string message1 = "asdfasdfa";
        private const string field1 = "fieeeeeld";
        private const string message2 = "as8df7a89psdfp";
        private const string field2 = "sdk;kl;hl;";


 

        [Fact]
        public void ConfirmDoesNotCactchDomainValidationException()
        {
            Assert.Throws<DomainValidationException>(() => RunValidationTest());
        }

        private InvalidModelStateResult RunValidationTest()
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

            
            return BuildSystemWithSanitizer().Put(new WorkLogModel()) as InvalidModelStateResult;

        }
    }
}
