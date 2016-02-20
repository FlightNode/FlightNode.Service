using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Moq;
using FlightNode.DataCollection.Domain.Interfaces.Persistence;
using FlightNode.DataCollection.Domain.Managers;
using FlightNode.DataCollection.Domain.Entities;
using FlightNode.Common.Exceptions;

namespace FlightNode.DataCollection.Domain.UnitTests.Domain.Managers
{
    public class WaterbirdForagingManagerTests
    {
        public class Fixture : IDisposable
        {
            protected MockRepository MockRepository = new MockRepository(MockBehavior.Strict);
            protected Mock<ISurveyPersistence> SurveyPersistenceMock;
            protected Mock<ICrudSet<SurveyPending>> SurveyPendingSet;
            protected Mock<ICrudSet<SurveyCompleted>> SurveyCompletedSet;
            protected Mock<ICrudSet<Disturbance>> DisturbanceSet;
            protected Mock<ICrudSet<Observation>> ObservationSet;


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
                DisturbanceSet = MockRepository.Create<ICrudSet<Disturbance>>();
                SurveyPersistenceMock.SetupGet(x => x.Disturbances)
                    .Returns(DisturbanceSet.Object);

                ObservationSet = MockRepository.Create<ICrudSet<Observation>>();
                SurveyPersistenceMock.SetupGet(x => x.Observations)
                    .Returns(ObservationSet.Object);

                SurveyPendingSet = MockRepository.Create<ICrudSet<SurveyPending>>();
                SurveyPersistenceMock.SetupGet(x => x.SurveysPending)
                    .Returns(SurveyPendingSet.Object);
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
                public void SaveReturnsTheNewSurveyId()
                {
                    var result = RunTest();

                    Assert.Equal(SURVEY_ID, result.Id);
                }


                [Fact]
                public void SaveReturnsTheNewObservationId()
                {
                    var result = RunTest();

                    Assert.Equal(OBSERVATION_ID, result.Observations.First().Id);
                }


                [Fact]
                public void SaveReturnsTheNewDisturbanceId()
                {
                    var result = RunTest();

                    Assert.Equal(DISTURBANCE_ID, result.Disturbances.First().Id);
                }

                private SurveyPending RunTest()
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
                    DisturbanceSet.Setup(x => x.Add(It.IsAny<Disturbance>()))
                        .Callback((Disturbance actual) =>
                        {
                            actual.Id = DISTURBANCE_ID;
                            Assert.Same(disturb, actual);
                        })
                        .Returns((Disturbance actual) => actual);
                    ObservationSet.Setup(x => x.Add(It.IsAny<Observation>()))
                        .Callback((Observation actual) =>
                        {
                            actual.Id = OBSERVATION_ID;
                            Assert.Same(observation, actual);
                        })
                        .Returns((Observation actual) => actual);
                    SurveyPendingSet.Setup(x => x.Add(It.IsAny<SurveyPending>()))
                        .Callback((SurveyPending actual) =>
                        {
                            actual.Id = SURVEY_ID;
                            Assert.Same(input, actual);
                        })
                        .Returns((SurveyPending actual) => actual);
                    SurveyPersistenceMock.Setup(x => x.SaveChanges())
                        .Returns(1);


                    //
                    // Act
                    var result = BuildSystem().Create(input);
                    return result;
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
                    DisturbanceSet.Setup(x => x.Add(It.IsAny<Disturbance>()))
                        .Returns(disturb);
                    ObservationSet.Setup(x => x.Add(It.IsAny<Observation>()))
                        .Returns(observation);
                    SurveyPendingSet.Setup(x => x.Add(It.IsAny<SurveyPending>()))
                        .Returns(input);
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

        public class Update : Fixture
        {
            public class ValidInput : Update
            {

                [Fact]
                public void Step1SavesPendingData()
                {
                    //
                    // Arrange

                    // Don't extract this to a method for reuse, with the variable at the 
                    // class level. Will cause interacting tests.
                    var modifiedWasCalled = 0;

                    WaterbirdForagingManager.SetModifiedState = (ISurveyPersistence persistenceLayer, object i) =>
                    {
                        modifiedWasCalled++;
                    };

                    var step = 1;

                    //
                    // Act
                    RunTest(step);

                    //
                    // Assert
                    Assert.Equal(3, modifiedWasCalled);
                }

                [Fact]
                public void Step2SavesPendingData()
                {
                    //
                    // Arrange

                    // Don't extract this to a method for reuse, with the variable at the 
                    // class level. Will cause interacting tests.
                    var modifiedWasCalled = 0;

                    WaterbirdForagingManager.SetModifiedState = (ISurveyPersistence persistenceLayer, object i) =>
                    {
                        modifiedWasCalled++;
                    };

                    var step = 2;

                    //
                    // Act
                    RunTest(step);

                    //
                    // Assert
                    Assert.Equal(3, modifiedWasCalled);
                }

                [Fact]
                public void Step3SavesPendingData()
                {
                    //
                    // Arrange

                    // Don't extract this to a method for reuse, with the variable at the 
                    // class level. Will cause interacting tests.
                    var modifiedWasCalled = 0;

                    WaterbirdForagingManager.SetModifiedState = (ISurveyPersistence persistenceLayer, object i) =>
                    {
                        modifiedWasCalled++;
                    };

                    var step = 3;

                    //
                    // Act
                    RunTest(step);

                    //
                    // Assert
                    Assert.Equal(3, modifiedWasCalled);
                }

                [Fact]
                public void Step4ConvertsToCompletedSurvey()
                {

                    //
                    // Arrange
                    const int id = 3233;

                    var input = new SurveyPending();
                    input.SubmittedBy = id;
                    input.StartDate = DateTime.MinValue;
                    input.EndDate = DateTime.MaxValue;

                    var observation = new Observation();
                    input.Observations.Add(observation);
                    var disturb = new Disturbance();
                    input.Disturbances.Add(disturb);
                    

                    // Mocks
                    SetupCrudSets();

                    DisturbanceSet.Setup(x => x.Add(It.IsAny<Disturbance>()))
                        .Callback((Disturbance actual) => Assert.Same(disturb, actual))
                        .Returns(disturb);
                    ObservationSet.Setup(x => x.Add(It.IsAny<Observation>()))
                        .Callback((Observation actual) => Assert.Same(observation, actual))
                        .Returns(observation);
                    SurveyPersistenceMock.Setup(x => x.SaveChanges())
                        .Returns(1);



                    // bypass the EF update process
                    var modifiedWasCalled = 0;

                    WaterbirdForagingManager.SetModifiedState = (ISurveyPersistence persistenceLayer, object i) =>
                    {
                        modifiedWasCalled++;
                    };


                    // Need to add the pending survey to EF and then remove it to achieve a delete
                    SurveyPendingSet.Setup(x => x.Add(It.IsAny<SurveyPending>()))
                        .Returns(input);
                    SurveyPendingSet.Setup(x => x.Remove(It.IsAny<SurveyPending>()))
                        .Callback((SurveyPending actual) => Assert.Same(input, actual))
                        .Returns(input);

                    // Now setup the new completed record
                    SurveyCompletedSet = MockRepository.Create<ICrudSet<SurveyCompleted>>();
                    SurveyPersistenceMock.SetupGet(x => x.SurveysCompleted)
                        .Returns(SurveyCompletedSet.Object);
                    SurveyCompletedSet.Setup(x => x.Add(It.IsAny<SurveyCompleted>()))
                        .Callback((SurveyCompleted actual) =>
                        {
                            input.Id = id;
                            Assert.Equal(id, actual.SubmittedBy);
                        })
                        .Returns(new SurveyCompleted());

                    //
                    // Act
                    BuildSystem().Update(input, 4);

                    //
                    // Assert
                    Assert.Equal(3, modifiedWasCalled);

                }

                private void RunTest(int step)
                {
                    //
                    // Arrange
                    const int id = 3233;

                    var input = new SurveyPending();
                    var observation = new Observation();
                    input.Observations.Add(observation);
                    var disturb = new Disturbance();
                    input.Disturbances.Add(disturb);



                    // Mocks
                    SetupCrudSets();

                    DisturbanceSet.Setup(x => x.Add(It.IsAny<Disturbance>()))
                        .Callback((Disturbance actual) => Assert.Same(disturb, actual))
                        .Returns(disturb);
                    ObservationSet.Setup(x => x.Add(It.IsAny<Observation>()))
                        .Callback((Observation actual) => Assert.Same(observation, actual))
                        .Returns(observation);
                    SurveyPendingSet.Setup(x => x.Add(It.IsAny<SurveyPending>()))
                        .Callback((SurveyPending actual) =>
                        {
                            input.Id = id;
                            Assert.Same(input, actual);
                        })
                        .Returns(input);
                    SurveyPersistenceMock.Setup(x => x.SaveChanges())
                        .Returns(1);


                    //
                    // Act
                    BuildSystem().Update(input, step);
                }

                [Fact]
                public void RejectsNullArgument()
                {
                    Assert.Throws<ArgumentNullException>(() => BuildSystem().Update(null, 1));
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

                    WaterbirdForagingManager.SetModifiedState = (ISurveyPersistence persistenceLayer, object i) =>
                    {
                        ModifiedWasCalled++;
                    };


                    // Mocks

                    SurveyPendingSet = MockRepository.Create<ICrudSet<SurveyPending>>();
                    SurveyPersistenceMock.SetupGet(x => x.SurveysPending)
                        .Returns(SurveyPendingSet.Object);
                    SurveyPendingSet.Setup(x => x.Add(It.IsAny<SurveyPending>()))
                        .Throws<InvalidOperationException>();


                    //
                    // Act & Assert
                    Assert.Throws<InvalidOperationException>(() => BuildSystem().Update(input, 1));
                }
            }

            public class InvalidInput : Update
            {
                private void RunNegativeTest(SurveyPending item, string memberName)
                {
                    try
                    {
                        WaterbirdForagingManager.SetModifiedState = (ISurveyPersistence persistenceLayer, object i) => { /* do nothing */ };

                            BuildSystem().Update(item, 1);
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
    }
}
