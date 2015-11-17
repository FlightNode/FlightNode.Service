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
    public class WorkTypeDomainManagertests
    {
        public class Fixture : IDisposable
        {
            protected MockRepository MockRepository { get; set; }
            protected Mock<IWorkTypePersistence> WorkTypePersistenceMock { get; set; }


            public Fixture()
            {
                MockRepository = new MockRepository(MockBehavior.Strict);
                WorkTypePersistenceMock = MockRepository.Create<IWorkTypePersistence>();
            }

            protected WorkTypeDomainManager BuildSystem()
            {
                return new WorkTypeDomainManager(WorkTypePersistenceMock.Object);
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
                Assert.Throws<ArgumentNullException>(() => new WorkTypeDomainManager(null));
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
            protected FakeDbSet<WorkType> FakeSet = new FakeDbSet<WorkType>();

            protected void SetupWorkTypesCollection()
            {
                base.WorkTypePersistenceMock.SetupGet(x => x.Collection)
                    .Returns(FakeSet);
            }
        }

        public class Create : CreateFakeSet
        {
            private const int RECORD_COUNT = 1;
            private WorkType item = new WorkType
            {
                Description = "this is a valid description",
                WorkTypeId = 1,
                WorkLogs = null
            };


            [Fact]
            public void ConfirmReturnsModifiedRecord()
            {

                // Arrange
                var id = 34234;
                base.SetupWorkTypesCollection();
                base.WorkTypePersistenceMock.Setup(x => x.SaveChanges())
                    .Callback(() => item.WorkTypeId = id)
                    .Returns(RECORD_COUNT);

                // Act
                var result = BuildSystem().Create(item);

                // Assert
                Assert.Equal(id, item.WorkTypeId);
            }


            [Fact]
            public void ConfirmInputIsAddedToCollection()
            {

                // Arrange
                var id = 34234;
                base.SetupWorkTypesCollection();
                base.WorkTypePersistenceMock.Setup(x => x.SaveChanges())
                    .Callback(() => item.WorkTypeId = id)
                    .Returns(RECORD_COUNT);

                // Act
                var result = BuildSystem().Create(item);

                // Assert
                Assert.Same(item, base.FakeSet.List[0]);
            }


            public class Validation : CreateFakeSet
            {
                private WorkType item = new WorkType
                {
                    Description = "this is a valid description",
                    WorkTypeId = 1,
                    WorkLogs = null
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
                    base.SetupWorkTypesCollection();
                    WorkTypePersistenceMock.Setup(x => x.SaveChanges())
                        .Returns(1);

                    BuildSystem().Create(item);
                }

                [Fact]
                public void ConfirmDescriptionCannotBeNull()
                {
                    item.Description = null;

                    RunNegativeTest("Description");
                }

                [Fact]
                public void ConfirmDescriptionIsRequired()
                {
                    item.Description = string.Empty;

                    RunNegativeTest("Description");
                }

                [Fact]
                public void ConfirmDescriptionRejectsGreaterThan100()
                {
                    item.Description = "a".PadLeft(101, '0');

                    RunNegativeTest("Description");
                }


            }
        }

        public class FindAll : CreateFakeSet
        {
            [Fact]
            public void ConfirmHappyPath()
            {
                // Arrange
                base.SetupWorkTypesCollection();
                var one = new WorkType();
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
                base.SetupWorkTypesCollection();
                var one = new WorkType();
                var two = new WorkType { WorkTypeId = id };
                base.FakeSet.List.AddRange(new List<WorkType>() { one, two });

                // Act
                var result = BuildSystem().FindById(id);

                // Assert
                Assert.Same(two, result);
            }
        }

        public class Update : CreateFakeSet
        {
            private const int RECORD_COUNT = 1;
            private WorkType item = new WorkType
            {
                Description = "this is a valid description",
                WorkTypeId = 0,
                WorkLogs = null
            };


            [Fact]
            public void ConfirmHappyPath()
            {

                // Arrange
                base.SetupWorkTypesCollection();
                base.WorkTypePersistenceMock.Setup(x => x.SaveChanges())
                    .Returns(RECORD_COUNT);

                // Act
                BuildSystem().Update(item);
            }

            public class Validation : CreateFakeSet
            {
                private WorkType item = new WorkType
                {
                    Description = "this is a valid description",
                    WorkTypeId = 0,
                    WorkLogs = null
                };

                private void RunPositiveTest()
                {
                    base.SetupWorkTypesCollection();
                    WorkTypePersistenceMock.Setup(x => x.SaveChanges())
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
                public void ConfirmDescriptionCannotBeNull()
                {
                    item.Description = null;

                    RunNegativeTest("Description");
                }

                [Fact]
                public void ConfirmDescriptionIsRequired()
                {
                    item.Description = string.Empty;

                    RunNegativeTest("Description");
                }

                [Fact]
                public void ConfirmDescriptionMaxLength100()
                {
                    item.Description = "a".PadLeft(101, '0');

                    RunNegativeTest("Description");
                }

                [Fact]
                public void ConfirmDescriptionRejectsGreaterThan100()
                {
                    item.Description = "a".PadLeft(101, '0');

                    RunNegativeTest("Description");
                }
            }
        }
    }
}
