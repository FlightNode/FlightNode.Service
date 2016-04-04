using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Moq;
using FlightNode.DataCollection.Domain.Interfaces.Persistence;
using FlightNode.DataCollection.Domain.Entities;
using FlightNode.DataCollection.Domain.Managers;

namespace FlightNode.DataCollection.Domain.UnitTests.Domain.Managers
{
    public class BirdSpeciesDomainManagerTests
    {
        public class Fixture
        {
            protected MockRepository MockRepository { get; set; }
            protected Mock<IBirdSpeciesPersistence> BirdSpeciesPersistenceMock { get; set; }


            public Fixture()
            {
                MockRepository = new MockRepository(MockBehavior.Strict);
                BirdSpeciesPersistenceMock = MockRepository.Create<IBirdSpeciesPersistence>();
            }
        }

        public class GetBirdSpeciesBySurveyTypeId : Fixture
        {
            [Fact]
            public void ThereAreNoBirds()
            {
                // Arrange
                BirdSpeciesPersistenceMock.SetupGet(x => x.Collection)
                    .Returns(new FakeDbSet<BirdSpecies>());

                // Act
                var result = new BirdSpeciesDomainManager(BirdSpeciesPersistenceMock.Object)
                                        .GetBirdSpeciesBySurveyTypeId(0);

                // Assert
                Assert.Equal(0, result.Count());
            }

            [Fact]
            public void ThereIsOneMatch()
            {
                // Arrange
                const int surveyTypeId = 23;
                const string species = "Blue Jay";
                var bird = new BirdSpecies
                {
                    SurveyTypes = new List<SurveyType>
                    {
                       new SurveyType { Id = surveyTypeId }
                    },
                    Species = species
                };
                var set = new FakeDbSet<BirdSpecies>();
                set.Add(bird);

                BirdSpeciesPersistenceMock.SetupGet(x => x.Collection)
                    .Returns(set);

                // Act
                var result = new BirdSpeciesDomainManager(BirdSpeciesPersistenceMock.Object)
                                        .GetBirdSpeciesBySurveyTypeId(surveyTypeId);

                // Assert
                Assert.Equal(species, result.First().Species);
            }


            [Fact]
            public void ThereIsABirdButNotWithThisSurvey()
            {
                // Arrange
                const int surveyTypeId = 23;
                const string species = "Blue Jay";
                var bird = new BirdSpecies
                {
                    SurveyTypes = new List<SurveyType>
                    {
                       new SurveyType { Id = surveyTypeId + 100}
                    },
                    Species = species
                };
                var set = new FakeDbSet<BirdSpecies>();
                set.Add(bird);

                BirdSpeciesPersistenceMock.SetupGet(x => x.Collection)
                    .Returns(set);

                // Act
                var result = new BirdSpeciesDomainManager(BirdSpeciesPersistenceMock.Object)
                                        .GetBirdSpeciesBySurveyTypeId(surveyTypeId);

                // Assert
                Assert.Equal(0, result.Count());
            }
        }
    }
}
