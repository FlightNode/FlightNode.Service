using FlightNode.Common.BaseClasses;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FlightNode.DataCollection.Domain.Entities
{
    public class BirdSpecies : IEntity
    {
        public int Id { get; set; }
        
        [StringLength(4)]
        [Required]
        public string CommonAlphaCode { get; set; }
        
        [StringLength(50)]
        [Required]
        public string CommonName { get; set; }

        [MaxLength(50)]
        [StringLength(50)]
        [Required]
        public string Order { get; set; }

        [MaxLength(50)]
        [StringLength(50)]
        [Required]
        public string Family { get; set; }

        [MaxLength(50)]
        [StringLength(50)]
        public string SubFamily { get; set; }

        [MaxLength(50)]
        [StringLength(50)]
        [Required]
        public string Genus { get; set; }

        [MaxLength(50)]
        [StringLength(50)]
        [Required]
        public string Species { get; set; }

        public virtual ICollection<SurveyType> SurveyTypes { get; private set; }

        public BirdSpecies()
        {
            SurveyTypes = new HashSet<SurveyType>();
        }
    }
}
