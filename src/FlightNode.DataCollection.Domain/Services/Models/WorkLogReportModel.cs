using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightNode.DataCollection.Domain.Entities
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
