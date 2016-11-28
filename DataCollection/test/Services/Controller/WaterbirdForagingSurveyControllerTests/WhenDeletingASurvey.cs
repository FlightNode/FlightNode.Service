using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Moq;
using System.Web.Http;
using System.Web.Http.Results;
using System.Net;
using FlightNode.DataCollection.Domain.Entities;

namespace FlightNode.DataCollection.Domain.UnitTests.Services.Controller.WaterbirdForagingSurveyControllerTests
{
    public class WhenDeletingASurvey 
    {
        public class GivenTheSurveyExists : Fixture
        {

            private IHttpActionResult RunTest()
            {
                // Arrange
                MockDomainManager.Setup(x => x.FindBySurveyId(IDENTIFIER, SurveyType.Foraging))
                    .Returns(CreateDefaultEntity());

                MockDomainManager.Setup(x => x.Delete(IDENTIFIER))
                    .Returns(true);

                // Act
                return BuildSystem().Delete(IDENTIFIER);
            }

            [Fact]
            public void ThenDeleteIt()
            {
                RunTest();

                MockDomainManager.Verify(x => x.Delete(IDENTIFIER), Times.Once());
            }

            [Fact]
            public void ThenReturn204NoContent()
            {
                var result = RunTest() as StatusCodeResult;

                Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
            }
        }

        public class GivenTheSurveyDoesNotExist : Fixture
        {
            [Fact]
            public void ThenReturn404NotFound()
            {
                // Arrange
                MockDomainManager.Setup(x => x.FindBySurveyId(IDENTIFIER, SurveyType.Foraging))
                    .Returns(null as ISurvey);

                // Act
                var result = BuildSystem().Delete(IDENTIFIER);

                // Assert
                Assert.IsType<NotFoundResult>(result);
            }
        }

        public class GivenSomethingBadHappens : Fixture
        {
            [Fact]
            public void ThenLetTheExceptionBubbleThrough()
            {

                // Arrange
                MockDomainManager.Setup(x => x.FindBySurveyId(IDENTIFIER, SurveyType.Foraging))
                    .Throws<InvalidOperationException>();

                // Act
                Func<IHttpActionResult> act = () => BuildSystem().Delete(IDENTIFIER);

                // Assert
                Assert.Throws<InvalidOperationException>(act);
            }
        }

    }
}
