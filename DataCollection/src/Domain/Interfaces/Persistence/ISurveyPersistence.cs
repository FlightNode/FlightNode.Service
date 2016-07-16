using FlightNode.DataCollection.Domain.Entities;

namespace FlightNode.DataCollection.Domain.Interfaces.Persistence
{
    public interface ISurveyPersistence : IModifiable
    {
        ICrudSet<SurveyCompleted> SurveysCompleted { get; }
        ICrudSet<SurveyPending> SurveysPending { get; }
        ICrudSet<Disturbance> Disturbances { get; }
        ICrudSet<Observation> Observations { get; }
        ICrudSet<Location> Locations { get; }
        ICrudSet<User> Users { get; }
    }
}
