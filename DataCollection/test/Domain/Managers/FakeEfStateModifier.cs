using FlightNode.DataCollection.Domain.Interfaces.Persistence;
using FlightNode.DataCollection.Domain.Managers;

namespace FlightNode.DataCollection.Domain.UnitTests.Domain.Managers
{
    public class FakeEfStateModifier : EfStateModifier
    {
        public int CountSetModifiedStateCalls { get; set; }

        public bool StateModifierWasCalled => CountSetModifiedStateCalls > 0;

        public override void SetModifiedState<TEntity>(IPersistenceBase<TEntity> persistenceLayer, TEntity input)
        {
            CountSetModifiedStateCalls++;
        }


    }
}