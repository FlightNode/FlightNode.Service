using FlightNode.DataCollection.Domain.Entities;
using System.Collections.Generic;
using System.Linq;

namespace FlightNode.DataCollection.Services.Models
{

    public class WorkLogReportModel
    {
        public IList<WorkLogReportRecord> Records { get; protected set; }

        public WorkLogReportModel(IEnumerable<WorkLogReportRecord> records)
        {
            Records = records.ToList();
        }
    }
}
