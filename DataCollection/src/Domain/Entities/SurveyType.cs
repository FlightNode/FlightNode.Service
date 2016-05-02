using System.Collections.Generic;

namespace FlightNode.DataCollection.Domain.Entities
{
    public class SurveyType : EnumBase
    {
        public const int TERN_ROOKERY = 1;
        public const int TERN_FORAGING = 2;

        public virtual ICollection<BirdSpecies> BirdSpecies { get; private set; }

        public SurveyType()
        {
            Description = string.Empty;

            BirdSpecies = new HashSet<BirdSpecies>();
        }
    }
}
