using FlightNode.Common.BaseClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        public int? AssessmentId { get; set; }

        public int? VantagePointId { get; set; }

        public int? AccessPointId { get; set; }

        [StringLength(500)]
        [MaxLength(500)]
        public string GeneralComments { get; set; }

        [StringLength(500)]
        [MaxLength(500)]
        public string DisturbanceComments { get; set; }

        public int SurveyTypeId { get; set; }

        public int SubmittedBy { get; set; }

        public int? WeatherId { get; set; }

        public int? StartTemperature { get; set; }

        public int? EndTemperature { get; set; }

        public int? WindSpeedId { get; set; }

        public int? TideId { get; set; }

        public DateTime? TimeOfLowTide { get; set; }

        [NotMapped] // Prefer to handle this relationship manually
        public List<Observation> Observations { get; private set; }

        [NotMapped] // Prefer to handle this relationship manually
        public List<Disturbance> Disturbances { get; private set; }

        [NotMapped] // Prefer to handle this relationship manually
        public List<int> Observers { get; private set; }


        public SurveyCompleted()
        {
            Observations = new List<Observation>();
            Disturbances = new List<Disturbance>();
            Observers = new List<int>();
        }


        public SurveyCompleted Add(Observation item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            Observations.Add(item);
            return this;
        }
        
        public SurveyCompleted Add(Disturbance item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            Disturbances.Add(item);
            return this;
        }

        public SurveyCompleted Add(int userId)
        {
            Observers.Add(userId);
            return this;
        }
    }
}
