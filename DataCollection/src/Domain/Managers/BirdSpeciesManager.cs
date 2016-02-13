using FlightNode.DataCollection.Domain.Entities;
using FlightNode.DataCollection.Domain.Interfaces.Persistence;
using System.Collections.Generic;

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
        /// <remarks>
        /// This property is not in use at this time, and has been created just to illustrate
        /// how to access the specific persistence layer when overriding the base class
        /// methods or adding methods not in the base class.
        /// </remarks>
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
            var retVal = BirdSpeciesPersistence.FindBirdSpeciesBySurveyTypeId(surveyTypeId);
            return retVal;
        }

    }
}
