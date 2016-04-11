using FlightNode.DataCollection.Infrastructure.Persistence;
using System.Data.Entity.Infrastructure;

namespace FlightNode.DataCollection.Domain.Interfaces.Persistence
{
    public interface IModifiable
    {
        int SaveChanges();
        IDbEntityEntryDecorator Entry(object entity);
    }
}
