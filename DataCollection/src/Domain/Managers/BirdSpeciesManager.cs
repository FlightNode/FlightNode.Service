using FlightNode.DataCollection.Domain.Entities;
using FlightNode.DataCollection.Domain.Interfaces.Persistence;
using System.Collections.Generic;
using System.Linq;

namespace FlightNode.DataCollection.Domain.Managers
{
    public interface IBirdSpeciesDomainManager : ICrudManager<BirdSpecies>
    {
        IEnumerable<BirdSpecies> GetBirdSpeciesBySurveyTypeId(int surveyTypeId);
    }

    public class BirdSpeciesDomainManager : DomainManagerBase<BirdSpecies>, IBirdSpeciesDomainManager
    {
        /// <summary>
        /// Returns the persistence layer as the specific type instead of generic type
        /// </summary>
        private IBirdSpeciesPersistence BirdSpeciesPersistence
        {
            get
            {
                return _persistence as IBirdSpeciesPersistence;
            }
        }

        public BirdSpeciesDomainManager(IBirdSpeciesPersistence birdSpeciesPersistence) : base(birdSpeciesPersistence)
        {
        }

        public IEnumerable<BirdSpecies> GetBirdSpeciesBySurveyTypeId(int surveyTypeId)
        {
            var returnVal = BirdSpeciesPersistence.Collection
                                    .Where(birdItem => birdItem.SurveyTypes.Any(surveyItem => surveyItem.Id == surveyTypeId))
                                    .ToList();
            return returnVal;
        }
    }
}
