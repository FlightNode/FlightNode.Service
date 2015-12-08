using System;

namespace FlightNode.DataCollection.Services.Models
{
    public class WorkLogModel
    {
        public DateTime WorkDate { get; set; }

        public int Id { get; set; }

        public int WorkTypeId { get; set; }

        public decimal WorkHours { get; set; }

        public decimal TravelTimeHours { get; set; }

        public int UserId { get; set; }
        
        public int LocationId { get; set; }
    }
}
