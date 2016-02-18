using System;

namespace FlightNode.DataCollection.Domain.Entities
{
    // linking class for User to Survey
    public class Observer
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public Guid SurveyIdentifier { get; set; }

    }
}