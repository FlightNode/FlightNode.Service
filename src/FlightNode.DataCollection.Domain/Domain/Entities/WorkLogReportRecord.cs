using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightNode.DataCollection.Domain.Entities
{

    public class WorkLogReportRecord
    {
        public int Id { get; set; }

        public DateTime WorkDate { get; set; }

        public decimal WorkHours { get; set; }

        public decimal TravelTimeHours { get; set;  }

        public string WorkType { get; set; }

        public string LocationName { get; set; }

        public decimal Longitude { get; set; }

        public decimal Latitude { get; set; }

        public string DisplayName { get; set; }
    }
}
