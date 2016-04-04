using FlightNode.Common.BaseClasses;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlightNode.DataCollection.Domain.Entities
{
    public class SurveyType : IEntity
    {
        public const int TERN_ROOKERY = 1;
        public const int TERN_FORAGING = 2;


        [Required]
        [MaxLength(100)]
        [StringLength(100)]
        public string Description { get; set; }

        public int Id { get; set; }

        public virtual ICollection<BirdSpecies> BirdSpecies { get; private set; }

        public SurveyType()
        {
            Description = string.Empty;

            // TODO: using Set this way... in general makes sense, but probably need to define equality on the BirdSpecies.
            BirdSpecies = new HashSet<BirdSpecies>();
        }
    }
}
