//TODO: I have messed up the Update functionality.But this feature is not immediately
//critical - what is more important is that the rest of the existing features work.
//Therefore commenting this out temporarily in order to focus on other failures first.


using FlightNode.Common.Exceptions;
using FlightNode.DataCollection.Domain.Entities;
using FlightNode.DataCollection.Domain.Interfaces.Persistence;
using FlightNode.DataCollection.Domain.Managers;
using Moq;
using System;
using System.Linq;
using Xunit;

namespace FlightNode.DataCollection.Domain.UnitTests.Domain.Managers
{
    public class WaterbirdForagingManagerTests
    {
        public class Fixture : IDisposable
        {

            protected readonly Guid IDENTIFIER = new Guid("a507f681-c111-447a-bc1f-195916891226");

            protected MockRepository MockRepository = new MockRepository(MockBehavior.Strict);
            protected Mock<ISurveyPersistence> SurveyPersistenceMock;
            //protected Mock<ICrudSet<SurveyPending>> SurveyPendingSet;
            //protected Mock<ICrudSet<SurveyCompleted>> SurveyCompletedSet;
            //protected Mock<ICrudSet<Disturbance>> DisturbanceSet;
            //protected Mock<ICrudSet<Observation>> ObservationSet;
            protected FakeDbSet<SurveyPending> SurveyPendingSet = new FakeDbSet<SurveyPending>();
            protected FakeDbSet<SurveyCompleted> SurveyCompletedSet = new FakeDbSet<SurveyCompleted>();
            protected FakeDbSet<Disturbance> DisturbanceSet = new FakeDbSet<Disturbance>();
            protected FakeDbSet<Observation> ObservationSet = new FakeDbSet<Observation>();


            public Fixture()
            {
                SurveyPersistenceMock = MockRepository.Create<ISurveyPersistence>();
            }

            protected WaterbirdForagingManager BuildSystem()
            {
                return new WaterbirdForagingManager(SurveyPersistenceMock.Object);
            }

            public void Dispose()
            {
                MockRepository.VerifyAll();
            }

            protected void SetupCrudSets()
            {
                //DisturbanceSet = MockRepository.Create<ICrudSet<Disturbance>>();
                //SurveyPersistenceMock.SetupGet(x => x.Disturbances)
                //    .Returns(DisturbanceSet.Object);
                SurveyPersistenceMock.SetupGet(x => x.Disturbances)
                    .Returns(DisturbanceSet);

                //ObservationSet = MockRepository.Create<ICrudSet<Observation>>();
                //SurveyPersistenceMock.SetupGet(x => x.Observations)
                //    .Returns(ObservationSet.Object);
                SurveyPersistenceMock.SetupGet(x => x.Observations)
                    .Returns(ObservationSet);

                //SurveyPendingSet = MockRepository.Create<ICrudSet<SurveyPending>>();
                //SurveyPersistenceMock.SetupGet(x => x.SurveysPending)
                //    .Returns(SurveyPendingSet.Object);
                SurveyPersistenceMock.SetupGet(x => x.SurveysPending)
                    .Returns(SurveyPendingSet);
            }
        }

        public class Constructor : Fixture
        {
            [Fact]
            public void HappyPathDoesNotThrowException()
            {
                BuildSystem();
            }

            [Fact]
            public void RejectsNullArgument()
            {
                Assert.Throws<ArgumentNullException>(() => new WaterbirdForagingManager(null));
            }
        }

        public class Create : Fixture
        {
            protected const int SURVEY_ID = 23;
            protected const int OBSERVATION_ID = 234;
            protected const int DISTURBANCE_ID = 4643;


            public class HappyPath : Create
            {
                
                [Fact]
                public void GivenValidInputThenSaveDisturbancesObservationsAndPendingSurvey()
                {
                    //
                    // Arrange
                    var input = new SurveyPending();
                    var observation = new Observation();
                    input.Observations.Add(observation);
                    var disturb = new Disturbance();
                    input.Disturbances.Add(disturb);


                    // Mocks
                    SetupCrudSets();
                    SurveyPersistenceMock.Setup(x => x.SaveChanges())
                        .Returns(1);


                    //
                    // Act
                    var result = BuildSystem().Create(input);

                    // Assert
                    Assert.Same(disturb, DisturbanceSet.First());
                    Assert.Same(observation, ObservationSet.First());
                    Assert.Same(input, SurveyPendingSet.First());
                }
            }

            public class ErrorHandling : Create
            {
                [Fact]
                public void RejectsNullArgument()
                {
                    Assert.Throws<ArgumentNullException>(() => BuildSystem().Create(null));
                }

                [Fact]
                public void IgnoresExceptions()
                {

                    //
                    // Arrange
                    var input = new SurveyPending();
                    var observation = new Observation();
                    input.Observations.Add(observation);
                    var disturb = new Disturbance();
                    input.Disturbances.Add(disturb);


                    // Mocks
                    SetupCrudSets();
                    //DisturbanceSet.Setup(x => x.Add(It.IsAny<Disturbance>()))
                    //    .Returns(disturb);
                    DisturbanceSet.Add(disturb);
                    //ObservationSet.Setup(x => x.Add(It.IsAny<Observation>()))
                    //    .Returns(observation);
                    ObservationSet.Add(observation);
                    //SurveyPendingSet.Setup(x => x.Add(It.IsAny<SurveyPending>()))
                    //    .Returns(input);
                    SurveyPendingSet.Add(input);

                    SurveyPersistenceMock.Setup(x => x.SaveChanges())
                        .Throws<InvalidOperationException>();


                    //
                    // Act & Assert
                    Assert.Throws<InvalidOperationException>(() => BuildSystem().Create(input));
                }
            }

            public class Validation : Create
            {
                private void RunNegativeTest(SurveyPending item, string memberName)
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

                [Fact]
                public void GeneralCommentsCannotBeLongerThan500Characters()
                {
                    //
                    // Arrange
                    var input = new SurveyPending();
                    input.GeneralComments = "a".PadRight(501, 'a');

                    //
                    // Act
                    RunNegativeTest(input, "GeneralComments");
                }

                [Fact]
                public void DisturbanceCommentsCannotBeLongerThan500Characters()
                {
                    //
                    // Arrange
                    var input = new SurveyPending();
                    input.DisturbanceComments = "a".PadRight(501, 'a');

                    //
                    // Act
                    RunNegativeTest(input, "DisturbanceComments");
                }
            }
        }

        public class Finish : Fixture
        {
            [Fact]
            public void ConvertsToCompletedSurvey()
            {

                //
                // Arrange
                const int id = 3233;

                var input = new SurveyPending();
                input.SubmittedBy = id;
                input.StartDate = DateTime.MinValue;
                input.EndDate = DateTime.MaxValue;
                input.Step = 4;
                input.WaterHeightId = 23;

                var observation = new Observation { Id = 1 };   // causes an update 
                input.Observations.Add(observation);
                var disturb = new Disturbance { Id = 2 };   // causes an update 
                input.Disturbances.Add(disturb);


                // Mocks
                SetupCrudSets();

                //DisturbanceSet.Setup(x => x.Add(It.IsAny<Disturbance>()))
                //    .Callback((Disturbance actual) => Assert.Same(disturb, actual))
                //    .Returns(disturb);
                //ObservationSet.Setup(x => x.Add(It.IsAny<Observation>()))
                //    .Callback((Observation actual) => Assert.Same(observation, actual))
                //    .Returns(observation);
                SurveyPersistenceMock.Setup(x => x.SaveChanges())
                    .Returns(1);



                // bypass the EF update process
                var modifiedWasCalled = 0;
                const int expectedModifications = 3; // observation, disturbance, survey

                ExtensionDelegate.SetModifiedStateDelegate = (IModifiable persistenceLayer, object i) =>
                {
                    modifiedWasCalled++;
                };


                // Need to add the pending survey to EF and then remove it to achieve a delete
                //SurveyPendingSet.Setup(x => x.Add(It.IsAny<SurveyPending>()))
                //    .Returns(input);
                //SurveyPendingSet.Setup(x => x.Remove(It.IsAny<SurveyPending>()))
                //    .Callback((SurveyPending actual) => Assert.Same(input, actual))
                //    .Returns(input);

                // Now setup the new completed record
                //SurveyCompletedSet = MockRepository.Create<ICrudSet<SurveyCompleted>>();
                SurveyPersistenceMock.SetupGet(x => x.SurveysCompleted)
                                        .Returns(SurveyCompletedSet);
                //.Returns(SurveyCompletedSet.Object);
                //SurveyCompletedSet.Setup(x => x.Add(It.IsAny<SurveyCompleted>()))
                //    .Callback((SurveyCompleted actual) =>
                //    {
                //        input.Id = id;
                //    })
                //    .Returns(new SurveyCompleted());

                //
                // Act
                BuildSystem().Finish(input);

                //
                // Assert
                Assert.Equal(expectedModifications, modifiedWasCalled);

                Assert.Equal(id, SurveyCompletedSet.First().SubmittedBy);
                Assert.Equal(input.WaterHeightId, SurveyCompletedSet.First().WaterHeightId);
            }

            [Fact]
            public void RejectsNullArgument()
            {
                Assert.Throws<ArgumentNullException>(() => BuildSystem().Finish(null));
            }

        }

        public class Update : Fixture
        {
            public class ValidInput : Update
            {

                [Fact]
                public void SavesPendingSurvey()
                {
                    //
                    // Arrange
                    const int expectedCount = 3; // Observation, Disturbance, and Survey

                    // Don't extract this to a method for reuse, with the variable at the 
                    // class level. Will cause interacting tests.
                    var modifiedWasCalled = 0;

                    ExtensionDelegate.SetModifiedStateDelegate = (IModifiable persistenceLayer, object i) =>
                    {
                        modifiedWasCalled++;
                    };


                    const int id = 3233;

                    var input = new SurveyPending { Id = id };
                    var observation = new Observation { Id = 1 };
                    input.Observations.Add(observation);
                    var disturb = new Disturbance { Id = 2 };
                    input.Disturbances.Add(disturb);



                    // Mocks
                    SetupCrudSets();

                    //DisturbanceSet.Setup(x => x.Add(It.IsAny<Disturbance>()))
                    //    .Callback((Disturbance actual) => Assert.Same(disturb, actual))
                    //    .Returns(disturb);
                    //ObservationSet.Setup(x => x.Add(It.IsAny<Observation>()))
                    //    .Callback((Observation actual) => Assert.Same(observation, actual))
                    //    .Returns(observation);
                    //SurveyPendingSet.Setup(x => x.Add(It.IsAny<SurveyPending>()))
                    //    .Callback((SurveyPending actual) =>
                    //    {
                    //        input.Id = id;
                    //        Assert.Same(input, actual);
                    //    })
                    //    .Returns(input);
                    SurveyPendingSet.Add(input);
                    SurveyPersistenceMock.Setup(x => x.SaveChanges())
                        .Returns(1);


                    //
                    // Act
                    BuildSystem().Update(input);


                    //
                    // Assert
                    Assert.Equal(expectedCount, modifiedWasCalled);
                    Assert.Same(input, SurveyPendingSet.First());
                }

                [Fact]
                public void RejectsNullArgument()
                {
                    Assert.Throws<ArgumentNullException>(() => BuildSystem().Update(null));
                }



                [Fact]
                public void IgnoresExceptions()
                {

                    //
                    // Arrange
                    var input = new SurveyPending();
                    var observation = new Observation();
                    input.Observations.Add(observation);
                    var disturb = new Disturbance();
                    input.Disturbances.Add(disturb);



                    // Don't extract this to a method for reuse, with the variable at the 
                    // class level. Will cause interacting tests.
                    var ModifiedWasCalled = 0;

                    ExtensionDelegate.SetModifiedStateDelegate = (IModifiable persistenceLayer, object i) =>
                    {
                        ModifiedWasCalled++;
                    };


                    // Mocks
                    SurveyPersistenceMock.SetupGet(x => x.SurveysPending)
                        .Throws<InvalidOperationException>();

                    //
                    // Act & Assert
                    Assert.Throws<InvalidOperationException>(() => BuildSystem().Update(input));
                }
            }

            public class InvalidInput : Update
            {
                private void RunNegativeTest(SurveyPending item, string memberName)
                {
                    try
                    {
                        ExtensionDelegate.SetModifiedStateDelegate = (IModifiable persistenceLayer, object i) => { /* do nothing */ };

                        BuildSystem().Update(item);
                        throw new Exception("this should have failed");
                    }
                    catch (DomainValidationException de)
                    {
                        Assert.True(de.ValidationResults.Any(x => x.MemberNames.Any(y => y == memberName)));
                    }
                }

                [Fact]
                public void GeneralCommentsCannotBeLongerThan500Characters()
                {
                    //
                    // Arrange
                    var input = new SurveyPending();
                    input.GeneralComments = "a".PadRight(501, 'a');

                    //
                    // Act
                    RunNegativeTest(input, "GeneralComments");
                }

                [Fact]
                public void DisturbanceCommentsCannotBeLongerThan500Characters()
                {
                    //
                    // Arrange
                    var input = new SurveyPending();
                    input.DisturbanceComments = "a".PadRight(501, 'a');

                    //
                    // Act
                    RunNegativeTest(input, "DisturbanceComments");
                }
            }
        }

        public class CreateFakeSet : Fixture
        {
            protected const int LOCATION_ID = 234234;
            protected const string LOCATION_NAME = "fasdfasdf";


            protected FakeDbSet<SurveyCompleted> FakeSurveysCompleted = new FakeDbSet<SurveyCompleted>();
            protected FakeDbSet<SurveyPending> FakeSurveysPending = new FakeDbSet<SurveyPending>();
            protected FakeDbSet<Location> FakeLocations = new FakeDbSet<Location>();

            protected void UseFakePendingSet()
            {
                SurveyPersistenceMock.SetupGet(x => x.SurveysPending)
                    .Returns(FakeSurveysPending);
            }

            protected void UseFakeCompletedSet()
            {
                SurveyPersistenceMock.SetupGet(x => x.SurveysCompleted)
                    .Returns(FakeSurveysCompleted);
            }

            protected void UseFakeLocations()
            {
                SurveyPersistenceMock.SetupGet(x => x.Locations)
                    .Returns(FakeLocations);


                FakeLocations.Add(new Location { Id = LOCATION_ID, SiteName = LOCATION_NAME });
            }
        }


        public class FindBySurveyIdentifier : CreateFakeSet
        {
            [Fact]
            public void ReturnNullWhenSurveyDoesNotExist()
            {
                UseFakePendingSet();
                UseFakeCompletedSet();

                var result = BuildSystem().FindBySurveyId(IDENTIFIER);

                Assert.Null(result);
            }

            [Fact]
            public void FindAPendingSurvey()
            {
                //
                // Arrange
                UseFakePendingSet();
                var pending = new SurveyPending { SurveyIdentifier = IDENTIFIER };
                FakeSurveysPending.Add(pending);

                //
                // Act
                var result = BuildSystem().FindBySurveyId(IDENTIFIER);

                //
                // Assert
                Assert.Same(pending, result);
            }

            [Fact]
            public void FindACompletedSurvey()
            {
                //
                // Arrange
                UseFakePendingSet();
                UseFakeCompletedSet();

                var completed = new SurveyCompleted { SurveyIdentifier = IDENTIFIER };
                FakeSurveysCompleted.Add(completed);

                //
                // Act
                var result = BuildSystem().FindBySurveyId(IDENTIFIER);

                //
                // Assert
                Assert.Same(completed, result);
            }

            [Fact]
            public void IgnoreExceptions()
            {
                Assert.Throws<MockException>(() => BuildSystem().FindBySurveyId(IDENTIFIER));
            }
        }


        public class FindBySubmitterId : CreateFakeSet
        {
            private const int USER_ID = 234234;

            [Fact]
            public void ReturnEmptyListWhenSurveyDoesNotExist()
            {
                //
                // Arrange
                UseFakePendingSet();
                UseFakeCompletedSet();

                //
                // Act
                var result = BuildSystem().FindBySubmitterId(USER_ID);

                //
                // Assert
                Assert.False(result.Any());
            }

            [Fact]
            public void FindAPendingSurvey()
            {
                //
                // Arrange
                UseFakePendingSet();
                UseFakeCompletedSet();
                UseFakeLocations();

                var pending = new SurveyPending { SubmittedBy = USER_ID, LocationId = LOCATION_ID };
                FakeSurveysPending.Add(pending);

                //
                // Act
                var result = BuildSystem().FindBySubmitterId(USER_ID);

                //
                // Assert
                Assert.Same(pending, result.First());
            }

            [Fact]
            public void FindACompletedSurvey()
            {
                //
                // Arrange
                UseFakePendingSet();
                UseFakeCompletedSet();
                UseFakeLocations();

                var completed = new SurveyCompleted { SubmittedBy = USER_ID, LocationId = LOCATION_ID };
                FakeSurveysCompleted.Add(completed);

                //
                // Act
                var result = BuildSystem().FindBySubmitterId(USER_ID);

                //
                // Assert
                Assert.Same(completed, result.First());
            }


            [Fact]
            public void AddLocationToCompletedSurvey()
            {
                //
                // Arrange
                UseFakePendingSet();
                UseFakeCompletedSet();
                UseFakeLocations();

                var completed = new SurveyCompleted { SubmittedBy = USER_ID, LocationId = LOCATION_ID };
                FakeSurveysCompleted.Add(completed);

                //
                // Act
                var result = BuildSystem().FindBySubmitterId(USER_ID);

                //
                // Assert
                Assert.Equal(LOCATION_NAME, result.First().LocationName);
            }

            [Fact]
            public void AddLocationToPendingSurvey()
            {
                //
                // Arrange
                UseFakePendingSet();
                UseFakeCompletedSet();
                UseFakeLocations();

                var completed = new SurveyPending { SubmittedBy = USER_ID, LocationId = LOCATION_ID };
                FakeSurveysPending.Add(completed);

                //
                // Act
                var result = BuildSystem().FindBySubmitterId(USER_ID);

                //
                // Assert
                Assert.Equal(LOCATION_NAME, result.First().LocationName);
            }

            [Fact]
            public void FindBothPendingAndCompletedAtSameTime()
            {
                //
                // Arrange
                UseFakePendingSet();
                UseFakeCompletedSet();
                UseFakeLocations();

                var pending = new SurveyPending { SubmittedBy = USER_ID, LocationId = LOCATION_ID };
                FakeSurveysPending.Add(pending);

                var completed = new SurveyCompleted { SubmittedBy = USER_ID, LocationId = LOCATION_ID };
                FakeSurveysCompleted.Add(completed);

                //
                // Act
                var result = BuildSystem().FindBySubmitterId(USER_ID);

                //
                // Assert
                Assert.True(result.Contains(completed));
                Assert.True(result.Contains(pending));
            }

            [Fact]
            public void IgnoreExceptions()
            {
                Assert.Throws<MockException>(() => BuildSystem().FindBySubmitterId(USER_ID));
            }
        }
    }
}
