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
            protected FakeDbSet<SurveyPending> SurveyPendingSet = new FakeDbSet<SurveyPending>();
            protected FakeDbSet<SurveyCompleted> SurveyCompletedSet = new FakeDbSet<SurveyCompleted>();
            protected FakeDbSet<Disturbance> DisturbanceSet = new FakeDbSet<Disturbance>();
            protected FakeDbSet<Observation> ObservationSet = new FakeDbSet<Observation>();


            public Fixture()
            {
                SurveyPersistenceMock = MockRepository.Create<ISurveyPersistence>();
            }

            protected SurveyManager BuildSystem()
            {
                return new SurveyManager(SurveyPersistenceMock.Object);
            }

            public void Dispose()
            {
                MockRepository.VerifyAll();
            }

            protected void SetupCrudSets()
            {
                SurveyPersistenceMock.SetupGet(x => x.Disturbances)
                    .Returns(DisturbanceSet);

                SurveyPersistenceMock.SetupGet(x => x.Observations)
                    .Returns(ObservationSet);
            }

            protected void ExpectToWorkWithPendingSurveys()
            {
                SurveyPersistenceMock.SetupGet(x => x.SurveysPending)
                    .Returns(SurveyPendingSet);
            }

            protected void ExpectToWorkWithCompletedSurveys()
            {
                SurveyPersistenceMock.SetupGet(x => x.SurveysCompleted)
                    .Returns(SurveyCompletedSet);
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
                Assert.Throws<ArgumentNullException>(() => new SurveyManager(null));
            }
        }

        public class Delete : CreateFakeSet
        {
            [Fact]
            public void GivenRecordExistsThenReturnTrue()
            {
                // Arrange
                var survey = new SurveyPending { SurveyIdentifier = IDENTIFIER };
                FakeSurveysPending.Add(survey);

                SurveyPersistenceMock.Setup(x => x.SurveysPending)
                    .Returns(FakeSurveysPending);

                SurveyPersistenceMock.Setup(x => x.SaveChanges())
                    .Returns(1);

                // Act
                var result = BuildSystem().Delete(IDENTIFIER);

                // Assert
                Assert.True(result);
            }

            [Fact]
            public void GivenRecordDoesNotExistsThenReturnFalse()
            {
                // Arrange
                
                SurveyPersistenceMock.Setup(x => x.SurveysPending)
                    .Returns(FakeSurveysPending);
                

                // Act
                var result = BuildSystem().Delete(IDENTIFIER);

                // Assert
                Assert.False(result);
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
                    ExpectToWorkWithPendingSurveys();
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
                    ExpectToWorkWithPendingSurveys();
                    DisturbanceSet.Add(disturb);
                    ObservationSet.Add(observation);
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

                var input = new SurveyPending
                {
                    SubmittedBy = id,
                    StartDate = DateTime.MinValue,
                    EndDate = DateTime.MaxValue,
                    WaterHeightId = 23,
                    SurveyIdentifier = Guid.Empty,
                    Id = id
                };

                var observation = new Observation { Id = 1 };   // causes an update 
                input.Observations.Add(observation);
                var disturb = new Disturbance { Id = 2 };   // causes an update 
                input.Disturbances.Add(disturb);


                // Mocks
                SetupCrudSets();
                ExpectToWorkWithPendingSurveys();
                SurveyPersistenceMock.Setup(x => x.SaveChanges())
                    .Returns(1);



                // bypass the EF update process
                var modifiedWasCalled = 0;
                const int expectedModifications = 3; // observation, disturbance, survey

                ExtensionDelegate.SetModifiedStateDelegate = (IModifiable persistenceLayer, object i) =>
                {
                    modifiedWasCalled++;
                };

                // The pending survey needs to exist in order to delete it
                SurveyPendingSet.Add(input);

                // Now setup the new completed record
                SurveyPersistenceMock.SetupGet(x => x.SurveysCompleted)
                                        .Returns(SurveyCompletedSet);


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

        public class UpdatePending : Fixture
        {
            public class ValidInput : UpdatePending
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
                    ExpectToWorkWithPendingSurveys();
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
                    Assert.Throws<ArgumentNullException>(() => BuildSystem().Update(null as SurveyPending));
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

            public class InvalidInput : UpdatePending
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

        public class UpdateCompleted : Fixture
        {
            public class ValidInput : UpdateCompleted
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

                    var input = new SurveyCompleted { Id = id };
                    var observation = new Observation { Id = 1 };
                    input.Observations.Add(observation);
                    var disturb = new Disturbance { Id = 2 };
                    input.Disturbances.Add(disturb);



                    // Mocks
                    SetupCrudSets();
                    ExpectToWorkWithCompletedSurveys();
                    SurveyCompletedSet.Add(input);
                    SurveyPersistenceMock.Setup(x => x.SaveChanges())
                        .Returns(1);


                    //
                    // Act
                    BuildSystem().Update(input);


                    //
                    // Assert
                    Assert.Equal(expectedCount, modifiedWasCalled);
                    Assert.Same(input, SurveyCompletedSet.First());
                }

                [Fact]
                public void RejectsNullArgument()
                {
                    Assert.Throws<ArgumentNullException>(() => BuildSystem().Update(null as SurveyCompleted));
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

            public class InvalidInput : UpdateCompleted
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
            protected FakeDbSet<Observation> FakeObservations = new FakeDbSet<Observation>();
            protected FakeDbSet<Disturbance> FakeDisturbances = new FakeDbSet<Disturbance>();

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

            protected void UseFakeObservations()
            {
                SurveyPersistenceMock.SetupGet(x => x.Observations)
                    .Returns(FakeObservations);
            }

            protected void UseFakeDisturbances()
            {
                SurveyPersistenceMock.SetupGet(x => x.Disturbances)
                    .Returns(FakeDisturbances);
            }
        }


        public class FindBySurveyIdentifier : CreateFakeSet
        {
            [Fact]
            public void ReturnNullWhenSurveyDoesNotExist()
            {
                UseFakePendingSet();
                UseFakeCompletedSet();

                var result = BuildSystem().FindBySurveyId(IDENTIFIER, 1);

                Assert.Null(result);
            }

            [Fact]
            public void FindAPendingForagingSurvey()
            {
                //
                // Arrange
                UseFakePendingSet();
                UseFakeDisturbances();
                UseFakeObservations();

                var pending = new SurveyPending { SurveyIdentifier = IDENTIFIER, SurveyTypeId = SurveyType.Foraging };
                var pending2 = new SurveyPending { SurveyIdentifier = IDENTIFIER, SurveyTypeId = SurveyType.Rookery };
                FakeSurveysPending.Add(pending);
                FakeSurveysPending.Add(pending2);

                //
                // Act
                var result = BuildSystem().FindBySurveyId(IDENTIFIER, SurveyType.Foraging);

                //
                // Assert
                Assert.Same(pending, result);
            }


            [Fact]
            public void FindAPendingRookerySurvey()
            {
                //
                // Arrange
                UseFakePendingSet();
                UseFakeDisturbances();
                UseFakeObservations();

                var pending2 = new SurveyPending { SurveyIdentifier = IDENTIFIER, SurveyTypeId = SurveyType.Foraging };
                var pending = new SurveyPending { SurveyIdentifier = IDENTIFIER, SurveyTypeId = SurveyType.Rookery };
                FakeSurveysPending.Add(pending);
                FakeSurveysPending.Add(pending2);

                //
                // Act
                var result = BuildSystem().FindBySurveyId(IDENTIFIER, SurveyType.Rookery);

                //
                // Assert
                Assert.Same(pending, result);
            }

            [Fact]
            public void LoadObservationsIntoPendingSurvey()
            {
                //
                // Arrange
                UseFakePendingSet();
                UseFakeDisturbances();
                UseFakeObservations();

                var pending = new SurveyPending { SurveyIdentifier = IDENTIFIER, SurveyTypeId = SurveyType.Rookery };
                FakeSurveysPending.Add(pending);

                var observation = new Observation { SurveyIdentifier = IDENTIFIER };
                FakeObservations.Add(observation);

                //
                // Act
                var result = BuildSystem().FindBySurveyId(IDENTIFIER, SurveyType.Rookery);

                //
                // Assert
                Assert.Same(observation, result.Observations.First());
            }

            [Fact]
            public void LoadDisturbancesIntoPendingSurvey()
            {
                //
                // Arrange
                UseFakePendingSet();
                UseFakeDisturbances();
                UseFakeObservations();

                var pending = new SurveyPending { SurveyIdentifier = IDENTIFIER, SurveyTypeId = SurveyType.Rookery };
                FakeSurveysPending.Add(pending);

                var disturbance = new Disturbance { SurveyIdentifier = IDENTIFIER };
                FakeDisturbances.Add(disturbance);

                //
                // Act
                var result = BuildSystem().FindBySurveyId(IDENTIFIER, SurveyType.Rookery);

                //
                // Assert
                Assert.Same(disturbance, result.Disturbances.First());
            }

            [Fact]
            public void FindACompletedSurvey()
            {
                //
                // Arrange
                UseFakePendingSet();
                UseFakeCompletedSet();
                UseFakeDisturbances();
                UseFakeObservations();

                var completed = new SurveyCompleted { SurveyIdentifier = IDENTIFIER, SurveyTypeId = SurveyType.Rookery };
                FakeSurveysCompleted.Add(completed);

                //
                // Act
                var result = BuildSystem().FindBySurveyId(IDENTIFIER, SurveyType.Rookery);

                //
                // Assert
                Assert.Same(completed, result);
            }


            [Fact]
            public void LoadObservationsIntoCompletedSurvey()
            {
                //
                // Arrange
                UseFakePendingSet();
                UseFakeCompletedSet();
                UseFakeDisturbances();
                UseFakeObservations();

                var completed = new SurveyCompleted { SurveyIdentifier = IDENTIFIER, SurveyTypeId = SurveyType.Rookery };
                FakeSurveysCompleted.Add(completed);

                var observation = new Observation { SurveyIdentifier = IDENTIFIER };
                FakeObservations.Add(observation);

                //
                // Act
                var result = BuildSystem().FindBySurveyId(IDENTIFIER, SurveyType.Rookery);

                //
                // Assert
                Assert.Same(observation, result.Observations.First());
            }


            [Fact]
            public void LoadDisturbancesIntoCompletedSurvey()
            {
                //
                // Arrange
                UseFakePendingSet();
                UseFakeCompletedSet();
                UseFakeDisturbances();
                UseFakeObservations();

                var completed = new SurveyCompleted { SurveyIdentifier = IDENTIFIER, SurveyTypeId = SurveyType.Rookery };
                FakeSurveysCompleted.Add(completed);

                var disturbance = new Disturbance { SurveyIdentifier = IDENTIFIER };
                FakeDisturbances.Add(disturbance);

                //
                // Act
                var result = BuildSystem().FindBySurveyId(IDENTIFIER, SurveyType.Rookery);

                //
                // Assert
                Assert.Same(disturbance, result.Disturbances.First());
            }

            [Fact]
            public void IgnoreExceptions()
            {
                Assert.Throws<MockException>(() => BuildSystem().FindBySurveyId(IDENTIFIER, SurveyType.Rookery));
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
                var result = BuildSystem().FindBySubmitterIdAndSurveyType(USER_ID, SurveyType.Foraging);

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

                var pending = new SurveyPending { SubmittedBy = USER_ID, LocationId = LOCATION_ID, SurveyTypeId = SurveyType.Foraging };
                FakeSurveysPending.Add(pending);

                //
                // Act
                var result = BuildSystem().FindBySubmitterIdAndSurveyType(USER_ID, SurveyType.Foraging);

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

                var completed = new SurveyCompleted { SubmittedBy = USER_ID, LocationId = LOCATION_ID, SurveyTypeId = SurveyType.Foraging };
                FakeSurveysCompleted.Add(completed);

                //
                // Act
                var result = BuildSystem().FindBySubmitterIdAndSurveyType(USER_ID, SurveyType.Foraging);

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

                var completed = new SurveyCompleted { SubmittedBy = USER_ID, LocationId = LOCATION_ID, SurveyTypeId = SurveyType.Foraging };
                FakeSurveysCompleted.Add(completed);

                //
                // Act
                var result = BuildSystem().FindBySubmitterIdAndSurveyType(USER_ID, SurveyType.Foraging);

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

                var completed = new SurveyPending { SubmittedBy = USER_ID, LocationId = LOCATION_ID, SurveyTypeId = SurveyType.Foraging };
                FakeSurveysPending.Add(completed);

                //
                // Act
                var result = BuildSystem().FindBySubmitterIdAndSurveyType(USER_ID, SurveyType.Foraging);

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

                var pending = new SurveyPending { SubmittedBy = USER_ID, LocationId = LOCATION_ID, SurveyTypeId = SurveyType.Foraging };
                FakeSurveysPending.Add(pending);

                var completed = new SurveyCompleted { SubmittedBy = USER_ID, LocationId = LOCATION_ID, SurveyTypeId = SurveyType.Foraging };
                FakeSurveysCompleted.Add(completed);

                //
                // Act
                var result = BuildSystem().FindBySubmitterIdAndSurveyType(USER_ID, SurveyType.Foraging);

                //
                // Assert
                Assert.True(result.Contains(completed));
                Assert.True(result.Contains(pending));
            }


            [Fact]
            public void GivenQueryForForagingThenDoNotReturnRookerySurveys()
            {
                //
                // Arrange
                UseFakePendingSet();
                UseFakeCompletedSet();
                UseFakeLocations();

                var pending = new SurveyPending { SubmittedBy = USER_ID, LocationId = LOCATION_ID, SurveyTypeId = SurveyType.Rookery };
                FakeSurveysPending.Add(pending);

                var completed = new SurveyCompleted { SubmittedBy = USER_ID, LocationId = LOCATION_ID, SurveyTypeId = SurveyType.Foraging };
                FakeSurveysCompleted.Add(completed);

                //
                // Act
                var result = BuildSystem().FindBySubmitterIdAndSurveyType(USER_ID, SurveyType.Foraging);

                //
                // Assert
                Assert.False(result.Contains(pending));
            }

            [Fact]
            public void IgnoreExceptions()
            {
                Assert.Throws<MockException>(() => BuildSystem().FindBySubmitterIdAndSurveyType(USER_ID, SurveyType.Foraging));
            }
        }
    }
}
