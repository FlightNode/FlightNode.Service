using FlightNode.Common.Exceptions;
using FlightNode.DataCollection.Domain.Entities;
using FlightNode.DataCollection.Domain.Interfaces.Persistence;
using FlightNode.DataCollection.Domain.Managers;
using Moq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace FlightNode.DataCollection.Domain.UnitTests.Domain.Managers
{
    public class LocationDomainManagertests
    {
        public class Fixture : IDisposable
        {
            protected MockRepository MockRepository { get; set; }
            protected Mock<ILocationPersistence> LocationPersistenceMock { get; set; }


            public Fixture()
            {
                MockRepository = new MockRepository(MockBehavior.Strict);
                LocationPersistenceMock = MockRepository.Create<ILocationPersistence>();
            }

            protected LocationDomainManager BuildSystem()
            {
                return new LocationDomainManager(LocationPersistenceMock.Object);
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
                Assert.Throws<ArgumentNullException>(() => new LocationDomainManager(null));
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
            protected FakeDbSet<Location> FakeSet = new FakeDbSet<Location>();

            protected void SetupLocationsCollection()
            {
                base.LocationPersistenceMock.SetupGet(x => x.Collection)
                    .Returns(FakeSet);
            }

        }

        public class Create : CreateFakeSet
        {
            private const int RECORD_COUNT = 1;
            private Location item = new Location
            {
                SiteCode = "code1",
                SiteName = "name1",
                Latitude = 0m,
                Longitude = 0m,
                Id = 0,
                WorkLogs = null
            };


            [Fact]
            public void ConfirmReturnsModifiedRecord()
            {

                // Arrange
                var id = 34234;
                base.SetupLocationsCollection();
                base.LocationPersistenceMock.Setup(x => x.SaveChanges())
                    .Callback(() => item.Id = id)
                    .Returns(RECORD_COUNT);

                // Act
                var result = BuildSystem().Create(item);

                // Assert
                Assert.Equal(id, item.Id);
            }


            [Fact]
            public void ConfirmInputIsAddedToCollection()
            {

                // Arrange
                var id = 34234;
                base.SetupLocationsCollection();
                base.LocationPersistenceMock.Setup(x => x.SaveChanges())
                    .Callback(() => item.Id = id)
                    .Returns(RECORD_COUNT);

                // Act
                var result = BuildSystem().Create(item);

                // Assert
                Assert.Same(item, base.FakeSet.List[0]);
            }


            public class Validation : CreateFakeSet
            {
                private Location item = new Location
                {
                    SiteCode = "code1",
                    SiteName = "name1",
                    Latitude = 0m,
                    Longitude = 0m,
                    Id = 0,
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
                    base.SetupLocationsCollection();
                    LocationPersistenceMock.Setup(x => x.SaveChanges())
                        .Returns(1);

                    BuildSystem().Create(item);
                }

                [Fact]
                public void ConfirmDescriptionCannotBeNull()
                {
                    item.SiteName = null;

                    RunNegativeTest("Description");
                }

                [Fact]
                public void ConfirmSiteCodeCannotBeNull()
                {
                    item.SiteCode = null;

                    RunNegativeTest("SiteCode");
                }

                [Fact]
                public void ConfirmSiteNameCannotBeNull()
                {
                    item.SiteName = null;

                    RunNegativeTest("SiteName");
                }

                [Fact]
                public void ConfirmDescriptionIsRequired()
                {
                    item.SiteName = string.Empty;

                    RunNegativeTest("Description");
                }

                [Fact]
                public void ConfirmSiteCodeIsRequired()
                {
                    item.SiteCode = string.Empty;

                    RunNegativeTest("SiteCode");
                }

                [Fact]
                public void ConfirmSiteNameIsRequired()
                {
                    item.SiteName = string.Empty;

                    RunNegativeTest("SiteName");
                }


                [Fact]
                public void ConfirmDescriptionRejectsGreaterThan100()
                {
                    item.SiteName = "a".PadLeft(101, '0');

                    RunNegativeTest("Description");
                }

                [Fact]
                public void ConfirmSiteCodeRejectsGreaterThan100()
                {
                    item.SiteCode = "a".PadLeft(101, '0');

                    RunNegativeTest("SiteCode");
                }

                [Fact]
                public void ConfirmSiteNameRejectsGreaterThan100()
                {
                    item.SiteName = "a".PadLeft(101, '0');

                    RunNegativeTest("SiteName");
                }

                [Fact]
                public void ConfirmLatitudeRejectsGreaterThan90()
                {
                    item.Latitude = 90.1m;

                    RunNegativeTest("Latitude");
                }

                [Fact]
                public void ConfirmLatitudeAccepts90()
                {
                    item.Latitude = 90.0m;
                    RunPositiveTest();
                }

                [Fact]
                public void ConfirmLatitudeRejectsLessThanNeg90()
                {
                    item.Latitude = -90.1m;

                    RunNegativeTest("Latitude");
                }

                [Fact]
                public void ConfirmLatitudeAcceptsNeg90()
                {
                    item.Latitude = -90.0m;
                    RunPositiveTest();
                }

                [Fact]
                public void ConfirmLongitudeAccepts180()
                {
                    item.Longitude = 180.0m;
                    RunPositiveTest();
                }

                [Fact]
                public void ConfirmLongitudeRejectsGreaterThan180()
                {
                    item.Longitude = 182.1m;

                    RunNegativeTest("Longitude");
                }

                [Fact]
                public void ConfirmLongitudeAcceptsNeg180()
                {
                    item.Longitude = -180.0m;
                    RunPositiveTest();
                }

                [Fact]
                public void ConfirmLongitudeRejectsGreaterThanNeg180()
                {
                    item.Longitude = -180.1m;

                    RunNegativeTest("Longitude");
                }


            }
        }

        public class FindAll : CreateFakeSet
        {
            [Fact]
            public void ConfirmHappyPath()
            {
                // Arrange
                base.SetupLocationsCollection();
                var one = new Location();
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
                base.SetupLocationsCollection();
                var one = new Location();
                var two = new Location { Id = id };
                base.FakeSet.List.AddRange(new List<Location>() { one, two });

                // Act
                var result = BuildSystem().FindById(id);

                // Assert
                Assert.Same(two, result);
            }
        }

        public class Update : CreateFakeSet
        {
            private const int RECORD_COUNT = 1;
            private Location item = new Location
            {
                SiteCode = "code1",
                SiteName = "name1",
                Latitude = 0m,
                Longitude = 0m,
                Id = 0,
                WorkLogs = null
            };

            protected bool SetModifiedWasCalled = false;

            protected void BypassEntryMethod()
            {
                DomainManagerBase<Location>.SetModifiedState = (IPersistenceBase<Location> persistenceLayer, Location input) =>
                {
                    SetModifiedWasCalled = true;
                };
            }

            [Fact]
            public void ConfirmHappyPath()
            {

                // Arrange
                base.SetupLocationsCollection();
                BypassEntryMethod();
                base.LocationPersistenceMock.Setup(x => x.SaveChanges())
                    .Returns(RECORD_COUNT);

                // Act
                BuildSystem().Update(item);

                Assert.True(SetModifiedWasCalled);
            }

            public class Validation : Update
            {
                private Location item = new Location
                {
                    SiteCode = "code1",
                    SiteName = "name1",
                    Latitude = 0m,
                    Longitude = 0m,
                    Id = 0,
                    WorkLogs = null
                };

                private void RunPositiveTest()
                {
                    base.SetupLocationsCollection();
                    BypassEntryMethod();
                    LocationPersistenceMock.Setup(x => x.SaveChanges())
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
                    item.SiteName = null;

                    RunNegativeTest("Description");
                }

                [Fact]
                public void ConfirmSiteCodeCannotBeNull()
                {
                    item.SiteCode = null;

                    RunNegativeTest("SiteCode");
                }

                [Fact]
                public void ConfirmSiteNameCannotBeNull()
                {
                    item.SiteName = null;

                    RunNegativeTest("SiteName");
                }

                [Fact]
                public void ConfirmDescriptionIsRequired()
                {
                    item.SiteName = string.Empty;

                    RunNegativeTest("Description");
                }

                [Fact]
                public void ConfirmSiteCodeIsRequired()
                {
                    item.SiteCode = string.Empty;

                    RunNegativeTest("SiteCode");
                }

                [Fact]
                public void ConfirmSiteNameIsRequired()
                {
                    item.SiteName = string.Empty;

                    RunNegativeTest("SiteName");
                }

                [Fact]
                public void ConfirmDescriptionRejectsGreaterThan100()
                {
                    item.SiteName = "a".PadLeft(101, '0');

                    RunNegativeTest("Description");
                }

                [Fact]
                public void ConfirmSiteCodeRejectsGreaterThan100()
                {
                    item.SiteCode = "a".PadLeft(101, '0');

                    RunNegativeTest("SiteCode");
                }

                [Fact]
                public void ConfirmSiteNameRejectsGreaterThan100()
                {
                    item.SiteName = "a".PadLeft(101, '0');

                    RunNegativeTest("SiteName");
                }

                [Fact]
                public void ConfirmLatitudeRejectsGreaterThan90()
                {
                    item.Latitude = 90.1m;

                    RunNegativeTest("Latitude");
                }

                [Fact]
                public void ConfirmLatitudeAccepts90()
                {
                    item.Latitude = 90.0m;
                    RunPositiveTest();
                }

                [Fact]
                public void ConfirmLatitudeRejectsLessThanNeg90()
                {
                    item.Latitude = -90.1m;

                    RunNegativeTest("Latitude");
                }

                [Fact]
                public void ConfirmLatitudeAcceptsNeg90()
                {
                    item.Latitude = -90.0m;
                    RunPositiveTest();
                }

                [Fact]
                public void ConfirmLongitudeAccepts180()
                {
                    item.Longitude = 180.0m;
                    RunPositiveTest();
                }

                [Fact]
                public void ConfirmLongitudeRejectsGreaterThan180()
                {
                    item.Longitude = 180.1m;

                    RunNegativeTest("Longitude");
                }
                [Fact]
                public void ConfirmLongitudeAcceptsNeg180()
                {
                    item.Longitude = -180.0m;
                    RunPositiveTest();
                }

                [Fact]
                public void ConfirmLongitudeRejectsGreaterThanNeg180()
                {
                    item.Longitude = -180.1m;

                    RunNegativeTest("Longitude");
                }
            }
        }
    }
}
