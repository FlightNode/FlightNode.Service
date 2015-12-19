using System.Collections.Generic;
using System.Linq;
using FlightNode.DataCollection.Domain.Entities;
using FlightNode.DataCollection.Domain.Interfaces.Persistence;

namespace FlightNode.DataCollection.Domain.Managers
{
    public interface IWorkLogDomainManager : ICrudManager<WorkLog>
    {
        List<WorkLogReportRecord> GetReport();
    }

    public class WorkLogDomainManager : DomainManagerBase<WorkLog>, IWorkLogDomainManager
    {
        /// <summary>
        /// Returns the persistence layer as the specific type instead of generic type
        /// </summary>
        /// <remarks>
        /// This property is not in use at this time, and has been created just to illuatrate
        /// how to access the specific persistence layer when overriding the base class
        /// methods or adding methods not in the base class.
        /// </remarks>
        private IWorkLogPersistence WorkLogPersistence
        {
            get
            {
                return _persistence as IWorkLogPersistence;
            }
        }

        public WorkLogDomainManager(IWorkLogPersistence persistence) : base(persistence)
        {
        }

        public List<WorkLogReportRecord> GetReport()
        {
            return WorkLogPersistence.WorkLogReportRecords.ToList();
        }
    }
}
