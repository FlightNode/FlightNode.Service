using FlightNode.DataCollection.Domain.Entities;
using FlightNode.DataCollection.Domain.Interfaces.Persistence;

namespace FlightNode.DataCollection.Domain.Managers
{
    public interface ISurveyTypeDomainManager : ICrudManager<SurveyType>
    {
    }

    public class SurveyTypeDomainManager : DomainManagerBase<SurveyType>, ISurveyTypeDomainManager
    {
        /// <summary>
        /// Returns the persistence layer as the specific type instead of generic type
        /// </summary>
        /// <remarks>
        /// This property is not in use at this time, and has been created just to illuatrate
        /// how to access the specific persistence layer when overriding the base class
        /// methods or adding methods not in the base class.
        /// </remarks>
        private ISurveyTypePersistence LocationPersistence
        {
            get
            {
                return _persistence as ISurveyTypePersistence;
            }
        }

        public SurveyTypeDomainManager(ISurveyTypePersistence locationPersistence) : base(locationPersistence)
        {
        }
    }
}
