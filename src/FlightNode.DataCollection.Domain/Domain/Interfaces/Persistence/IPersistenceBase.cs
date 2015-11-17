using FlightNode.Common.BaseClasses;
using System.Data.Entity;

namespace FlightNode.DataCollection.Domain.Interfaces.Persistence
{
    public interface IPersistenceBase<TEntity>
        where TEntity : class, IEntity
    {
        int SaveChanges();
        IDbSet<TEntity> Collection { get; }

    }
}
