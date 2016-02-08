using FlightNode.Common.BaseClasses;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FlightNode.DataCollection.Domain.Entities
{
    public class DisturbanceType : IEntity
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        [StringLength(100)]
        public string Description { get; set; }

        public virtual ICollection<Disturbance> Disturbances { get; set; }
    }
}