using FlightNode.Common.BaseClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlightNode.DataCollection.Domain.Entities
{
    public class SurveyPending : IEntity, ISurvey
    {
        [Required]
        public Guid SurveyIdentifier { get; set; }

        public int Id { get; set; }

        [Required]
        public int LocationId { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int? AssessmentId { get; set; }

        public int? VantagePointId { get; set; }

        public int? AccessPointId { get; set; }

        [StringLength(500)]
        [MaxLength(500)]
        public string GeneralComments { get; set; }

        [StringLength(500)]
        [MaxLength(500)]
        public string DisturbanceComments { get; set; }

        [Required]
        public int SurveyTypeId { get; set; }

        [Required]
        public int SubmittedBy { get; set; }

        public int? WeatherId { get; set; }

        public int? StartTemperature { get; set; }

        public int? EndTemperature { get; set; }

        public int? WindSpeed { get; set; }

        public int? TideId { get; set; }

        public DateTime? TimeOfLowTide { get; set; }

        [NotMapped] // Prefer to handle this relationship manually
        public List<Observation> Observations { get; private set; }

        [NotMapped] // Prefer to handle this relationship manually
        public List<Disturbance> Disturbances { get; private set; }

        [NotMapped] // Prefer to handle this relationship manually
        public List<int> Observers { get; private set; }

        // The specific value doesn't matter, so long as it is not COMPLETED_FORAGING_STEP_NUMBER (4).
        [NotMapped]
        public int Step { get; set; } = 0;

        [NotMapped]
        public string LocationName { get; set; }

        public SurveyPending()
        {
            Observations = new List<Observation>();
            Disturbances = new List<Disturbance>();
            Observers = new List<int>();
        }


        ISurvey ISurvey.Add(Observation item)
        {
            return Add(item);
        }

        ISurvey ISurvey.Add(Disturbance item)
        {
            return Add(item);
        }

        ISurvey ISurvey.Add(int userId)
        {
            return Add(userId);
        }

        public SurveyPending Add(Observation item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            Observations.Add(item);
            return this;
        }


        public SurveyPending Add(Disturbance item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            Disturbances.Add(item);
            return this;
        }
        public SurveyPending Add(int userId)
        {
            Observers.Add(userId);
            return this;
        }

        public SurveyCompleted ToSurveyCompleted()
        {
            var completed = new SurveyCompleted
            {
                AccessPointId = this.AccessPointId,
                AssessmentId = this.AssessmentId,
                DisturbanceComments = this.DisturbanceComments,
                EndDate = this.EndDate.Value,
                EndTemperature = this.EndTemperature,
                GeneralComments = this.GeneralComments,
                LocationId = this.LocationId,
                StartDate = this.StartDate.Value,
                StartTemperature = this.StartTemperature,
                SubmittedBy = this.SubmittedBy,
                SurveyIdentifier = this.SurveyIdentifier,
                SurveyTypeId = this.SurveyTypeId,
                TideId = this.TideId,
                TimeOfLowTide = this.TimeOfLowTide,
                VantagePointId = this.VantagePointId,
                WeatherId = this.WeatherId,
                WindSpeed = this.WindSpeed
            };

            completed.Observations.AddRange(this.Observations);
            completed.Disturbances.AddRange(this.Disturbances);
            completed.Observers.AddRange(this.Observers);

            return completed;
        }
    }
}
