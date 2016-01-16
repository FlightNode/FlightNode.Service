using FlightNode.DataCollection.Domain.Entities;
using System.Collections.Generic;

namespace FlightNode.DataCollection.Domain.Interfaces.Persistence
{
    public interface IWorkLogPersistence : IPersistenceBase<WorkLog>
    {
        IEnumerable<WorkLogReportRecord> GetWorkLogReportRecords();
        IEnumerable<WorkLogReportRecord> GetWorkLogReportRecords(int userId);
    }
}
