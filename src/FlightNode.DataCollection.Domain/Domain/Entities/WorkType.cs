using FlightNode.Common.BaseClasses;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FlightNode.DataCollection.Domain.Entities
{
    public class WorkType :IEntity
    {
        public int WorkTypeId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Description { get; set; }

        public virtual List<WorkLog> WorkLogs { get; set; }

        public int Id { get { return WorkTypeId; } }
    }
}
