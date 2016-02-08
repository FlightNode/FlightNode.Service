using FlightNode.Common.BaseClasses;
using System.ComponentModel.DataAnnotations;

namespace FlightNode.DataCollection.Domain.Entities
{
    public class Disturbance : IEntity
    {
        public int Id { get; set; }

        public DisturbanceType DisturbanceType { get; set; }
        
        public int Quantity { get; set; }

        public int DurationMinutes { get; set; }

        [MaxLength(150)]
        [StringLength(150)]
        public string Result { get; set; }

        public virtual SurveyCompleted SurveyCompleted { get; set; }
    }
}