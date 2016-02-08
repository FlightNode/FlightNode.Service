using FlightNode.Common.BaseClasses;
using FlightNode.Identity.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FlightNode.DataCollection.Domain.Entities
{
    public class SurveyCompleted : IEntity
    {
        public Guid SurveyIdentifier { get; set; }

        public int Id { get; set; }

        [Required]
        public int LocationId { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public int Assessment { get; set; }

        [Required]
        public int Vantage { get; set; }

        [Required]
        public int Access { get; set; }

        [StringLength(500)]
        [MaxLength(500)]
        public string GeneralComments { get; set; }

        [StringLength(500)]
        [MaxLength(500)]
        public string DisturbanceComments { get; set; }

        public virtual ICollection<Disturbance> Disturbances { get; set; }

        public virtual ICollection<Observation> Observations { get; set; }

        public SurveyType SurveyType { get; set; }

        public int SubmittedBy { get; set; }
    }
}
