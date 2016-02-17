using FlightNode.Common.BaseClasses;
using System;

namespace FlightNode.DataCollection.Domain.Entities
{
    public class Observation : IEntity
    {
        public int Id { get; set; }

        public BirdSpecies BirdSpecies { get; set; }

        public bool NestPresent { get; set; }

        public bool ChicksPresent { get; set; }

        public bool FledglingPresent { get; set; }

        // Some surveys methodologies use exact counts
        public int RawCount { get; set; }

        // While others use bins that could be a count per distance or a
        // range of numbers. The exact meaning of the following fields
        // will depend on the survey type

        public int Bin1 { get; set; }

        public int Bin2 { get; set; }

        public int Bin3 { get; set; }

        public Guid SurveyIdentifier { get; set; }
    }
}