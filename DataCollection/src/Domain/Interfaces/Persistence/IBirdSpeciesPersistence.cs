using FlightNode.DataCollection.Domain.Entities;
using System.Collections.Generic;

namespace FlightNode.DataCollection.Domain.Interfaces.Persistence
{
    public interface IBirdSpeciesPersistence : IPersistenceBase<BirdSpecies>
    {
        IEnumerable<BirdSpecies> GetBirdSpeciesBySurveyTypeId(int surveyTypeId);
    }
}
