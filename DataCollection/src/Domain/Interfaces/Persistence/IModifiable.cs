using FlightNode.DataCollection.Infrastructure.Persistence;

namespace FlightNode.DataCollection.Domain.Interfaces.Persistence
{
    public interface IModifiable
    {
        int SaveChanges();
        IDbEntityEntryDecorator Entry(object entity);
    }
}
