using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightNode.DataCollection.Domain.Entities
{
    public class BirdSpecies
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
        [Required]
        public string SubFamily { get; set; }

        [MaxLength(50)]
        [StringLength(50)]
        [Required]
        public string Genus { get; set; }

        [MaxLength(50)]
        [StringLength(50)]
        [Required]
        public string Species { get; set; }

        public virtual ICollection<SurveyType> SurveyTypes { get; set; }
    }
}
