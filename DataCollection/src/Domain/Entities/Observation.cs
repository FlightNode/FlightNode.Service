using FlightNode.Common.BaseClasses;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlightNode.DataCollection.Domain.Entities
{
    public class Observation : IEntity
    {
        public int Id { get; set; }

        public int BirdSpeciesId { get; set; }

        [ForeignKey("BirdSpeciesId")]
        public BirdSpecies BirdSpecies { get; set; }

        public bool NestPresent { get; set; }

        public bool ChicksPresent { get; set; }

        public bool FledglingPresent { get; set; }

        /// <summary>
        /// Foraging survey: count of Adults.
        /// </summary>
        public int Bin1 { get; set; }

        /// <summary>
        /// Foraging survey: count of juveniles.
        /// </summary>
        public int Bin2 { get; set; }

        /// <summary>
        /// Foraging survey: does not use.
        /// </summary>
        public int Bin3 { get; set; }

        public Guid SurveyIdentifier { get; set; }

        public int? PrimaryActivityId { get; set; }

        public int? SecondaryActivityId { get; set; }

        public int? HabitatTypeId { get; set; }

        public int? FeedingSuccessRate { get; set; }

    }
}