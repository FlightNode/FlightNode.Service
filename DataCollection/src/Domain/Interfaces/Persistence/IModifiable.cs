using System.Data.Entity.Infrastructure;

namespace FlightNode.DataCollection.Domain.Interfaces.Persistence
{
    public interface IModifiable
    {
        int SaveChanges();
        DbEntityEntry Entry(object entity);
    }
}
