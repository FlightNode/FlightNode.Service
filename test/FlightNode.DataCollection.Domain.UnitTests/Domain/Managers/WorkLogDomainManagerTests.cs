using FlightNode.Common.Exceptions;
using FlightNode.DataCollection.Domain.Entities;
using FlightNode.DataCollection.Domain.Interfaces.Persistence;
using FlightNode.DataCollection.Domain.Managers;
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

            protected void SetupWorkLogsCollection()
            {
                base.WorkLogPersistenceMock.SetupGet(x => x.Collection)
                    .Returns(FakeSet);
            }
        }

        public class Create : CreateFakeSet
        {
            private const int RECORD_COUNT = 1;
            private WorkLog item = new WorkLog
            {
                WorkLogId = 1,
                LocationId = 1,
                TravelTimeHours = 1.0m,
                UserId = 1,
                WorkDate = DateTime.Parse("2015-11-12 6:04 PM"),
                WorkHours = 1.1m,
                WorkTypeId = 3
            };


            [Fact]
            public void ConfirmReturnsModifiedRecord()
            {

                // Arrange
                var id = 34234;
                base.SetupWorkLogsCollection();
                base.WorkLogPersistenceMock.Setup(x => x.SaveChanges())
                    .Callback(() => item.WorkLogId = id)
                    .Returns(RECORD_COUNT);

                // Act
                var result = BuildSystem().Create(item);

                // Assert
                Assert.Equal(id, item.WorkLogId);
            }


            [Fact]
            public void ConfirmInputIsAddedToCollection()
            {

                // Arrange
                var id = 34234;
                base.SetupWorkLogsCollection();
                base.WorkLogPersistenceMock.Setup(x => x.SaveChanges())
                    .Callback(() => item.WorkLogId = id)
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
                    WorkLogId = 1,
                    LocationId = 1,
                    TravelTimeHours = 1.0m,
                    UserId = 1,
                    WorkDate = DateTime.Parse("2015-11-12 6:04 PM"),
                    WorkHours = 1.1m,
                    WorkTypeId = 3
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
                    base.SetupWorkLogsCollection();
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
                base.SetupWorkLogsCollection();
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
            [Fact]
            public void ConfirmHappyPath()
            {
                // Arrange
                var id = 23423;
                base.SetupWorkLogsCollection();
                var one = new WorkLog();
                var two = new WorkLog { WorkLogId = id };
                base.FakeSet.List.AddRange(new List<WorkLog>() { one, two });

                // Act
                var result = BuildSystem().FindById(id);

                // Assert
                Assert.Same(two, result);
            }
        }

        public class Update : CreateFakeSet
        {
            private const int RECORD_COUNT = 1;
            private WorkLog item = new WorkLog
            {
                WorkLogId = 1,
                LocationId = 1,
                TravelTimeHours = 1.0m,
                UserId = 1,
                WorkDate = DateTime.Parse("2015-11-12 6:04 PM"),
                WorkHours = 1.1m,
                WorkTypeId = 3
            };

            [Fact]
            public void ConfirmHappyPath()
            {

                // Arrange
                base.SetupWorkLogsCollection();
                base.WorkLogPersistenceMock.Setup(x => x.SaveChanges())
                    .Returns(RECORD_COUNT);

                // Act
                BuildSystem().Update(item);
            }

            public class Validation : CreateFakeSet
            {
                private WorkLog item = new WorkLog
                {
                    WorkLogId = 1,
                    LocationId = 1,
                    TravelTimeHours = 1.0m,
                    UserId = 1,
                    WorkDate = DateTime.Parse("2015-11-12 6:04 PM"),
                    WorkHours = 1.1m,
                    WorkTypeId = 3
                };

                private void RunPositiveTest()
                {
                    base.SetupWorkLogsCollection();
                    WorkLogPersistenceMock.Setup(x => x.SaveChanges())
                           .Returns(1);

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
    }
}
