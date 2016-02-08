using FlightNode.Identity.Domain.Entities;

namespace FlightNode.DataCollection.Domain.Entities
{
    // linking class for User to Survey
    public class Observer
    {
        public User User { get; set; }

        public SurveyCompleted SurveyCompleted { get;set;}

    }
}