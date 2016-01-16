
namespace FlightNode.DataCollection.Domain.Entities
{

    public class WorkLogReportRecord
    {
        public int Id { get; set; }

        public string WorkDate { get; set; }

        public decimal WorkHours { get; set; }

        public decimal TravelTimeHours { get; set;  }

        public int WorkTypeId { get; set; }

        public string WorkType { get; set; }

        public int LocationId { get; set; }

        public string LocationName { get; set; }

        public decimal Longitude { get; set; }

        public decimal Latitude { get; set; }

        public int UserId { get; set; }

        public string Person { get; set; }
    }
}
