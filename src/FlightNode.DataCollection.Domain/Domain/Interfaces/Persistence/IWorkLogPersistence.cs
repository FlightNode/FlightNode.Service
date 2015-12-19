using System.Data.Entity;
using FlightNode.DataCollection.Domain.Entities;

namespace FlightNode.DataCollection.Domain.Interfaces.Persistence
{
    public interface IWorkLogPersistence : IPersistenceBase<WorkLog>
    {

        DbSet<WorkLogReportRecord> WorkLogReportRecords { get; set; }
    }
}
