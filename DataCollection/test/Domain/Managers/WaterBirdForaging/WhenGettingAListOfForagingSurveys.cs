using Moq;
using FlightNode.DataCollection.Domain.Interfaces.Persistence;
using Xunit;
using FlightNode.DataCollection.Domain.Entities;
using System;
using FlightNode.DataCollection.Domain.UnitTests.Domain.Managers;
using FlightNode.DataCollection.Domain.Managers;
using System.Linq;

namespace FlightNode.DataCollection.Domain.UnitTests.Domain.Managers.WaterBirdForaging
{

    public class WhenGettingAListOfForagingSurveys
    {
        public class Fixture : IDisposable
        {
            protected MockRepository MockRepository { get; set; }
            protected Mock<ISurveyPersistence> SurveyPersistenceMock { get; set; }
            protected SurveyManager Manager { get; set; }

            public Fixture()
            {
                MockRepository = new MockRepository(MockBehavior.Strict);
                SurveyPersistenceMock = MockRepository.Create<ISurveyPersistence>();

                Manager = new SurveyManager(SurveyPersistenceMock.Object);
            }

            public void Dispose()
            {
                MockRepository.VerifyAll();
            }
        }

        public class GivenThereIsAPendingForagingSurvey : Fixture
        {
            const int locationId = 13;
            const int userId = 99;
            static Guid identifier = new Guid("a507f681-c111-447a-bc1f-195916891226");
            const string siteCode = "3333";
            const string siteName = "897987987";
            static DateTime startDate = new DateTime(2000, 1, 2, 3, 4, 5);
            const string firstName = "C3";
            const string lastName = "PIO";
            const string expectedName = "C3 PIO";
            const int surveyTypeId = 2;

            [Fact]
            public void ThenReturnOneRecord()
            {
                ArrangeTest();

                var result = Manager.GetSurveyListByTypeAndUser(surveyTypeId);

                Assert.Equal(1, result.Count);
            }

            [Fact]
            public void ThenMapStatusAsPending()
            {
                ArrangeTest();

                var result = Manager.GetSurveyListByTypeAndUser(surveyTypeId);

                Assert.Equal("Pending", result.First().Status);
            }

            [Fact]
            public void ThenMapTheSurveyIdentifier()
            {
                ArrangeTest();

                var result = Manager.GetSurveyListByTypeAndUser(surveyTypeId);

                Assert.Equal(identifier, result.First().SurveyIdentifier);
            }

            [Fact]
            public void ThenMapTheSiteName()
            {
                ArrangeTest();

                var result = Manager.GetSurveyListByTypeAndUser(surveyTypeId);

                Assert.Equal(siteName, result.First().SiteName);
            }

            [Fact]
            public void ThenMapTheSiteCode()
            {
                ArrangeTest();

                var result = Manager.GetSurveyListByTypeAndUser(surveyTypeId);

                Assert.Equal(siteCode, result.First().SiteCode);
            }

            [Fact]
            public void ThenMapTheStartdate()
            {
                ArrangeTest();

                var result = Manager.GetSurveyListByTypeAndUser(surveyTypeId);

                Assert.Equal(startDate, result.First().StartDate);
            }

            [Fact]
            public void ThenMapTheUserFullName()
            {
                ArrangeTest();

                var result = Manager.GetSurveyListByTypeAndUser(surveyTypeId);

                Assert.Equal(expectedName, result.First().SubmittedBy);
            }

            private void ArrangeTest()
            {
                var survey = new SurveyPending
                {
                    LocationId = locationId,
                    SubmittedBy = userId,
                    SurveyIdentifier = identifier,
                    StartDate = startDate,
                    SurveyTypeId = surveyTypeId
                };
                var location = new Location
                {
                    Id = locationId,
                    SiteCode = siteCode,
                    SiteName = siteName
                };
                var user = new User
                {
                    FamilyName = lastName,
                    GivenName = firstName,
                    Id = userId
                };

                SurveyPersistenceMock.SetupGet(x => x.SurveysPending)
                    .Returns(FakeDbSet<SurveyPending>.Create(survey));

                SurveyPersistenceMock.SetupGet(x => x.Locations)
                    .Returns(FakeDbSet<Location>.Create(location));

                SurveyPersistenceMock.SetupGet(x => x.Users)
                    .Returns(FakeDbSet<User>.Create(user));

                SurveyPersistenceMock.SetupGet(x => x.SurveysCompleted)
                    .Returns(new FakeDbSet<SurveyCompleted>());
            }
        }


        public class GivenThereIsACompletedForagingSurvey : Fixture
        {
            const int locationId = 13;
            const int userId = 99;
            static Guid identifier = new Guid("a507f681-c111-447a-bc1f-195916891226");
            const string siteCode = "3333";
            const string siteName = "897987987";
            static DateTime startDate = new DateTime(2000, 1, 2, 3, 4, 5);
            const string firstName = "C3";
            const string lastName = "PIO";
            const string expectedName = "C3 PIO";
            const int surveyTypeId = 2;

            [Fact]
            public void ThenReturnOneRecord()
            {
                ArrangeTest();

                var result = Manager.GetSurveyListByTypeAndUser(surveyTypeId);

                Assert.Equal(1, result.Count);
            }

            [Fact]
            public void ThenMapStatusAsPending()
            {
                ArrangeTest();

                var result = Manager.GetSurveyListByTypeAndUser(surveyTypeId);

                Assert.Equal("Complete", result.First().Status);
            }

            [Fact]
            public void ThenMapTheSurveyIdentifier()
            {
                ArrangeTest();

                var result = Manager.GetSurveyListByTypeAndUser(surveyTypeId);

                Assert.Equal(identifier, result.First().SurveyIdentifier);
            }

            [Fact]
            public void ThenMapTheSiteName()
            {
                ArrangeTest();

                var result = Manager.GetSurveyListByTypeAndUser(surveyTypeId);

                Assert.Equal(siteName, result.First().SiteName);
            }

            [Fact]
            public void ThenMapTheSiteCode()
            {
                ArrangeTest();

                var result = Manager.GetSurveyListByTypeAndUser(surveyTypeId);

                Assert.Equal(siteCode, result.First().SiteCode);
            }

            [Fact]
            public void ThenMapTheStartdate()
            {
                ArrangeTest();

                var result = Manager.GetSurveyListByTypeAndUser(surveyTypeId);

                Assert.Equal(startDate, result.First().StartDate);
            }

            [Fact]
            public void ThenMapTheUserFullName()
            {
                ArrangeTest();

                var result = Manager.GetSurveyListByTypeAndUser(surveyTypeId);

                Assert.Equal(expectedName, result.First().SubmittedBy);
            }

            private void ArrangeTest()
            {
                var survey = new SurveyCompleted
                {
                    LocationId = locationId,
                    SubmittedBy = userId,
                    SurveyIdentifier = identifier,
                    StartDate = startDate,
                    SurveyTypeId = surveyTypeId
                };
                var location = new Location
                {
                    Id = locationId,
                    SiteCode = siteCode,
                    SiteName = siteName
                };
                var user = new User
                {
                    FamilyName = lastName,
                    GivenName = firstName,
                    Id = userId
                };

                SurveyPersistenceMock.SetupGet(x => x.SurveysCompleted)
                    .Returns(FakeDbSet<SurveyCompleted>.Create(survey));

                SurveyPersistenceMock.SetupGet(x => x.Locations)
                    .Returns(FakeDbSet<Location>.Create(location));

                SurveyPersistenceMock.SetupGet(x => x.Users)
                    .Returns(FakeDbSet<User>.Create(user));

                SurveyPersistenceMock.SetupGet(x => x.SurveysPending)
                    .Returns(new FakeDbSet<SurveyPending>());
            }
        }

        public class GivenThereAreBothPendingAndCompleteForagingSurveys : Fixture
        {
            const int locationId = 13;
            const int userId = 99;
            static Guid identifier = new Guid("a507f681-c111-447a-bc1f-195916891226");
            const string siteCode = "3333";
            const string siteName = "897987987";
            static DateTime startDate = new DateTime(2000, 1, 2, 3, 4, 5);
            const string firstName = "C3";
            const string lastName = "PIO";
            const string expectedName = "C3 PIO";
            const int surveyTypeId = 2;

            [Fact]
            public void ThenReturnBothRecords()
            {
                ArrangeTest();

                var result = Manager.GetSurveyListByTypeAndUser(surveyTypeId);

                Assert.Equal(2, result.Count);
            }


            private void ArrangeTest()
            {
                var pending = new SurveyPending
                {
                    LocationId = locationId,
                    SubmittedBy = userId,
                    SurveyIdentifier = identifier,
                    StartDate = startDate,
                    SurveyTypeId = surveyTypeId
                };
                var complete = new SurveyCompleted
                {
                    LocationId = locationId,
                    SubmittedBy = userId,
                    SurveyIdentifier = identifier,
                    StartDate = startDate,
                    SurveyTypeId = surveyTypeId
                };
                var location = new Location
                {
                    Id = locationId,
                    SiteCode = siteCode,
                    SiteName = siteName
                };
                var user = new User
                {
                    FamilyName = lastName,
                    GivenName = firstName,
                    Id = userId
                };

                SurveyPersistenceMock.SetupGet(x => x.SurveysCompleted)
                    .Returns(FakeDbSet<SurveyCompleted>.Create(complete));

                SurveyPersistenceMock.SetupGet(x => x.SurveysPending)
                    .Returns(FakeDbSet<SurveyPending>.Create(pending));

                SurveyPersistenceMock.SetupGet(x => x.Locations)
                    .Returns(FakeDbSet<Location>.Create(location));

                SurveyPersistenceMock.SetupGet(x => x.Users)
                    .Returns(FakeDbSet<User>.Create(user));
            }
        }

        public class GivenThereIsAPendingOtherSurvey : Fixture
        {
            const int locationId = 13;
            const int userId = 99;
            static Guid identifier = new Guid("a507f681-c111-447a-bc1f-195916891226");
            const string siteCode = "3333";
            const string siteName = "897987987";
            static DateTime startDate = new DateTime(2000, 1, 2, 3, 4, 5);
            const string firstName = "C3";
            const string lastName = "PIO";
            const string expectedName = "C3 PIO";
            const int surveyTypeId = 22;

            [Fact]
            public void ThenReturnNoRecords()
            {
                ArrangeTest();

                var result = Manager.GetSurveyListByTypeAndUser(SurveyType.Foraging);

                Assert.Equal(0, result.Count);
            }

            private void ArrangeTest()
            {
                var survey = new SurveyPending
                {
                    LocationId = locationId,
                    SubmittedBy = userId,
                    SurveyIdentifier = identifier,
                    StartDate = startDate,
                    SurveyTypeId = surveyTypeId
                };
                var location = new Location
                {
                    Id = locationId,
                    SiteCode = siteCode,
                    SiteName = siteName
                };
                var user = new User
                {
                    FamilyName = lastName,
                    GivenName = firstName,
                    Id = userId
                };

                SurveyPersistenceMock.SetupGet(x => x.SurveysPending)
                    .Returns(FakeDbSet<SurveyPending>.Create(survey));

                SurveyPersistenceMock.SetupGet(x => x.SurveysCompleted)
                    .Returns(new FakeDbSet<SurveyCompleted>());

                SurveyPersistenceMock.SetupGet(x => x.Locations)
                    .Returns(FakeDbSet<Location>.Create(location));

                SurveyPersistenceMock.SetupGet(x => x.Users)
                    .Returns(FakeDbSet<User>.Create(user));
            }
        }

        public class GivenThereIsACompleteOtherSurvey : Fixture
        {
            const int locationId = 13;
            const int userId = 99;
            static Guid identifier = new Guid("a507f681-c111-447a-bc1f-195916891226");
            const string siteCode = "3333";
            const string siteName = "897987987";
            static DateTime startDate = new DateTime(2000, 1, 2, 3, 4, 5);
            const string firstName = "C3";
            const string lastName = "PIO";
            const string expectedName = "C3 PIO";
            const int surveyTypeId = 22;

            [Fact]
            public void ThenReturnNoRecords()
            {
                ArrangeTest();

                var result = Manager.GetSurveyListByTypeAndUser(SurveyType.Foraging);

                Assert.Equal(0, result.Count);
            }

            private void ArrangeTest()
            {
                var survey = new SurveyCompleted
                {
                    LocationId = locationId,
                    SubmittedBy = userId,
                    SurveyIdentifier = identifier,
                    StartDate = startDate,
                    SurveyTypeId = surveyTypeId
                };
                var location = new Location
                {
                    Id = locationId,
                    SiteCode = siteCode,
                    SiteName = siteName
                };
                var user = new User
                {
                    FamilyName = lastName,
                    GivenName = firstName,
                    Id = userId
                };

                SurveyPersistenceMock.SetupGet(x => x.SurveysPending)
                    .Returns(new FakeDbSet<SurveyPending>());

                SurveyPersistenceMock.SetupGet(x => x.SurveysCompleted)
                    .Returns(FakeDbSet<SurveyCompleted>.Create(survey));

                SurveyPersistenceMock.SetupGet(x => x.Locations)
                    .Returns(FakeDbSet<Location>.Create(location));

                SurveyPersistenceMock.SetupGet(x => x.Users)
                    .Returns(FakeDbSet<User>.Create(user));
            }
        }
    }
}
