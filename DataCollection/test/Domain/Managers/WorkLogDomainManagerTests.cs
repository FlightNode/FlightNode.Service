using FlightNode.Common.Exceptions;
using FlightNode.DataCollection.Domain.Entities;
using FlightNode.DataCollection.Domain.Interfaces.Persistence;
using FlightNode.DataCollection.Domain.Managers;
using FlightNode.DataCollection.Infrastructure.Persistence;
using Moq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace FlightNode.DataCollection.Domain.UnitTests.Domain.Managers
{
    public class WorkLogDomainManagertests
    {
        public class Fixture : IDisposable
        {
            protected MockRepository MockRepository { get; set; }
            protected Mock<IWorkLogPersistence> WorkLogPersistenceMock { get; set; }


            public Fixture()
            {
                MockRepository = new MockRepository(MockBehavior.Strict);
                WorkLogPersistenceMock = MockRepository.Create<IWorkLogPersistence>();
            }

            protected WorkLogDomainManager BuildSystem()
            {
                return new WorkLogDomainManager(WorkLogPersistenceMock.Object);
            }

            public void Dispose()
            {
                MockRepository.VerifyAll();
            }
        }

        public class ArgumentTests : Fixture
        {
            [Fact]
            public void ConfirmHappyPath()
            {
                base.BuildSystem();
            }

            [Fact]
            public void ConfirmThatNullArgumentIsNotAllowed()
            {
                Assert.Throws<ArgumentNullException>(() => new WorkLogDomainManager(null));
            }

            [Fact]
            public void ConfirmCreateDoesNotAcceptNullArgument()
            {
                // need to move this to another class, maybe in a renamed constructor class.
                Assert.Throws<ArgumentNullException>(() => BuildSystem().Create(null));
            }


            [Fact]
            public void ConfirmUpdateDoesNotAcceptNullArgument()
            {
                Assert.Throws<ArgumentNullException>(() => BuildSystem().Update(null));
            }
        }

        public class CreateFakeSet : Fixture
        {
            protected FakeDbSet<WorkLog> FakeSet = new FakeDbSet<WorkLog>();


            protected void SetupFakeDbSets()
            {
                base.WorkLogPersistenceMock.SetupGet(x => x.Collection)
                    .Returns(FakeSet);

            }

            protected void ExpectToUpdateEntity()
            {
                var entityMock = base.MockRepository.Create<IDbEntityEntryDecorator>();
                entityMock.SetupSet(x => x.State = EntityState.Modified);

                base.WorkLogPersistenceMock.Setup(x => x.Entry(It.IsAny<object>()))
                    .Returns(entityMock.Object);
            }
        }

        public class Create : CreateFakeSet
        {
            private const int RECORD_COUNT = 1;
            private WorkLog item = new WorkLog
            {
                LocationId = 1,
                WorkTypeId = 1,
                TravelTimeHours = 1.0m,
                UserId = 1,
                WorkDate = DateTime.Parse("2015-11-12 6:04 PM"),
                WorkHours = 1.1m,
                Id = 3
            };


            [Fact]
            public void ConfirmReturnsModifiedRecord()
            {

                // Arrange
                var id = 34234;
                base.SetupFakeDbSets();
                base.WorkLogPersistenceMock.Setup(x => x.SaveChanges())
                    .Callback(() => item.LocationId = id)
                    .Returns(RECORD_COUNT);

                // Act
                var result = BuildSystem().Create(item);

                // Assert
                Assert.Equal(id, item.LocationId);
            }


            [Fact]
            public void ConfirmInputIsAddedToCollection()
            {

                // Arrange
                var id = 34234;
                base.SetupFakeDbSets();
                base.WorkLogPersistenceMock.Setup(x => x.SaveChanges())
                    .Callback(() => item.LocationId = id)
                    .Returns(RECORD_COUNT);

                // Act
                var result = BuildSystem().Create(item);

                // Assert
                Assert.Same(item, base.FakeSet.List[0]);
            }


            public class Validation : CreateFakeSet
            {
                private WorkLog item = new WorkLog
                {
                    LocationId = 1,
                    WorkTypeId = 1,
                    TravelTimeHours = 1.0m,
                    UserId = 1,
                    WorkDate = DateTime.Parse("2015-11-12 6:04 PM"),
                    WorkHours = 1.1m,
                    Id = 3
                };

                private void RunNegativeTest(string memberName)
                {
                    try
                    {
                        BuildSystem().Create(item);
                        throw new Exception("this should have failed");
                    }
                    catch (DomainValidationException de)
                    {
                        Assert.True(de.ValidationResults.Any(x => x.MemberNames.Any(y => y == memberName)));
                    }
                }

                private void RunPositiveTest()
                {
                    base.SetupFakeDbSets();
                    WorkLogPersistenceMock.Setup(x => x.SaveChanges())
                        .Returns(1);

                    BuildSystem().Create(item);
                }

                [Fact]
                public void ConfirmWorkHoursMustBeGreaterThanZero()
                {
                    item.WorkHours = 0.0m;

                    RunNegativeTest("WorkHours");
                }

                [Fact]
                public void ConfirmTravelTimeMustBeGreaterThanZero()
                {
                    item.TravelTimeHours = 0.0m;

                    RunNegativeTest("TravelTimeHours");
                }

                [Fact]
                public void ConfirmWorkHoursAcceptsUpTo24Hours()
                {
                    item.WorkHours = 24.0m;

                    RunPositiveTest();
                }

                [Fact]
                public void ConfirmTravelTimeAcceptsUpTo24Hours()
                {
                    item.TravelTimeHours = 24.0m;

                    RunPositiveTest();
                }

                [Fact]
                public void ConfirmWorkHoursCannotBeMoreThan24()
                {
                    item.WorkHours = 24.01m;

                    RunNegativeTest("WorkHours");
                }

                [Fact]
                public void ConfirmTravelTimeHoursCannotBeMoreThan24()
                {
                    item.TravelTimeHours = 24.01m;

                    RunNegativeTest("TravelTimeHours");
                }

            }
        }

        public class FindAll : CreateFakeSet
        {
            [Fact]
            public void ConfirmHappyPath()
            {
                // Arrange
                base.SetupFakeDbSets();
                var one = new WorkLog();
                base.FakeSet.List.Add(one);

                // Act
                var result = BuildSystem().FindAll();

                // Assert
                Assert.Collection(result, x => Assert.Same(one, x));
            }
        }

        public class FindById : CreateFakeSet
        {

            const int id = 23423;
            const int VolunteerId = 883;
            const string volunteerFirstName = "Juana";
            const string volunteerLastName = "Coneja";
            const string expectedName = "Juana Coneja";
            const int LocationId = 993;
            const int NumberOfVolunteers = 9923;
            const string TasksCompleted = "asdfasdf";
            const decimal TravelTimeHours = 20.39m;
            static DateTime WorkDate = new DateTime(2016, 10, 16);
            const int WorkHours = 93;
            const int WorkTypeId = 1;

            protected FakeDbSet<User> UsersSet = new FakeDbSet<User>();

            private void SetupUserSet()
            {

                base.WorkLogPersistenceMock.SetupGet(x => x.Users)
                    .Returns(UsersSet);
            }

            [Fact]
            public void ConfirmMapsVolunteerNameFromUserTable()
            {
                Assert.Equal(expectedName, RunTest().VolunteerName);
            }

            [Fact]
            public void ConfirmMapsUserId()
            {
                Assert.Equal(VolunteerId, RunTest().UserId);
            }

            [Fact]
            public void ConfirmMapsLocationId()
            {
                Assert.Equal(LocationId, RunTest().LocationId);
            }

            [Fact]
            public void ConfirmMapsNumberOfVolunteers()
            {
                Assert.Equal(NumberOfVolunteers, RunTest().NumberOfVolunteers);
            }

            [Fact]
            public void ConfirmMapsTasksCompleted()
            {
                Assert.Equal(TasksCompleted, RunTest().TasksCompleted);
            }

            [Fact]
            public void ConfirmMapsTravelTimeHours()
            {
                Assert.Equal(TravelTimeHours, RunTest().TravelTimeHours);
            }

            [Fact]
            public void ConfirmMapsWorkDate()
            {
                Assert.Equal(WorkDate, RunTest().WorkDate);
            }

            [Fact]
            public void ConfirmMapsWorkHours()
            {
                Assert.Equal(WorkHours, RunTest().WorkHours);
            }

            [Fact]
            public void ConfirmMapsWorkTypeId()
            {
                Assert.Equal(WorkTypeId, RunTest().WorkTypeId);
            }

            private WorkLogWithVolunteerName RunTest()
            {
                // Arrange
                SetupFakeDbSets();
                FakeSet.List.Add(new WorkLog {
                    Id = id,
                    UserId = VolunteerId,
                    LocationId = LocationId,
                    NumberOfVolunteers = NumberOfVolunteers,
                    TasksCompleted = TasksCompleted,
                    TravelTimeHours = TravelTimeHours,
                    WorkDate = WorkDate,
                    WorkHours = WorkHours,
                    WorkTypeId = WorkTypeId
                });

                SetupUserSet();
                UsersSet.List.Add(new User
                {
                    Id = VolunteerId,
                    GivenName = volunteerFirstName,
                    FamilyName = volunteerLastName
                });

                // Act
                var result = BuildSystem().FindById(id);
                return result;
            }
        }

        public class Update : CreateFakeSet
        {
            private const int RECORD_COUNT = 1;
            private WorkLog original = new WorkLog
            {
                LocationId = 2,
                WorkTypeId = 2,
                TravelTimeHours = 1.1m,
                UserId = 1,
                WorkDate = DateTime.Parse("2015-11-12 6:05 PM"),
                WorkHours = 1.2m,
                Id = 3
            };
            private WorkLog item = new WorkLog
            {
                LocationId = 1,
                WorkTypeId = 1,
                TravelTimeHours = 1.0m,
                UserId = 1,
                WorkDate = DateTime.Parse("2015-11-12 6:04 PM"),
                WorkHours = 1.1m,
                Id = 3
            };
            private WorkLog differentUser = new WorkLog
            {
                LocationId = 1,
                WorkTypeId = 1,
                TravelTimeHours = 1.0m,
                UserId = 2,
                WorkDate = DateTime.Parse("2015-11-12 6:04 PM"),
                WorkHours = 1.1m,
                Id = 3
            };


            [Fact]
            public void RunHappyPathTest()
            {
                // Arrange
                base.SetupFakeDbSets();
                base.WorkLogPersistenceMock.Setup(x => x.SaveChanges())
                    .Returns(RECORD_COUNT);
                ExpectToUpdateEntity();

                // ... for validating that the userid was not changed
                base.FakeSet.List.Add(original);

                // Act
                BuildSystem().Update(item);
            }


            private void RunPositiveTest()
            {
                base.SetupFakeDbSets();
                WorkLogPersistenceMock.Setup(x => x.SaveChanges())
                       .Returns(1);


                // ... for validating that the userid was not changed
                base.SetupFakeDbSets();
                base.FakeSet.List.Add(item);

                BuildSystem().Update(item);
            }

            private void RunNegativeTest(string memberName)
            {
                try
                {
                    BuildSystem().Update(item);
                    throw new Exception("this should have failed");
                }
                catch (DomainValidationException de)
                {
                    Assert.True(de.ValidationResults.Any(x => x.MemberNames.Any(y => y == memberName)));
                }
            }

            [Fact]
            public void ConfirmWorkHoursMustBeGreaterThanZero()
            {
                item.WorkHours = 0.0m;

                RunNegativeTest("WorkHours");
            }

            [Fact]
            public void ConfirmTravelTimeMustBeGreaterThanZero()
            {
                item.TravelTimeHours = 0.0m;

                RunNegativeTest("TravelTimeHours");
            }

            [Fact]
            public void ConfirmWorkHoursAcceptsUpTo24Hours()
            {
                item.WorkHours = 24.0m;
                ExpectToUpdateEntity();

                RunPositiveTest();
            }

            [Fact]
            public void ConfirmTravelTimeAcceptsUpTo24Hours()
            {
                item.TravelTimeHours = 24.0m;
                ExpectToUpdateEntity();

                RunPositiveTest();
            }

            [Fact]
            public void ConfirmWorkHoursCannotBeMoreThan24()
            {
                item.WorkHours = 24.01m;

                RunNegativeTest("WorkHours");
            }

            [Fact]
            public void ConfirmTravelTimeHoursCannotBeMoreThan24()
            {
                item.TravelTimeHours = 24.01m;

                RunNegativeTest("TravelTimeHours");
            }
        }

        public class GetForUser : CreateFakeSet
        {
            [Fact]
            public void RetrievesRecordCreatedByThisUser()
            {
                // Arrange
                const int userId = 323;
                WorkLogPersistenceMock.Setup(x => x.GetWorkLogReportRecords(It.IsAny<int>()))
                    .Returns(new List<WorkLogReportRecord>() { new WorkLogReportRecord() });

                // Act
                var result = BuildSystem().GetForUser(userId);

                // Assert
                Assert.Equal(1, result.Count());
            }

            [Fact]
            public void QueriesByProperId()
            {
                // Arrange
                const int userId = 323;
                WorkLogPersistenceMock.Setup(x => x.GetWorkLogReportRecords(It.IsAny<int>()))
                    .Returns(new List<WorkLogReportRecord>() { new WorkLogReportRecord() });

                // Act
                var result = BuildSystem().GetForUser(userId);

                // Assert
                WorkLogPersistenceMock.Verify(x => x.GetWorkLogReportRecords(userId));
            }
        }
    }
}
