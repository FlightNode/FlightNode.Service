using FlightNode.Common.BaseClasses;
using System;
using System.ComponentModel.DataAnnotations;

namespace FlightNode.DataCollection.Domain.Entities
{
    public class WorkLog : IEntity
    {
        [Required]
        public DateTime WorkDate { get; set; }

        [Required]
        public int Id { get; set; }

        [Required]
        public int WorkTypeId { get; set; }

        [Required]
        [Range(0.001, 24.0)]
        public decimal WorkHours { get; set; }

        [Required]
        [Range(0.001, 24.0)]
        public decimal TravelTimeHours { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int LocationId { get; set; }
    }
}
