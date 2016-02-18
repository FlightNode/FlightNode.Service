using FlightNode.Common.Exceptions;
using FlightNode.DataCollection.Domain.Entities;
using FlightNode.DataCollection.Domain.Interfaces.Persistence;
using FlightNode.DataCollection.Domain.Managers;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
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

            protected Location BuildDefault()
            {
                return new Location
                {
                    SiteCode = "code1",
                    SiteName = "name1",
                    Latitude = 0m,
                    Longitude = 0m,
                    Id = 0,
                    WorkLogs = null,
                    City = "city",
                    County = "county"
                };
            }
        }

        public class Create : CreateFakeSet
        {
            private const int RECORD_COUNT = 1;


            [Fact]
            public void ConfirmReturnsModifiedRecord()
            {

                // Arrange
                var item = BuildDefault();
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
                var item = BuildDefault();
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

                private void RunNegativeTest(Location item, string memberName)
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

                private void RunPositiveTest(Location item)
                {
                    base.SetupLocationsCollection();
                    LocationPersistenceMock.Setup(x => x.SaveChanges())
                        .Returns(1);

                    BuildSystem().Create(item);
                }

                [Fact]
                public void ConfirmCityCannotBeNull()
                {
                    var item = BuildDefault();
                    item.City = null;

                    RunNegativeTest(item, "City");
                }

                [Fact]
                public void ConfirmCityCanBeBlank()
                {
                    var item = BuildDefault();
                    item.City = string.Empty;

                    RunPositiveTest(item);
                }

                [Fact]
                public void ConfirmCountyCannotBeNull()
                {
                    var item = BuildDefault();
                    item.County = null;

                    RunNegativeTest(item, "County");
                }

                [Fact]
                public void ConfirmCountyCanBeBlank()
                {
                    var item = BuildDefault();
                    item.County = string.Empty;

                    RunPositiveTest(item);
                }


                [Fact]
                public void ConfirmSiteCodeCannotBeNull()
                {
                    var item = BuildDefault();
                    item.SiteCode = null;

                    RunNegativeTest(item, "SiteCode");
                }

                [Fact]
                public void ConfirmSiteNameCannotBeNull()
                {
                    var item = BuildDefault();
                    item.SiteName = null;

                    RunNegativeTest(item, "SiteName");
                }
                
                [Fact]
                public void ConfirmSiteCodeIsRequired()
                {
                    var item = BuildDefault();
                    item.SiteCode = string.Empty;

                    RunNegativeTest(item, "SiteCode");
                }

                [Fact]
                public void ConfirmSiteNameIsRequired()
                {
                    var item = BuildDefault();
                    item.SiteName = string.Empty;

                    RunNegativeTest(item, "SiteName");
                }


         

                [Fact]
                public void ConfirmSiteCodeRejectsGreaterThan100()
                {
                    var item = BuildDefault();
                    item.SiteCode = "a".PadLeft(101, '0');

                    RunNegativeTest(item, "SiteCode");
                }

                [Fact]
                public void ConfirmSiteNameRejectsGreaterThan100()
                {
                    var item = BuildDefault();
                    item.SiteName = "a".PadLeft(101, '0');

                    RunNegativeTest(item, "SiteName");
                }

                [Fact]
                public void ConfirmLatitudeRejectsGreaterThan90()
                {
                    var item = BuildDefault();
                    item.Latitude = 90.1m;

                    RunNegativeTest(item, "Latitude");
                }

                [Fact]
                public void ConfirmLatitudeAccepts90()
                {
                    var item = BuildDefault();
                    item.Latitude = 90.0m;
                    RunPositiveTest(item);
                }

                [Fact]
                public void ConfirmLatitudeRejectsLessThanNeg90()
                {
                    var item = BuildDefault();
                    item.Latitude = -90.1m;

                    RunNegativeTest(item, "Latitude");
                }

                [Fact]
                public void ConfirmLatitudeAcceptsNeg90()
                {
                    var item = BuildDefault();
                    item.Latitude = -90.0m;
                    RunPositiveTest(item);
                }

                [Fact]
                public void ConfirmLongitudeAccepts180()
                {
                    var item = BuildDefault();
                    item.Longitude = 180.0m;
                    RunPositiveTest(item);
                }

                [Fact]
                public void ConfirmLongitudeRejectsGreaterThan180()
                {
                    var item = BuildDefault();
                    item.Longitude = 182.1m;

                    RunNegativeTest(item, "Longitude");
                }

                [Fact]
                public void ConfirmLongitudeAcceptsNeg180()
                {
                    var item = BuildDefault();
                    item.Longitude = -180.0m;
                    RunPositiveTest(item);
                }

                [Fact]
                public void ConfirmLongitudeRejectsGreaterThanNeg180()
                {
                    var item = BuildDefault();
                    item.Longitude = -180.1m;

                    RunNegativeTest(item, "Longitude");
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
                var item = BuildDefault();
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

                private void RunPositiveTest(Location item)
                {
                    base.SetupLocationsCollection();
                    BypassEntryMethod();
                    LocationPersistenceMock.Setup(x => x.SaveChanges())
                           .Returns(1);

                    BuildSystem().Update(item);
                }

                private void RunNegativeTest(Location item, string memberName)
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
                public void ConfirmSiteCodeCannotBeNull()
                {
                    var item = BuildDefault();
                    item.SiteCode = null;

                    RunNegativeTest(item, "SiteCode");
                }

                [Fact]
                public void ConfirmSiteNameCannotBeNull()
                {
                    var item = BuildDefault();
                    item.SiteName = null;

                    RunNegativeTest(item, "SiteName");
                }
                
                [Fact]
                public void ConfirmSiteCodeIsRequired()
                {
                    var item = BuildDefault();
                    item.SiteCode = string.Empty;

                    RunNegativeTest(item, "SiteCode");
                }

                [Fact]
                public void ConfirmSiteNameIsRequired()
                {
                    var item = BuildDefault();
                    item.SiteName = string.Empty;

                    RunNegativeTest(item, "SiteName");
                }
                

                [Fact]
                public void ConfirmSiteCodeRejectsGreaterThan100()
                {
                    var item = BuildDefault();
                    item.SiteCode = "a".PadLeft(101, '0');

                    RunNegativeTest(item, "SiteCode");
                }

                [Fact]
                public void ConfirmSiteNameRejectsGreaterThan100()
                {
                    var item = BuildDefault();
                    item.SiteName = "a".PadLeft(101, '0');

                    RunNegativeTest(item, "SiteName");
                }

                [Fact]
                public void ConfirmLatitudeRejectsGreaterThan90()
                {
                    var item = BuildDefault();
                    item.Latitude = 90.1m;

                    RunNegativeTest(item, "Latitude");
                }

                [Fact]
                public void ConfirmLatitudeAccepts90()
                {
                    var item = BuildDefault();
                    item.Latitude = 90.0m;
                    RunPositiveTest(item);
                }

                [Fact]
                public void ConfirmLatitudeRejectsLessThanNeg90()
                {
                    var item = BuildDefault();
                    item.Latitude = -90.1m;

                    RunNegativeTest(item, "Latitude");
                }

                [Fact]
                public void ConfirmLatitudeAcceptsNeg90()
                {
                    var item = BuildDefault();
                    item.Latitude = -90.0m;
                    RunPositiveTest(item);
                }

                [Fact]
                public void ConfirmLongitudeAccepts180()
                {
                    var item = BuildDefault();
                    item.Longitude = 180.0m;
                    RunPositiveTest(item);
                }

                [Fact]
                public void ConfirmLongitudeRejectsGreaterThan180()
                {
                    var item = BuildDefault();
                    item.Longitude = 180.1m;

                    RunNegativeTest(item, "Longitude");
                }
                [Fact]
                public void ConfirmLongitudeAcceptsNeg180()
                {
                    var item = BuildDefault();
                    item.Longitude = -180.0m;
                    RunPositiveTest(item);
                }

                [Fact]
                public void ConfirmLongitudeRejectsGreaterThanNeg180()
                {
                    var item = BuildDefault();
                    item.Longitude = -180.1m;

                    RunNegativeTest(item, "Longitude");
                }
            }
        }
    }
}
