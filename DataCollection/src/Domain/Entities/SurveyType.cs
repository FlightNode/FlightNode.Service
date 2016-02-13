using FlightNode.Common.BaseClasses;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlightNode.DataCollection.Domain.Entities
{
    public class SurveyType : IEntity
    {
        [Required]
        [MaxLength(100)]
        [StringLength(100)]
        public string Description { get; set; }

        public int Id { get; set; }

        public virtual ICollection<BirdSpecies> BirdSpecies { get; set; }
    }
}
