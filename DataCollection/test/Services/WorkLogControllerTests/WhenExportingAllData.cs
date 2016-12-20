using System;
using System.Collections.Generic;
using System.Linq;
using FlightNode.Common.Exceptions;
using FlightNode.DataCollection.Domain.Entities;
using FlightNode.DataCollection.Services.Models.WorkLog;
using System.Net;
using System.Net.Http;
using Xunit;


namespace FlightNode.DataCollection.Domain.UnitTests.Services.WorkLogControllerTests
{
    public class WhenExportingAllData : Fixture
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
    }
}
